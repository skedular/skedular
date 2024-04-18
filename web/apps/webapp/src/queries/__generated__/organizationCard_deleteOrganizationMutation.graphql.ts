/**
 * @generated SignedSource<<9f25755108ef741883c0d9d5715143de>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type DeleteOrganizationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationCard_deleteOrganizationMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteOrganizationInput;
};
export type organizationCard_deleteOrganizationMutation$data = {
  readonly deleteOrganization: {
    readonly organization: {
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationCard_deleteOrganizationMutation = {
  response: organizationCard_deleteOrganizationMutation$data;
  variables: organizationCard_deleteOrganizationMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationCard_deleteOrganizationMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationPayload",
        "kind": "LinkedField",
        "name": "deleteOrganization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              (v2/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationCard_deleteOrganizationMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationPayload",
        "kind": "LinkedField",
        "name": "deleteOrganization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "85d0d91e7e61735eecd9d4d4eb031afa",
    "id": null,
    "metadata": {},
    "name": "organizationCard_deleteOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationCard_deleteOrganizationMutation(\n  $input: DeleteOrganizationInput!\n) {\n  deleteOrganization(input: $input) {\n    organization {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f662362a1b5a924d7dcc8bb47a84675b";

export default node;
