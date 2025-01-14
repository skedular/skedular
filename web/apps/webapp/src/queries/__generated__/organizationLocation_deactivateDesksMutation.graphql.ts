/**
 * @generated SignedSource<<b7fb7ad61c52d5ae24ff858f1a3cb557>>
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
    "cacheID": "4a9151495db1cc84379226cff370bc75",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_deactivateDesksMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_deactivateDesksMutation(\n  $input: DeactivateDesksInput!\n) {\n  deactivateDesks(input: $input) {\n    desks {\n      id\n      name\n      deactivated\n      requireBookingApproval\n      customTags {\n        uniqueId\n        name\n      }\n      zones {\n        uniqueId\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7fd3945f0b9a927ffa7c25cf8c16a27d";

export default node;
