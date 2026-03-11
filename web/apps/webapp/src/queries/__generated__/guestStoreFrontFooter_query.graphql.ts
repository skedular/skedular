/**
 * @generated SignedSource<<fc9de336d037ab72cb5c5bb81f38303b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontFooter_query$data = {
  readonly organizationPublic: {
    readonly contactEmail: string | null | undefined;
    readonly contactPhone: string | null | undefined;
    readonly name: string;
    readonly physicalAddress: {
      readonly addressLine1: string;
      readonly addressLine2: string | null | undefined;
      readonly city: string | null | undefined;
      readonly country: string;
      readonly province: string | null | undefined;
      readonly suburb: string | null | undefined;
      readonly zipcode: string;
    } | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "guestStoreFrontFooter_query";
};
export type guestStoreFrontFooter_query$key = {
  readonly " $data"?: guestStoreFrontFooter_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontFooter_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "guestStoreFrontFooter_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "uniqueAlphanumericName",
          "variableName": "organizationUniqueAlphanumericName"
        }
      ],
      "concreteType": "OrganizationPublicDetails",
      "kind": "LinkedField",
      "name": "organizationPublic",
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
          "name": "contactPhone",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "contactEmail",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationPhysicalAddressDetails",
          "kind": "LinkedField",
          "name": "physicalAddress",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "addressLine1",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "addressLine2",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "suburb",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "city",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "province",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "zipcode",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "country",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "962ea81fed54722a54f0c7d20f725e82";

export default node;
