import { Environment, Network, RecordSource, Store } from 'relay-runtime';
import { v7 as uuid } from 'uuid';

export function createNetwork(endpoint: string, token?: string | null | undefined) {
  return Network.create(async (params, variables) => {
    const headers: { [key: string]: string } = {
      'Content-Type': 'application/json',
      'X-Correlation-Id': uuid(),
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(endpoint, {
      method: 'POST',
      headers,
      body: JSON.stringify({
        query: params.text,
        variables,
      }),
    });

    return await response.json();
  });
}

let clientEnvironment: Environment | undefined;

export function getEnvironment(endpoint: string, token?: string | null | undefined): Environment {
  if (typeof window === 'undefined') {
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
