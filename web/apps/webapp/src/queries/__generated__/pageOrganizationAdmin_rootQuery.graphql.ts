/**
 * @generated SignedSource<<12c845a99ab3fad93e1cc31ec903132e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationAdmin_rootQuery$variables = {
  customTagNameSearchText?: string | null | undefined;
  organizationId: string;
  zoneNameSearchText?: string | null | undefined;
};
export type pageOrganizationAdmin_rootQuery$data = {
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAdmin_customTags_query" | "organizationAdmin_organizationPaymentMethodsDetails_query" | "organizationAdmin_query" | "organizationAdmin_zones_query">;
};
export type pageOrganizationAdmin_rootQuery = {
  response: pageOrganizationAdmin_rootQuery$data;
  variables: pageOrganizationAdmin_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagNameSearchText"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneNameSearchText"
},
v3 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationId"
  }
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v6 = [
  (v5/*: any*/),
  (v4/*: any*/)
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "unitPrice",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "description",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationFeatureSetDetails",
  "kind": "LinkedField",
  "name": "featureSet",
  "plural": true,
  "selections": [
    (v4/*: any*/),
    (v8/*: any*/)
  ],
  "storageKey": null
},
v10 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v11 = [
  (v10/*: any*/)
],
v12 = [
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "zoneNameSearchText"
      },
      (v10/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v13 = [
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
          (v5/*: any*/),
          (v4/*: any*/),
          (v8/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "color",
            "storageKey": null
          },
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
v14 = [
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "Ascending",
        "field": "Name"
      }
    ]
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "customTagNameSearchText"
      },
      (v10/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationAdmin_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v4/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAdmin_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAdmin_organizationPaymentMethodsDetails_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAdmin_zones_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAdmin_customTags_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v2/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationAdmin_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v4/*: any*/),
          (v5/*: any*/),
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
            "name": "about",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "website",
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
            "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
            "kind": "LinkedField",
            "name": "industrySubCategories",
            "plural": true,
            "selections": (v6/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "hasAttachedPaymentMethod",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationOfferingDetails",
            "kind": "LinkedField",
            "name": "offering",
            "plural": false,
            "selections": [
              (v5/*: any*/),
              (v4/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "start",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "end",
                "storageKey": null
              },
              (v7/*: any*/),
              (v9/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "free",
                "storageKey": null
              }
            ],
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
              },
              (v4/*: any*/),
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
        "args": null,
        "concreteType": "OrganizationIndustryMainCategoryReferenceDetails",
        "kind": "LinkedField",
        "name": "organizationIndustryMainCategoriesReferences",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
            "kind": "LinkedField",
            "name": "subCategories",
            "plural": true,
            "selections": (v6/*: any*/),
            "storageKey": null
          },
          (v5/*: any*/),
          (v4/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v11/*: any*/),
        "concreteType": "OrganizationBillingInfo",
        "kind": "LinkedField",
        "name": "organizationBillingInfo",
        "plural": false,
        "selections": [
          (v5/*: any*/),
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
            "name": "addressLine1",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "addressLine2",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "suburb",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "city",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "province",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "zipcode",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "country",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v11/*: any*/),
        "concreteType": "OrganizationPaymentMethod",
        "kind": "LinkedField",
        "name": "organizationPaymentMethodsDetails",
        "plural": true,
        "selections": [
          (v5/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardBrand",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardExpiryMonth",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardExpiryYear",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardLastFourDigit",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v12/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v13/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v12/*: any*/),
        "filters": [
          "where"
        ],
        "handle": "connection",
        "key": "organizationAdmin_zones",
        "kind": "LinkedHandle",
        "name": "zones"
      },
      {
        "alias": null,
        "args": (v14/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "customTags",
        "plural": false,
        "selections": (v13/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v14/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationAdmin_customTags",
        "kind": "LinkedHandle",
        "name": "customTags"
      }
    ]
  },
  "params": {
    "cacheID": "5ac6b7da3b7ce0864c5dd5a8ed60c51e",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationAdmin_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationAdmin_rootQuery(\n  $organizationId: String!\n  $zoneNameSearchText: String\n  $customTagNameSearchText: String\n) {\n  organization(id: $organizationId) {\n    name\n    id\n  }\n  ...organizationAdmin_query\n  ...organizationAdmin_organizationPaymentMethodsDetails_query\n  ...organizationAdmin_zones_query\n  ...organizationAdmin_customTags_query\n}\n\nfragment organizationAdmin_customTags_query on Query {\n  customTags(where: {organizationId: $organizationId, nameContains: $customTagNameSearchText}, orderBy: [{direction: Ascending, field: Name}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationAdmin_organizationPaymentMethodsDetails_query on Query {\n  organizationPaymentMethodsDetails(organizationId: $organizationId) {\n    id\n    cardBrand\n    cardExpiryMonth\n    cardExpiryYear\n    cardLastFourDigit\n  }\n}\n\nfragment organizationAdmin_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    logoUrl\n    about\n    website\n    canModify\n    industrySubCategories {\n      id\n      name\n    }\n    hasAttachedPaymentMethod\n    offering {\n      id\n      name\n      start\n      end\n      unitPrice\n      featureSet {\n        name\n        description\n      }\n      free\n    }\n    availableOfferings {\n      code\n      name\n      unitPrice\n      featureSet {\n        name\n        description\n      }\n    }\n  }\n  organizationIndustryMainCategoriesReferences {\n    subCategories {\n      id\n      name\n    }\n    id\n  }\n  organizationBillingInfo(organizationId: $organizationId) {\n    id\n    email\n    addressLine1\n    addressLine2\n    suburb\n    city\n    province\n    zipcode\n    country\n  }\n  ...organizationMultipleChoicesIndustries_query\n}\n\nfragment organizationAdmin_zones_query on Query {\n  zones(where: {organizationId: $organizationId, nameContains: $zoneNameSearchText}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMultipleChoicesIndustries_query on Query {\n  organizationIndustryMainCategoriesReferences {\n    id\n    name\n    subCategories {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "bc6e1d370ae975baa3217aa900aa0a10";

export default node;
