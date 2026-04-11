/**
 * @generated SignedSource<<48c376095193332aff688d3cabb86848>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationAdminTagsSectionQuery$variables = {
  customTagNameSearchText?: string | null | undefined;
  organizationCustomDomain: string;
};
export type organizationAdminTagsSectionQuery$data = {
  readonly me: {
    readonly id: string;
    readonly preferredCustomTags: ReadonlyArray<{
      readonly id: string;
    }>;
  };
  readonly organization: {
    readonly customTags: {
      readonly __id: string;
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly color: string | null | undefined;
          readonly description: string | null | undefined;
          readonly id: string;
          readonly name: string;
        };
      }>;
    };
  } | null | undefined;
};
export type organizationAdminTagsSectionQuery = {
  response: organizationAdminTagsSectionQuery$data;
  variables: organizationAdminTagsSectionQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagNameSearchText"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v2/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "preferredCustomTags",
      "plural": true,
      "selections": [
        (v2/*: any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v4 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v5 = {
  "alias": null,
  "args": [
    {
      "kind": "Literal",
      "name": "first",
      "value": 100
    },
    {
      "kind": "Literal",
      "name": "orderBy",
      "value": [
        {
          "direction": "ASCENDING",
          "field": "NAME"
        }
      ]
    },
    {
      "fields": [
        {
          "kind": "Variable",
          "name": "nameContains",
          "variableName": "customTagNameSearchText"
        }
      ],
      "kind": "ObjectValue",
      "name": "where"
    }
  ],
  "concreteType": "ConnectionOfOrganizationTagEdge",
  "kind": "LinkedField",
  "name": "customTags",
  "plural": false,
  "selections": [
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
            (v2/*: any*/),
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
              "name": "description",
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
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdminTagsSectionQuery",
    "selections": [
      (v3/*: any*/),
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v5/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "organizationAdminTagsSectionQuery",
    "selections": [
      (v3/*: any*/),
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          (v2/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "85c281603e91117c5edcbe7fccb3f91c",
    "id": null,
    "metadata": {},
    "name": "organizationAdminTagsSectionQuery",
    "operationKind": "query",
    "text": "query organizationAdminTagsSectionQuery(\n  $organizationCustomDomain: String!\n  $customTagNameSearchText: String\n) {\n  me {\n    id\n    preferredCustomTags {\n      id\n    }\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    customTags(first: 100, where: {nameContains: $customTagNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n      edges {\n        node {\n          id\n          name\n          description\n          color\n        }\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "69a7b62bd89b7d7cea31670957e047ba";

export default node;
