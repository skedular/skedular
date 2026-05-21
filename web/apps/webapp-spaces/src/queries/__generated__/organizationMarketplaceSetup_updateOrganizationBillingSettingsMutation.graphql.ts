/**
 * @generated SignedSource<<4cea314527ebbf9d41200d0d635e1f89>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type OrganizationPatchField = "BILLING_CYCLE" | "CONTACT_EMAIL" | "CONTACT_PHONE" | "CUSTOMER_FACING_TERMS_AND_CONDITIONS_URL" | "CUSTOM_DOMAIN" | "DESCRIPTION" | "FEATURE_IMAGES" | "INDUSTRY_SUB_CATEGORIES" | "INVOICE_DUE_IN_DAYS" | "LOGO_URL" | "MARKETPLACE_LISTING_METADATA" | "NAME" | "PHYSICAL_ADDRESS" | "REFUND_NOTIFICATION_EMAILS" | "SUB_TITLE" | "TITLE" | "WEBSITE" | "%future added value";
export type UpdateOrganizationInput = {
  billingCycle?: OrganizationBillingCycle | null | undefined;
  clientMutationId?: string | null | undefined;
  contactEmail?: string | null | undefined;
  contactPhone?: string | null | undefined;
  customDomain?: string | null | undefined;
  customerFacingTermsAndConditionsUrl?: string | null | undefined;
  description?: string | null | undefined;
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
  subTitle?: string | null | undefined;
  title?: string | null | undefined;
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
export type organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$variables = {
  input: UpdateOrganizationInput;
};
export type organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$data = {
  readonly updateOrganization: {
    readonly organization: {
      readonly billingCycle: {
        readonly name: string;
        readonly type: OrganizationBillingCycle;
      };
      readonly id: string;
      readonly invoiceDueInDays: number;
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$rawResponse = {
  readonly updateOrganization: {
    readonly organization: {
      readonly billingCycle: {
        readonly name: string;
        readonly type: OrganizationBillingCycle;
      };
      readonly id: string;
      readonly invoiceDueInDays: number;
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation = {
  rawResponse: organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$rawResponse;
  response: organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$data;
  variables: organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$variables;
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
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "name",
                "storageKey": null
              }
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
    "name": "organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "536a78fa903da940508bc425730810c8",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation(\n  $input: UpdateOrganizationInput!\n) {\n  updateOrganization(input: $input) {\n    organization {\n      id\n      billingCycle {\n        type\n        name\n      }\n      invoiceDueInDays\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7a67a0ac993819df3b7f57a4cdb2761b";

export default node;
