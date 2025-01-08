/**
 * @generated SignedSource<<7f5ed237aa3cf1964ff22274d506c49d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeactivateDesksInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationLocation_deactivateDesksMutation$variables = {
  input: DeactivateDesksInput;
};
export type organizationLocation_deactivateDesksMutation$data = {
  readonly deactivateDesks: {
    readonly desks: ReadonlyArray<{
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
    }>;
  } | null | undefined;
};
export type organizationLocation_deactivateDesksMutation = {
  response: organizationLocation_deactivateDesksMutation$data;
  variables: organizationLocation_deactivateDesksMutation$variables;
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
    "concreteType": "DesksPayload",
    "kind": "LinkedField",
    "name": "deactivateDesks",
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
    "name": "organizationLocation_deactivateDesksMutation",
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_deactivateDesksMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "e98229f779bf9ef8aca9060b855d615c",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_deactivateDesksMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_deactivateDesksMutation(\n  $input: DeactivateDesksInput!\n) {\n  deactivateDesks(input: $input) {\n    desks {\n      id\n      name\n      deactivated\n      requireBookingApproval\n      deskTypes {\n        uniqueId\n        name\n      }\n      zones {\n        uniqueId\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f45c441e2c4c931d2ea44ce90c2485df";

export default node;
