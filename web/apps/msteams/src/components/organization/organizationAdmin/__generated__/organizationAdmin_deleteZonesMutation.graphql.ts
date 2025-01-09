/**
 * @generated SignedSource<<15415f2346f531c49f0ec7f897a178aa>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteZonesInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationAdmin_deleteZonesMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteZonesInput;
};
export type organizationAdmin_deleteZonesMutation$data = {
  readonly deleteZones: {
    readonly organizationTags: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationAdmin_deleteZonesMutation = {
  response: organizationAdmin_deleteZonesMutation$data;
  variables: organizationAdmin_deleteZonesMutation$variables;
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
    "name": "organizationAdmin_deleteZonesMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagsPayload",
        "kind": "LinkedField",
        "name": "deleteZones",
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
    "name": "organizationAdmin_deleteZonesMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagsPayload",
        "kind": "LinkedField",
        "name": "deleteZones",
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
    "cacheID": "d39f9cc607939899d5fe98f1e7bb144c",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_deleteZonesMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_deleteZonesMutation(\n  $input: DeleteZonesInput!\n) {\n  deleteZones(input: $input) {\n    organizationTags {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0b7bc8bd55161338234087123eec7cc7";

export default node;
