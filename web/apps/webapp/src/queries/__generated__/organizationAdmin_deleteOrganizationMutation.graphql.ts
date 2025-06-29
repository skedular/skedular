/**
 * @generated SignedSource<<655d53f6e29e8c31ec87b244c85b317e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteOrganizationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationAdmin_deleteOrganizationMutation$variables = {
  input: DeleteOrganizationInput;
};
export type organizationAdmin_deleteOrganizationMutation$data = {
  readonly deleteOrganization: {
    readonly organization: {
      readonly id: string;
    };
  };
};
export type organizationAdmin_deleteOrganizationMutation = {
  response: organizationAdmin_deleteOrganizationMutation$data;
  variables: organizationAdmin_deleteOrganizationMutation$variables;
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
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "deleteOrganization",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
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
    "name": "organizationAdmin_deleteOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_deleteOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "3030b9cb96c6d4f034e8fe25ae357f01",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_deleteOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_deleteOrganizationMutation(\n  $input: DeleteOrganizationInput!\n) {\n  deleteOrganization(input: $input) {\n    organization {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "541befc543b8d6b610d21b55fc85c78c";

export default node;
