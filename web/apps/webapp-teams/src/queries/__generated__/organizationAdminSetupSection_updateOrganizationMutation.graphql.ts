/**
 * @generated SignedSource<<06983a2d7e7d9f782a9b3a8743c6c013>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type UpdateOrganizationInput = {
  billingCycle: OrganizationBillingCycle;
  clientMutationId?: string | null | undefined;
  contactEmail?: string | null | undefined;
  contactPhone?: string | null | undefined;
  customDomain?: string | null | undefined;
  customerFacingTermsAndConditionsUrl?: string | null | undefined;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  id?: string | null | undefined;
  industrySubCategoryIds: ReadonlyArray<string>;
  invoiceDueInDays: number;
  listingMetadata?: ListingMetadataInput | null | undefined;
  logoUrl?: string | null | undefined;
  marketplaceListingMetadata?: ListingMetadataInput | null | undefined;
  name: string;
  refundNotificationEmails: ReadonlyArray<string>;
  website?: string | null | undefined;
};
export type CdnImageFileInput = {
  original?: CdnFileInput | null | undefined;
  thumbnail?: CdnFileInput | null | undefined;
};
export type CdnFileInput = {
  height?: number | null | undefined;
  url: string;
  width?: number | null | undefined;
};
export type ListingMetadataInput = {
  about?: string | null | undefined;
  includedFeatures?: ReadonlyArray<string> | null | undefined;
  subTitle?: string | null | undefined;
  title?: string | null | undefined;
};
export type organizationAdminSetupSection_updateOrganizationMutation$variables = {
  input: UpdateOrganizationInput;
};
export type organizationAdminSetupSection_updateOrganizationMutation$data = {
  readonly updateOrganization: {
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
      readonly logoUrl: string | null | undefined;
      readonly marketplaceListingMetadata: {
        readonly about: string | null | undefined;
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly name: string;
      readonly refundNotificationEmails: ReadonlyArray<string>;
      readonly website: string | null | undefined;
    };
  };
};
export type organizationAdminSetupSection_updateOrganizationMutation$rawResponse = {
  readonly updateOrganization: {
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
      readonly logoUrl: string | null | undefined;
      readonly marketplaceListingMetadata: {
        readonly about: string | null | undefined;
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly name: string;
      readonly refundNotificationEmails: ReadonlyArray<string>;
      readonly website: string | null | undefined;
    };
  };
};
export type organizationAdminSetupSection_updateOrganizationMutation = {
  rawResponse: organizationAdminSetupSection_updateOrganizationMutation$rawResponse;
  response: organizationAdminSetupSection_updateOrganizationMutation$data;
  variables: organizationAdminSetupSection_updateOrganizationMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
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
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "about",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v6 = [
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
v7 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "updateOrganization",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "customDomain",
            "storageKey": null
          },
          (v2/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "ListingMetadata",
            "kind": "LinkedField",
            "name": "listingMetadata",
            "plural": false,
            "selections": [
              (v3/*:: as any*/),
              (v4/*:: as any*/),
              (v5/*:: as any*/)
            ],
            "storageKey": null
          },
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
            "concreteType": "ListingMetadata",
            "kind": "LinkedField",
            "name": "marketplaceListingMetadata",
            "plural": false,
            "selections": [
              (v3/*:: as any*/),
              (v4/*:: as any*/),
              (v5/*:: as any*/),
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
            "selections": [
              (v1/*:: as any*/),
              (v2/*:: as any*/)
            ],
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
                "selections": (v6/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v6/*:: as any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
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
              (v2/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "invoiceDueInDays",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdminSetupSection_updateOrganizationMutation",
    "selections": (v7/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationAdminSetupSection_updateOrganizationMutation",
    "selections": (v7/*:: as any*/)
  },
  "params": {
    "cacheID": "05457baa29efbe18c4d963a6583451d0",
    "id": null,
    "metadata": {},
    "name": "organizationAdminSetupSection_updateOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminSetupSection_updateOrganizationMutation(\n  $input: UpdateOrganizationInput!\n) {\n  updateOrganization(input: $input) {\n    organization {\n      id\n      customDomain\n      name\n      listingMetadata {\n        about\n        title\n        subTitle\n      }\n      logoUrl\n      marketplaceListingMetadata {\n        about\n        title\n        subTitle\n        includedFeatures\n      }\n      website\n      customerFacingTermsAndConditionsUrl\n      industrySubCategories {\n        id\n        name\n      }\n      contactEmail\n      contactPhone\n      refundNotificationEmails\n      featureImages {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n      billingCycle {\n        type\n        name\n      }\n      invoiceDueInDays\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "788ab11eaf4769c152e070bf95b0606c";

export default node;
