/**
 * @generated SignedSource<<5290bc22ec747c3de6963494e546179e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageOrganizationSsoSignin_rootQuery$variables = {
  organizationUniqueAlphanumericName: string;
  redirectUrl: string;
};
export type pageOrganizationSsoSignin_rootQuery$data = {
  readonly organization: {
    readonly logoUrl: string | null | undefined;
    readonly name: string;
    readonly ssoLoginUrl: string;
  } | null | undefined;
};
export type pageOrganizationSsoSignin_rootQuery = {
  response: pageOrganizationSsoSignin_rootQuery$data;
  variables: pageOrganizationSsoSignin_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationUniqueAlphanumericName"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "redirectUrl"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "uniqueAlphanumericName",
    "variableName": "organizationUniqueAlphanumericName"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "logoUrl",
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
      "name": "redirectUrl",
      "variableName": "redirectUrl"
    }
  ],
  "kind": "ScalarField",
  "name": "ssoLoginUrl",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationSsoSignin_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageOrganizationSsoSignin_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "82adcc2b96f2bde8cee51cef5e76ef11",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSsoSignin_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationSsoSignin_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $redirectUrl: String!\n) {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    logoUrl\n    name\n    ssoLoginUrl(redirectUrl: $redirectUrl)\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "9daf75e80ba3f094483573ab29b7280a";

export default node;
