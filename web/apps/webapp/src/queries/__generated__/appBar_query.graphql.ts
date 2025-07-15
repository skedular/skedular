/**
 * @generated SignedSource<<85650fdd35f5347d30b837c155842586>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type appBar_query$data = {
  readonly me: {
    readonly email: string | null | undefined;
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly id: string;
    readonly middleName: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  };
  readonly myOrganizations: ReadonlyArray<{
    readonly canModify: boolean;
    readonly canViewAnalytics: boolean;
    readonly id: string;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  }>;
  readonly pendingOrganizationInvitationsCount: number;
  readonly pendingTeamInvitationsCount: number;
  readonly " $fragmentSpreads": FragmentRefs<"mobileLeftSideNavigationMenu_query" | "newFeedbackDialog_query">;
  readonly " $fragmentType": "appBar_query";
};
export type appBar_query$key = {
  readonly " $data"?: appBar_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"appBar_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "appBar_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "email",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "givenName",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "middleName",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "familyName",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "photoUrl",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "myOrganizations",
      "plural": true,
      "selections": [
        (v0/*: any*/),
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
          "name": "name",
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
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "pendingOrganizationInvitationsCount",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "pendingTeamInvitationsCount",
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "mobileLeftSideNavigationMenu_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "newFeedbackDialog_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "136591c28d3b19681fbe4409d0fc218d";

export default node;
