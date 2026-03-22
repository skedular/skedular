import { isServer } from '@/libs/utils';
import { createClient } from 'graphql-sse';
import type { FetchFunction, GraphQLResponse, SubscribeFunction } from 'relay-runtime';
import { Environment, Network, Observable, RecordSource, Store } from 'relay-runtime';
import { v7 as uuid } from 'uuid';

const HTTP_RETRY_ATTEMPTS = 5;
const HTTP_RETRY_DELAY_MS = 1000;

const sleep = async (milliseconds: number) => {
  await new Promise((resolve) => setTimeout(resolve, milliseconds));
};

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

    for (let attempt = 1; attempt <= HTTP_RETRY_ATTEMPTS; attempt += 1) {
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: buildHeaders(),
        body: JSON.stringify({
          query: params.text,
          variables,
        }),
      });

      if (response.status !== 504 || attempt === HTTP_RETRY_ATTEMPTS) {
        return (await response.json()) as GraphQLResponse;
      }

      await sleep(HTTP_RETRY_DELAY_MS);
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

export function getEnvironment(endpoint: string, token?: string | null | undefined): Environment {
  if (isServer) {
    return new Environment({
      network: createNetwork(endpoint, token),
      store: new Store(new RecordSource()),
      isServer: true,
    });
  } else {
    if (clientEnvironment == null) {
      clientEnvironment = new Environment({
        network: createNetwork(endpoint, token),
        store: new Store(new RecordSource()),
        isServer: false,
      });
    }

    return clientEnvironment;
  }
}
