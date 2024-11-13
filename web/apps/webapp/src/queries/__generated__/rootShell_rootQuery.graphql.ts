/**
 * @generated SignedSource<<478b2284984364f43269b1497782df3c>>
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
  readonly billingCustomerRecordSynced: boolean;
  readonly bookingCustomerRecordSynced: boolean;
  readonly locationCustomerRecordSynced: boolean;
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly msTeamsCustomerRecordSynced: boolean;
  readonly myOrganizations: ReadonlyArray<{
    readonly id: string;
  }> | null | undefined;
  readonly notificationCustomerRecordSynced: boolean;
  readonly organizationCustomerRecordSynced: boolean;
  readonly paymentCustomerRecordSynced: boolean;
  readonly slackCustomerRecordSynced: boolean;
  readonly teamCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"appBar_query">;
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
v1 = [
  (v0/*: any*/)
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "billingCustomerRecordSynced",
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
  "name": "msTeamsCustomerRecordSynced",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "notificationCustomerRecordSynced",
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
  "name": "paymentCustomerRecordSynced",
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
  "name": "name",
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
        "selections": (v1/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "myOrganizations",
        "plural": true,
        "selections": (v1/*: any*/),
        "storageKey": null
      },
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v7/*: any*/),
      (v8/*: any*/),
      (v9/*: any*/),
      (v10/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "appBar_query"
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
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "verified",
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
          (v11/*: any*/)
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
          (v0/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "logoUrl",
            "storageKey": null
          },
          (v11/*: any*/),
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
          }
        ],
        "storageKey": null
      },
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v7/*: any*/),
      (v8/*: any*/),
      (v9/*: any*/),
      (v10/*: any*/)
    ]
  },
  "params": {
    "cacheID": "82e4ab02b798af27f78912e4b05b3e7f",
    "id": null,
    "metadata": {},
    "name": "rootShell_rootQuery",
    "operationKind": "query",
    "text": "query rootShell_rootQuery {\n  me {\n    id\n  }\n  myOrganizations {\n    id\n  }\n  billingCustomerRecordSynced\n  bookingCustomerRecordSynced\n  locationCustomerRecordSynced\n  msTeamsCustomerRecordSynced\n  notificationCustomerRecordSynced\n  organizationCustomerRecordSynced\n  paymentCustomerRecordSynced\n  slackCustomerRecordSynced\n  teamCustomerRecordSynced\n  ...appBar_query\n}\n\nfragment appBar_query on Query {\n  me {\n    email {\n      email\n      verified\n      id\n    }\n    givenName\n    middleName\n    familyName\n    photoUrl\n    id\n  }\n  myOrganizations {\n    id\n    logoUrl\n    name\n    canModify\n    canViewAnalytics\n  }\n  ...newFeedbackDialog_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "9b04fd6ba85ea9419fef41ddd4ec94e6";

export default node;
