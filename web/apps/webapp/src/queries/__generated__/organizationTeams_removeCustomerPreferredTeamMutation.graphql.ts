/**
 * @generated SignedSource<<0bc51228bbc5bae98bd0809914d7e9c2>>
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
        readonly id: string;
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
    "name": "organizationTeams_removeCustomerPreferredTeamMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeams_removeCustomerPreferredTeamMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "225c56321ab67f1a1ba350e5bb7eab44",
    "id": null,
    "metadata": {},
    "name": "organizationTeams_removeCustomerPreferredTeamMutation",
    "operationKind": "mutation",
    "text": "mutation organizationTeams_removeCustomerPreferredTeamMutation(\n  $input: RemoveCustomerPreferredTeamInput!\n) {\n  removeCustomerPreferredTeam(input: $input) {\n    customer {\n      id\n      preferredTeams {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "475d8bad1db0747cb7ef273385b94977";

export default node;
