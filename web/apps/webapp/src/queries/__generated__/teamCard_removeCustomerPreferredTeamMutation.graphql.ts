/**
 * @generated SignedSource<<3770561d269815e26e2c44b0f25b31fb>>
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
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
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
    "name": "teamCard_removeCustomerPreferredTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamCard_removeCustomerPreferredTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "4835326f2d7d268e8028febd3b257df5",
    "id": null,
    "metadata": {},
    "name": "teamCard_removeCustomerPreferredTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamCard_removeCustomerPreferredTeamMutation(\n  $input: RemoveCustomerPreferredTeamInput!\n) {\n  removeCustomerPreferredTeam(input: $input) {\n    customer {\n      id\n      preferredTeams {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cb984ddc7a1daa14091b401c100496f8";

export default node;
