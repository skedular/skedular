/**
 * @generated SignedSource<<69de553e44f08fd196d9371e73e1f70e>>
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
        readonly id: string;
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
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "preferredLocations",
            "plural": true,
            "selections": [
              (v1/*: any*/)
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
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationCard_removeCustomerPreferredLocationMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "9bda2afa425b45be3bc96ff4d7ad72bb",
    "id": null,
    "metadata": {},
    "name": "locationCard_removeCustomerPreferredLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationCard_removeCustomerPreferredLocationMutation(\n  $input: RemoveCustomerPreferredLocationInput!\n) {\n  removeCustomerPreferredLocation(input: $input) {\n    customer {\n      id\n      preferredLocations {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c1da8058a501fae088d71e3f2cf538e5";

export default node;
