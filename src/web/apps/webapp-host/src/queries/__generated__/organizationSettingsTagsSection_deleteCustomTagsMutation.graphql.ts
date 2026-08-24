/**
 * @generated SignedSource<<d7df06bc4fa47cadb9202587c1530bc6>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteCustomTagsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationSettingsTagsSection_deleteCustomTagsMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteCustomTagsInput;
};
export type organizationSettingsTagsSection_deleteCustomTagsMutation$data = {
  readonly deleteCustomTags: {
    readonly organizationTags: ReadonlyArray<{
      readonly id: string;
    }>;
  };
};
export type organizationSettingsTagsSection_deleteCustomTagsMutation = {
  response: organizationSettingsTagsSection_deleteCustomTagsMutation$data;
  variables: organizationSettingsTagsSection_deleteCustomTagsMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationSettingsTagsSection_deleteCustomTagsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationTagsPayload",
        "kind": "LinkedField",
        "name": "deleteCustomTags",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "organizationTags",
            "plural": true,
            "selections": [
              (v2/*:: as any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsTagsSection_deleteCustomTagsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationTagsPayload",
        "kind": "LinkedField",
        "name": "deleteCustomTags",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "organizationTags",
            "plural": true,
            "selections": [
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "fa84f127bc6a0726730252ded03beb7c",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsTagsSection_deleteCustomTagsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsTagsSection_deleteCustomTagsMutation(\n  $input: DeleteCustomTagsInput!\n) {\n  deleteCustomTags(input: $input) {\n    organizationTags {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5f5024c665d0a03d4c0dd1c7a5a8b8eb";

export default node;
