/**
 * @generated SignedSource<<0ad59927f280d5bd20f1f6ec124c59f2>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageAuthSignInQuery$variables = {
  organizationCustomDomain: string;
};
export type pageAuthSignInQuery$data = {
  readonly organizationPublic: {
    readonly featureImages: ReadonlyArray<{
      readonly original: {
        readonly url: string;
      } | null | undefined;
      readonly thumbnail: {
        readonly url: string;
      } | null | undefined;
    }>;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
};
export type pageAuthSignInQuery = {
  response: pageAuthSignInQuery$data;
  variables: pageAuthSignInQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  }
],
v1 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "url",
    "storageKey": null
  }
],
v2 = [
  {
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
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "name",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "logoUrl",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CdnImageFile",
        "kind": "LinkedField",
        "name": "featureImages",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "CdnFile",
            "kind": "LinkedField",
            "name": "original",
            "plural": false,
            "selections": (v1/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CdnFile",
            "kind": "LinkedField",
            "name": "thumbnail",
            "plural": false,
            "selections": (v1/*:: as any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageAuthSignInQuery",
    "selections": (v2/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageAuthSignInQuery",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "083296e5c59c10d29b435f01fabbd3a6",
    "id": null,
    "metadata": {},
    "name": "pageAuthSignInQuery",
    "operationKind": "query",
    "text": "query pageAuthSignInQuery(\n  $organizationCustomDomain: String!\n) {\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n    logoUrl\n    featureImages {\n      original {\n        url\n      }\n      thumbnail {\n        url\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e960400c67ed4f41550728a43e2b492c";

export default node;
