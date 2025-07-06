/**
 * @generated SignedSource<<e602f04f48eb62f6656b029837a4130b>>
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
  readonly " $fragmentSpreads": FragmentRefs<"organizationAdmin_customTags_query" | "organizationAdmin_organization_query" | "organizationAdmin_query" | "organizationAdmin_zones_query">;
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
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v4/*: any*/)
],
v7 = [
  (v5/*: any*/),
  (v4/*: any*/)
],
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "addressLine1",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "addressLine2",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "suburb",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "city",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "province",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "zipcode",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "country",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isEnterprise",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "unitPrice",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "featureSet",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "underPriceLines",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "free",
  "storageKey": null
},
v20 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  }
],
v21 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v22 = [
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "zoneNameSearchText"
      },
      (v21/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v23 = [
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "description",
            "storageKey": null
          },
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
v24 = [
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "ASCENDING",
        "field": "NAME"
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
      (v21/*: any*/)
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
        "name": "organizationAdmin_organization_query"
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
            "concreteType": "OrganizationTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": (v6/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationMemberVisibilityPolicyDetails",
            "kind": "LinkedField",
            "name": "memberVisibilityPolicy",
            "plural": false,
            "selections": (v6/*: any*/),
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
            "selections": (v7/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "contactEmail",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "contactPhone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              (v8/*: any*/),
              (v9/*: any*/),
              (v10/*: any*/),
              (v11/*: any*/),
              (v12/*: any*/),
              (v13/*: any*/),
              (v14/*: any*/)
            ],
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
            "concreteType": "OrganizationPaymentMethod",
            "kind": "LinkedField",
            "name": "paymentMethods",
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
            "args": null,
            "concreteType": "OrganizationActiveOfferingDetails",
            "kind": "LinkedField",
            "name": "activeOffering",
            "plural": false,
            "selections": [
              (v5/*: any*/),
              (v15/*: any*/),
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
              (v16/*: any*/),
              (v17/*: any*/),
              (v18/*: any*/),
              (v19/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationOfferingDetails",
            "kind": "LinkedField",
            "name": "availableOfferings",
            "plural": true,
            "selections": [
              (v15/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "code",
                "storageKey": null
              },
              (v4/*: any*/),
              (v16/*: any*/),
              (v17/*: any*/),
              (v18/*: any*/),
              (v19/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationSsoSettingsDetails",
            "kind": "LinkedField",
            "name": "ssoSettings",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "entityId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "loginUrl",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "appFederationMetadataUrl",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTaxDetails",
            "kind": "LinkedField",
            "name": "taxDetails",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "gstNumber",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "gstPercentage",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationBillingDetails",
            "kind": "LinkedField",
            "name": "billingDetails",
            "plural": false,
            "selections": [
              (v5/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "companyName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
              (v8/*: any*/),
              (v9/*: any*/),
              (v10/*: any*/),
              (v11/*: any*/),
              (v12/*: any*/),
              (v13/*: any*/),
              (v14/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "Customer_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredZones",
            "plural": true,
            "selections": (v20/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Customer_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredCustomTags",
            "plural": true,
            "selections": (v20/*: any*/),
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
            "selections": (v7/*: any*/),
            "storageKey": null
          },
          (v5/*: any*/),
          (v4/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTypeDetails",
        "kind": "LinkedField",
        "name": "organizationTypes",
        "plural": true,
        "selections": (v6/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationMemberVisibilityPolicyDetails",
        "kind": "LinkedField",
        "name": "organizationMemberVisibilityPolicies",
        "plural": true,
        "selections": (v6/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v23/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
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
        "args": (v24/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "customTags",
        "plural": false,
        "selections": (v23/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v24/*: any*/),
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
    "cacheID": "bed358f5b06340d1c55a1daa7a5f5442",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationAdmin_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationAdmin_rootQuery(\n  $organizationId: String!\n  $zoneNameSearchText: String\n  $customTagNameSearchText: String\n) {\n  organization(id: $organizationId) {\n    name\n    id\n  }\n  ...organizationAdmin_query\n  ...organizationAdmin_organization_query\n  ...organizationAdmin_zones_query\n  ...organizationAdmin_customTags_query\n}\n\nfragment organizationAdmin_customTags_query on Query {\n  customTags(where: {organizationId: $organizationId, nameContains: $customTagNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationAdmin_organization_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    logoUrl\n    about\n    type {\n      type\n      name\n    }\n    memberVisibilityPolicy {\n      type\n      name\n    }\n    website\n    canModify\n    industrySubCategories {\n      id\n      name\n    }\n    contactEmail\n    contactPhone\n    physicalAddress {\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n    }\n    hasAttachedPaymentMethod\n    paymentMethods {\n      id\n      cardBrand\n      cardExpiryMonth\n      cardExpiryYear\n      cardLastFourDigit\n    }\n    activeOffering {\n      id\n      isEnterprise\n      name\n      start\n      end\n      unitPrice\n      featureSet\n      underPriceLines\n      free\n    }\n    availableOfferings {\n      isEnterprise\n      code\n      name\n      unitPrice\n      featureSet\n      underPriceLines\n      free\n    }\n    ssoSettings {\n      entityId\n      loginUrl\n      appFederationMetadataUrl\n    }\n    taxDetails {\n      gstNumber\n      gstPercentage\n    }\n    billingDetails {\n      id\n      companyName\n      email\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n    }\n  }\n}\n\nfragment organizationAdmin_query on Query {\n  me {\n    id\n    preferredZones {\n      uniqueId\n    }\n    preferredCustomTags {\n      uniqueId\n    }\n  }\n  organizationIndustryMainCategoriesReferences {\n    subCategories {\n      id\n      name\n    }\n    id\n  }\n  ...organizationMultipleChoicesIndustries_query\n  ...singleChoiceOrganizationType_query\n  ...singleChoiceOrganizationMemberVisibilityPolicyquery\n}\n\nfragment organizationAdmin_zones_query on Query {\n  zones(where: {organizationId: $organizationId, nameContains: $zoneNameSearchText}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMultipleChoicesIndustries_query on Query {\n  organizationIndustryMainCategoriesReferences {\n    id\n    name\n    subCategories {\n      id\n      name\n    }\n  }\n}\n\nfragment singleChoiceOrganizationMemberVisibilityPolicyquery on Query {\n  organizationMemberVisibilityPolicies {\n    type\n    name\n  }\n}\n\nfragment singleChoiceOrganizationType_query on Query {\n  organizationTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "84e250da65843c237706120c230062cf";

export default node;
