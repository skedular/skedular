/**
 * @generated SignedSource<<2725287c07b3fb4faf0edc466b9bbcb5>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateDeskInput = {
  clientMutationId?: string | null | undefined;
  deactivated: boolean;
  id: string;
  locationTagIds: ReadonlyArray<string>;
  name: string;
  requireBookingApproval: boolean;
};
export type deskCard_updateDeskMutation$variables = {
  input: UpdateDeskInput;
};
export type deskCard_updateDeskMutation$data = {
  readonly updateDesk: {
    readonly desk: {
      readonly deactivated: boolean;
      readonly id: string;
      readonly locationTags: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly name: string;
      readonly requireBookingApproval: boolean;
    };
  } | null | undefined;
};
export type deskCard_updateDeskMutation$rawResponse = {
  readonly updateDesk: {
    readonly desk: {
      readonly deactivated: boolean;
      readonly id: string;
      readonly locationTags: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly name: string;
      readonly requireBookingApproval: boolean;
    };
  } | null | undefined;
};
export type deskCard_updateDeskMutation = {
  rawResponse: deskCard_updateDeskMutation$rawResponse;
  response: deskCard_updateDeskMutation$data;
  variables: deskCard_updateDeskMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "DeskPayload",
    "kind": "LinkedField",
    "name": "updateDesk",
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
          (v1/*: any*/),
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "deactivated",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "requireBookingApproval",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationTagDetails",
            "kind": "LinkedField",
            "name": "locationTags",
            "plural": true,
            "selections": [
              (v1/*: any*/),
              (v2/*: any*/)
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
    "name": "deskCard_updateDeskMutation",
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "deskCard_updateDeskMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "69958c0aafa373cfdfccef9e5d9123e1",
    "id": null,
    "metadata": {},
    "name": "deskCard_updateDeskMutation",
    "operationKind": "mutation",
    "text": "mutation deskCard_updateDeskMutation(\n  $input: UpdateDeskInput!\n) {\n  updateDesk(input: $input) {\n    desk {\n      id\n      name\n      deactivated\n      requireBookingApproval\n      locationTags {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e36fecba566d0fef2a08be8c9365626b";

export default node;
