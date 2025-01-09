/**
 * @generated SignedSource<<9330786abbdfa10ef078d768e8b1dc53>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteDeskTypesInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationAdmin_deleteDeskTypesMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteDeskTypesInput;
};
export type organizationAdmin_deleteDeskTypesMutation$data = {
  readonly deleteDeskTypes: {
    readonly organizationTags: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationAdmin_deleteDeskTypesMutation = {
  response: organizationAdmin_deleteDeskTypesMutation$data;
  variables: organizationAdmin_deleteDeskTypesMutation$variables;
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdmin_deleteDeskTypesMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagsPayload",
        "kind": "LinkedField",
        "name": "deleteDeskTypes",
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
              (v2/*: any*/)
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_deleteDeskTypesMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagsPayload",
        "kind": "LinkedField",
        "name": "deleteDeskTypes",
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
              (v2/*: any*/),
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
    "cacheID": "4bdf3e6e9481954ff9440e581a3e0ff9",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_deleteDeskTypesMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_deleteDeskTypesMutation(\n  $input: DeleteDeskTypesInput!\n) {\n  deleteDeskTypes(input: $input) {\n    organizationTags {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ed48c9e5b94828f0f5bb5862911cefa8";

export default node;
