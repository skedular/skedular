/**
 * @generated SignedSource<<47accb6f558bf613f663ed20848b6e5d>>
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
        readonly id: string;
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
    "name": "organizationTeams_addCustomerPreferredTeamMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeams_addCustomerPreferredTeamMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "e1901aeb8275dd1c8b4762dc1fec2b9b",
    "id": null,
    "metadata": {},
    "name": "organizationTeams_addCustomerPreferredTeamMutation",
    "operationKind": "mutation",
    "text": "mutation organizationTeams_addCustomerPreferredTeamMutation(\n  $input: AddCustomerPreferredTeamInput!\n) {\n  addCustomerPreferredTeam(input: $input) {\n    customer {\n      id\n      preferredTeams {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b42a837b868fd922870a6675a24882ea";

export default node;
