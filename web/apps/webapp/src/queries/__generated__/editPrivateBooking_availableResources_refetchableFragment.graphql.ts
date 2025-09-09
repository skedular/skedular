/**
 * @generated SignedSource<<c7ccd159909541b021df3ca83f274134>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editPrivateBooking_availableResources_refetchableFragment$variables = {
  dateFromToGetAvailableResources: any;
  dateUntilToGetAvailableResources: any;
  locationId?: string | null | undefined;
  organizationUniqueAlphanumericName?: string | null | undefined;
};
export type editPrivateBooking_availableResources_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"editPrivateBooking_availableResources_query">;
};
export type editPrivateBooking_availableResources_refetchableFragment = {
  response: editPrivateBooking_availableResources_refetchableFragment$data;
  variables: editPrivateBooking_availableResources_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "dateFromToGetAvailableResources"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "dateUntilToGetAvailableResources"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationUniqueAlphanumericName"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  (v1/*: any*/),
  (v2/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editPrivateBooking_availableResources_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateBooking_availableResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editPrivateBooking_availableResources_refetchableFragment",
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
              (v1/*: any*/),
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagDetails",
                "kind": "LinkedField",
                "name": "customTags",
                "plural": true,
                "selections": (v3/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagDetails",
                "kind": "LinkedField",
                "name": "zones",
                "plural": true,
                "selections": (v3/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "628baee3f09997fa8a140071ffdbb9b9",
    "id": null,
    "metadata": {},
    "name": "editPrivateBooking_availableResources_refetchableFragment",
    "operationKind": "query",
    "text": "query editPrivateBooking_availableResources_refetchableFragment(\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $locationId: String\n  $organizationUniqueAlphanumericName: String\n) {\n  ...editPrivateBooking_availableResources_query\n}\n\nfragment editPrivateBooking_availableResources_query on Query {\n  availableResources(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    resource {\n      id\n      name\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5f3a5084a760eaf687912332e907fb9d";

export default node;
