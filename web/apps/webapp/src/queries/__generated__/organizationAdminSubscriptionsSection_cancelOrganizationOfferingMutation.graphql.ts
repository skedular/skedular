/**
 * @generated SignedSource<<bb0ab08cd201bff62233ef84bda3c5b8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CancelOrganizationOfferingInput = {
  clientMutationId?: string | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation$variables = {
  input: CancelOrganizationOfferingInput;
};
export type organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation$data = {
  readonly cancelOrganizationOffering: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation = {
  response: organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation$data;
  variables: organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation$variables;
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
    "concreteType": "CancelOrganizationOfferingPayload",
    "kind": "LinkedField",
    "name": "cancelOrganizationOffering",
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
    "name": "organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "2218da4795f98819d2d7dc2af6bc6034",
    "id": null,
    "metadata": {},
    "name": "organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation(\n  $input: CancelOrganizationOfferingInput!\n) {\n  cancelOrganizationOffering(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "41d41f0177aaa2958796a68f7944b774";

export default node;
