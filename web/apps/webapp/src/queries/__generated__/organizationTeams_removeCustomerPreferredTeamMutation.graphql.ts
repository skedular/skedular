/**
 * @generated SignedSource<<9e17f4916a2580caa70cf32068b05fd2>>
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
export type organizationTeams_removeCustomerPreferredTeamMutation$variables = {
  input: RemoveCustomerPreferredTeamInput;
};
export type organizationTeams_removeCustomerPreferredTeamMutation$data = {
  readonly removeCustomerPreferredTeam: {
    readonly customer: {
      readonly id: string;
      readonly preferredTeams: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  };
};
export type organizationTeams_removeCustomerPreferredTeamMutation = {
  response: organizationTeams_removeCustomerPreferredTeamMutation$data;
  variables: organizationTeams_removeCustomerPreferredTeamMutation$variables;
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
    "name": "organizationTeams_removeCustomerPreferredTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeams_removeCustomerPreferredTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "271b11ccbaa77fb9cca4e456284cf85b",
    "id": null,
    "metadata": {},
    "name": "organizationTeams_removeCustomerPreferredTeamMutation",
    "operationKind": "mutation",
    "text": "mutation organizationTeams_removeCustomerPreferredTeamMutation(\n  $input: RemoveCustomerPreferredTeamInput!\n) {\n  removeCustomerPreferredTeam(input: $input) {\n    customer {\n      id\n      preferredTeams {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "83f52870cbb22239698a01469a6208c9";

export default node;
