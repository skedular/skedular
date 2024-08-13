/**
 * @generated SignedSource<<2600d71a852bed3888310781c7d21b9e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type RemoveCustomerDefaultLocationInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type locationCard_removeCustomerDefaultLocationMutation$variables = {
  input: RemoveCustomerDefaultLocationInput;
};
export type locationCard_removeCustomerDefaultLocationMutation$data = {
  readonly removeCustomerDefaultLocation: {
    readonly customer: {
      readonly defaultLocations: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
    };
  } | null | undefined;
};
export type locationCard_removeCustomerDefaultLocationMutation = {
  response: locationCard_removeCustomerDefaultLocationMutation$data;
  variables: locationCard_removeCustomerDefaultLocationMutation$variables;
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
    "name": "removeCustomerDefaultLocation",
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
            "name": "defaultLocations",
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
    "name": "locationCard_removeCustomerDefaultLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationCard_removeCustomerDefaultLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "369dc32ee28bc8a9dda1fb9422815330",
    "id": null,
    "metadata": {},
    "name": "locationCard_removeCustomerDefaultLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationCard_removeCustomerDefaultLocationMutation(\n  $input: RemoveCustomerDefaultLocationInput!\n) {\n  removeCustomerDefaultLocation(input: $input) {\n    customer {\n      id\n      defaultLocations {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f358772180e2a5b7cdfd08ab87dd38d3";

export default node;
