/**
 * @generated SignedSource<<08be10b2f4099c48f549e7a6f05023cd>>
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
export type myTeamCard_removeCustomerDefaultTeamMutation$variables = {
  input: RemoveCustomerDefaultTeamInput;
};
export type myTeamCard_removeCustomerDefaultTeamMutation$data = {
  readonly removeCustomerDefaultTeam: {
    readonly customer: {
      readonly defaultTeams: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
    };
  } | null | undefined;
};
export type myTeamCard_removeCustomerDefaultTeamMutation = {
  response: myTeamCard_removeCustomerDefaultTeamMutation$data;
  variables: myTeamCard_removeCustomerDefaultTeamMutation$variables;
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
    "name": "myTeamCard_removeCustomerDefaultTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myTeamCard_removeCustomerDefaultTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "6686139e8d8d0691c0808c75b205708e",
    "id": null,
    "metadata": {},
    "name": "myTeamCard_removeCustomerDefaultTeamMutation",
    "operationKind": "mutation",
    "text": "mutation myTeamCard_removeCustomerDefaultTeamMutation(\n  $input: RemoveCustomerDefaultTeamInput!\n) {\n  removeCustomerDefaultTeam(input: $input) {\n    customer {\n      id\n      defaultTeams {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "87fb2fa3f4c2030a6c855ac5fd10372d";

export default node;
