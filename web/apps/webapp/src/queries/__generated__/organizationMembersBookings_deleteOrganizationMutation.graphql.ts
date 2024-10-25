/**
 * @generated SignedSource<<c9ed7dd7efc232a823bd6924625b133c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteOrganizationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationMembersBookings_deleteOrganizationMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteOrganizationInput;
};
export type organizationMembersBookings_deleteOrganizationMutation$data = {
  readonly deleteOrganization: {
    readonly organization: {
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationMembersBookings_deleteOrganizationMutation = {
  response: organizationMembersBookings_deleteOrganizationMutation$data;
  variables: organizationMembersBookings_deleteOrganizationMutation$variables;
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
    "name": "organizationMembersBookings_deleteOrganizationMutation",
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
    "name": "organizationMembersBookings_deleteOrganizationMutation",
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
    "cacheID": "76544082b71d43134244d8fd5f9e8ba4",
    "id": null,
    "metadata": {},
    "name": "organizationMembersBookings_deleteOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMembersBookings_deleteOrganizationMutation(\n  $input: DeleteOrganizationInput!\n) {\n  deleteOrganization(input: $input) {\n    organization {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "591a8645e78307fc95c2f78d1b464f85";

export default node;
