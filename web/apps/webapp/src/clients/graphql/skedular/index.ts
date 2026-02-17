import { isServer } from '@/libs/utils';
import { createClient } from 'graphql-sse';
import type { FetchFunction, GraphQLResponse, SubscribeFunction } from 'relay-runtime';
import { Environment, Network, Observable, RecordSource, Store } from 'relay-runtime';
import { v7 as uuid } from 'uuid';

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

    const response = await fetch(endpoint, {
      method: 'POST',
      headers: buildHeaders(),
      body: JSON.stringify({
        query: params.text,
        variables,
      }),
    });

    return await response.json();
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
