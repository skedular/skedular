/**
 * @generated SignedSource<<62adf7a62fffbeaf9ddfc7198a370483>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageHostOnboardingQuery$variables = Record<PropertyKey, never>;
export type pageHostOnboardingQuery$data = {
  readonly activeOrganizationTermsOfUse: {
    readonly id: string;
  };
  readonly myOrganizations: ReadonlyArray<{
    readonly uniqueId: string;
  }>;
};
export type pageHostOnboardingQuery = {
  response: pageHostOnboardingQuery$data;
  variables: pageHostOnboardingQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "OrganizationTermsOfUse",
    "kind": "LinkedField",
    "name": "activeOrganizationTermsOfUse",
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
      }
    ],
    "storageKey": "myOrganizations(types:[\"HOST\"])"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageHostOnboardingQuery",
    "selections": (v0/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageHostOnboardingQuery",
    "selections": (v0/*:: as any*/)
  },
  "params": {
    "cacheID": "c4b25aefccc0329122f50ee93be1932e",
    "id": null,
    "metadata": {},
    "name": "pageHostOnboardingQuery",
    "operationKind": "query",
    "text": "query pageHostOnboardingQuery {\n  activeOrganizationTermsOfUse {\n    id\n  }\n  myOrganizations(types: [HOST]) {\n    uniqueId\n  }\n}\n"
  }
};
})();

(node as any).hash = "fab216fb74409c7b80abf17cc94cc4b5";

export default node;
