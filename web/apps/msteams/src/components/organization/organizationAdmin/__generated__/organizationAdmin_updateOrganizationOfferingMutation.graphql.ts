/**
 * @generated SignedSource<<638cb4ce79e7095d0c744115f9e6e766>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationOfferingInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  offeringCode: string;
};
export type organizationAdmin_updateOrganizationOfferingMutation$variables = {
  input: UpdateOrganizationOfferingInput;
};
export type organizationAdmin_updateOrganizationOfferingMutation$data = {
  readonly updateOrganizationOffering: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type organizationAdmin_updateOrganizationOfferingMutation = {
  response: organizationAdmin_updateOrganizationOfferingMutation$data;
  variables: organizationAdmin_updateOrganizationOfferingMutation$variables;
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
    "concreteType": "UpdateOrganizationOfferingPayload",
    "kind": "LinkedField",
    "name": "updateOrganizationOffering",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "clientMutationId",
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
    "name": "organizationAdmin_updateOrganizationOfferingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_updateOrganizationOfferingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "776322f435ab5bfc27205912a7f281c4",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_updateOrganizationOfferingMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_updateOrganizationOfferingMutation(\n  $input: UpdateOrganizationOfferingInput!\n) {\n  updateOrganizationOffering(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "72b1b9e7a86af19af362fa81d61a7297";

export default node;
