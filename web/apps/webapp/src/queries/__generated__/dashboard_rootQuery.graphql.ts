/**
 * @generated SignedSource<<150c609fe9367607336e4a85df6c7aad>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type dashboard_rootQuery$variables = {
  nullableOrganizationId?: string | null | undefined;
  organizationExists: boolean;
  organizationId: string;
};
export type dashboard_rootQuery$data = {
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
  }> | null | undefined;
  readonly myTeams: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
  }> | null | undefined;
  readonly organization?: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type dashboard_rootQuery = {
  response: dashboard_rootQuery$data;
  variables: dashboard_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "nullableOrganizationId"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = [
  {
    "kind": "Variable",
    "name": "organizationId",
    "variableName": "nullableOrganizationId"
  }
],
v6 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v4/*: any*/)
],
v7 = [
  {
    "condition": "organizationExists",
    "kind": "Condition",
    "passingValue": true,
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "organizationId"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          (v4/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  {
    "alias": null,
    "args": (v5/*: any*/),
    "concreteType": "LocationDetails",
    "kind": "LinkedField",
    "name": "myLocations",
    "plural": true,
    "selections": [
      (v3/*: any*/),
      (v4/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationOrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v6/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": (v5/*: any*/),
    "concreteType": "TeamDetails",
    "kind": "LinkedField",
    "name": "myTeams",
    "plural": true,
    "selections": [
      (v3/*: any*/),
      (v4/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "TeamOrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v6/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
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
    "name": "dashboard_rootQuery",
    "selections": (v7/*: any*/),
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
    "name": "dashboard_rootQuery",
    "selections": (v7/*: any*/)
  },
  "params": {
    "cacheID": "11069e721dfac04f4319fc1586ad375f",
    "id": null,
    "metadata": {},
    "name": "dashboard_rootQuery",
    "operationKind": "query",
    "text": "query dashboard_rootQuery(\n  $organizationId: String!\n  $nullableOrganizationId: String\n  $organizationExists: Boolean!\n) {\n  organization(id: $organizationId) @include(if: $organizationExists) {\n    id\n    name\n  }\n  myLocations(organizationId: $nullableOrganizationId) {\n    id\n    name\n    organization {\n      uniqueId\n      name\n    }\n  }\n  myTeams(organizationId: $nullableOrganizationId) {\n    id\n    name\n    organization {\n      uniqueId\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "dd81a42b9b17777e4ee4f406159746a4";

export default node;
