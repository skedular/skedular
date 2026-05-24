import { createClient } from 'graphql-sse';
import type { FetchFunction, GraphQLResponse, SubscribeFunction } from 'relay-runtime';
import { Environment, Network, Observable, RecordSource, Store } from 'relay-runtime';
import { v7 as uuid } from 'uuid';
import { isServer } from './constants';

const HTTP_RETRY_ATTEMPTS = 5;
const HTTP_RETRY_DELAY_MS = 1000;
const GRAPHQL_ERROR_RETRY_ATTEMPTS = 3;

const sleep = async (milliseconds: number) => {
  await new Promise((resolve) => setTimeout(resolve, milliseconds));
};

const getGraphqlErrors = (payload: GraphQLResponse) => ('errors' in payload && Array.isArray(payload.errors) ? payload.errors : undefined);

export function createNetwork(endpoint: string, token?: string | null | undefined) {
  const buildHeaders = () => {
    const headers: { [key: string]: string } = {
      'Content-Type': 'application/json',
      'X-Correlation-Id': uuid(),
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    return headers;
  };

  const fetchFn: FetchFunction = async (params, variables, _cacheConfig, _uploadables) => {
    void _cacheConfig;
    void _uploadables;

    const isQueryOperation = params.operationKind === 'query';
    let graphqlErrorAttempts = 0;

    for (let attempt = 1; attempt <= HTTP_RETRY_ATTEMPTS; attempt += 1) {
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: buildHeaders(),
        body: JSON.stringify({
          query: params.text,
          variables,
        }),
      });

      if (response.status === 504 && attempt < HTTP_RETRY_ATTEMPTS) {
        await sleep(HTTP_RETRY_DELAY_MS);
        continue;
      }

      const payload = (await response.json()) as GraphQLResponse;
      const errors = getGraphqlErrors(payload);
      const hasGraphqlErrors = response.status === 200 && errors != null && errors.length > 0;
      if (isQueryOperation && hasGraphqlErrors && graphqlErrorAttempts < GRAPHQL_ERROR_RETRY_ATTEMPTS) {
        graphqlErrorAttempts += 1;
        await sleep(HTTP_RETRY_DELAY_MS);
        continue;
      }

      return payload;
    }

    throw new Error('GraphQL request retries exhausted.');
  };

  const sseFetch: typeof fetch = (input, init) =>
    fetch(input, {
      ...init,
      headers: {
        ...(init?.headers ?? {}),
        'Content-Type': 'application/json',
      },
    });

  const sseClient = createClient({
    url: endpoint,
    headers: buildHeaders,
    fetchFn: sseFetch,
  });

  const subscribeFn: SubscribeFunction = (params, variables, _cacheConfig) =>
    Observable.create<GraphQLResponse>((sink) => {
      void _cacheConfig;

      const dispose = sseClient.subscribe(
        {
          query: params.text ?? '',
          variables,
        },
        {
          next: (value) => sink.next(value as GraphQLResponse),
          error: (err) => sink.error(err instanceof Error ? err : new Error(String(err))),
          complete: () => sink.complete(),
        },
      );

      return () => {
        dispose();
      };
    });

  return Network.create(fetchFn, subscribeFn);
}

let clientEnvironment: Environment | undefined;
let clientEnvironmentKey: string | undefined;

export function getEnvironment(endpoint: string, token?: string | null | undefined): Environment {
  if (isServer) {
    return new Environment({
      network: createNetwork(endpoint, token),
      store: new Store(new RecordSource()),
      isServer: true,
    });
  } else {
    const environmentKey = `${endpoint}:${token ?? ''}`;

    if (clientEnvironment == null || clientEnvironmentKey !== environmentKey) {
      clientEnvironment = new Environment({
        network: createNetwork(endpoint, token),
        store: new Store(new RecordSource()),
      });
      clientEnvironmentKey = environmentKey;
    }

    return clientEnvironment;
  }
}
