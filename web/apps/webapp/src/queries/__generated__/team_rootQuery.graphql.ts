/**
 * @generated SignedSource<<a60d4a6a4ea650c4911a48b05d58424c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type team_rootQuery$variables = {
  organizationExists: boolean;
  organizationId: string;
  teamId: string;
};
export type team_rootQuery$data = {
  readonly organization?: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly team: {
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
  } | null | undefined;
};
export type team_rootQuery = {
  response: team_rootQuery$data;
  variables: team_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamId"
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
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "teamId"
      }
    ],
    "concreteType": "TeamDetails",
    "kind": "LinkedField",
    "name": "team",
    "plural": false,
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
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "uniqueId",
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
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "team_rootQuery",
    "selections": (v5/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "team_rootQuery",
    "selections": (v5/*: any*/)
  },
  "params": {
    "cacheID": "95e953f0635367e0b6406d509903d31b",
    "id": null,
    "metadata": {},
    "name": "team_rootQuery",
    "operationKind": "query",
    "text": "query team_rootQuery(\n  $organizationId: String!\n  $organizationExists: Boolean!\n  $teamId: String!\n) {\n  organization(id: $organizationId) @include(if: $organizationExists) {\n    id\n    name\n  }\n  team(id: $teamId) {\n    id\n    name\n    organization {\n      uniqueId\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0113139c76bb85baa37874e54bd5742d";

export default node;
