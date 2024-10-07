/**
 * @generated SignedSource<<916692087f9dc9d0e28f491d34e73dd0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationPage_query$data = {
  readonly organization: {
    readonly canModify: boolean;
    readonly canViewAnalytics: boolean;
    readonly id: string;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAboutTab_query" | "organizationBillingTab_query" | "organizationBookingsTab_query" | "organizationLocationsTab_query" | "organizationMultipleChoicesIndustries_query" | "organizationOfferingTab_query" | "organizationPeopleTab_query" | "organizationTeamsTab_query">;
  readonly " $fragmentType": "organizationPage_query";
};
export type organizationPage_query$key = {
  readonly " $data"?: organizationPage_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationPage_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "bookingDetailsSelectorOrganizationMembersSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "bookingPeopleNameSearchText"
    },
    {
      "kind": "RootArgument",
      "name": "bookingSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "bookingsSearchCriteriaFrom"
    },
    {
      "kind": "RootArgument",
      "name": "bookingsSearchCriteriaUntil"
    },
    {
      "kind": "RootArgument",
      "name": "dateToGetAvailableDesks"
    },
    {
      "kind": "RootArgument",
      "name": "deskIdsToIncludeToGetAvailableDesks"
    },
    {
      "kind": "RootArgument",
      "name": "locationExists"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "locationNameSearchText"
    },
    {
      "kind": "RootArgument",
      "name": "organizationExists"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationLocationsSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "organizationPeopleSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "organizationTeamsSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "peopleNameSearchText"
    },
    {
      "kind": "RootArgument",
      "name": "teamNameSearchText"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./organizationPageQuery.graphql')
    }
  },
  "name": "organizationPage_query",
  "selections": [
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
          "name": "logoUrl",
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
          "name": "canViewAnalytics",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationAboutTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationBookingsTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationMultipleChoicesIndustries_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationPeopleTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationLocationsTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationTeamsTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationBillingTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationOfferingTab_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "6918a9cfd17d10963050cddf2a72c76c";

export default node;
