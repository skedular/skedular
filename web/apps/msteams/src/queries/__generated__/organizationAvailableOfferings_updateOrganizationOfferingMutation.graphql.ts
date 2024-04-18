/**
 * @generated SignedSource<<0b84b9356598cebcdcefe7bbb334707c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type UpdateOrganizationOfferingInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  offeringCode: string;
};
export type organizationAvailableOfferings_updateOrganizationOfferingMutation$variables = {
  input: UpdateOrganizationOfferingInput;
};
export type organizationAvailableOfferings_updateOrganizationOfferingMutation$data = {
  readonly updateOrganizationOffering: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type organizationAvailableOfferings_updateOrganizationOfferingMutation = {
  response: organizationAvailableOfferings_updateOrganizationOfferingMutation$data;
  variables: organizationAvailableOfferings_updateOrganizationOfferingMutation$variables;
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
    "name": "organizationAvailableOfferings_updateOrganizationOfferingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAvailableOfferings_updateOrganizationOfferingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "5fe810ad7fb87dbfa70030cd35f9d381",
    "id": null,
    "metadata": {},
    "name": "organizationAvailableOfferings_updateOrganizationOfferingMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAvailableOfferings_updateOrganizationOfferingMutation(\n  $input: UpdateOrganizationOfferingInput!\n) {\n  updateOrganizationOffering(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "873ab6d0f6c6569d70f133085eab07ea";

export default node;
