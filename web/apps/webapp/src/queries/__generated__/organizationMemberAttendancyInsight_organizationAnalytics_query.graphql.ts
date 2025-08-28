/**
 * @generated SignedSource<<0daa7ca4bc528ab490dbb87fed781475>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMemberAttendancyInsight_organizationAnalytics_query$data = {
  readonly organizationAnalytics: {
    readonly memberAttendancePercentage: ReadonlyArray<{
      readonly date: any;
      readonly percentage: number;
    }>;
  } | null | undefined;
  readonly " $fragmentType": "organizationMemberAttendancyInsight_organizationAnalytics_query";
};
export type organizationMemberAttendancyInsight_organizationAnalytics_query$key = {
  readonly " $data"?: organizationMemberAttendancyInsight_organizationAnalytics_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMemberAttendancyInsight_organizationAnalytics_query">;
};

import organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment_graphql from './organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment.graphql';

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "from"
    },
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
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
      "operation": organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment_graphql
    }
  },
  "name": "organizationMemberAttendancyInsight_organizationAnalytics_query",
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
          "name": "uniqueAlphanumericName",
          "variableName": "organizationUniqueAlphanumericName"
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
          "concreteType": "OrganizationMemberAttendancePercentage",
          "kind": "LinkedField",
          "name": "memberAttendancePercentage",
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
              "name": "percentage",
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

(node as any).hash = "9c5f3bbefaa574927946ad36a42a2213";

export default node;
