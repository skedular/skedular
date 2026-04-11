/**
 * @generated SignedSource<<d17eaea17675673ebab244a0762ae421>>
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
export type organizationAdminZonesSection_deleteZonesMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteZonesInput;
};
export type organizationAdminZonesSection_deleteZonesMutation$data = {
  readonly deleteZones: {
    readonly organizationTags: ReadonlyArray<{
      readonly id: string;
    }>;
  };
};
export type organizationAdminZonesSection_deleteZonesMutation = {
  response: organizationAdminZonesSection_deleteZonesMutation$data;
  variables: organizationAdminZonesSection_deleteZonesMutation$variables;
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
    "name": "organizationAdminZonesSection_deleteZonesMutation",
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
    "name": "organizationAdminZonesSection_deleteZonesMutation",
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
    "cacheID": "df98b0c874fc8e9e415ee1d41af3deb7",
    "id": null,
    "metadata": {},
    "name": "organizationAdminZonesSection_deleteZonesMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminZonesSection_deleteZonesMutation(\n  $input: DeleteZonesInput!\n) {\n  deleteZones(input: $input) {\n    organizationTags {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "85cfafa9f6e70ddfc0e9923a56dc21be";

export default node;
