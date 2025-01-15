/**
 * @generated SignedSource<<b5509f7ad274dee6363bb45b1c8dc31d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationTagOrderField = "Description" | "Name" | "TagType" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type bulkAddDeskDialog_rootQuery$variables = {
  multipleChoicesCustomTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  multipleChoicesZonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationId: string;
};
export type bulkAddDeskDialog_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesCustomTags_query" | "multipleChoicesZones_query">;
};
export type bulkAddDeskDialog_rootQuery = {
  response: bulkAddDeskDialog_rootQuery$data;
  variables: bulkAddDeskDialog_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesCustomTagsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesZonesSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "fields": [
    {
      "kind": "Variable",
      "name": "organizationId",
      "variableName": "organizationId"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v4 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "totalCount",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "OrganizationTagEdge",
    "kind": "LinkedField",
    "name": "edges",
    "plural": true,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTagDetails",
        "kind": "LinkedField",
        "name": "node",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
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
            "name": "color",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "kind": "ClientExtension",
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "__id",
        "storageKey": null
      }
    ]
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "bulkAddDeskDialog_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesCustomTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesZones_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Operation",
    "name": "bulkAddDeskDialog_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "multipleChoicesCustomTagsSortingValues"
          },
          (v3/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "customTags",
        "plural": false,
        "selections": (v4/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "multipleChoicesZonesSortingValues"
          },
          (v3/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v4/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "1fc8b0fd3f4fc04eb9aedb75ffd7a0ff",
    "id": null,
    "metadata": {},
    "name": "bulkAddDeskDialog_rootQuery",
    "operationKind": "query",
    "text": "query bulkAddDeskDialog_rootQuery(\n  $organizationId: String!\n  $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]\n) {\n  ...multipleChoicesCustomTags_query\n  ...multipleChoicesZones_query\n}\n\nfragment multipleChoicesCustomTags_query on Query {\n  customTags(where: {organizationId: $organizationId}, orderBy: $multipleChoicesCustomTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n\nfragment multipleChoicesZones_query on Query {\n  zones(where: {organizationId: $organizationId}, orderBy: $multipleChoicesZonesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "da2733b123cd5206b2b95ac9272b8157";

export default node;
