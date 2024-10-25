/**
 * @generated SignedSource<<c785c3d274dc23159dc74d70ab75ad7c>>
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
  "name": "name",
  "storageKey": null
},
v1 = [
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
      "name": "teamId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
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
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        },
        (v0/*: any*/),
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
            (v0/*: any*/)
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
            {
              "alias": null,
              "args": null,
              "concreteType": "TeamCustomerDetails",
              "kind": "LinkedField",
              "name": "customer",
              "plural": false,
              "selections": (v1/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "TeamOrganizationMemberDetails",
              "kind": "LinkedField",
              "name": "organizationMember",
              "plural": false,
              "selections": (v1/*: any*/),
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

(node as any).hash = "475f7b08caef5983648a221913bb7a0a";

export default node;
