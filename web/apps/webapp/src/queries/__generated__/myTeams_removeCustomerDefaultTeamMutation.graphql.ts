/**
 * @generated SignedSource<<4a496ce14e530622750fe07224203f8a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerDefaultTeamInput = {
  clientMutationId?: string | null | undefined;
  teamId: string;
};
export type myTeams_removeCustomerDefaultTeamMutation$variables = {
  input: RemoveCustomerDefaultTeamInput;
};
export type myTeams_removeCustomerDefaultTeamMutation$data = {
  readonly removeCustomerDefaultTeam: {
    readonly customer: {
      readonly defaultTeams: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
    };
  } | null | undefined;
};
export type myTeams_removeCustomerDefaultTeamMutation = {
  response: myTeams_removeCustomerDefaultTeamMutation$data;
  variables: myTeams_removeCustomerDefaultTeamMutation$variables;
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
    "name": "myTeams_removeCustomerDefaultTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myTeams_removeCustomerDefaultTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "7b868d2230feecb9ea59776ba601f1ff",
    "id": null,
    "metadata": {},
    "name": "myTeams_removeCustomerDefaultTeamMutation",
    "operationKind": "mutation",
    "text": "mutation myTeams_removeCustomerDefaultTeamMutation(\n  $input: RemoveCustomerDefaultTeamInput!\n) {\n  removeCustomerDefaultTeam(input: $input) {\n    customer {\n      id\n      defaultTeams {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ac78eeb8462ff38b13cc2df417a527b8";

export default node;
