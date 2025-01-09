/**
 * @generated SignedSource<<48a801fdc390a3f4b98953f0ea54dc65>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateZoneInput = {
  clientMutationId?: string | null | undefined;
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
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type editOrganizationZoneDialog_updateZoneMutation$rawResponse = {
  readonly updateZone: {
    readonly organizationTag: {
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
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
    "name": "editOrganizationZoneDialog_updateZoneMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editOrganizationZoneDialog_updateZoneMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "39cde93a6114a50da82aadaa54f89395",
    "id": null,
    "metadata": {},
    "name": "editOrganizationZoneDialog_updateZoneMutation",
    "operationKind": "mutation",
    "text": "mutation editOrganizationZoneDialog_updateZoneMutation(\n  $input: UpdateZoneInput!\n) {\n  updateZone(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5fe85c5946f967fb83718b5e1e2a81c8";

export default node;
