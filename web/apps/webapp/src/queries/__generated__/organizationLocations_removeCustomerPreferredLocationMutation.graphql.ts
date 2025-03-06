/**
 * @generated SignedSource<<d23a87e5d9f2b27892d6f9f4f0f5a9fd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPreferredLocationInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type organizationLocations_removeCustomerPreferredLocationMutation$variables = {
  input: RemoveCustomerPreferredLocationInput;
};
export type organizationLocations_removeCustomerPreferredLocationMutation$data = {
  readonly removeCustomerPreferredLocation: {
    readonly customer: {
      readonly id: string;
      readonly preferredLocations: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type organizationLocations_removeCustomerPreferredLocationMutation = {
  response: organizationLocations_removeCustomerPreferredLocationMutation$data;
  variables: organizationLocations_removeCustomerPreferredLocationMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "CustomerPayload",
    "kind": "LinkedField",
    "name": "removeCustomerPreferredLocation",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "customer",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerLocationDetails",
            "kind": "LinkedField",
            "name": "preferredLocations",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocations_removeCustomerPreferredLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocations_removeCustomerPreferredLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "4cb9991bbf4282eb78272652d3fdc79a",
    "id": null,
    "metadata": {},
    "name": "organizationLocations_removeCustomerPreferredLocationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocations_removeCustomerPreferredLocationMutation(\n  $input: RemoveCustomerPreferredLocationInput!\n) {\n  removeCustomerPreferredLocation(input: $input) {\n    customer {\n      id\n      preferredLocations {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6431abdaa1d46962e933749d4a02b347";

export default node;
