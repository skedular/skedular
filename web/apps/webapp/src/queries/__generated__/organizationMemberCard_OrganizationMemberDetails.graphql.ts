/**
 * @generated SignedSource<<c2e041bf2d7a4c08992a26cc4ace9b55>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationMemberMembershipType = "Administrator" | "Member" | "Owner" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationMemberCard_OrganizationMemberDetails$data = {
  readonly customer: {
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  };
  readonly id: string;
  readonly membershipType: OrganizationMemberMembershipType | null | undefined;
  readonly " $fragmentType": "organizationMemberCard_OrganizationMemberDetails";
};
export type organizationMemberCard_OrganizationMemberDetails$key = {
  readonly " $data"?: organizationMemberCard_OrganizationMemberDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMemberCard_OrganizationMemberDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationMemberCard_OrganizationMemberDetails",
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
      "name": "membershipType",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationCustomerDetails",
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
  "type": "OrganizationMemberDetails",
  "abstractKey": null
};

(node as any).hash = "75b5f92169923a1f7a1fd4165c3a996f";

export default node;
