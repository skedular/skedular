/**
 * @generated SignedSource<<a380213b6df8d3a00585e6fcdbaeda3b>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationStoreFrontRootShell_rootQuery$variables = {
  organizationCustomDomain: string;
};
export type organizationStoreFrontRootShell_rootQuery$data = {
  readonly customerReadinessSynced: boolean;
  readonly me: {
    readonly id: string;
  };
  readonly organizationPublic: {
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"observability_query" | "organizationStoreFrontAppBar_query">;
};
export type organizationStoreFrontRootShell_rootQuery = {
  response: organizationStoreFrontRootShell_rootQuery$data;
  variables: organizationStoreFrontRootShell_rootQuery$variables;
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
  "name": "customerReadinessSynced",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "customDomain",
      "variableName": "organizationCustomDomain"
    }
  ],
  "concreteType": "OrganizationPublicDetails",
  "kind": "LinkedField",
  "name": "organizationPublic",
  "plural": false,
  "selections": [
    (v3/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "logoUrl",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationStoreFrontRootShell_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v1/*:: as any*/)
        ],
        "storageKey": null
      },
      (v2/*:: as any*/),
      (v4/*:: as any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationStoreFrontAppBar_query"
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
    "name": "organizationStoreFrontRootShell_rootQuery",
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
          (v3/*:: as any*/),
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
      (v4/*:: as any*/)
    ]
  },
  "params": {
    "cacheID": "0bd6380449eac36730bf4c6885ee9ee0",
    "id": null,
    "metadata": {},
    "name": "organizationStoreFrontRootShell_rootQuery",
    "operationKind": "query",
    "text": "query organizationStoreFrontRootShell_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  me {\n    id\n  }\n  customerReadinessSynced\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n    logoUrl\n  }\n  ...organizationStoreFrontAppBar_query\n  ...observability_query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n\nfragment organizationStoreFrontAppBar_query on Query {\n  me {\n    id\n    email\n    emails\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n  }\n  ...newFeedbackDialog_query\n}\n"
  }
};
})();

(node as any).hash = "f5448c34a0e44748dc9c4790629db5c0";

export default node;
