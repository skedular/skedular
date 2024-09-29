/**
 * @generated SignedSource<<b8a41e23145e79c464b76ff2f9d6f105>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationBookingInsight_query$data = {
  readonly organization: {
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
  readonly organizationAnalytics: {
    readonly dailyBookingsTotals: ReadonlyArray<{
      readonly date: any;
      readonly total: number;
    }>;
  };
  readonly " $fragmentType": "organizationBookingInsight_query";
};
export type organizationBookingInsight_query$key = {
  readonly " $data"?: organizationBookingInsight_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationBookingInsight_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "from"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
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
      "operation": require('./organizationBookingInsight_organizationAnalytics.graphql')
    }
  },
  "name": "organizationBookingInsight_query",
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
          "name": "organizationId",
          "variableName": "organizationId"
        },
        {
          "kind": "Variable",
          "name": "until",
          "variableName": "to"
        }
      ],
      "concreteType": "OrganizationAnalytics",
      "kind": "LinkedField",
      "name": "organizationAnalytics",
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
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "organizationId"
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
          "kind": "ScalarField",
          "name": "logoUrl",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "94b88194e3b5185712c1ea22a327415c";

export default node;
