/**
 * @generated SignedSource<<fcdc80f08a48a49c6bbf28ec75447ade>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationOfferingTab_query$data = {
  readonly organization: {
    readonly availableOfferings: ReadonlyArray<{
      readonly code: string;
    }>;
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAvailableOfferings_query" | "organizationOffering_query">;
  readonly " $fragmentType": "organizationOfferingTab_query";
};
export type organizationOfferingTab_query$key = {
  readonly " $data"?: organizationOfferingTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationOfferingTab_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationOfferingTab_query",
  "selections": [
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationOffering_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationAvailableOfferings_query"
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
          "name": "id",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationAvailableOfferingDetails",
          "kind": "LinkedField",
          "name": "availableOfferings",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "code",
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

(node as any).hash = "38d89ab175574c4b5b3dcb8f95368472";

export default node;
