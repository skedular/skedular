/**
 * @generated SignedSource<<30b09042e291b206475f762ce181b484>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type OrganizationXeroBillingMode = "DISABLED" | "ENABLED" | "REPEATING_INVOICES" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationMarketplaceSetup_query$data = {
  readonly organization: {
    readonly billingCycle: {
      readonly name: string;
      readonly type: OrganizationBillingCycle;
    };
    readonly billingDetails: {
      readonly addressLine1: string;
      readonly addressLine2: string | null | undefined;
      readonly city: string | null | undefined;
      readonly companyName: string | null | undefined;
      readonly country: string;
      readonly countryCode: string | null | undefined;
      readonly email: string;
      readonly formattedAddress: string | null | undefined;
      readonly id: string;
      readonly invoiceDueInDays: number;
      readonly latitude: number | null | undefined;
      readonly longitude: number | null | undefined;
      readonly osmId: string | null | undefined;
      readonly osmType: string | null | undefined;
      readonly placeId: string | null | undefined;
      readonly province: string | null | undefined;
      readonly suburb: string | null | undefined;
      readonly zipcode: string;
    } | null | undefined;
    readonly id: string;
    readonly marketplaceListingMetadata: {
      readonly about: string | null | undefined;
      readonly includedFeatures: ReadonlyArray<string> | null | undefined;
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly name: string;
    readonly xeroConnection: {
      readonly autoReconcilePayments: boolean;
      readonly billingMode: OrganizationXeroBillingMode;
      readonly defaultBrandingThemeId: string | null | undefined;
      readonly defaultReceivablesAccountCode: string | null | undefined;
      readonly defaultReferencePrefix: string | null | undefined;
      readonly defaultSalesAccountCode: string | null | undefined;
      readonly defaultTrackingCategory1: string | null | undefined;
      readonly defaultTrackingCategory2: string | null | undefined;
      readonly hasAccessToken: boolean;
      readonly hasRefreshToken: boolean;
      readonly id: string;
      readonly isActive: boolean;
      readonly lastError: string | null | undefined;
      readonly lastSuccessfulSyncAt: any | null | undefined;
      readonly scopes: string | null | undefined;
      readonly sendInvoicesViaXero: boolean;
      readonly tenantId: string;
      readonly tenantName: string;
    } | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"existingStripeConnectAccountButton_query" | "singleChoiceOrganizationBillingCycle_query" | "singleChoiceOrganizationXeroBillingMode_query">;
  readonly " $fragmentType": "organizationMarketplaceSetup_query";
};
export type organizationMarketplaceSetup_query$key = {
  readonly " $data"?: organizationMarketplaceSetup_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationMarketplaceSetup_query",
  "selections": [
    {
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
        (v0/*: any*/),
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationBillingDetails",
          "kind": "LinkedField",
          "name": "billingDetails",
          "plural": false,
          "selections": [
            (v0/*: any*/),
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
            (v1/*: any*/)
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationXeroConnection",
          "kind": "LinkedField",
          "name": "xeroConnection",
          "plural": false,
          "selections": [
            (v0/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "tenantId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "tenantName",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "billingMode",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "scopes",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "isActive",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "sendInvoicesViaXero",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "autoReconcilePayments",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "defaultSalesAccountCode",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "defaultReceivablesAccountCode",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "defaultTrackingCategory1",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "defaultTrackingCategory2",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "defaultBrandingThemeId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "defaultReferencePrefix",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "lastSuccessfulSyncAt",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "lastError",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "hasAccessToken",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "hasRefreshToken",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "existingStripeConnectAccountButton_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceOrganizationBillingCycle_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceOrganizationXeroBillingMode_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "a1471049e44009a5cea421942c73722d";

export default node;
