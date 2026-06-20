/**
 * @generated SignedSource<<93e6df199c899b26129de619b4b3eb59>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrganizationType = "HOST" | "MARKETPLACE" | "PRIVATE" | "%future added value";
export type SpacesAccessReasonCode = "ACTION_NOT_ALLOWED" | "ALLOWED_COMPLIMENTARY_BRIDGE" | "ALLOWED_PAID" | "ALLOWED_PROTECTIVE_ACTION" | "ALLOWED_READ_OR_RECOVERY" | "ALLOWED_TRIAL" | "MISSING_OFFERING_STATE" | "MISSING_TRIAL_STATE" | "NOT_SET" | "PAID_INACTIVE" | "TRIAL_EXPIRED" | "%future added value";
export type SpacesSubscriptionStatus = "COMPLIMENTARY_BRIDGE" | "LEGACY_ACTIVE" | "MISSING_STATE" | "NOT_SET" | "PAID_ACTIVE" | "PAID_INACTIVE" | "TRIAL_ACTIVE" | "TRIAL_EXPIRED" | "TRIAL_EXPIRING" | "%future added value";
export type rootShell_rootQuery$variables = {
  organizationCustomDomain: string;
};
export type rootShell_rootQuery$data = {
  readonly azureTenantOrganization: {
    readonly id: string;
  } | null | undefined;
  readonly customerReadinessSynced: boolean;
  readonly isAzureTenantInstalled: boolean;
  readonly me: {
    readonly id: string;
    readonly isOnboardingDone: boolean;
  };
  readonly organization: {
    readonly isOwnershipVerified: boolean;
    readonly isSsoTokenValid: boolean;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
    readonly type: {
      readonly type: OrganizationType;
    };
  } | null | undefined;
  readonly organizationSpacesSubscription: {
    readonly accessReason: SpacesAccessReasonCode;
    readonly canAcceptBookings: boolean;
    readonly canProtectExistingCommitments: boolean;
    readonly canUseProduct: boolean;
    readonly isComplimentaryBridge: boolean;
    readonly nextBillingAt: any | null | undefined;
    readonly remainingTrialDays: number;
    readonly subscriptionStatus: SpacesSubscriptionStatus;
    readonly trialEndsAt: any | null | undefined;
    readonly trialStartedAt: any | null | undefined;
    readonly upgradeRequired: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"appBar_query" | "leftSideNavigationMenu_query" | "observability_query">;
};
export type rootShell_rootQuery = {
  response: rootShell_rootQuery$data;
  variables: rootShell_rootQuery$variables;
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
  "name": "isOnboardingDone",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "customerReadinessSynced",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isAzureTenantInstalled",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "azureTenantOrganization",
  "plural": false,
  "selections": [
    (v1/*:: as any*/)
  ],
  "storageKey": null
},
v6 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "logoUrl",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isSsoTokenValid",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isOwnershipVerified",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTypeDetails",
  "kind": "LinkedField",
  "name": "type",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "type",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v12 = {
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
      "name": "accessReason",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "trialStartedAt",
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
      "name": "remainingTrialDays",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canUseProduct",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canAcceptBookings",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canProtectExistingCommitments",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "upgradeRequired",
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
    }
  ],
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "customDomain",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "rootShell_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
          (v2/*:: as any*/)
        ],
        "storageKey": null
      },
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      {
        "alias": null,
        "args": (v6/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v7/*:: as any*/),
          (v8/*:: as any*/),
          (v9/*:: as any*/),
          (v10/*:: as any*/),
          (v11/*:: as any*/)
        ],
        "storageKey": null
      },
      (v12/*:: as any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "appBar_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "leftSideNavigationMenu_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "observability_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "rootShell_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
          (v2/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "emails",
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
            "name": "givenName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "middleName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "familyName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "photoUrl",
            "storageKey": null
          },
          (v8/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "title",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      {
        "alias": null,
        "args": (v6/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v7/*:: as any*/),
          (v8/*:: as any*/),
          (v9/*:: as any*/),
          (v10/*:: as any*/),
          (v11/*:: as any*/),
          (v1/*:: as any*/),
          (v13/*:: as any*/),
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
            "kind": "ScalarField",
            "name": "canViewAnalytics",
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
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "free",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "earlyBird",
                "storageKey": null
              },
              (v1/*:: as any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      (v12/*:: as any*/),
      {
        "alias": null,
        "args": [
          {
            "kind": "Literal",
            "name": "types",
            "value": [
              "MARKETPLACE"
            ]
          }
        ],
        "concreteType": "MyOrganizationDetails",
        "kind": "LinkedField",
        "name": "myOrganizations",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "uniqueId",
            "storageKey": null
          },
          (v13/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/)
        ],
        "storageKey": "myOrganizations(types:[\"MARKETPLACE\"])"
      }
    ]
  },
  "params": {
    "cacheID": "a0e69087eb0ffb0976b0e4fca7acde5c",
    "id": null,
    "metadata": {},
    "name": "rootShell_rootQuery",
    "operationKind": "query",
    "text": "query rootShell_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  me {\n    id\n    isOnboardingDone\n  }\n  customerReadinessSynced\n  isAzureTenantInstalled\n  azureTenantOrganization {\n    id\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    logoUrl\n    name\n    isSsoTokenValid\n    isOwnershipVerified\n    type {\n      type\n    }\n    id\n  }\n  organizationSpacesSubscription(organizationId: $organizationCustomDomain) {\n    subscriptionStatus\n    accessReason\n    trialStartedAt\n    trialEndsAt\n    remainingTrialDays\n    canUseProduct\n    canAcceptBookings\n    canProtectExistingCommitments\n    upgradeRequired\n    isComplimentaryBridge\n    nextBillingAt\n  }\n  ...appBar_query\n  ...leftSideNavigationMenu_query\n  ...observability_query\n}\n\nfragment appBar_query on Query {\n  me {\n    id\n    emails\n    email\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  myOrganizations(types: [MARKETPLACE]) {\n    uniqueId\n    customDomain\n    logoUrl\n    name\n  }\n  ...mobileLeftSideNavigationMenu_query\n  ...newFeedbackDialog_query\n}\n\nfragment leftSideNavigationMenuContent_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    customDomain\n    type {\n      type\n    }\n    canModify\n    canViewAnalytics\n    activeOffering {\n      free\n      earlyBird\n      id\n    }\n  }\n}\n\nfragment leftSideNavigationMenu_query on Query {\n  ...leftSideNavigationMenuContent_query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment mobileLeftSideNavigationMenu_query on Query {\n  ...leftSideNavigationMenuContent_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n"
  }
};
})();

(node as any).hash = "4653c325889aaa9801ad2374bcbed7e1";

export default node;
