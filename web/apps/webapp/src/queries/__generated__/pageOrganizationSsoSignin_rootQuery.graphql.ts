/**
 * @generated SignedSource<<277c736392c45574034ae0d74af1d07a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageOrganizationSsoSignin_rootQuery$variables = {
  organizationId: string;
  redirectUrl: string;
};
export type pageOrganizationSsoSignin_rootQuery$data = {
  readonly organization: {
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
  readonly ssoLoginUrl: string;
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
    "name": "organizationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "redirectUrl"
  }
],
v1 = {
  "kind": "Variable",
  "name": "id",
  "variableName": "organizationId"
},
v2 = [
  (v1/*: any*/)
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "logoUrl",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": [
    (v1/*: any*/),
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
        "args": (v2/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          (v4/*: any*/)
        ],
        "storageKey": null
      },
      (v5/*: any*/)
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
        "args": (v2/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
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
      },
      (v5/*: any*/)
    ]
  },
  "params": {
    "cacheID": "aebca35badea99232342d4727c94fa4d",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSsoSignin_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationSsoSignin_rootQuery(\n  $organizationId: String!\n  $redirectUrl: String!\n) {\n  organization(id: $organizationId) {\n    logoUrl\n    name\n    id\n  }\n  ssoLoginUrl(id: $organizationId, redirectUrl: $redirectUrl)\n}\n"
  }
};
})();

(node as any).hash = "ee0796b70b46f11a88177c91ebdc4c37";

export default node;
