/**
 * @generated SignedSource<<24b49bdd28642511e6d15ab871e8e1fc>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type customerCard_CustomerDetails$data = {
  readonly familyName: string | null | undefined;
  readonly givenName: string | null | undefined;
  readonly middleName: string | null | undefined;
  readonly name: string | null | undefined;
  readonly photoUrl: string | null | undefined;
  readonly " $fragmentType": "customerCard_CustomerDetails";
};
export type customerCard_CustomerDetails$key = {
  readonly " $data"?: customerCard_CustomerDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"customerCard_CustomerDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "customerCard_CustomerDetails",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "name",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "givenName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "middleName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "familyName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "photoUrl",
      "storageKey": null
    }
  ],
  "type": "CustomerDetails",
  "abstractKey": null
};

(node as any).hash = "734384ae4c6599ed0693f3f6e661516d";

export default node;
