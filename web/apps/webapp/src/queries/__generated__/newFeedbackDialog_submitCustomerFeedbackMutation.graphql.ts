/**
 * @generated SignedSource<<d3c582bff47a5d01201126381a743486>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type SubmitCustomerFeedbackInput = {
  clientMutationId?: string | null | undefined;
  feedbackContent: string;
  id: string;
};
export type newFeedbackDialog_submitCustomerFeedbackMutation$variables = {
  input: SubmitCustomerFeedbackInput;
};
export type newFeedbackDialog_submitCustomerFeedbackMutation$data = {
  readonly submitCustomerFeedback: {
    readonly id: string;
  } | null | undefined;
};
export type newFeedbackDialog_submitCustomerFeedbackMutation = {
  response: newFeedbackDialog_submitCustomerFeedbackMutation$data;
  variables: newFeedbackDialog_submitCustomerFeedbackMutation$variables;
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
    "concreteType": "SubmitCustomerFeedbackPayload",
    "kind": "LinkedField",
    "name": "submitCustomerFeedback",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "newFeedbackDialog_submitCustomerFeedbackMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "newFeedbackDialog_submitCustomerFeedbackMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "ca4c346d6838c0985a1a479ed022d377",
    "id": null,
    "metadata": {},
    "name": "newFeedbackDialog_submitCustomerFeedbackMutation",
    "operationKind": "mutation",
    "text": "mutation newFeedbackDialog_submitCustomerFeedbackMutation(\n  $input: SubmitCustomerFeedbackInput!\n) {\n  submitCustomerFeedback(input: $input) {\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "820940254740f1f536c18002decbed21";

export default node;
