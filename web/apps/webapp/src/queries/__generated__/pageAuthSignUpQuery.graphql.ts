/**
 * @generated SignedSource<<e124ad8184141329664e46d23b36aebc>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageAuthSignUpQuery$variables = {
  organizationCustomDomain: string;
};
export type pageAuthSignUpQuery$data = {
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
export type pageAuthSignUpQuery = {
  response: pageAuthSignUpQuery$data;
  variables: pageAuthSignUpQuery$variables;
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
    "name": "pageAuthSignUpQuery",
    "selections": (v2/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageAuthSignUpQuery",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "7f5663081e810afe8a551a16a2c3d54a",
    "id": null,
    "metadata": {},
    "name": "pageAuthSignUpQuery",
    "operationKind": "query",
    "text": "query pageAuthSignUpQuery(\n  $organizationCustomDomain: String!\n) {\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n    logoUrl\n    featureImages {\n      original {\n        url\n      }\n      thumbnail {\n        url\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cea08c6c5f0095a84320aabff34ae5f2";

export default node;
