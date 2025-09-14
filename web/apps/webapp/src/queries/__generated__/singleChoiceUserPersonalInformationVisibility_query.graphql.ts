/**
 * @generated SignedSource<<bb5a2802e36d44dcd44c4abb489ef638>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PersonalInformationVisibility = "REDACTED" | "VISIBLE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceUserPersonalInformationVisibility_query$data = {
  readonly personalInformationVisibilityTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: PersonalInformationVisibility;
  }>;
  readonly " $fragmentType": "singleChoiceUserPersonalInformationVisibility_query";
};
export type singleChoiceUserPersonalInformationVisibility_query$key = {
  readonly " $data"?: singleChoiceUserPersonalInformationVisibility_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceUserPersonalInformationVisibility_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceUserPersonalInformationVisibility_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "PersonalInformationVisibilityDetails",
      "kind": "LinkedField",
      "name": "personalInformationVisibilityTypes",
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

(node as any).hash = "2958b974bce8a8ab42abe60db346f064";

export default node;
