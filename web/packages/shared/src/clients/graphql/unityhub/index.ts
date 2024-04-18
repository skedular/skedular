import { v4 as uuidv4 } from 'uuid';
import { Environment, Network, RecordSource, Store } from 'relay-runtime';

export function createNetwork() {
  return Network.create(async (params, variables) => {
    const headers = {
      'Content-Type': 'application/json',
      'X-Correlation-Id': uuidv4(),
    };

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

export function getEnvironment(): Environment {
  if (typeof window === 'undefined') {
    return new Environment({
      network: createNetwork(),
      store: new Store(new RecordSource()),
      isServer: true,
    });
  } else {
    if (clientEnvironment == null) {
      clientEnvironment = new Environment({
        network: createNetwork(),
        store: new Store(new RecordSource()),
        isServer: false,
      });
    }

    return clientEnvironment;
  }
}
