/**
 * @generated SignedSource<<a3a4e86156e155687221f7a85e3092f4>>
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
export type deskCard_updateDeskMutation$variables = {
  input: UpdateDeskInput;
};
export type deskCard_updateDeskMutation$data = {
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
export type deskCard_updateDeskMutation$rawResponse = {
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
    "cacheID": "594df5d631e05afc7e1e571944b8e162",
    "id": null,
    "metadata": {},
    "name": "deskCard_updateDeskMutation",
    "operationKind": "mutation",
    "text": "mutation deskCard_updateDeskMutation(\n  $input: UpdateDeskInput!\n) {\n  updateDesk(input: $input) {\n    desk {\n      id\n      name\n      deactivated\n      requireBookingApproval\n      deskTypes {\n        uniqueId\n        name\n      }\n      zones {\n        uniqueId\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "23b49abe4ef44f82b6b039e01bb04bb6";

export default node;
