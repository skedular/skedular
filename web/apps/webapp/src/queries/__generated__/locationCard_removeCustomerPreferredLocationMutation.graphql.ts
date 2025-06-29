/**
 * @generated SignedSource<<ef72b90675f46f535a3245706c22e564>>
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
export type locationCard_removeCustomerPreferredLocationMutation$variables = {
  input: RemoveCustomerPreferredLocationInput;
};
export type locationCard_removeCustomerPreferredLocationMutation$data = {
  readonly removeCustomerPreferredLocation: {
    readonly customer: {
      readonly id: string;
      readonly preferredLocations: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  };
};
export type locationCard_removeCustomerPreferredLocationMutation = {
  response: locationCard_removeCustomerPreferredLocationMutation$data;
  variables: locationCard_removeCustomerPreferredLocationMutation$variables;
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
            "concreteType": "Customer_LocationDetails",
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
    "name": "locationCard_removeCustomerPreferredLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationCard_removeCustomerPreferredLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "7e390d74896808e8f6c0327b59fa147f",
    "id": null,
    "metadata": {},
    "name": "locationCard_removeCustomerPreferredLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationCard_removeCustomerPreferredLocationMutation(\n  $input: RemoveCustomerPreferredLocationInput!\n) {\n  removeCustomerPreferredLocation(input: $input) {\n    customer {\n      id\n      preferredLocations {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4161a95347a33b35752a236c9641aea5";

export default node;
