/**
 * @generated SignedSource<<2eaecbca2615eea720fd1a1f0e91e3f7>>
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
  readonly " $fragmentSpreads": FragmentRefs<"organizationAdmin_query">;
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
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
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
        "name": "organizationAdmin_query"
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
          (v3/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationPhysicalAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "formattedAddress",
                "storageKey": null
              },
              (v3/*:: as any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "ea1554872042b728b247376a5de618d2",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationTagsGroups_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationTagsGroups_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    id\n  }\n  ...organizationAdmin_query\n}\n\nfragment organizationAdmin_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    physicalAddress {\n      formattedAddress\n      id\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "8c4cb34b4bbf87d6f60e37ecd9693af0";

export default node;
