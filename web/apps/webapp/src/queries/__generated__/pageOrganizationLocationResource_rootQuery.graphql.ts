/**
 * @generated SignedSource<<a2bd98f8272b7babce32d492b1622105>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagOrderField = "DESCRIPTION" | "NAME" | "TYPE" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type pageOrganizationLocationResource_rootQuery$variables = {
  locationId: string;
  multipleChoicesCustomTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  multipleChoicesProductTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  multipleChoicesZonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationId: string;
  resourceId: string;
};
export type pageOrganizationLocationResource_rootQuery$data = {
  readonly resource: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"editResource_query">;
};
export type pageOrganizationLocationResource_rootQuery = {
  response: pageOrganizationLocationResource_rootQuery$data;
  variables: pageOrganizationLocationResource_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesCustomTagsSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesProductTagsSortingValues"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesZonesSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourceId"
},
v6 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "resourceId"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v10 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v7/*: any*/),
  (v9/*: any*/)
],
v11 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "closed",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "openAllDay",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "from",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "until",
    "storageKey": null
  }
],
v12 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "WeekOpeningHours",
    "kind": "LinkedField",
    "name": "weekOpeningHours",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "monday",
        "plural": false,
        "selections": (v11/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "tuesday",
        "plural": false,
        "selections": (v11/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "wednesday",
        "plural": false,
        "selections": (v11/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "thursday",
        "plural": false,
        "selections": (v11/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "friday",
        "plural": false,
        "selections": (v11/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "saturday",
        "plural": false,
        "selections": (v11/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "sunday",
        "plural": false,
        "selections": (v11/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
  }
],
v13 = {
  "fields": [
    {
      "kind": "Variable",
      "name": "organizationId",
      "variableName": "organizationId"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v14 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesCustomTagsSortingValues"
  },
  (v13/*: any*/)
],
v15 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "totalCount",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "OrganizationTagEdge",
    "kind": "LinkedField",
    "name": "edges",
    "plural": true,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTagDetails",
        "kind": "LinkedField",
        "name": "node",
        "plural": false,
        "selections": [
          (v8/*: any*/),
          (v7/*: any*/),
          (v9/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "__typename",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "cursor",
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "PageInfo",
    "kind": "LinkedField",
    "name": "pageInfo",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "endCursor",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "hasNextPage",
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "kind": "ClientExtension",
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "__id",
        "storageKey": null
      }
    ]
  }
],
v16 = [
  "where",
  "orderBy"
],
v17 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesZonesSortingValues"
  },
  (v13/*: any*/)
],
v18 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesProductTagsSortingValues"
  },
  (v13/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationLocationResource_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "ResourceDetails",
        "kind": "LinkedField",
        "name": "resource",
        "plural": false,
        "selections": [
          (v7/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editResource_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v4/*: any*/),
      (v0/*: any*/),
      (v5/*: any*/),
      (v1/*: any*/),
      (v3/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationLocationResource_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "ResourceDetails",
        "kind": "LinkedField",
        "name": "resource",
        "plural": false,
        "selections": [
          (v7/*: any*/),
          (v8/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "inactive",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "requireBookingApproval",
            "storageKey": null
          },
          (v9/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "capacity",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Location_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "customTags",
            "plural": true,
            "selections": (v10/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Location_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "zones",
            "plural": true,
            "selections": (v10/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Location_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": true,
            "selections": (v10/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Location_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "resourceType",
            "plural": false,
            "selections": (v10/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "isAvailableHoursOverridden",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OpeningHours",
            "kind": "LinkedField",
            "name": "availableHours",
            "plural": false,
            "selections": (v12/*: any*/),
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
            "concreteType": "OrganizationTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "type",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v8/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "resourceTypes",
            "plural": true,
            "selections": [
              (v8/*: any*/),
              (v7/*: any*/),
              (v9/*: any*/)
            ],
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
            "name": "id",
            "variableName": "locationId"
          }
        ],
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OpeningHours",
            "kind": "LinkedField",
            "name": "openingHours",
            "plural": false,
            "selections": (v12/*: any*/),
            "storageKey": null
          },
          (v8/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v14/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "customTags",
        "plural": false,
        "selections": (v15/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v14/*: any*/),
        "filters": (v16/*: any*/),
        "handle": "connection",
        "key": "multipleChoicesCustomTags_customTags",
        "kind": "LinkedHandle",
        "name": "customTags"
      },
      {
        "alias": null,
        "args": (v17/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v15/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v17/*: any*/),
        "filters": (v16/*: any*/),
        "handle": "connection",
        "key": "multipleChoicesZones_zones",
        "kind": "LinkedHandle",
        "name": "zones"
      },
      {
        "alias": null,
        "args": (v18/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "productTags",
        "plural": false,
        "selections": (v15/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v18/*: any*/),
        "filters": (v16/*: any*/),
        "handle": "connection",
        "key": "multipleChoicesProductTags_productTags",
        "kind": "LinkedHandle",
        "name": "productTags"
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "openingHoursMinutesStep",
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "16a2376b6d0c5f83a79aadbb338a8781",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationLocationResource_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationLocationResource_rootQuery(\n  $organizationId: String!\n  $locationId: String!\n  $resourceId: String!\n  $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]\n) {\n  resource(id: $resourceId) {\n    name\n    id\n  }\n  ...editResource_query\n}\n\nfragment editResource_query on Query {\n  organization(id: $organizationId) {\n    type {\n      type\n    }\n    id\n  }\n  location(id: $locationId) {\n    openingHours {\n      weekOpeningHours {\n        monday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        tuesday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        wednesday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        thursday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        friday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        saturday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        sunday {\n          closed\n          openAllDay\n          from\n          until\n        }\n      }\n    }\n    id\n  }\n  resource(id: $resourceId) {\n    id\n    name\n    inactive\n    requireBookingApproval\n    color\n    capacity\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n    productTags {\n      uniqueId\n      name\n      color\n    }\n    resourceType {\n      uniqueId\n      name\n      color\n    }\n    isAvailableHoursOverridden\n    availableHours {\n      weekOpeningHours {\n        monday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        tuesday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        wednesday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        thursday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        friday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        saturday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        sunday {\n          closed\n          openAllDay\n          from\n          until\n        }\n      }\n    }\n  }\n  ...singleChoiceResourceType_query\n  ...multipleChoicesCustomTags_query\n  ...multipleChoicesZones_query\n  ...multipleChoicesProductTags_query\n  ...weekOpeningHours_query\n}\n\nfragment multipleChoicesCustomTags_query on Query {\n  customTags(where: {organizationId: $organizationId}, orderBy: $multipleChoicesCustomTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment multipleChoicesProductTags_query on Query {\n  productTags(where: {organizationId: $organizationId}, orderBy: $multipleChoicesProductTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment multipleChoicesZones_query on Query {\n  zones(where: {organizationId: $organizationId}, orderBy: $multipleChoicesZonesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment singleChoiceResourceType_query on Query {\n  organization(id: $organizationId) {\n    resourceTypes {\n      id\n      name\n      color\n    }\n    id\n  }\n}\n\nfragment weekOpeningHours_query on Query {\n  openingHoursMinutesStep\n}\n"
  }
};
})();

(node as any).hash = "1f324288f8f3ac5b9c103c12c05e726f";

export default node;
