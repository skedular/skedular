/**
 * @generated SignedSource<<b1967130b2ecb8ff17d6437b14215e87>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationTagsGroups_rootQuery$variables = {
  organizationCustomDomain: string;
};
export type pageOrganizationTagsGroups_rootQuery$data = {
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationTagsGroups_query">;
};
export type pageOrganizationTagsGroups_rootQuery = {
  response: pageOrganizationTagsGroups_rootQuery$data;
  variables: pageOrganizationTagsGroups_rootQuery$variables;
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
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationTagsGroups_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationTagsGroups_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationTagsGroups_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
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
    "cacheID": "b551320707e2b64c229504d932b511bd",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationTagsGroups_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationTagsGroups_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    id\n  }\n  ...organizationTagsGroups_query\n}\n\nfragment organizationTagsGroups_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "5e8bf1113a640754a539f9aa12f3d389";

export default node;
