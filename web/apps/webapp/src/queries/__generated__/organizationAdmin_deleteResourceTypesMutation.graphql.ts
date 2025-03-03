/**
 * @generated SignedSource<<185da01ba61e27bb68446e601b470c71>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteResourceTypesInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationAdmin_deleteResourceTypesMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteResourceTypesInput;
};
export type organizationAdmin_deleteResourceTypesMutation$data = {
  readonly deleteResourceTypes: {
    readonly organizationResourceTypes: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationAdmin_deleteResourceTypesMutation = {
  response: organizationAdmin_deleteResourceTypesMutation$data;
  variables: organizationAdmin_deleteResourceTypesMutation$variables;
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
    "name": "organizationAdmin_deleteResourceTypesMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationResourceTypesPayload",
        "kind": "LinkedField",
        "name": "deleteResourceTypes",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationResourceTypeDetails",
            "kind": "LinkedField",
            "name": "organizationResourceTypes",
            "plural": true,
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
    "name": "organizationAdmin_deleteResourceTypesMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationResourceTypesPayload",
        "kind": "LinkedField",
        "name": "deleteResourceTypes",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationResourceTypeDetails",
            "kind": "LinkedField",
            "name": "organizationResourceTypes",
            "plural": true,
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
    "cacheID": "737a4b10b82836e50f71413ba8fcd0cf",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_deleteResourceTypesMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_deleteResourceTypesMutation(\n  $input: DeleteResourceTypesInput!\n) {\n  deleteResourceTypes(input: $input) {\n    organizationResourceTypes {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d926988406787bee2ae6080404bef512";

export default node;
