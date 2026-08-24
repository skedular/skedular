/**
 * @generated SignedSource<<906b6b8bf17778f9d917b3d983357184>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationSettings_query$data = {
  readonly organization: {
    readonly marketplaceListingMetadata: {
      readonly title: string | null | undefined;
    };
    readonly name: string;
    readonly physicalAddress: {
      readonly formattedAddress: string | null | undefined;
    } | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "organizationSettings_query";
};
export type organizationSettings_query$key = {
  readonly " $data"?: organizationSettings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationSettings_query">;
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
  "name": "organizationSettings_query",
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
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
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
          "concreteType": "OrganizationPhysicalAddressDetails",
          "kind": "LinkedField",
          "name": "physicalAddress",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "formattedAddress",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "ListingMetadata",
          "kind": "LinkedField",
          "name": "marketplaceListingMetadata",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "title",
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

(node as any).hash = "45378908c38f6e370e8f463e39a10c98";

export default node;
