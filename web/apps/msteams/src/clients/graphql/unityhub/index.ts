import { Environment, Network, RecordSource, Store } from 'relay-runtime';
import { v4 as uuidv4 } from 'uuid';

export function createNetwork(token: string | null) {
  return Network.create(async (params, variables) => {
    const headers: { [key: string]: string } = {
      'Content-Type': 'application/json',
      'X-Correlation-Id': uuidv4(),
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(process.env.REACT_APP_API_GATEWAY_PUBLIC_URL!, {
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

export function getEnvironment(token: string | null): Environment {
  if (clientEnvironment == null) {
    clientEnvironment = new Environment({
      network: createNetwork(token),
      store: new Store(new RecordSource()),
      isServer: false,
    });
  }

  return clientEnvironment;
}
