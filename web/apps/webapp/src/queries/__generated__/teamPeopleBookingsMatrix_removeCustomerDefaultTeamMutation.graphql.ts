/**
 * @generated SignedSource<<7bc8b85c26e1d355b3d39841e27f7f1d>>
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
export type teamPeopleBookingsMatrix_removeCustomerDefaultTeamMutation$variables = {
  input: RemoveCustomerDefaultTeamInput;
};
export type teamPeopleBookingsMatrix_removeCustomerDefaultTeamMutation$data = {
  readonly removeCustomerDefaultTeam: {
    readonly customer: {
      readonly defaultTeams: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
    };
  } | null | undefined;
};
export type teamPeopleBookingsMatrix_removeCustomerDefaultTeamMutation = {
  response: teamPeopleBookingsMatrix_removeCustomerDefaultTeamMutation$data;
  variables: teamPeopleBookingsMatrix_removeCustomerDefaultTeamMutation$variables;
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
    "name": "teamPeopleBookingsMatrix_removeCustomerDefaultTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamPeopleBookingsMatrix_removeCustomerDefaultTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "7fb7549caafbf6415b8e25b039745d6b",
    "id": null,
    "metadata": {},
    "name": "teamPeopleBookingsMatrix_removeCustomerDefaultTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamPeopleBookingsMatrix_removeCustomerDefaultTeamMutation(\n  $input: RemoveCustomerDefaultTeamInput!\n) {\n  removeCustomerDefaultTeam(input: $input) {\n    customer {\n      id\n      defaultTeams {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "91dceab03e435dcf09827f4e19505a17";

export default node;
