/**
 * @generated SignedSource<<bf4d5a7f960f767a33e75948f6083199>>
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
        readonly id: string;
      }>;
    };
  };
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
    "name": "teamCard_addCustomerPreferredTeamMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamCard_addCustomerPreferredTeamMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "b7881929d4c16c1cec76d8def67d5762",
    "id": null,
    "metadata": {},
    "name": "teamCard_addCustomerPreferredTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamCard_addCustomerPreferredTeamMutation(\n  $input: AddCustomerPreferredTeamInput!\n) {\n  addCustomerPreferredTeam(input: $input) {\n    customer {\n      id\n      preferredTeams {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4db66b58ebc58081c4f75038ae7e4373";

export default node;
