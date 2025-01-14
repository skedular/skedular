/**
 * @generated SignedSource<<4bca10ecae347c1dbbb526a39cf68ea2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ActivateDesksInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationLocation_activateDesksMutation$variables = {
  input: ActivateDesksInput;
};
export type organizationLocation_activateDesksMutation$data = {
  readonly activateDesks: {
    readonly desks: ReadonlyArray<{
      readonly customTags: ReadonlyArray<{
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly deactivated: boolean;
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
export type organizationLocation_activateDesksMutation = {
  response: organizationLocation_activateDesksMutation$data;
  variables: organizationLocation_activateDesksMutation$variables;
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
    "name": "activateDesks",
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
    "name": "organizationLocation_activateDesksMutation",
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_activateDesksMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "eaedc553b1a3951b0f42ce5dfb08f5e9",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_activateDesksMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_activateDesksMutation(\n  $input: ActivateDesksInput!\n) {\n  activateDesks(input: $input) {\n    desks {\n      id\n      name\n      deactivated\n      requireBookingApproval\n      customTags {\n        uniqueId\n        name\n      }\n      zones {\n        uniqueId\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a3f26aea286c1d17329acfec631b9244";

export default node;
