/**
 * @generated SignedSource<<9db8dd67a54b984c78205e75c598cc11>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationOfferingPatchField = "OFFERING_CODE" | "%future added value";
export type UpdateOrganizationOfferingInput = {
  clientMutationId?: string | null | undefined;
  fieldsToUpdate: ReadonlyArray<OrganizationOfferingPatchField>;
  offeringCode?: string | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type organizationSettingsSubscriptionsSection_updateOrganizationOfferingMutation$variables = {
  input: UpdateOrganizationOfferingInput;
};
export type organizationSettingsSubscriptionsSection_updateOrganizationOfferingMutation$data = {
  readonly updateOrganizationOffering: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type organizationSettingsSubscriptionsSection_updateOrganizationOfferingMutation = {
  response: organizationSettingsSubscriptionsSection_updateOrganizationOfferingMutation$data;
  variables: organizationSettingsSubscriptionsSection_updateOrganizationOfferingMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationSettingsSubscriptionsSection_updateOrganizationOfferingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsSubscriptionsSection_updateOrganizationOfferingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "215f3f0f1e6c265c66da7ad49060700d",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsSubscriptionsSection_updateOrganizationOfferingMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsSubscriptionsSection_updateOrganizationOfferingMutation(\n  $input: UpdateOrganizationOfferingInput!\n) {\n  updateOrganizationOffering(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "b3065cb4ece196f370edad1dc5d618a1";

export default node;
