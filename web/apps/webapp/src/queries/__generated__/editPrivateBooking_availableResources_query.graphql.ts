/**
 * @generated SignedSource<<c0a7a8acbc43d8790546789bc89ecb29>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editPrivateBooking_availableResources_query$data = {
  readonly availableResources: ReadonlyArray<{
    readonly resource: {
      readonly customTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly id: string;
      readonly name: string;
      readonly zones: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
    };
  }>;
  readonly " $fragmentType": "editPrivateBooking_availableResources_query";
};
export type editPrivateBooking_availableResources_query$key = {
  readonly " $data"?: editPrivateBooking_availableResources_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editPrivateBooking_availableResources_query">;
};

import editPrivateBooking_availableResources_refetchableFragment_graphql from './editPrivateBooking_availableResources_refetchableFragment.graphql';

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
  (v0/*: any*/),
  (v1/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "dateFromToGetAvailableResources"
    },
    {
      "kind": "RootArgument",
      "name": "dateUntilToGetAvailableResources"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": editPrivateBooking_availableResources_refetchableFragment_graphql
    }
  },
  "name": "editPrivateBooking_availableResources_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "from",
              "variableName": "dateFromToGetAvailableResources"
            },
            {
              "kind": "Variable",
              "name": "locationId",
              "variableName": "locationId"
            },
            {
              "kind": "Variable",
              "name": "organizationUniqueAlphanumericName",
              "variableName": "organizationUniqueAlphanumericName"
            },
            {
              "kind": "Variable",
              "name": "until",
              "variableName": "dateUntilToGetAvailableResources"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "BookingResourceDetails",
      "kind": "LinkedField",
      "name": "availableResources",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "ResourceDetails",
          "kind": "LinkedField",
          "name": "resource",
          "plural": false,
          "selections": [
            (v0/*: any*/),
            (v1/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "customTags",
              "plural": true,
              "selections": (v2/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "zones",
              "plural": true,
              "selections": (v2/*: any*/),
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

(node as any).hash = "5f3a5084a760eaf687912332e907fb9d";

export default node;
