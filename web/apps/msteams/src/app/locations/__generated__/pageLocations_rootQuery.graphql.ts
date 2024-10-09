/**
 * @generated SignedSource<<e7b8d7eaf3d06081ad81387cc719ed27>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageLocations_rootQuery$variables = Record<PropertyKey, never>;
export type pageLocations_rootQuery$data = {
  readonly locationCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"rootShell_query">;
};
export type pageLocations_rootQuery = {
  response: pageLocations_rootQuery$data;
  variables: pageLocations_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "locationCustomerRecordSynced",
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
    "name": "pageLocations_rootQuery",
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
    "name": "pageLocations_rootQuery",
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
              (v1/*: any*/)
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
    ]
  },
  "params": {
    "cacheID": "547364ced2396711e96090fd7494c4b2",
    "id": null,
    "metadata": {},
    "name": "pageLocations_rootQuery",
    "operationKind": "query",
    "text": "query pageLocations_rootQuery {\n  locationCustomerRecordSynced\n  ...rootShell_query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n\nfragment rootShell_query on Query {\n  me {\n    id\n  }\n  isAzureTenantInstalled\n  azureTenantAdminConsentUrl\n  ...observability_query\n}\n"
  }
};
})();

(node as any).hash = "c29a2d889bda6ccd72f888a34a5178ea";

export default node;
