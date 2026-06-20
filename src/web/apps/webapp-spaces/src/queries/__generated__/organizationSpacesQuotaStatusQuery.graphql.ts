/**
 * @generated SignedSource<<68a75e7b1a928d507158b76a633da5a0>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type SpacesQuotaReasonCode = "CUSTOM_CAPACITY_EXCEEDED" | "FREE_TIER_LIMIT_EXCEEDED" | "MISSING_OFFERING_STATE" | "NOT_SET" | "OUT_OF_PERIOD_EXCLUDED" | "PAID_TIER_LIMIT_EXCEEDED" | "TRIAL_EXPIRED" | "WITHIN_QUOTA" | "%future added value";
export type organizationSpacesQuotaStatusQuery$variables = {
  organizationId: string;
};
export type organizationSpacesQuotaStatusQuery$data = {
  readonly bookingSpacesQuotaStatus: {
    readonly attemptedCurrentPeriodCount: number;
    readonly currentPeriodEndUtc: any;
    readonly currentPeriodStartUtc: any;
    readonly currentUsage: number;
    readonly excludedOutOfPeriodCount: number;
    readonly organizationId: string;
    readonly planCode: number | null | undefined;
    readonly quotaExceeded: boolean;
    readonly quotaLimit: number | null | undefined;
    readonly reasonCode: {
      readonly name: string;
      readonly type: SpacesQuotaReasonCode;
    } | null | undefined;
    readonly remainingQuota: number | null | undefined;
    readonly totalAttemptedInstanceCount: number;
    readonly upgradePlans: ReadonlyArray<{
      readonly availability: string;
      readonly name: string;
      readonly planCode: number;
      readonly priceDescription: string | null | undefined;
    }>;
  };
};
export type organizationSpacesQuotaStatusQuery = {
  response: organizationSpacesQuotaStatusQuery$data;
  variables: organizationSpacesQuotaStatusQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "planCode",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "organizationId",
        "variableName": "organizationId"
      }
    ],
    "concreteType": "BookingSpacesQuotaStatusDetails",
    "kind": "LinkedField",
    "name": "bookingSpacesQuotaStatus",
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
        "name": "currentPeriodStartUtc",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "currentPeriodEndUtc",
        "storageKey": null
      },
      (v1/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "quotaLimit",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "currentUsage",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "attemptedCurrentPeriodCount",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "excludedOutOfPeriodCount",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "totalAttemptedInstanceCount",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "remainingQuota",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "quotaExceeded",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "SpacesQuotaReasonCodeDetails",
        "kind": "LinkedField",
        "name": "reasonCode",
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
        "concreteType": "UpgradePlanDetails",
        "kind": "LinkedField",
        "name": "upgradePlans",
        "plural": true,
        "selections": [
          (v1/*:: as any*/),
          (v2/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "availability",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "priceDescription",
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
    "name": "organizationSpacesQuotaStatusQuery",
    "selections": (v3/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSpacesQuotaStatusQuery",
    "selections": (v3/*:: as any*/)
  },
  "params": {
    "cacheID": "73c343cd5f571551456b97e677c719ee",
    "id": null,
    "metadata": {},
    "name": "organizationSpacesQuotaStatusQuery",
    "operationKind": "query",
    "text": "query organizationSpacesQuotaStatusQuery(\n  $organizationId: String!\n) {\n  bookingSpacesQuotaStatus(organizationId: $organizationId) {\n    organizationId\n    currentPeriodStartUtc\n    currentPeriodEndUtc\n    planCode\n    quotaLimit\n    currentUsage\n    attemptedCurrentPeriodCount\n    excludedOutOfPeriodCount\n    totalAttemptedInstanceCount\n    remainingQuota\n    quotaExceeded\n    reasonCode {\n      type\n      name\n    }\n    upgradePlans {\n      planCode\n      name\n      availability\n      priceDescription\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "401fcb8a6dd24d8723d64f509fafb126";

export default node;
