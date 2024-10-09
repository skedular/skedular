/**
 * @generated SignedSource<<5bf490dced15bb73b3932348f15fa0b6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageAddOrganization_rootQuery$variables = Record<PropertyKey, never>;
export type pageAddOrganization_rootQuery$data = {
  readonly organizationCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"rootShell_query">;
};
export type pageAddOrganization_rootQuery = {
  response: pageAddOrganization_rootQuery$data;
  variables: pageAddOrganization_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "organizationCustomerRecordSynced",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageAddOrganization_rootQuery",
    "selections": [
      (v0/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "rootShell_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageAddOrganization_rootQuery",
    "selections": [
      (v0/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v1/*: any*/),
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
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "verified",
                "storageKey": null
              }
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
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "photoUrl",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "7b58d513506b31ea48805b47df31aeac",
    "id": null,
    "metadata": {},
    "name": "pageAddOrganization_rootQuery",
    "operationKind": "query",
    "text": "query pageAddOrganization_rootQuery {\n  organizationCustomerRecordSynced\n  ...rootShell_query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment mainRootLayout_query on Query {\n  me {\n    email {\n      email\n      verified\n      id\n    }\n    givenName\n    middleName\n    familyName\n    photoUrl\n    id\n  }\n  ...newFeedbackDialog_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n\nfragment rootShell_query on Query {\n  me {\n    id\n  }\n  ...observability_query\n  ...mainRootLayout_query\n}\n"
  }
};
})();

(node as any).hash = "67382161915140d92a12a689cedba7f8";

export default node;
