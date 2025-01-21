/**
 * @generated SignedSource<<e932e0b35851279366e8dfd419c44047>>
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
export type organizationAdmin_removeCustomerDefaultOrganizationTagMutation$variables = {
  input: RemoveCustomerDefaultOrganizationTagInput;
};
export type organizationAdmin_removeCustomerDefaultOrganizationTagMutation$data = {
  readonly removeCustomerDefaultOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type organizationAdmin_removeCustomerDefaultOrganizationTagMutation = {
  response: organizationAdmin_removeCustomerDefaultOrganizationTagMutation$data;
  variables: organizationAdmin_removeCustomerDefaultOrganizationTagMutation$variables;
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
    "name": "organizationAdmin_removeCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_removeCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "bf863da91d87e259db3fbac9f8b6e8ae",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_removeCustomerDefaultOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_removeCustomerDefaultOrganizationTagMutation(\n  $input: RemoveCustomerDefaultOrganizationTagInput!\n) {\n  removeCustomerDefaultOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f2e4cc603dbc6e6289c8883c7edaf480";

export default node;
