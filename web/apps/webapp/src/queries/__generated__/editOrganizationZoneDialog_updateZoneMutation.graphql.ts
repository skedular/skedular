/**
 * @generated SignedSource<<a6adb17dff8141aa0f2ed7ae76edbb60>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateZoneInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
};
export type editOrganizationZoneDialog_updateZoneMutation$variables = {
  input: UpdateZoneInput;
};
export type editOrganizationZoneDialog_updateZoneMutation$data = {
  readonly updateZone: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  };
};
export type editOrganizationZoneDialog_updateZoneMutation$rawResponse = {
  readonly updateZone: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  };
};
export type editOrganizationZoneDialog_updateZoneMutation = {
  rawResponse: editOrganizationZoneDialog_updateZoneMutation$rawResponse;
  response: editOrganizationZoneDialog_updateZoneMutation$data;
  variables: editOrganizationZoneDialog_updateZoneMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "OrganizationTagPayload",
    "kind": "LinkedField",
    "name": "updateZone",
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "description",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "color",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editOrganizationZoneDialog_updateZoneMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editOrganizationZoneDialog_updateZoneMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "bc340e23c758ec6defe27201d3e646fb",
    "id": null,
    "metadata": {},
    "name": "editOrganizationZoneDialog_updateZoneMutation",
    "operationKind": "mutation",
    "text": "mutation editOrganizationZoneDialog_updateZoneMutation(\n  $input: UpdateZoneInput!\n) {\n  updateZone(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c0e5e10234b30f8c1a33ae1ed9b061ae";

export default node;
