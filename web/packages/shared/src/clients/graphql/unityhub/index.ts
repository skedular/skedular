import { Environment, Network, RecordSource, Store } from 'relay-runtime';
import { v4 as uuidv4 } from 'uuid';

export function createNetwork(authorization: string | null) {
  return Network.create(async (params, variables) => {
    const headers: { [key: string]: string } = {
      'Content-Type': 'application/json',
      'X-Correlation-Id': uuidv4(),
    };

    if (authorization) {
      headers['Authorization'] = authorization;
    }

    const response = await fetch('/api/graphql', {
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

export function getEnvironment(authorization: string | null): Environment {
  if (typeof window === 'undefined') {
    return new Environment({
      network: createNetwork(authorization),
      store: new Store(new RecordSource()),
      isServer: true,
    });
  } else {
    if (clientEnvironment == null) {
      clientEnvironment = new Environment({
        network: createNetwork(authorization),
        store: new Store(new RecordSource()),
        isServer: false,
      });
    }

    return clientEnvironment;
  }
}
