/**
 * @generated SignedSource<<af8574240cb3c777582b1d8d9953499c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerDefaultLocationTagInput = {
  clientMutationId?: string | null | undefined;
  locationTagId: string;
};
export type oldZoneCard_removeCustomerDefaultLocationTagMutation$variables = {
  input: RemoveCustomerDefaultLocationTagInput;
};
export type oldZoneCard_removeCustomerDefaultLocationTagMutation$data = {
  readonly removeCustomerDefaultLocationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type oldZoneCard_removeCustomerDefaultLocationTagMutation = {
  response: oldZoneCard_removeCustomerDefaultLocationTagMutation$data;
  variables: oldZoneCard_removeCustomerDefaultLocationTagMutation$variables;
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
    "name": "oldZoneCard_removeCustomerDefaultLocationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "oldZoneCard_removeCustomerDefaultLocationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "a4a48d8264e208d69d44dd4e5eac9588",
    "id": null,
    "metadata": {},
    "name": "oldZoneCard_removeCustomerDefaultLocationTagMutation",
    "operationKind": "mutation",
    "text": "mutation oldZoneCard_removeCustomerDefaultLocationTagMutation(\n  $input: RemoveCustomerDefaultLocationTagInput!\n) {\n  removeCustomerDefaultLocationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4748a8bcc0652efc51eb033ef1fc786a";

export default node;
