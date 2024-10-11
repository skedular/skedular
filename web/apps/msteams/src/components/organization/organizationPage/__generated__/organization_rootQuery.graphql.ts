/**
 * @generated SignedSource<<4592eb81c3fc68c94fcc6fcea2cb1cbe>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organization_rootQuery$variables = {
  organizationExists: boolean;
  organizationId: string;
};
export type organization_rootQuery$data = {
  readonly organization: {
    readonly canModify: boolean;
    readonly canViewAnalytics: boolean;
    readonly id: string;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationBillingTab_query" | "organizationOfferingTab_query">;
};
export type organization_rootQuery = {
  response: organization_rootQuery$data;
  variables: organization_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v2 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationId"
  }
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
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
  "name": "logoUrl",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canViewAnalytics",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "unitPrice",
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
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "description",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v10 = [
  {
    "kind": "Variable",
    "name": "organizationId",
    "variableName": "organizationId"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organization_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v2/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          (v4/*: any*/),
          (v5/*: any*/),
          (v6/*: any*/),
          (v7/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationBillingTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationOfferingTab_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "organization_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v2/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          (v4/*: any*/),
          (v5/*: any*/),
          (v6/*: any*/),
          (v7/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationOfferingDetails",
            "kind": "LinkedField",
            "name": "offering",
            "plural": false,
            "selections": [
              (v3/*: any*/),
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
              (v8/*: any*/),
              (v9/*: any*/)
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
              (v8/*: any*/),
              (v9/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v10/*: any*/),
            "concreteType": "OrganizationPaymentMethod",
            "kind": "LinkedField",
            "name": "organizationPaymentMethodsDetails",
            "plural": true,
            "selections": [
              (v3/*: any*/),
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
          }
        ]
      },
      {
        "alias": null,
        "args": (v10/*: any*/),
        "concreteType": "OrganizationBillingInfo",
        "kind": "LinkedField",
        "name": "organizationBillingInfo",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "organizationId",
            "storageKey": null
          },
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
      }
    ]
  },
  "params": {
    "cacheID": "33deeac8c970557a40cb241cb695ba47",
    "id": null,
    "metadata": {},
    "name": "organization_rootQuery",
    "operationKind": "query",
    "text": "query organization_rootQuery(\n  $organizationId: String!\n  $organizationExists: Boolean!\n) {\n  organization(id: $organizationId) {\n    id\n    name\n    logoUrl\n    canModify\n    canViewAnalytics\n  }\n  ...organizationBillingTab_query\n  ...organizationOfferingTab_query\n}\n\nfragment organizationAvailableOfferings_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    hasAttachedPaymentMethod\n    availableOfferings {\n      code\n      name\n      unitPrice\n      featureSet {\n        name\n        description\n      }\n    }\n  }\n}\n\nfragment organizationBillingInfo_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n  }\n  organizationBillingInfo(organizationId: $organizationId) {\n    organizationId\n    email\n    addressLine1\n    addressLine2\n    suburb\n    city\n    province\n    zipcode\n    country\n  }\n}\n\nfragment organizationBillingTab_query on Query {\n  ...organizationPaymentMethods_query\n  ...organizationBillingInfo_query\n}\n\nfragment organizationOfferingTab_query on Query {\n  ...organizationOffering_query\n  ...organizationAvailableOfferings_query\n  organization(id: $organizationId) {\n    id\n    availableOfferings {\n      code\n    }\n  }\n}\n\nfragment organizationOffering_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    offering {\n      id\n      name\n      start\n      end\n      unitPrice\n      featureSet {\n        name\n        description\n      }\n    }\n  }\n}\n\nfragment organizationPaymentMethods_query on Query {\n  organization(id: $organizationId) {\n    id\n  }\n  organizationPaymentMethodsDetails(organizationId: $organizationId) @include(if: $organizationExists) {\n    id\n    cardBrand\n    cardExpiryMonth\n    cardExpiryYear\n    cardLastFourDigit\n  }\n}\n"
  }
};
})();

(node as any).hash = "961d11d1892ddb1f6cd82b95b54c8b92";

export default node;
