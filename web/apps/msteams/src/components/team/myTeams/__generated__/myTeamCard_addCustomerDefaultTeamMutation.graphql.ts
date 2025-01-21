/**
 * @generated SignedSource<<3dbd6d02704949f4c152bc3c001078e3>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerDefaultTeamInput = {
  clientMutationId?: string | null | undefined;
  teamId: string;
};
export type myTeamCard_addCustomerDefaultTeamMutation$variables = {
  input: AddCustomerDefaultTeamInput;
};
export type myTeamCard_addCustomerDefaultTeamMutation$data = {
  readonly addCustomerDefaultTeam: {
    readonly customer: {
      readonly defaultTeams: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
    };
  } | null | undefined;
};
export type myTeamCard_addCustomerDefaultTeamMutation = {
  response: myTeamCard_addCustomerDefaultTeamMutation$data;
  variables: myTeamCard_addCustomerDefaultTeamMutation$variables;
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
    "name": "addCustomerDefaultTeam",
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
    "name": "myTeamCard_addCustomerDefaultTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myTeamCard_addCustomerDefaultTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "50b2941d54a107843903af70043f55fb",
    "id": null,
    "metadata": {},
    "name": "myTeamCard_addCustomerDefaultTeamMutation",
    "operationKind": "mutation",
    "text": "mutation myTeamCard_addCustomerDefaultTeamMutation(\n  $input: AddCustomerDefaultTeamInput!\n) {\n  addCustomerDefaultTeam(input: $input) {\n    customer {\n      id\n      defaultTeams {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "91eac41183b85fb6e8d5c0185e498b1c";

export default node;
