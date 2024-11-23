/**
 * @generated SignedSource<<765ee589d98f468e97c27280445c7739>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type deskTypeCard_deleteOrganizationTagMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteOrganizationTagInput;
};
export type deskTypeCard_deleteOrganizationTagMutation$data = {
  readonly deleteOrganizationTag: {
    readonly organizationTag: {
      readonly id: string;
    };
  } | null | undefined;
};
export type deskTypeCard_deleteOrganizationTagMutation = {
  response: deskTypeCard_deleteOrganizationTagMutation$data;
  variables: deskTypeCard_deleteOrganizationTagMutation$variables;
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
    "name": "deskTypeCard_deleteOrganizationTagMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "deleteOrganizationTag",
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
    "name": "deskTypeCard_deleteOrganizationTagMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "deleteOrganizationTag",
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
    "cacheID": "e49e08fc83958b88700a43bf98af24be",
    "id": null,
    "metadata": {},
    "name": "deskTypeCard_deleteOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation deskTypeCard_deleteOrganizationTagMutation(\n  $input: DeleteOrganizationTagInput!\n) {\n  deleteOrganizationTag(input: $input) {\n    organizationTag {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1652939601dd40f73393a5740d0ff552";

export default node;
