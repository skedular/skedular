/**
 * @generated SignedSource<<14b4ce5f80b187da7c11e000e59c6a07>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type RemoveCustomerDefaultLocationTagInput = {
  clientMutationId?: string | null | undefined;
  locationTagId: string;
};
export type zoneCard_removeCustomerDefaultLocationTagMutation$variables = {
  input: RemoveCustomerDefaultLocationTagInput;
};
export type zoneCard_removeCustomerDefaultLocationTagMutation$data = {
  readonly removeCustomerDefaultLocationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type zoneCard_removeCustomerDefaultLocationTagMutation = {
  response: zoneCard_removeCustomerDefaultLocationTagMutation$data;
  variables: zoneCard_removeCustomerDefaultLocationTagMutation$variables;
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
    "name": "removeCustomerDefaultLocationTag",
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
            "concreteType": "CustomerLocationTagDetails",
            "kind": "LinkedField",
            "name": "preferredZones",
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
    "name": "zoneCard_removeCustomerDefaultLocationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "zoneCard_removeCustomerDefaultLocationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "2cbb3cd15f4145c4ee4dce5abfe71df4",
    "id": null,
    "metadata": {},
    "name": "zoneCard_removeCustomerDefaultLocationTagMutation",
    "operationKind": "mutation",
    "text": "mutation zoneCard_removeCustomerDefaultLocationTagMutation(\n  $input: RemoveCustomerDefaultLocationTagInput!\n) {\n  removeCustomerDefaultLocationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a67d2fff177b059eae21aa9ef30ca464";

export default node;
