/**
 * @generated SignedSource<<f611154e48b33dae9d43e8a378c6ef46>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamMembersTab_query$data = {
  readonly team: {
    readonly about: string | null | undefined;
    readonly canModify: boolean;
    readonly id: string;
    readonly members: ReadonlyArray<{
      readonly customer: {
        readonly uniqueId: string;
      };
      readonly id: string;
      readonly organizationMember: {
        readonly uniqueId: string;
      } | null | undefined;
    }>;
    readonly name: string;
    readonly organization: {
      readonly name: string;
    } | null | undefined;
    readonly timezone: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMemberSelector_query" | "teamMemberCard_query">;
  readonly " $fragmentType": "teamMembersTab_query";
};
export type teamMembersTab_query$key = {
  readonly " $data"?: teamMembersTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamMembersTab_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "bookingPeopleNameSearchText"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationMemberSelectorOrganizationMembersSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "teamId"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./teamMembersTab_refetchableFragment.graphql')
    }
  },
  "name": "teamMembersTab_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "teamId"
        }
      ],
      "concreteType": "TeamDetails",
      "kind": "LinkedField",
      "name": "team",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "about",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "timezone",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "TeamOrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
          "plural": false,
          "selections": [
            (v1/*: any*/)
          ],
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
          "concreteType": "TeamMemberDetails",
          "kind": "LinkedField",
          "name": "members",
          "plural": true,
          "selections": [
            (v0/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "TeamCustomerDetails",
              "kind": "LinkedField",
              "name": "customer",
              "plural": false,
              "selections": (v2/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "TeamOrganizationMemberDetails",
              "kind": "LinkedField",
              "name": "organizationMember",
              "plural": false,
              "selections": (v2/*: any*/),
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "teamMemberCard_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationMemberSelector_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "5cc74900fa9bfa3d9c61c278d6b7bb9c";

export default node;
