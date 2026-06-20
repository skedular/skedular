/**
 * @generated SignedSource<<95872aa98561803c9ebc05661ffcfb53>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type noOrganizationRootShell_rootQuery$variables = Record<PropertyKey, never>;
export type noOrganizationRootShell_rootQuery$data = {
  readonly azureTenantOrganization: {
    readonly id: string;
  } | null | undefined;
  readonly customerReadinessSynced: boolean;
  readonly isAzureTenantInstalled: boolean;
  readonly me: {
    readonly id: string;
    readonly isOnboardingDone: boolean;
  };
  readonly " $fragmentSpreads": FragmentRefs<"noOrganizationAppBar_query" | "observability_query">;
};
export type noOrganizationRootShell_rootQuery = {
  response: noOrganizationRootShell_rootQuery$data;
  variables: noOrganizationRootShell_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
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
  "name": "isOnboardingDone",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "customerReadinessSynced",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isAzureTenantInstalled",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "azureTenantOrganization",
  "plural": false,
  "selections": [
    (v0/*:: as any*/)
  ],
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "noOrganizationRootShell_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v0/*:: as any*/),
          (v1/*:: as any*/)
        ],
        "storageKey": null
      },
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "noOrganizationAppBar_query"
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
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "noOrganizationRootShell_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v0/*:: as any*/),
          (v1/*:: as any*/),
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
            "name": "emails",
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
          (v5/*:: as any*/),
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
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      {
        "alias": null,
        "args": [
          {
            "kind": "Literal",
            "name": "types",
            "value": [
              "HOST"
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "customDomain",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "logoUrl",
            "storageKey": null
          },
          (v5/*:: as any*/)
        ],
        "storageKey": "myOrganizations(types:[\"HOST\"])"
      }
    ]
  },
  "params": {
    "cacheID": "dbe5bc70e2b04c2b35a3d9d8bba77c1a",
    "id": null,
    "metadata": {},
    "name": "noOrganizationRootShell_rootQuery",
    "operationKind": "query",
    "text": "query noOrganizationRootShell_rootQuery {\n  me {\n    id\n    isOnboardingDone\n  }\n  customerReadinessSynced\n  isAzureTenantInstalled\n  azureTenantOrganization {\n    id\n  }\n  ...noOrganizationAppBar_query\n  ...observability_query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment noOrganizationAppBar_query on Query {\n  me {\n    id\n    email\n    emails\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  myOrganizations(types: [HOST]) {\n    uniqueId\n    customDomain\n    logoUrl\n    name\n  }\n  ...newFeedbackDialog_query\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n"
  }
};
})();

(node as any).hash = "38e02ef46e8bcfd4491ddf09c3a51528";

export default node;
