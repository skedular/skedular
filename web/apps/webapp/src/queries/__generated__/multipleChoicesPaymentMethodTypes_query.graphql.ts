/**
 * @generated SignedSource<<584b74574d7fcfbf91aeed8005a1141b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type multipleChoicesPaymentMethodTypes_query$data = {
  readonly paymentMethodTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: PaymentMethod;
  }>;
  readonly " $fragmentType": "multipleChoicesPaymentMethodTypes_query";
};
export type multipleChoicesPaymentMethodTypes_query$key = {
  readonly " $data"?: multipleChoicesPaymentMethodTypes_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesPaymentMethodTypes_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "multipleChoicesPaymentMethodTypes_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "PaymentMethodTypeDetails",
      "kind": "LinkedField",
      "name": "paymentMethodTypes",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "name",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "fc35414006513b71925ab303d1f93dd5";

export default node;
