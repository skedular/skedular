/**
 * @generated SignedSource<<6cae3147ae2ba871e0978c1e5c9bac8f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type newBookingDialog_query$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly organizationBookingPermissions: {
    readonly canAddBookingOnBehalf: boolean;
  };
  readonly " $fragmentSpreads": FragmentRefs<"bookingDetailsSelector_query">;
  readonly " $fragmentType": "newBookingDialog_query";
};
export type newBookingDialog_query$key = {
  readonly " $data"?: newBookingDialog_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"newBookingDialog_query">;
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
  "name": "newBookingDialog_query",
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
          "kind": "ScalarField",
          "name": "id",
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
          "name": "organizationId",
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationBookingPermissions",
      "kind": "LinkedField",
      "name": "organizationBookingPermissions",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canAddBookingOnBehalf",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingDetailsSelector_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "a75782e50fecb33b803ddb1777425da1";

export default node;
