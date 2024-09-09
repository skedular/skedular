/**
 * @generated SignedSource<<5ac6a5e1486ffe189097a4345b1937b8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationPaymentMethods_query$data = {
  readonly organization: {
    readonly id: string;
  } | null | undefined;
  readonly organizationPaymentMethodsDetails: ReadonlyArray<{
    readonly cardBrand: string | null | undefined;
    readonly cardExpiryMonth: number | null | undefined;
    readonly cardExpiryYear: number | null | undefined;
    readonly cardLastFourDigit: string | null | undefined;
    readonly id: string;
  }>;
  readonly " $fragmentType": "organizationPaymentMethods_query";
};
export type organizationPaymentMethods_query$key = {
  readonly " $data"?: organizationPaymentMethods_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationPaymentMethods_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationPaymentMethods_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        (v0/*: any*/)
      ],
      "storageKey": null
    },
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
        (v0/*: any*/),
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
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "07ee450d9b1de5f937e01d2d6d832387";

export default node;
