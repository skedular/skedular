/**
 * @generated SignedSource<<0e8af7332ca12f72d8946373ca0e4f9c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type organizationAdminSetupSectionQuery$variables = {
  organizationCustomDomain: string;
};
export type organizationAdminSetupSectionQuery$data = {
  readonly emailsToShowLatestCapabilities: ReadonlyArray<string>;
  readonly me: {
    readonly emails: ReadonlyArray<string>;
    readonly id: string;
  };
  readonly organization: {
    readonly billingCycle: {
      readonly name: string;
      readonly type: OrganizationBillingCycle;
    };
    readonly contactEmail: string | null | undefined;
    readonly contactPhone: string | null | undefined;
    readonly customDomain: string | null | undefined;
    readonly customerFacingTermsAndConditionsUrl: string | null | undefined;
    readonly featureImages: ReadonlyArray<{
      readonly original: {
        readonly height: number | null | undefined;
        readonly url: string;
        readonly width: number | null | undefined;
      } | null | undefined;
      readonly thumbnail: {
        readonly height: number | null | undefined;
        readonly url: string;
        readonly width: number | null | undefined;
      } | null | undefined;
    }>;
    readonly id: string;
    readonly industrySubCategories: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
    readonly invoiceDueInDays: number;
    readonly listingMetadata: {
      readonly about: string | null | undefined;
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly marketplaceListingMetadata: {
      readonly about: string | null | undefined;
      readonly includedFeatures: ReadonlyArray<string> | null | undefined;
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly name: string;
    readonly refundNotificationEmails: ReadonlyArray<string>;
    readonly website: string | null | undefined;
  } | null | undefined;
  readonly organizationIndustryMainCategoriesReferences: ReadonlyArray<{
    readonly subCategories: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
  }>;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMultipleChoicesIndustries_query">;
};
export type organizationAdminSetupSectionQuery = {
  response: organizationAdminSetupSectionQuery$data;
  variables: organizationAdminSetupSectionQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "emailsToShowLatestCapabilities",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v2/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "emails",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = [
  (v2/*: any*/),
  (v4/*: any*/)
],
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
  "kind": "LinkedField",
  "name": "subCategories",
  "plural": true,
  "selections": (v5/*: any*/),
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "about",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v10 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "url",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "height",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "width",
    "storageKey": null
  }
],
v11 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "customDomain",
      "variableName": "organizationCustomDomain"
    }
  ],
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "organization",
  "plural": false,
  "selections": [
    (v2/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "customDomain",
      "storageKey": null
    },
    (v4/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationBillingCycleDetails",
      "kind": "LinkedField",
      "name": "billingCycle",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
          "storageKey": null
        },
        (v4/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "invoiceDueInDays",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "ListingMetadata",
      "kind": "LinkedField",
      "name": "listingMetadata",
      "plural": false,
      "selections": [
        (v7/*: any*/),
        (v8/*: any*/),
        (v9/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "ListingMetadata",
      "kind": "LinkedField",
      "name": "marketplaceListingMetadata",
      "plural": false,
      "selections": [
        (v7/*: any*/),
        (v8/*: any*/),
        (v9/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "includedFeatures",
          "storageKey": null
        }
      ],
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
      "name": "customerFacingTermsAndConditionsUrl",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
      "kind": "LinkedField",
      "name": "industrySubCategories",
      "plural": true,
      "selections": (v5/*: any*/),
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
      "kind": "ScalarField",
      "name": "refundNotificationEmails",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CdnImageFile",
      "kind": "LinkedField",
      "name": "featureImages",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "CdnFile",
          "kind": "LinkedField",
          "name": "original",
          "plural": false,
          "selections": (v10/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "CdnFile",
          "kind": "LinkedField",
          "name": "thumbnail",
          "plural": false,
          "selections": (v10/*: any*/),
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdminSetupSectionQuery",
    "selections": [
      (v1/*: any*/),
      (v3/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationIndustryMainCategoryReferenceDetails",
        "kind": "LinkedField",
        "name": "organizationIndustryMainCategoriesReferences",
        "plural": true,
        "selections": [
          (v6/*: any*/)
        ],
        "storageKey": null
      },
      (v11/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMultipleChoicesIndustries_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdminSetupSectionQuery",
    "selections": [
      (v1/*: any*/),
      (v3/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationIndustryMainCategoryReferenceDetails",
        "kind": "LinkedField",
        "name": "organizationIndustryMainCategoriesReferences",
        "plural": true,
        "selections": [
          (v6/*: any*/),
          (v2/*: any*/),
          (v4/*: any*/)
        ],
        "storageKey": null
      },
      (v11/*: any*/)
    ]
  },
  "params": {
    "cacheID": "b0ff64502876b76dd5038f1d75b2e295",
    "id": null,
    "metadata": {},
    "name": "organizationAdminSetupSectionQuery",
    "operationKind": "query",
    "text": "query organizationAdminSetupSectionQuery(\n  $organizationCustomDomain: String!\n) {\n  emailsToShowLatestCapabilities\n  me {\n    id\n    emails\n  }\n  organizationIndustryMainCategoriesReferences {\n    subCategories {\n      id\n      name\n    }\n    id\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    customDomain\n    name\n    billingCycle {\n      type\n      name\n    }\n    invoiceDueInDays\n    listingMetadata {\n      about\n      title\n      subTitle\n    }\n    marketplaceListingMetadata {\n      about\n      title\n      subTitle\n      includedFeatures\n    }\n    website\n    customerFacingTermsAndConditionsUrl\n    industrySubCategories {\n      id\n      name\n    }\n    contactEmail\n    contactPhone\n    refundNotificationEmails\n    featureImages {\n      original {\n        url\n        height\n        width\n      }\n      thumbnail {\n        url\n        height\n        width\n      }\n    }\n  }\n  ...organizationMultipleChoicesIndustries_query\n}\n\nfragment organizationMultipleChoicesIndustries_query on Query {\n  organizationIndustryMainCategoriesReferences {\n    id\n    name\n    subCategories {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2254e303831410d5d45b52acf8e6d66f";

export default node;
