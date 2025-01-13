/**
 * @generated SignedSource<<a5d7bf937a9d8c6430a155e0bf14c464>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CancelOrganizationOfferingInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationAdmin_cancelOrganizationOfferingMutation$variables = {
  input: CancelOrganizationOfferingInput;
};
export type organizationAdmin_cancelOrganizationOfferingMutation$data = {
  readonly cancelOrganizationOffering: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type organizationAdmin_cancelOrganizationOfferingMutation = {
  response: organizationAdmin_cancelOrganizationOfferingMutation$data;
  variables: organizationAdmin_cancelOrganizationOfferingMutation$variables;
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
    "name": "organizationAdmin_cancelOrganizationOfferingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_cancelOrganizationOfferingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "f66b42f8409220bb4159f2dbbe2af878",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_cancelOrganizationOfferingMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_cancelOrganizationOfferingMutation(\n  $input: CancelOrganizationOfferingInput!\n) {\n  cancelOrganizationOffering(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "e1db40ed318dab768198ab07ef4821e8";

export default node;
