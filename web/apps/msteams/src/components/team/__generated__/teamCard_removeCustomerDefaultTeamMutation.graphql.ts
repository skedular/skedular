/**
 * @generated SignedSource<<1270b6c4886010f3c418dcd2da31042c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type RemoveCustomerDefaultTeamInput = {
  clientMutationId?: string | null | undefined;
  teamId: string;
};
export type teamCard_removeCustomerDefaultTeamMutation$variables = {
  input: RemoveCustomerDefaultTeamInput;
};
export type teamCard_removeCustomerDefaultTeamMutation$data = {
  readonly removeCustomerDefaultTeam: {
    readonly customer: {
      readonly defaultTeams: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
    };
  } | null | undefined;
};
export type teamCard_removeCustomerDefaultTeamMutation = {
  response: teamCard_removeCustomerDefaultTeamMutation$data;
  variables: teamCard_removeCustomerDefaultTeamMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "CustomerPayload",
    "kind": "LinkedField",
    "name": "removeCustomerDefaultTeam",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "customer",
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
            "concreteType": "CustomerTeamDetails",
            "kind": "LinkedField",
            "name": "defaultTeams",
            "plural": true,
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
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "teamCard_removeCustomerDefaultTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamCard_removeCustomerDefaultTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "3c4ac61965fc2f98bdfa0f3d490aa2e3",
    "id": null,
    "metadata": {},
    "name": "teamCard_removeCustomerDefaultTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamCard_removeCustomerDefaultTeamMutation(\n  $input: RemoveCustomerDefaultTeamInput!\n) {\n  removeCustomerDefaultTeam(input: $input) {\n    customer {\n      id\n      defaultTeams {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5c9d0d6b0f75b0e19841ac3fc5d3813c";

export default node;
