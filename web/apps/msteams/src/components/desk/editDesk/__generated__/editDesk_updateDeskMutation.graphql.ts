/**
 * @generated SignedSource<<f39d482ba974317c5b117c97fae37aea>>
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
  deskTypeIds: ReadonlyArray<string>;
  id: string;
  name: string;
  requireBookingApproval: boolean;
  zoneIds: ReadonlyArray<string>;
};
export type editDesk_updateDeskMutation$variables = {
  input: UpdateDeskInput;
};
export type editDesk_updateDeskMutation$data = {
  readonly updateDesk: {
    readonly desk: {
      readonly deactivated: boolean;
      readonly deskTypes: ReadonlyArray<{
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly id: string;
      readonly name: string;
      readonly requireBookingApproval: boolean;
      readonly zones: ReadonlyArray<{
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type editDesk_updateDeskMutation$rawResponse = {
  readonly updateDesk: {
    readonly desk: {
      readonly deactivated: boolean;
      readonly deskTypes: ReadonlyArray<{
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly id: string;
      readonly name: string;
      readonly requireBookingApproval: boolean;
      readonly zones: ReadonlyArray<{
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type editDesk_updateDeskMutation = {
  rawResponse: editDesk_updateDeskMutation$rawResponse;
  response: editDesk_updateDeskMutation$data;
  variables: editDesk_updateDeskMutation$variables;
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
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v1/*: any*/)
],
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          (v1/*: any*/),
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
            "concreteType": "Organization_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "deskTypes",
            "plural": true,
            "selections": (v2/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Organization_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "zones",
            "plural": true,
            "selections": (v2/*: any*/),
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
    "name": "editDesk_updateDeskMutation",
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editDesk_updateDeskMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "a201bbeb94636ac0faa16aad91e22694",
    "id": null,
    "metadata": {},
    "name": "editDesk_updateDeskMutation",
    "operationKind": "mutation",
    "text": "mutation editDesk_updateDeskMutation(\n  $input: UpdateDeskInput!\n) {\n  updateDesk(input: $input) {\n    desk {\n      id\n      name\n      deactivated\n      requireBookingApproval\n      deskTypes {\n        uniqueId\n        name\n      }\n      zones {\n        uniqueId\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b6901d334a7d063a06d68dee13f1b77b";

export default node;
