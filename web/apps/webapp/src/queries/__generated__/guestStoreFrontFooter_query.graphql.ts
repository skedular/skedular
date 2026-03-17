/**
 * @generated SignedSource<<76a38eb7d0b2b08bc444d601bf198afc>>
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
      "name": "organizationCustomDomain"
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
          "name": "customDomain",
          "variableName": "organizationCustomDomain"
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

(node as any).hash = "ba39ab472ce587a962228a296cf23757";

export default node;
