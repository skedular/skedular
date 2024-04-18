/**
 * @generated SignedSource<<5f5eb46b10789d282891c14b0aef50a0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type AddCustomerDefaultLocationTagInput = {
  clientMutationId?: string | null | undefined;
  locationTagId: string;
};
export type zoneCard_addCustomerDefaultLocationTagMutation$variables = {
  input: AddCustomerDefaultLocationTagInput;
};
export type zoneCard_addCustomerDefaultLocationTagMutation$data = {
  readonly addCustomerDefaultLocationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type zoneCard_addCustomerDefaultLocationTagMutation = {
  response: zoneCard_addCustomerDefaultLocationTagMutation$data;
  variables: zoneCard_addCustomerDefaultLocationTagMutation$variables;
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
    "name": "addCustomerDefaultLocationTag",
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
    "name": "zoneCard_addCustomerDefaultLocationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "zoneCard_addCustomerDefaultLocationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "8abb3fe085bbcf570658ab9dfd286b66",
    "id": null,
    "metadata": {},
    "name": "zoneCard_addCustomerDefaultLocationTagMutation",
    "operationKind": "mutation",
    "text": "mutation zoneCard_addCustomerDefaultLocationTagMutation(\n  $input: AddCustomerDefaultLocationTagInput!\n) {\n  addCustomerDefaultLocationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "34ebe211beaf349697974f1b6161d017";

export default node;
