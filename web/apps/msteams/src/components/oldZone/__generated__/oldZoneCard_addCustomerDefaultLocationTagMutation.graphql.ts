/**
 * @generated SignedSource<<f587f848c72abc973513f578ba3533b9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerDefaultLocationTagInput = {
  clientMutationId?: string | null | undefined;
  locationTagId: string;
};
export type oldZoneCard_addCustomerDefaultLocationTagMutation$variables = {
  input: AddCustomerDefaultLocationTagInput;
};
export type oldZoneCard_addCustomerDefaultLocationTagMutation$data = {
  readonly addCustomerDefaultLocationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type oldZoneCard_addCustomerDefaultLocationTagMutation = {
  response: oldZoneCard_addCustomerDefaultLocationTagMutation$data;
  variables: oldZoneCard_addCustomerDefaultLocationTagMutation$variables;
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
    "name": "oldZoneCard_addCustomerDefaultLocationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "oldZoneCard_addCustomerDefaultLocationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "cb3997fa59acf7dc91926a4b22371c39",
    "id": null,
    "metadata": {},
    "name": "oldZoneCard_addCustomerDefaultLocationTagMutation",
    "operationKind": "mutation",
    "text": "mutation oldZoneCard_addCustomerDefaultLocationTagMutation(\n  $input: AddCustomerDefaultLocationTagInput!\n) {\n  addCustomerDefaultLocationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "505205bb1bee9c00954c65db6efde17c";

export default node;
