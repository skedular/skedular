/**
 * @generated SignedSource<<f35e2c9e5aa4997f3f0ce23212c9403b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type newBookingDialog_availableResources_refetchableFragment$variables = {
  dateFromToGetAvailableResources: any;
  dateUntilToGetAvailableResources: any;
  locationId?: string | null | undefined;
  organizationUniqueAlphanumericName?: string | null | undefined;
};
export type newBookingDialog_availableResources_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"newBookingDialog_availableResources_query">;
};
export type newBookingDialog_availableResources_refetchableFragment = {
  response: newBookingDialog_availableResources_refetchableFragment$data;
  variables: newBookingDialog_availableResources_refetchableFragment$variables;
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
    "name": "newBookingDialog_availableResources_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newBookingDialog_availableResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "newBookingDialog_availableResources_refetchableFragment",
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
    "cacheID": "95ee821e099419972fa0eddc6afd61e2",
    "id": null,
    "metadata": {},
    "name": "newBookingDialog_availableResources_refetchableFragment",
    "operationKind": "query",
    "text": "query newBookingDialog_availableResources_refetchableFragment(\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $locationId: String\n  $organizationUniqueAlphanumericName: String\n) {\n  ...newBookingDialog_availableResources_query\n}\n\nfragment newBookingDialog_availableResources_query on Query {\n  availableResources(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    resource {\n      id\n      name\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2524a5b130370fe41a0798bd1d9a5d5c";

export default node;
