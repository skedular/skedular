/**
 * @generated SignedSource<<00725c2eae44d11221f58645f94186c2>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type ProductType = "EVENT" | "RESOURCE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceProductType_query$data = {
  readonly productTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductType;
  }>;
  readonly " $fragmentType": "singleChoiceProductType_query";
};
export type singleChoiceProductType_query$key = {
  readonly " $data"?: singleChoiceProductType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceProductType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceProductType_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ProductTypeDetails",
      "kind": "LinkedField",
      "name": "productTypes",
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

(node as any).hash = "a14efc97f3e4507cf8d75624a1443543";

export default node;
