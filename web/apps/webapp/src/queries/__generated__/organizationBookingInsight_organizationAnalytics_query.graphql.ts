/**
 * @generated SignedSource<<f82d3720024b2918b2df6b66b4a4f075>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationBookingInsight_organizationAnalytics_query$data = {
  readonly organization: {
    readonly analytics: {
      readonly dailyBookingsTotals: ReadonlyArray<{
        readonly date: any;
        readonly total: number;
      }>;
    };
  } | null | undefined;
  readonly " $fragmentType": "organizationBookingInsight_organizationAnalytics_query";
};
export type organizationBookingInsight_organizationAnalytics_query$key = {
  readonly " $data"?: organizationBookingInsight_organizationAnalytics_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationBookingInsight_organizationAnalytics_query">;
};

import organizationBookingInsight_organizationAnalytics_refetchableFragment_graphql from './organizationBookingInsight_organizationAnalytics_refetchableFragment.graphql';

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "from"
    },
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    },
    {
      "kind": "RootArgument",
      "name": "to"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": organizationBookingInsight_organizationAnalytics_refetchableFragment_graphql
    }
  },
  "name": "organizationBookingInsight_organizationAnalytics_query",
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
          "args": [
            {
              "kind": "Variable",
              "name": "from",
              "variableName": "from"
            },
            {
              "kind": "Variable",
              "name": "until",
              "variableName": "to"
            }
          ],
          "concreteType": "OrganizationAnalytics",
          "kind": "LinkedField",
          "name": "analytics",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationDailyBookingsTotal",
              "kind": "LinkedField",
              "name": "dailyBookingsTotals",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "date",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "total",
                  "storageKey": null
                }
              ],
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

(node as any).hash = "f2a3fe1aea82220d5dcdc2ed44dad610";

export default node;
