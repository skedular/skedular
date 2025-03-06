/**
 * @generated SignedSource<<ad917acb630363a034f685f3b46a58fd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerPreferredTeamInput = {
  clientMutationId?: string | null | undefined;
  teamId: string;
};
export type teamCard_addCustomerPreferredTeamMutation$variables = {
  input: AddCustomerPreferredTeamInput;
};
export type teamCard_addCustomerPreferredTeamMutation$data = {
  readonly addCustomerPreferredTeam: {
    readonly customer: {
      readonly id: string;
      readonly preferredTeams: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type teamCard_addCustomerPreferredTeamMutation = {
  response: teamCard_addCustomerPreferredTeamMutation$data;
  variables: teamCard_addCustomerPreferredTeamMutation$variables;
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
    "name": "addCustomerPreferredTeam",
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
            "name": "preferredTeams",
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
    "name": "teamCard_addCustomerPreferredTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamCard_addCustomerPreferredTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "7f3d35afb7383e5a9a8c80edeb3ef766",
    "id": null,
    "metadata": {},
    "name": "teamCard_addCustomerPreferredTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamCard_addCustomerPreferredTeamMutation(\n  $input: AddCustomerPreferredTeamInput!\n) {\n  addCustomerPreferredTeam(input: $input) {\n    customer {\n      id\n      preferredTeams {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e582ebae67508caa65ceb85842e36ffd";

export default node;
