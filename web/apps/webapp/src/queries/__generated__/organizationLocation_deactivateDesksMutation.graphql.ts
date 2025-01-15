/**
 * @generated SignedSource<<4fbe2f3edbfbe3f2b2a9f16189f32958>>
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
      readonly customTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly deactivated: boolean;
      readonly id: string;
      readonly name: string;
      readonly requireBookingApproval: boolean;
      readonly zones: ReadonlyArray<{
        readonly color: string | null | undefined;
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
  (v1/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
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
            "name": "customTags",
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
    "cacheID": "1f74f37cd3e1abc6c826c4927cac0def",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_deactivateDesksMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_deactivateDesksMutation(\n  $input: DeactivateDesksInput!\n) {\n  deactivateDesks(input: $input) {\n    desks {\n      id\n      name\n      deactivated\n      requireBookingApproval\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "51983f5f3785087b3df52d48a8fdaae6";

export default node;
