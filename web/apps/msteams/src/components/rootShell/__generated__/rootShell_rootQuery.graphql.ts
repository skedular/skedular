/**
 * @generated SignedSource<<9512fbe6d9ecd501afa6f4d87c84251d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
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
};
export type rootShell_rootQuery = {
  response: rootShell_rootQuery$data;
  variables: rootShell_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "CustomerDetails",
    "kind": "LinkedField",
    "name": "me",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "id",
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "billingCustomerRecordSynced",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "bookingCustomerRecordSynced",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "locationCustomerRecordSynced",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "msTeamsCustomerRecordSynced",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "notificationCustomerRecordSynced",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "organizationCustomerRecordSynced",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "paymentCustomerRecordSynced",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "slackCustomerRecordSynced",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "teamCustomerRecordSynced",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "isAzureTenantInstalled",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "azureTenantAdminConsentUrl",
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "rootShell_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "rootShell_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "694e48a381e6ff43d3b34f0963b0f97b",
    "id": null,
    "metadata": {},
    "name": "rootShell_rootQuery",
    "operationKind": "query",
    "text": "query rootShell_rootQuery {\n  me {\n    id\n  }\n  billingCustomerRecordSynced\n  bookingCustomerRecordSynced\n  locationCustomerRecordSynced\n  msTeamsCustomerRecordSynced\n  notificationCustomerRecordSynced\n  organizationCustomerRecordSynced\n  paymentCustomerRecordSynced\n  slackCustomerRecordSynced\n  teamCustomerRecordSynced\n  isAzureTenantInstalled\n  azureTenantAdminConsentUrl\n}\n"
  }
};
})();

(node as any).hash = "ed1d6c99a2ecca14e8762cd3b589d74c";

export default node;
