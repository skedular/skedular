/**
 * @generated SignedSource<<a2099c89b37b0cdc72940e400af54d90>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationAdminZonesSectionQuery$variables = {
  organizationCustomDomain: string;
  zoneNameSearchText?: string | null | undefined;
};
export type organizationAdminZonesSectionQuery$data = {
  readonly me: {
    readonly id: string;
    readonly preferredZones: ReadonlyArray<{
      readonly id: string;
    }>;
  };
  readonly organization: {
    readonly zones: {
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
export type organizationAdminZonesSectionQuery = {
  response: organizationAdminZonesSectionQuery$data;
  variables: organizationAdminZonesSectionQuery$variables;
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
    "name": "zoneNameSearchText"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v1/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "preferredZones",
      "plural": true,
      "selections": [
        (v1/*: any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v3 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v4 = {
  "alias": null,
  "args": [
    {
      "kind": "Literal",
      "name": "first",
      "value": 100
    },
    {
      "fields": [
        {
          "kind": "Variable",
          "name": "nameContains",
          "variableName": "zoneNameSearchText"
        }
      ],
      "kind": "ObjectValue",
      "name": "where"
    }
  ],
  "concreteType": "ConnectionOfOrganizationTagEdge",
  "kind": "LinkedField",
  "name": "zones",
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
            (v1/*: any*/),
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdminZonesSectionQuery",
    "selections": [
      (v2/*: any*/),
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
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
    "name": "organizationAdminZonesSectionQuery",
    "selections": [
      (v2/*: any*/),
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v4/*: any*/),
          (v1/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "aba707d292b4fe9f95edd901e0dbee5d",
    "id": null,
    "metadata": {},
    "name": "organizationAdminZonesSectionQuery",
    "operationKind": "query",
    "text": "query organizationAdminZonesSectionQuery(\n  $organizationCustomDomain: String!\n  $zoneNameSearchText: String\n) {\n  me {\n    id\n    preferredZones {\n      id\n    }\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    zones(first: 100, where: {nameContains: $zoneNameSearchText}) {\n      edges {\n        node {\n          id\n          name\n          description\n          color\n        }\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "dbbb9d834196add00966df2baaf92823";

export default node;
