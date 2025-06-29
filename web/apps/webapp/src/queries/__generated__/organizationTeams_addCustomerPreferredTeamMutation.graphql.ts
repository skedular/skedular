/**
 * @generated SignedSource<<e039770d025df8b29eb15b6af965cbe3>>
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
export type organizationTeams_addCustomerPreferredTeamMutation$variables = {
  input: AddCustomerPreferredTeamInput;
};
export type organizationTeams_addCustomerPreferredTeamMutation$data = {
  readonly addCustomerPreferredTeam: {
    readonly customer: {
      readonly id: string;
      readonly preferredTeams: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  };
};
export type organizationTeams_addCustomerPreferredTeamMutation = {
  response: organizationTeams_addCustomerPreferredTeamMutation$data;
  variables: organizationTeams_addCustomerPreferredTeamMutation$variables;
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
    "name": "organizationTeams_addCustomerPreferredTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeams_addCustomerPreferredTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "9b9f21c38681fdafdfe35512d283a93c",
    "id": null,
    "metadata": {},
    "name": "organizationTeams_addCustomerPreferredTeamMutation",
    "operationKind": "mutation",
    "text": "mutation organizationTeams_addCustomerPreferredTeamMutation(\n  $input: AddCustomerPreferredTeamInput!\n) {\n  addCustomerPreferredTeam(input: $input) {\n    customer {\n      id\n      preferredTeams {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7e7dcfa168045f2b9ce713693d2da23a";

export default node;
