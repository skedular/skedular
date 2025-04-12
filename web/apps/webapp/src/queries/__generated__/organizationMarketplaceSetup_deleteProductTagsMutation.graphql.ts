/**
 * @generated SignedSource<<63a92dd463f59df2005eb281ca49c4e1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteProductTagsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationMarketplaceSetup_deleteProductTagsMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteProductTagsInput;
};
export type organizationMarketplaceSetup_deleteProductTagsMutation$data = {
  readonly deleteProductTags: {
    readonly organizationTags: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationMarketplaceSetup_deleteProductTagsMutation = {
  response: organizationMarketplaceSetup_deleteProductTagsMutation$data;
  variables: organizationMarketplaceSetup_deleteProductTagsMutation$variables;
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
    "name": "organizationMarketplaceSetup_deleteProductTagsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagsPayload",
        "kind": "LinkedField",
        "name": "deleteProductTags",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "organizationTags",
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
    "name": "organizationMarketplaceSetup_deleteProductTagsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagsPayload",
        "kind": "LinkedField",
        "name": "deleteProductTags",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "organizationTags",
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
    "cacheID": "fe232e8824b357469302be6ab32f3513",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_deleteProductTagsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_deleteProductTagsMutation(\n  $input: DeleteProductTagsInput!\n) {\n  deleteProductTags(input: $input) {\n    organizationTags {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cfa4eca0ba377f275bf8666469bb8437";

export default node;
