/**
 * @generated SignedSource<<76dfda2816a448e997127015158e4953>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageOrganizationSsoSignin_rootQuery$variables = {
  organizationCustomDomain: string;
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
    "name": "organizationCustomDomain"
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
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
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
    "cacheID": "a49d054e63f213ef7bb15fce32553976",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSsoSignin_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationSsoSignin_rootQuery(\n  $organizationCustomDomain: String!\n  $redirectUrl: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    logoUrl\n    name\n    ssoLoginUrl(redirectUrl: $redirectUrl)\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "5044eeca6c6c8d60b380e81e569cf484";

export default node;
