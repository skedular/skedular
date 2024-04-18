/**
 * @generated SignedSource<<d146f08cca61f18c800f3aa123d00b4b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment, RefetchableFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationAnalyticsTab_query$data = {
  readonly organizationAnalytics: {
    readonly dailyBookingsTotals: ReadonlyArray<{
      readonly date: any;
      readonly total: number;
    }>;
    readonly memberAttendancePercentage: ReadonlyArray<{
      readonly date: any;
      readonly percentage: number;
    }>;
  };
  readonly " $fragmentType": "organizationAnalyticsTab_query";
};
export type organizationAnalyticsTab_query$key = {
  readonly " $data"?: organizationAnalyticsTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAnalyticsTab_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "date",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationAnalyticsFrom"
    },
    {
      "kind": "RootArgument",
      "name": "organizationAnalyticsUntil"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./organizationAnalyticsPaginationQuery.graphql')
    }
  },
  "name": "organizationAnalyticsTab_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "from",
          "variableName": "organizationAnalyticsFrom"
        },
        {
          "kind": "Variable",
          "name": "organizationId",
          "variableName": "organizationId"
        },
        {
          "kind": "Variable",
          "name": "until",
          "variableName": "organizationAnalyticsUntil"
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
          "concreteType": "OrganizationMemberAttendancePercentage",
          "kind": "LinkedField",
          "name": "memberAttendancePercentage",
          "plural": true,
          "selections": [
            (v0/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "percentage",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationDailyBookingsTotal",
          "kind": "LinkedField",
          "name": "dailyBookingsTotals",
          "plural": true,
          "selections": [
            (v0/*: any*/),
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
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "e3be4c7a5acfb28d20d245e6f248fa49";

export default node;
