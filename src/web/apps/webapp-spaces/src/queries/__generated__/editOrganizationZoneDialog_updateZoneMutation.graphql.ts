/**
 * @generated SignedSource<<bee7e1028e0518501b6e0004a47543e3>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationTagPatchField = "COLOR" | "DESCRIPTION" | "NAME" | "%future added value";
export type UpdateOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  description?: string | null | undefined;
  fieldsToUpdate: ReadonlyArray<OrganizationTagPatchField>;
  id: string;
  name?: string | null | undefined;
};
export type editOrganizationZoneDialog_updateZoneMutation$variables = {
  input: UpdateOrganizationTagInput;
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
    "cacheID": "5bb79dbd7109410247c0775b453cee30",
    "id": null,
    "metadata": {},
    "name": "editOrganizationZoneDialog_updateZoneMutation",
    "operationKind": "mutation",
    "text": "mutation editOrganizationZoneDialog_updateZoneMutation(\n  $input: UpdateOrganizationTagInput!\n) {\n  updateZone(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cd8c26374d787242b8cbfe474ebeab49";

export default node;
