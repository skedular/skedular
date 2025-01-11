/**
 * @generated SignedSource<<70eb74f953d1c992176d266175707e6f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment$variables = {
  organizationId: string;
};
export type organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationAdmin_organizationPaymentMethodsDetails_query">;
};
export type organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment = {
  response: organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment$data;
  variables: organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAdmin_organizationPaymentMethodsDetails_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "organizationId",
            "variableName": "organizationId"
          }
        ],
        "concreteType": "OrganizationPaymentMethod",
        "kind": "LinkedField",
        "name": "organizationPaymentMethodsDetails",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardBrand",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardExpiryMonth",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardExpiryYear",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardLastFourDigit",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "49c7de3065691bc2011605fb3114af4c",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment(\n  $organizationId: String!\n) {\n  ...organizationAdmin_organizationPaymentMethodsDetails_query\n}\n\nfragment organizationAdmin_organizationPaymentMethodsDetails_query on Query {\n  organizationPaymentMethodsDetails(organizationId: $organizationId) {\n    id\n    cardBrand\n    cardExpiryMonth\n    cardExpiryYear\n    cardLastFourDigit\n  }\n}\n"
  }
};
})();

(node as any).hash = "18910cbd4b5458b8e98595d4d0a5bb33";

export default node;
