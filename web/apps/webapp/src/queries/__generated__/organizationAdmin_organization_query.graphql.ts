/**
 * @generated SignedSource<<e8244fe2b7a64118b0f5ef74f87c1ab5>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationMemberVisibilityPolicy = "FULL_ACCESS" | "LIMITED_ACCESS" | "%future added value";
export type OrganizationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationAdmin_organization_query$data = {
  readonly organization: {
    readonly about: string | null | undefined;
    readonly activeOffering: {
      readonly end: any;
      readonly featureSet: ReadonlyArray<string>;
      readonly free: boolean;
      readonly id: string;
      readonly isEnterprise: boolean;
      readonly name: string;
      readonly start: any;
      readonly underPriceLines: ReadonlyArray<string>;
      readonly unitPrice: number;
    };
    readonly availableOfferings: ReadonlyArray<{
      readonly code: string;
      readonly featureSet: ReadonlyArray<string>;
      readonly free: boolean;
      readonly isEnterprise: boolean;
      readonly name: string;
      readonly underPriceLines: ReadonlyArray<string>;
      readonly unitPrice: number;
    }>;
    readonly billingDetails: {
      readonly addressLine1: string;
      readonly addressLine2: string | null | undefined;
      readonly city: string;
      readonly companyName: string | null | undefined;
      readonly country: string;
      readonly countryCode: string | null | undefined;
      readonly email: string;
      readonly formattedAddress: string | null | undefined;
      readonly id: string;
      readonly latitude: number | null | undefined;
      readonly longitude: number | null | undefined;
      readonly osmId: string | null | undefined;
      readonly osmType: string | null | undefined;
      readonly placeId: string | null | undefined;
      readonly province: string | null | undefined;
      readonly suburb: string;
      readonly zipcode: string;
    } | null | undefined;
    readonly canModify: boolean;
    readonly contactEmail: string | null | undefined;
    readonly contactPhone: string | null | undefined;
    readonly hasAttachedPaymentMethod: boolean;
    readonly id: string;
    readonly industrySubCategories: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
    readonly logoUrl: string | null | undefined;
    readonly memberVisibilityPolicy: {
      readonly name: string;
      readonly type: OrganizationMemberVisibilityPolicy;
    };
    readonly name: string;
    readonly paymentMethods: ReadonlyArray<{
      readonly cardBrand: string | null | undefined;
      readonly cardExpiryMonth: number | null | undefined;
      readonly cardExpiryYear: number | null | undefined;
      readonly cardLastFourDigit: string | null | undefined;
      readonly id: string;
    }>;
    readonly physicalAddress: {
      readonly addressLine1: string;
      readonly addressLine2: string | null | undefined;
      readonly city: string;
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
      readonly suburb: string;
      readonly zipcode: string;
    } | null | undefined;
    readonly ssoSettings: {
      readonly appFederationMetadataUrl: string;
      readonly entityId: string;
      readonly id: string;
      readonly isActive: boolean;
      readonly loginUrl: string;
    } | null | undefined;
    readonly taxDetails: {
      readonly taxId: string;
      readonly taxRatePercentage: any;
    } | null | undefined;
    readonly type: {
      readonly name: string;
      readonly type: OrganizationType;
    };
    readonly website: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "organizationAdmin_organization_query";
};
export type organizationAdmin_organization_query$key = {
  readonly " $data"?: organizationAdmin_organization_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAdmin_organization_query">;
};

import organizationAdmin_organization_refetchableFragment_graphql from './organizationAdmin_organization_refetchableFragment.graphql';

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
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v1/*: any*/)
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "osmType",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "osmId",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "placeId",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "longitude",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "latitude",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "formattedAddress",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "addressLine1",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "addressLine2",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "suburb",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "city",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "province",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "zipcode",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "country",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "countryCode",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isEnterprise",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "unitPrice",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "featureSet",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "underPriceLines",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "free",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": organizationAdmin_organization_refetchableFragment_graphql
    }
  },
  "name": "organizationAdmin_organization_query",
  "selections": [
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
        (v0/*: any*/),
        (v1/*: any*/),
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
          "selections": (v2/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationMemberVisibilityPolicyDetails",
          "kind": "LinkedField",
          "name": "memberVisibilityPolicy",
          "plural": false,
          "selections": (v2/*: any*/),
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
          "selections": [
            (v0/*: any*/),
            (v1/*: any*/)
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
          "concreteType": "OrganizationPhysicalAddressDetails",
          "kind": "LinkedField",
          "name": "physicalAddress",
          "plural": false,
          "selections": [
            (v0/*: any*/),
            (v3/*: any*/),
            (v4/*: any*/),
            (v5/*: any*/),
            (v6/*: any*/),
            (v7/*: any*/),
            (v8/*: any*/),
            (v9/*: any*/),
            (v10/*: any*/),
            (v11/*: any*/),
            (v12/*: any*/),
            (v13/*: any*/),
            (v14/*: any*/),
            (v15/*: any*/),
            (v16/*: any*/)
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
            (v0/*: any*/),
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
            (v0/*: any*/),
            (v17/*: any*/),
            (v1/*: any*/),
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
            (v18/*: any*/),
            (v19/*: any*/),
            (v20/*: any*/),
            (v21/*: any*/)
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
            (v17/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "code",
              "storageKey": null
            },
            (v1/*: any*/),
            (v18/*: any*/),
            (v19/*: any*/),
            (v20/*: any*/),
            (v21/*: any*/)
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
            (v0/*: any*/),
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
              "name": "taxId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "taxRatePercentage",
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
            (v3/*: any*/),
            (v4/*: any*/),
            (v5/*: any*/),
            (v6/*: any*/),
            (v7/*: any*/),
            (v8/*: any*/),
            (v9/*: any*/),
            (v10/*: any*/),
            (v11/*: any*/),
            (v12/*: any*/),
            (v13/*: any*/),
            (v14/*: any*/),
            (v15/*: any*/),
            (v16/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "c5a9dbfb2d5a6d1430b9f196b33ffe6a";

export default node;
