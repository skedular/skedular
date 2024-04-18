/**
 * @generated SignedSource<<14bc0320727674fbd4f41f93e89b8d82>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationCard_OrganizationDetails$data = {
  readonly about: string | null | undefined;
  readonly canDelete: boolean;
  readonly canModify: boolean;
  readonly hasFutureBooking: boolean;
  readonly hasLocation: boolean;
  readonly id: string;
  readonly logoUrl: string | null | undefined;
  readonly name: string;
  readonly website: string | null | undefined;
  readonly " $fragmentType": "organizationCard_OrganizationDetails";
};
export type organizationCard_OrganizationDetails$key = {
  readonly " $data"?: organizationCard_OrganizationDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationCard_OrganizationDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationCard_OrganizationDetails",
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
      "name": "name",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "about",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "website",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "logoUrl",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "hasFutureBooking",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "hasLocation",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canModify",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canDelete",
      "storageKey": null
    }
  ],
  "type": "OrganizationDetails",
  "abstractKey": null
};

(node as any).hash = "945fab42db4a81fb5f2f0d1b15d1880c";

export default node;
