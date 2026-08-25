/**
 * @generated SignedSource<<961ca3f25a5320e1567a75df3067da98>>
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
export type organizationSettingsPhysicalAddressSection_updateOrganizationMutation$variables = {
  input: UpdateOrganizationInput;
};
export type organizationSettingsPhysicalAddressSection_updateOrganizationMutation$data = {
  readonly updateOrganization: {
    readonly organization: {
      readonly id: string;
      readonly physicalAddress: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string | null | undefined;
        readonly country: string;
        readonly countryCode: string | null | undefined;
        readonly formattedAddress: string | null | undefined;
        readonly id: string;
        readonly latitude: number | null | undefined;
        readonly longitude: number | null | undefined;
        readonly osmId: string | null | undefined;
        readonly osmType: string | null | undefined;
        readonly placeId: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string | null | undefined;
        readonly zipcode: string;
      } | null | undefined;
    };
  };
};
export type organizationSettingsPhysicalAddressSection_updateOrganizationMutation$rawResponse = {
  readonly updateOrganization: {
    readonly organization: {
      readonly id: string;
      readonly physicalAddress: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string | null | undefined;
        readonly country: string;
        readonly countryCode: string | null | undefined;
        readonly formattedAddress: string | null | undefined;
        readonly id: string;
        readonly latitude: number | null | undefined;
        readonly longitude: number | null | undefined;
        readonly osmId: string | null | undefined;
        readonly osmType: string | null | undefined;
        readonly placeId: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string | null | undefined;
        readonly zipcode: string;
      } | null | undefined;
    };
  };
};
export type organizationSettingsPhysicalAddressSection_updateOrganizationMutation = {
  rawResponse: organizationSettingsPhysicalAddressSection_updateOrganizationMutation$rawResponse;
  response: organizationSettingsPhysicalAddressSection_updateOrganizationMutation$data;
  variables: organizationSettingsPhysicalAddressSection_updateOrganizationMutation$variables;
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
v2 = [
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
            "concreteType": "OrganizationPhysicalAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              (v1/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "osmType",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "osmId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "placeId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "longitude",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "latitude",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "formattedAddress",
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
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "countryCode",
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
    "name": "organizationSettingsPhysicalAddressSection_updateOrganizationMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsPhysicalAddressSection_updateOrganizationMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "0f2784a7d4f3d3bf694f8d064c96ce59",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsPhysicalAddressSection_updateOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsPhysicalAddressSection_updateOrganizationMutation(\n  $input: UpdateOrganizationInput!\n) {\n  updateOrganization(input: $input) {\n    organization {\n      id\n      physicalAddress {\n        id\n        osmType\n        osmId\n        placeId\n        longitude\n        latitude\n        formattedAddress\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n        countryCode\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "81927fb553acf2a082bfa1720a2359f9";

export default node;
