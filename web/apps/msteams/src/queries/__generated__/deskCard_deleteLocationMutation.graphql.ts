/**
 * @generated SignedSource<<d92894aa8c60db0342ff67f41dbdc5dd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type DeleteDeskInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type deskCard_deleteLocationMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteDeskInput;
};
export type deskCard_deleteLocationMutation$data = {
  readonly deleteDesk: {
    readonly desk: {
      readonly id: string;
    };
  } | null | undefined;
};
export type deskCard_deleteLocationMutation = {
  response: deskCard_deleteLocationMutation$data;
  variables: deskCard_deleteLocationMutation$variables;
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
    "name": "deskCard_deleteLocationMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "DeskPayload",
        "kind": "LinkedField",
        "name": "deleteDesk",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "DeskDetails",
            "kind": "LinkedField",
            "name": "desk",
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
    "name": "deskCard_deleteLocationMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "DeskPayload",
        "kind": "LinkedField",
        "name": "deleteDesk",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "DeskDetails",
            "kind": "LinkedField",
            "name": "desk",
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
    "cacheID": "f84805521d87f445ffc264bfbcad9fa5",
    "id": null,
    "metadata": {},
    "name": "deskCard_deleteLocationMutation",
    "operationKind": "mutation",
    "text": "mutation deskCard_deleteLocationMutation(\n  $input: DeleteDeskInput!\n) {\n  deleteDesk(input: $input) {\n    desk {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "bd4323320c352d28e125f6ba4408c3ad";

export default node;
