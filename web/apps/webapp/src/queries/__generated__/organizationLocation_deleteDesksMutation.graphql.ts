/**
 * @generated SignedSource<<9d9e5363f533350ac117a9ae0da084ef>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteDesksInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationLocation_deleteDesksMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteDesksInput;
};
export type organizationLocation_deleteDesksMutation$data = {
  readonly deleteDesks: {
    readonly desks: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationLocation_deleteDesksMutation = {
  response: organizationLocation_deleteDesksMutation$data;
  variables: organizationLocation_deleteDesksMutation$variables;
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
    "name": "organizationLocation_deleteDesksMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "DesksPayload",
        "kind": "LinkedField",
        "name": "deleteDesks",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "DeskDetails",
            "kind": "LinkedField",
            "name": "desks",
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
    "name": "organizationLocation_deleteDesksMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "DesksPayload",
        "kind": "LinkedField",
        "name": "deleteDesks",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "DeskDetails",
            "kind": "LinkedField",
            "name": "desks",
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
    "cacheID": "4f05c80d32907eed5bacf9c56aafab9b",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_deleteDesksMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_deleteDesksMutation(\n  $input: DeleteDesksInput!\n) {\n  deleteDesks(input: $input) {\n    desks {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ea4585a2cecf292511ae60aa6d8ae1f0";

export default node;
