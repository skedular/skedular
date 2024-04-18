/**
 * @generated SignedSource<<8e70064d29fa3e6c3c027c2de5d0c399>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type CancelOrganizationOfferingInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationOffering_cancelOrganizationOfferingMutation$variables = {
  input: CancelOrganizationOfferingInput;
};
export type organizationOffering_cancelOrganizationOfferingMutation$data = {
  readonly cancelOrganizationOffering: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type organizationOffering_cancelOrganizationOfferingMutation = {
  response: organizationOffering_cancelOrganizationOfferingMutation$data;
  variables: organizationOffering_cancelOrganizationOfferingMutation$variables;
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
    "name": "organizationOffering_cancelOrganizationOfferingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationOffering_cancelOrganizationOfferingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "7b578294c035700cdcd961aa46777244",
    "id": null,
    "metadata": {},
    "name": "organizationOffering_cancelOrganizationOfferingMutation",
    "operationKind": "mutation",
    "text": "mutation organizationOffering_cancelOrganizationOfferingMutation(\n  $input: CancelOrganizationOfferingInput!\n) {\n  cancelOrganizationOffering(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "49e29a93a42bb253291900ee582ac0c9";

export default node;
