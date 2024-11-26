/**
 * @generated SignedSource<<c6eade9b7522d52806adc4722135afa8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteZoneInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type zoneCard_deleteZoneMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteZoneInput;
};
export type zoneCard_deleteZoneMutation$data = {
  readonly deleteZone: {
    readonly organizationTag: {
      readonly id: string;
    };
  } | null | undefined;
};
export type zoneCard_deleteZoneMutation = {
  response: zoneCard_deleteZoneMutation$data;
  variables: zoneCard_deleteZoneMutation$variables;
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
    "name": "zoneCard_deleteZoneMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "deleteZone",
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
    "name": "zoneCard_deleteZoneMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "deleteZone",
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
    "cacheID": "fe8cacb1d750ab10b54e19979a8863ed",
    "id": null,
    "metadata": {},
    "name": "zoneCard_deleteZoneMutation",
    "operationKind": "mutation",
    "text": "mutation zoneCard_deleteZoneMutation(\n  $input: DeleteZoneInput!\n) {\n  deleteZone(input: $input) {\n    organizationTag {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7c621c3a540576bc0f23b8de0114afa3";

export default node;
