/**
 * @generated SignedSource<<761275d1699c6179f32c985b170267c3>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrganizationType = "INDIVIDUAL" | "MARKETPLACE" | "PRIVATE" | "%future added value";
export type rootShell_rootQuery$variables = {
  organizationCustomDomain: string;
};
export type rootShell_rootQuery$data = {
  readonly azureTenantOrganization: {
    readonly id: string;
  } | null | undefined;
  readonly bookingCustomerRecordSynced: boolean;
  readonly coreCustomerRecordSynced: boolean;
  readonly isAzureTenantInstalled: boolean;
  readonly locationCustomerRecordSynced: boolean;
  readonly marketplaceCustomerRecordSynced: boolean;
  readonly me: {
    readonly id: string;
    readonly isOnboardingDone: boolean;
  };
  readonly msTeamsCustomerRecordSynced: boolean;
  readonly organization: {
    readonly isOwnershipVerified: boolean;
    readonly isSsoTokenValid: boolean;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
    readonly type: {
      readonly type: OrganizationType;
    };
  } | null | undefined;
  readonly organizationCustomerRecordSynced: boolean;
  readonly slackCustomerRecordSynced: boolean;
  readonly teamCustomerRecordSynced: boolean;
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
  "name": "bookingCustomerRecordSynced",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "locationCustomerRecordSynced",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "marketplaceCustomerRecordSynced",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "msTeamsCustomerRecordSynced",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "organizationCustomerRecordSynced",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "slackCustomerRecordSynced",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "teamCustomerRecordSynced",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "coreCustomerRecordSynced",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isAzureTenantInstalled",
  "storageKey": null
},
v12 = {
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
v13 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "logoUrl",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isSsoTokenValid",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isOwnershipVerified",
  "storageKey": null
},
v18 = {
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
v19 = {
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
      (v6/*:: as any*/),
      (v7/*:: as any*/),
      (v8/*:: as any*/),
      (v9/*:: as any*/),
      (v10/*:: as any*/),
      (v11/*:: as any*/),
      (v12/*:: as any*/),
      {
        "alias": null,
        "args": (v13/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v14/*:: as any*/),
          (v15/*:: as any*/),
          (v16/*:: as any*/),
          (v17/*:: as any*/),
          (v18/*:: as any*/)
        ],
        "storageKey": null
      },
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
          (v15/*:: as any*/),
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
      (v6/*:: as any*/),
      (v7/*:: as any*/),
      (v8/*:: as any*/),
      (v9/*:: as any*/),
      (v10/*:: as any*/),
      (v11/*:: as any*/),
      (v12/*:: as any*/),
      {
        "alias": null,
        "args": (v13/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v14/*:: as any*/),
          (v15/*:: as any*/),
          (v16/*:: as any*/),
          (v17/*:: as any*/),
          (v18/*:: as any*/),
          (v1/*:: as any*/),
          (v19/*:: as any*/),
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
      {
        "alias": null,
        "args": [
          {
            "kind": "Literal",
            "name": "types",
            "value": [
              "MARKETPLACE",
              "INDIVIDUAL"
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
          (v19/*:: as any*/),
          (v14/*:: as any*/),
          (v15/*:: as any*/)
        ],
        "storageKey": "myOrganizations(types:[\"MARKETPLACE\",\"INDIVIDUAL\"])"
      }
    ]
  },
  "params": {
    "cacheID": "775da3ad2fe47eed9052037747be3474",
    "id": null,
    "metadata": {},
    "name": "rootShell_rootQuery",
    "operationKind": "query",
    "text": "query rootShell_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  me {\n    id\n    isOnboardingDone\n  }\n  bookingCustomerRecordSynced\n  locationCustomerRecordSynced\n  marketplaceCustomerRecordSynced\n  msTeamsCustomerRecordSynced\n  organizationCustomerRecordSynced\n  slackCustomerRecordSynced\n  teamCustomerRecordSynced\n  coreCustomerRecordSynced\n  isAzureTenantInstalled\n  azureTenantOrganization {\n    id\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    logoUrl\n    name\n    isSsoTokenValid\n    isOwnershipVerified\n    type {\n      type\n    }\n    id\n  }\n  ...appBar_query\n  ...leftSideNavigationMenu_query\n  ...observability_query\n}\n\nfragment appBar_query on Query {\n  me {\n    id\n    emails\n    email\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  myOrganizations(types: [MARKETPLACE, INDIVIDUAL]) {\n    uniqueId\n    customDomain\n    logoUrl\n    name\n  }\n  ...mobileLeftSideNavigationMenu_query\n  ...newFeedbackDialog_query\n}\n\nfragment leftSideNavigationMenuContent_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    customDomain\n    type {\n      type\n    }\n    canModify\n    canViewAnalytics\n    activeOffering {\n      free\n      earlyBird\n      id\n    }\n  }\n}\n\nfragment leftSideNavigationMenu_query on Query {\n  ...leftSideNavigationMenuContent_query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment mobileLeftSideNavigationMenu_query on Query {\n  ...leftSideNavigationMenuContent_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n"
  }
};
})();

(node as any).hash = "cd836b40dc359f4aeb8dfacb53e4824c";

export default node;
