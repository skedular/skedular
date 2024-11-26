/**
 * @generated SignedSource<<cb967a198d76cc6601bab531cdecfa24>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteDeskTypeInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type deskTypeCard_deleteDeskTypeMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteDeskTypeInput;
};
export type deskTypeCard_deleteDeskTypeMutation$data = {
  readonly deleteDeskType: {
    readonly organizationTag: {
      readonly id: string;
    };
  } | null | undefined;
};
export type deskTypeCard_deleteDeskTypeMutation = {
  response: deskTypeCard_deleteDeskTypeMutation$data;
  variables: deskTypeCard_deleteDeskTypeMutation$variables;
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
    "name": "deskTypeCard_deleteDeskTypeMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "deleteDeskType",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "organizationTag",
            "plural": false,
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
    "name": "deskTypeCard_deleteDeskTypeMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "deleteDeskType",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "organizationTag",
            "plural": false,
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
    "cacheID": "72bd5edef2060d9559f59e9f5313eb81",
    "id": null,
    "metadata": {},
    "name": "deskTypeCard_deleteDeskTypeMutation",
    "operationKind": "mutation",
    "text": "mutation deskTypeCard_deleteDeskTypeMutation(\n  $input: DeleteDeskTypeInput!\n) {\n  deleteDeskType(input: $input) {\n    organizationTag {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "997a5a3244e016532fd39aded8defa9c";

export default node;
