/**
 * @generated SignedSource<<ac3a5cfff140df7a78999f439ebb3031>>
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
export type addDeskDialog_rootQuery$variables = {
  multipleChoicesDeskTypesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  multipleChoicesZonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationId: string;
};
export type addDeskDialog_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesDeskTypes_query" | "multipleChoicesZones_query">;
};
export type addDeskDialog_rootQuery = {
  response: addDeskDialog_rootQuery$data;
  variables: addDeskDialog_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesDeskTypesSortingValues"
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
    "name": "addDeskDialog_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesDeskTypes_query"
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
    "name": "addDeskDialog_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "multipleChoicesDeskTypesSortingValues"
          },
          (v3/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "deskTypes",
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
    "cacheID": "6e0c0aab7cf26418e76e35d7954669f4",
    "id": null,
    "metadata": {},
    "name": "addDeskDialog_rootQuery",
    "operationKind": "query",
    "text": "query addDeskDialog_rootQuery(\n  $organizationId: String!\n  $multipleChoicesDeskTypesSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]\n) {\n  ...multipleChoicesDeskTypes_query\n  ...multipleChoicesZones_query\n}\n\nfragment multipleChoicesDeskTypes_query on Query {\n  deskTypes(where: {organizationId: $organizationId}, orderBy: $multipleChoicesDeskTypesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment multipleChoicesZones_query on Query {\n  zones(where: {organizationId: $organizationId}, orderBy: $multipleChoicesZonesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a2bb1ca14927bba08190722b70e0a4a8";

export default node;
