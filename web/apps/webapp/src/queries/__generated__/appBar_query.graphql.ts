/**
 * @generated SignedSource<<389cea51639060f0b25092b80df463a8>>
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
    readonly email: {
      readonly email: string;
      readonly verified: boolean;
    } | null | undefined;
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly middleName: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  } | null | undefined;
  readonly myOrganizations: ReadonlyArray<{
    readonly canModify: boolean;
    readonly canViewAnalytics: boolean;
    readonly id: string;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  }> | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"newFeedbackDialog_query">;
  readonly " $fragmentType": "appBar_query";
};
export type appBar_query$key = {
  readonly " $data"?: appBar_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"appBar_query">;
};

const node: ReaderFragment = {
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
        {
          "alias": null,
          "args": null,
          "concreteType": "CustomerEmail",
          "kind": "LinkedField",
          "name": "email",
          "plural": false,
          "selections": [
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
              "name": "verified",
              "storageKey": null
            }
          ],
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
      "args": null,
      "kind": "FragmentSpread",
      "name": "newFeedbackDialog_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "336d127ec0f49242c88c8103a5ce3a5a";

export default node;
