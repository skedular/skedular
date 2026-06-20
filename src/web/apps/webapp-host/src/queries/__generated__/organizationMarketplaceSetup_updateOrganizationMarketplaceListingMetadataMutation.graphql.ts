/**
 * @generated SignedSource<<ee5b8b37b91806a3be8336d91138bf1b>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type OrganizationPatchField = "BILLING_CYCLE" | "CONTACT_EMAIL" | "CONTACT_PHONE" | "CUSTOMER_FACING_TERMS_AND_CONDITIONS_URL" | "CUSTOM_DOMAIN" | "FEATURE_IMAGES" | "INDUSTRY_SUB_CATEGORIES" | "INVOICE_DUE_IN_DAYS" | "LOGO_URL" | "MARKETPLACE_LISTING_METADATA" | "NAME" | "PHYSICAL_ADDRESS" | "REFUND_NOTIFICATION_EMAILS" | "WEBSITE" | "%future added value";
export type UpdateOrganizationInput = {
  billingCycle?: OrganizationBillingCycle | null | undefined;
  clientMutationId?: string | null | undefined;
  contactEmail?: string | null | undefined;
  contactPhone?: string | null | undefined;
  customDomain?: string | null | undefined;
  customerFacingTermsAndConditionsUrl?: string | null | undefined;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  fieldsToUpdate: ReadonlyArray<OrganizationPatchField>;
  id?: string | null | undefined;
  industrySubCategoryIds?: ReadonlyArray<string> | null | undefined;
  invoiceDueInDays?: number | null | undefined;
  logoUrl?: string | null | undefined;
  marketplaceListingMetadata?: ListingMetadataInput | null | undefined;
  name?: string | null | undefined;
  physicalAddress?: OrganizationPhysicalAddressPatchInput | null | undefined;
  refundNotificationEmails?: ReadonlyArray<string> | null | undefined;
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
export type OrganizationPhysicalAddressPatchInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city?: string | null | undefined;
  country: string;
  countryCode?: string | null | undefined;
  formattedAddress?: string | null | undefined;
  latitude?: number | null | undefined;
  longitude?: number | null | undefined;
  osmId?: string | null | undefined;
  osmType?: string | null | undefined;
  placeId?: string | null | undefined;
  province?: string | null | undefined;
  suburb?: string | null | undefined;
  zipcode: string;
};
export type organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$variables = {
  input: UpdateOrganizationInput;
};
export type organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$data = {
  readonly updateOrganization: {
    readonly organization: {
      readonly id: string;
      readonly marketplaceListingMetadata: {
        readonly about: string | null | undefined;
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$rawResponse = {
  readonly updateOrganization: {
    readonly organization: {
      readonly id: string;
      readonly marketplaceListingMetadata: {
        readonly about: string | null | undefined;
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation = {
  rawResponse: organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$rawResponse;
  response: organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$data;
  variables: organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
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
                "name": "title",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "subTitle",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "includedFeatures",
                "storageKey": null
              }
            ],
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
    "name": "organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "99fec9bc82349b686f70691ada660907",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation(\n  $input: UpdateOrganizationInput!\n) {\n  updateOrganization(input: $input) {\n    organization {\n      id\n      marketplaceListingMetadata {\n        about\n        title\n        subTitle\n        includedFeatures\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "399d99ab88dcbf61453dd5a034d292dc";

export default node;
