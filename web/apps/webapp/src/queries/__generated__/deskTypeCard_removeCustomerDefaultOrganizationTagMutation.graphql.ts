/**
 * @generated SignedSource<<26cce94ff510e82036c1274763dce9d4>>
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
export type deskTypeCard_removeCustomerDefaultOrganizationTagMutation$variables = {
  input: RemoveCustomerDefaultOrganizationTagInput;
};
export type deskTypeCard_removeCustomerDefaultOrganizationTagMutation$data = {
  readonly removeCustomerDefaultOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredDeskTypes: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type deskTypeCard_removeCustomerDefaultOrganizationTagMutation = {
  response: deskTypeCard_removeCustomerDefaultOrganizationTagMutation$data;
  variables: deskTypeCard_removeCustomerDefaultOrganizationTagMutation$variables;
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
            "concreteType": "CustomerOrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredDeskTypes",
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
    "name": "deskTypeCard_removeCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "deskTypeCard_removeCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "860a698a9acce028cc6930bbb1fa751b",
    "id": null,
    "metadata": {},
    "name": "deskTypeCard_removeCustomerDefaultOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation deskTypeCard_removeCustomerDefaultOrganizationTagMutation(\n  $input: RemoveCustomerDefaultOrganizationTagInput!\n) {\n  removeCustomerDefaultOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredDeskTypes {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "da75a36093d4ce1fb4cfb189febe0b16";

export default node;
