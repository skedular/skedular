/**
 * @generated SignedSource<<a92ca5eea3462a45bb9cd594e1858cc4>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationOfferingInput = {
  clientMutationId?: string | null | undefined;
  offeringCode: string;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation$variables = {
  input: UpdateOrganizationOfferingInput;
};
export type organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation$data = {
  readonly updateOrganizationOffering: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation = {
  response: organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation$data;
  variables: organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation$variables;
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
    "name": "organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "568b7905a977aa5adc923e89012174cc",
    "id": null,
    "metadata": {},
    "name": "organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation(\n  $input: UpdateOrganizationOfferingInput!\n) {\n  updateOrganizationOffering(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "1ab377ad1fccd7a4e1284c26fb0d5ee5";

export default node;
