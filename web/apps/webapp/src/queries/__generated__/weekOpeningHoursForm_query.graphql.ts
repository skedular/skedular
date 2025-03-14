/**
 * @generated SignedSource<<37b3a406488f2ab879e997d5815a50b8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type weekOpeningHoursForm_query$data = {
  readonly openingHoursMinutesStep: number;
  readonly " $fragmentType": "weekOpeningHoursForm_query";
};
export type weekOpeningHoursForm_query$key = {
  readonly " $data"?: weekOpeningHoursForm_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"weekOpeningHoursForm_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "weekOpeningHoursForm_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "openingHoursMinutesStep",
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b3328dd902243136cb2bafaeab117922";

export default node;
