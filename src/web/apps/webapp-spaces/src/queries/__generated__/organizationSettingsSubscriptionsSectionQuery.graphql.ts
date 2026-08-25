/**
 * @generated SignedSource<<8fbe29de8d249fb3946cd635c89c8ba1>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type SpacesSubscriptionStatus = "COMPLIMENTARY_BRIDGE" | "LEGACY_ACTIVE" | "MISSING_STATE" | "NOT_SET" | "PAID_ACTIVE" | "PAID_INACTIVE" | "TRIAL_ACTIVE" | "TRIAL_EXPIRED" | "TRIAL_EXPIRING" | "%future added value";
export type organizationSettingsSubscriptionsSectionQuery$variables = {
  organizationCustomDomain: string;
};
export type organizationSettingsSubscriptionsSectionQuery$data = {
  readonly organization: {
    readonly activeOffering: {
      readonly canCancel: boolean;
      readonly code: string;
      readonly currency: {
        readonly name: string;
      };
      readonly end: any;
      readonly featureSet: ReadonlyArray<string>;
      readonly fixedPrice: number | null | undefined;
      readonly free: boolean;
      readonly id: string;
      readonly isEnterprise: boolean;
      readonly name: string;
      readonly start: any;
      readonly underPriceLines: ReadonlyArray<string>;
      readonly unitPrice: number | null | undefined;
    };
    readonly availableOfferings: ReadonlyArray<{
      readonly code: string;
      readonly currency: {
        readonly name: string;
      };
      readonly featureSet: ReadonlyArray<string>;
      readonly fixedPrice: number | null | undefined;
      readonly free: boolean;
      readonly isEnterprise: boolean;
      readonly name: string;
      readonly underPriceLines: ReadonlyArray<string>;
      readonly unitPrice: number | null | undefined;
    }>;
    readonly hasAttachedPaymentMethod: boolean;
    readonly id: string;
    readonly name: string;
    readonly paymentMethods: ReadonlyArray<{
      readonly cardBrand: string | null | undefined;
      readonly cardExpiryMonth: number | null | undefined;
      readonly cardExpiryYear: number | null | undefined;
      readonly cardLastFourDigit: string | null | undefined;
      readonly id: string;
    }>;
  } | null | undefined;
  readonly organizationSpacesSubscription: {
    readonly isComplimentaryBridge: boolean;
    readonly nextBillingAt: any | null | undefined;
    readonly remainingTrialDays: number;
    readonly subscriptionStatus: SpacesSubscriptionStatus;
    readonly trialEndsAt: any | null | undefined;
    readonly upgradeRequired: boolean;
  } | null | undefined;
};
export type organizationSettingsSubscriptionsSectionQuery = {
  response: organizationSettingsSubscriptionsSectionQuery$data;
  variables: organizationSettingsSubscriptionsSectionQuery$variables;
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
  "name": "code",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isEnterprise",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "unitPrice",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "fixedPrice",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "CurrencyDetails",
  "kind": "LinkedField",
  "name": "currency",
  "plural": false,
  "selections": [
    (v2/*:: as any*/)
  ],
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "featureSet",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "underPriceLines",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "free",
  "storageKey": null
},
v11 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "organizationId",
        "variableName": "organizationCustomDomain"
      }
    ],
    "concreteType": "OrganizationSpacesSubscriptionDetails",
    "kind": "LinkedField",
    "name": "organizationSpacesSubscription",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "subscriptionStatus",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "remainingTrialDays",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "trialEndsAt",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "isComplimentaryBridge",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "nextBillingAt",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "upgradeRequired",
        "storageKey": null
      }
    ],
    "storageKey": null
  },
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
      (v1/*:: as any*/),
      (v2/*:: as any*/),
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
          (v1/*:: as any*/),
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
          (v1/*:: as any*/),
          (v3/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canCancel",
            "storageKey": null
          },
          (v4/*:: as any*/),
          (v2/*:: as any*/),
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
          (v5/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/),
          (v9/*:: as any*/),
          (v10/*:: as any*/)
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
          (v4/*:: as any*/),
          (v3/*:: as any*/),
          (v2/*:: as any*/),
          (v5/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/),
          (v9/*:: as any*/),
          (v10/*:: as any*/)
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
    "name": "organizationSettingsSubscriptionsSectionQuery",
    "selections": (v11/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsSubscriptionsSectionQuery",
    "selections": (v11/*:: as any*/)
  },
  "params": {
    "cacheID": "b07e6af53b750dc37cf245828d1985d0",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsSubscriptionsSectionQuery",
    "operationKind": "query",
    "text": "query organizationSettingsSubscriptionsSectionQuery(\n  $organizationCustomDomain: String!\n) {\n  organizationSpacesSubscription(organizationId: $organizationCustomDomain) {\n    subscriptionStatus\n    remainingTrialDays\n    trialEndsAt\n    isComplimentaryBridge\n    nextBillingAt\n    upgradeRequired\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n    hasAttachedPaymentMethod\n    paymentMethods {\n      id\n      cardBrand\n      cardExpiryMonth\n      cardExpiryYear\n      cardLastFourDigit\n    }\n    activeOffering {\n      id\n      code\n      canCancel\n      isEnterprise\n      name\n      start\n      end\n      unitPrice\n      fixedPrice\n      currency {\n        name\n      }\n      featureSet\n      underPriceLines\n      free\n    }\n    availableOfferings {\n      isEnterprise\n      code\n      name\n      unitPrice\n      fixedPrice\n      currency {\n        name\n      }\n      featureSet\n      underPriceLines\n      free\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "687fdf3f07285656ad4f7cd2f887c142";

export default node;
