/**
 * @generated SignedSource<<a84ebe9332f42d5bf236f698a81021ec>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type LocationMemberRole = "Administrator" | "Member" | "Owner" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type locationMemberCard_LocationMemberDetails$data = {
  readonly customer: {
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  };
  readonly id: string;
  readonly role: LocationMemberRole | null | undefined;
  readonly " $fragmentType": "locationMemberCard_LocationMemberDetails";
};
export type locationMemberCard_LocationMemberDetails$key = {
  readonly " $data"?: locationMemberCard_LocationMemberDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationMemberCard_LocationMemberDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationMemberCard_LocationMemberDetails",
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
      "name": "role",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "LocationCustomerDetails",
      "kind": "LinkedField",
      "name": "customer",
      "plural": false,
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
      "storageKey": null
    }
  ],
  "type": "LocationMemberDetails",
  "abstractKey": null
};

(node as any).hash = "8be93580e4952197ccadba67b0546df3";

export default node;
