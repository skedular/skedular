/**
 * @generated SignedSource<<d448d8f3352751ff75d38c8b30db15ed>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrganizationType = "INDIVIDUAL" | "MARKETPLACE" | "PRIVATE" | "%future added value";
export type rootShell_rootQuery$variables = {
  organizationUniqueAlphanumericName: string;
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
  readonly myOrganizations: ReadonlyArray<{
    readonly id: string;
  }>;
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
  readonly pendingOrganizationInvitationsCount: number;
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
    "name": "organizationUniqueAlphanumericName"
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
v3 = [
  (v1/*: any*/)
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "bookingCustomerRecordSynced",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "locationCustomerRecordSynced",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "marketplaceCustomerRecordSynced",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "msTeamsCustomerRecordSynced",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "organizationCustomerRecordSynced",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "slackCustomerRecordSynced",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "teamCustomerRecordSynced",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "coreCustomerRecordSynced",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "pendingOrganizationInvitationsCount",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isAzureTenantInstalled",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "azureTenantOrganization",
  "plural": false,
  "selections": (v3/*: any*/),
  "storageKey": null
},
v15 = [
  {
    "kind": "Variable",
    "name": "uniqueAlphanumericName",
    "variableName": "organizationUniqueAlphanumericName"
  }
],
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "logoUrl",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isSsoTokenValid",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isOwnershipVerified",
  "storageKey": null
},
v20 = {
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
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueAlphanumericName",
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canViewAnalytics",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
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
          (v1/*: any*/),
          (v2/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "myOrganizations",
        "plural": true,
        "selections": (v3/*: any*/),
        "storageKey": null
      },
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
      {
        "alias": null,
        "args": (v15/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v16/*: any*/),
          (v17/*: any*/),
          (v18/*: any*/),
          (v19/*: any*/),
          (v20/*: any*/)
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
    "argumentDefinitions": (v0/*: any*/),
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
          (v1/*: any*/),
          (v2/*: any*/),
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
          (v17/*: any*/),
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
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "myOrganizations",
        "plural": true,
        "selections": [
          (v1/*: any*/),
          (v21/*: any*/),
          (v16/*: any*/),
          (v17/*: any*/),
          (v22/*: any*/),
          (v23/*: any*/)
        ],
        "storageKey": null
      },
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
      {
        "alias": null,
        "args": (v15/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v16/*: any*/),
          (v17/*: any*/),
          (v18/*: any*/),
          (v19/*: any*/),
          (v20/*: any*/),
          (v1/*: any*/),
          (v21/*: any*/),
          (v22/*: any*/),
          (v23/*: any*/),
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
              (v1/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "pendingTeamInvitationsCount",
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "fe7f376207e8aa5fff5809e95da47a0b",
    "id": null,
    "metadata": {},
    "name": "rootShell_rootQuery",
    "operationKind": "query",
    "text": "query rootShell_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n) {\n  me {\n    id\n    isOnboardingDone\n  }\n  myOrganizations {\n    id\n  }\n  bookingCustomerRecordSynced\n  locationCustomerRecordSynced\n  marketplaceCustomerRecordSynced\n  msTeamsCustomerRecordSynced\n  organizationCustomerRecordSynced\n  slackCustomerRecordSynced\n  teamCustomerRecordSynced\n  coreCustomerRecordSynced\n  pendingOrganizationInvitationsCount\n  isAzureTenantInstalled\n  azureTenantOrganization {\n    id\n  }\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    logoUrl\n    name\n    isSsoTokenValid\n    isOwnershipVerified\n    type {\n      type\n    }\n    id\n  }\n  ...appBar_query\n  ...leftSideNavigationMenu_query\n  ...observability_query\n}\n\nfragment appBar_query on Query {\n  me {\n    id\n    emails\n    email\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  myOrganizations {\n    id\n    uniqueAlphanumericName\n    logoUrl\n    name\n    canModify\n    canViewAnalytics\n  }\n  pendingOrganizationInvitationsCount\n  pendingTeamInvitationsCount\n  ...mobileLeftSideNavigationMenu_query\n  ...newFeedbackDialog_query\n}\n\nfragment leftSideNavigationMenuContent_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    id\n    uniqueAlphanumericName\n    type {\n      type\n    }\n    canModify\n    canViewAnalytics\n    activeOffering {\n      free\n      earlyBird\n      id\n    }\n  }\n}\n\nfragment leftSideNavigationMenu_query on Query {\n  ...leftSideNavigationMenuContent_query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment mobileLeftSideNavigationMenu_query on Query {\n  ...leftSideNavigationMenuContent_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n"
  }
};
})();

(node as any).hash = "e9290777a9b8bc8caa55154b24b243be";

export default node;
