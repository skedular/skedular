/**
 * @generated SignedSource<<93490a8ab97c1e8abd1468558aa61159>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPreferredTeamInput = {
  clientMutationId?: string | null | undefined;
  teamId: string;
};
export type teamCard_removeCustomerPreferredTeamMutation$variables = {
  input: RemoveCustomerPreferredTeamInput;
};
export type teamCard_removeCustomerPreferredTeamMutation$data = {
  readonly removeCustomerPreferredTeam: {
    readonly customer: {
      readonly id: string;
      readonly preferredTeams: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type teamCard_removeCustomerPreferredTeamMutation = {
  response: teamCard_removeCustomerPreferredTeamMutation$data;
  variables: teamCard_removeCustomerPreferredTeamMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
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
    "name": "removeCustomerPreferredTeam",
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamDetails",
            "kind": "LinkedField",
            "name": "preferredTeams",
            "plural": true,
            "selections": [
              (v1/*: any*/)
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
    "name": "teamCard_removeCustomerPreferredTeamMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamCard_removeCustomerPreferredTeamMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "ae8d6ab116d2ec7e27c85394e0c3cb0e",
    "id": null,
    "metadata": {},
    "name": "teamCard_removeCustomerPreferredTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamCard_removeCustomerPreferredTeamMutation(\n  $input: RemoveCustomerPreferredTeamInput!\n) {\n  removeCustomerPreferredTeam(input: $input) {\n    customer {\n      id\n      preferredTeams {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1d7f77ea5ffb830edecfa1d23e3ca042";

export default node;
