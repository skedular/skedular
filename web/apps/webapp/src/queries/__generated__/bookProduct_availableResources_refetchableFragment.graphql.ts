/**
 * @generated SignedSource<<8cffe6fb61f5fead36187b57f653a65a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookProduct_availableResources_refetchableFragment$variables = {
  dateFromToGetAvailableResources: any;
  dateUntilToGetAvailableResources: any;
  organizationId: string;
  productId?: string | null | undefined;
};
export type bookProduct_availableResources_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"bookProduct_availableResources_query">;
};
export type bookProduct_availableResources_refetchableFragment = {
  response: bookProduct_availableResources_refetchableFragment$data;
  variables: bookProduct_availableResources_refetchableFragment$variables;
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
    "name": "organizationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "productId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
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
    "name": "bookProduct_availableResources_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "bookProduct_availableResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookProduct_availableResources_refetchableFragment",
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
                "name": "organizationId",
                "variableName": "organizationId"
              },
              {
                "kind": "Variable",
                "name": "productId",
                "variableName": "productId"
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
          (v1/*: any*/),
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": [
              (v1/*: any*/),
              (v2/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_OrganizationCustomTagDetails",
            "kind": "LinkedField",
            "name": "customTags",
            "plural": true,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_OrganizationZoneDetails",
            "kind": "LinkedField",
            "name": "zones",
            "plural": true,
            "selections": (v3/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "aa453334c0f7727f4b3d45bbb1057de7",
    "id": null,
    "metadata": {},
    "name": "bookProduct_availableResources_refetchableFragment",
    "operationKind": "query",
    "text": "query bookProduct_availableResources_refetchableFragment(\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $organizationId: String!\n  $productId: String\n) {\n  ...bookProduct_availableResources_query\n}\n\nfragment bookProduct_availableResources_query on Query {\n  availableResources(where: {organizationId: $organizationId, productId: $productId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    uniqueId\n    name\n    location {\n      uniqueId\n      name\n    }\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9e2ab79dcce0b8fdaa8b8f183f8ff59b";

export default node;
