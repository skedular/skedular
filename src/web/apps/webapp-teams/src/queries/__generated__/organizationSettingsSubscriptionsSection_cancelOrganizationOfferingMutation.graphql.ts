/**
 * @generated SignedSource<<3c43061284ad56ee14326abcf72a0741>>
 * @lightSyntaxTransform
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
export type organizationSettingsSubscriptionsSection_cancelOrganizationOfferingMutation$variables = {
  input: CancelOrganizationOfferingInput;
};
export type organizationSettingsSubscriptionsSection_cancelOrganizationOfferingMutation$data = {
  readonly cancelOrganizationOffering: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type organizationSettingsSubscriptionsSection_cancelOrganizationOfferingMutation = {
  response: organizationSettingsSubscriptionsSection_cancelOrganizationOfferingMutation$data;
  variables: organizationSettingsSubscriptionsSection_cancelOrganizationOfferingMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationSettingsSubscriptionsSection_cancelOrganizationOfferingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsSubscriptionsSection_cancelOrganizationOfferingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "8d746b2f61bf4a20608fe964fc23e2ab",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsSubscriptionsSection_cancelOrganizationOfferingMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsSubscriptionsSection_cancelOrganizationOfferingMutation(\n  $input: CancelOrganizationOfferingInput!\n) {\n  cancelOrganizationOffering(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "69e0cc3fc6bee97b026f6ee39c81e65d";

export default node;
