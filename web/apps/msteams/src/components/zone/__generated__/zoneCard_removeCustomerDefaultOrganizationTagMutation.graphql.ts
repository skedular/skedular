/**
 * @generated SignedSource<<6f69a68c67cebe5cf12e4f4d84e5b15d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerDefaultOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  organizationTagId: string;
};
export type zoneCard_removeCustomerDefaultOrganizationTagMutation$variables = {
  input: RemoveCustomerDefaultOrganizationTagInput;
};
export type zoneCard_removeCustomerDefaultOrganizationTagMutation$data = {
  readonly removeCustomerDefaultOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type zoneCard_removeCustomerDefaultOrganizationTagMutation = {
  response: zoneCard_removeCustomerDefaultOrganizationTagMutation$data;
  variables: zoneCard_removeCustomerDefaultOrganizationTagMutation$variables;
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
    "name": "removeCustomerDefaultOrganizationTag",
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
    "name": "zoneCard_removeCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "zoneCard_removeCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "c631fbcf59ccdc0ced592198947d25bb",
    "id": null,
    "metadata": {},
    "name": "zoneCard_removeCustomerDefaultOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation zoneCard_removeCustomerDefaultOrganizationTagMutation(\n  $input: RemoveCustomerDefaultOrganizationTagInput!\n) {\n  removeCustomerDefaultOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3dca3d0ca8358578a7e8f453c7a5e098";

export default node;
