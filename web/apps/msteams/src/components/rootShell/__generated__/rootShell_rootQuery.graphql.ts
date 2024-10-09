/**
 * @generated SignedSource<<0be9450e18ccf961f6d7cbac7059450a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type rootShell_rootQuery$variables = Record<PropertyKey, never>;
export type rootShell_rootQuery$data = {
  readonly azureTenantAdminConsentUrl: string;
  readonly billingCustomerRecordSynced: boolean;
  readonly bookingCustomerRecordSynced: boolean;
  readonly isAzureTenantInstalled: boolean;
  readonly locationCustomerRecordSynced: boolean;
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly msTeamsCustomerRecordSynced: boolean;
  readonly notificationCustomerRecordSynced: boolean;
  readonly organizationCustomerRecordSynced: boolean;
  readonly paymentCustomerRecordSynced: boolean;
  readonly slackCustomerRecordSynced: boolean;
  readonly teamCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"observability_query">;
};
export type rootShell_rootQuery = {
  response: rootShell_rootQuery$data;
  variables: rootShell_rootQuery$variables;
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
  "name": "billingCustomerRecordSynced",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "bookingCustomerRecordSynced",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "locationCustomerRecordSynced",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "msTeamsCustomerRecordSynced",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "notificationCustomerRecordSynced",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "organizationCustomerRecordSynced",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentCustomerRecordSynced",
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
  "name": "isAzureTenantInstalled",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "azureTenantAdminConsentUrl",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
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
          (v0/*: any*/)
        ],
        "storageKey": null
      },
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v7/*: any*/),
      (v8/*: any*/),
      (v9/*: any*/),
      (v10/*: any*/),
      (v11/*: any*/),
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
          (v0/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerEmail",
            "kind": "LinkedField",
            "name": "email",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
              (v0/*: any*/)
            ],
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
          }
        ],
        "storageKey": null
      },
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v7/*: any*/),
      (v8/*: any*/),
      (v9/*: any*/),
      (v10/*: any*/),
      (v11/*: any*/)
    ]
  },
  "params": {
    "cacheID": "d6033b755b580961f9738aa7e850b30e",
    "id": null,
    "metadata": {},
    "name": "rootShell_rootQuery",
    "operationKind": "query",
    "text": "query rootShell_rootQuery {\n  me {\n    id\n  }\n  billingCustomerRecordSynced\n  bookingCustomerRecordSynced\n  locationCustomerRecordSynced\n  msTeamsCustomerRecordSynced\n  notificationCustomerRecordSynced\n  organizationCustomerRecordSynced\n  paymentCustomerRecordSynced\n  slackCustomerRecordSynced\n  teamCustomerRecordSynced\n  isAzureTenantInstalled\n  azureTenantAdminConsentUrl\n  ...observability_query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n"
  }
};
})();

(node as any).hash = "781bd6e437eb6cb67d2877e67a7c0cd3";

export default node;
