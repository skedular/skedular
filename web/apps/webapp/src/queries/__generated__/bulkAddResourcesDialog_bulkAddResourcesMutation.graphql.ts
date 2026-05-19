/**
 * @generated SignedSource<<554e2f978ffb6b9b6dd2b736f8b35b0d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BulkAddResourcesInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
  rows: ReadonlyArray<BulkAddResourceRowInput>;
};
export type BulkAddResourceRowInput = {
  baseName?: string | null | undefined;
  customTagIds: ReadonlyArray<string>;
  organizationResourceTypeTagId: string;
  productTagIds: ReadonlyArray<string>;
  quantity: number;
  zoneIds: ReadonlyArray<string>;
};
export type bulkAddResourcesDialog_bulkAddResourcesMutation$variables = {
  input: BulkAddResourcesInput;
};
export type bulkAddResourcesDialog_bulkAddResourcesMutation$data = {
  readonly bulkAddResources: {
    readonly clientMutationId: string | null | undefined;
    readonly results: ReadonlyArray<{
      readonly createdResources: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly failureReason: string | null | undefined;
      readonly rowIndex: number;
    }>;
  };
};
export type bulkAddResourcesDialog_bulkAddResourcesMutation = {
  response: bulkAddResourcesDialog_bulkAddResourcesMutation$data;
  variables: bulkAddResourcesDialog_bulkAddResourcesMutation$variables;
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
    "concreteType": "BulkAddResourcesPayload",
    "kind": "LinkedField",
    "name": "bulkAddResources",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "clientMutationId",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "BulkAddResourceRowResult",
        "kind": "LinkedField",
        "name": "results",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "rowIndex",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "ResourceDetails",
            "kind": "LinkedField",
            "name": "createdResources",
            "plural": true,
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
                "kind": "ScalarField",
                "name": "name",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "failureReason",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "bulkAddResourcesDialog_bulkAddResourcesMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "bulkAddResourcesDialog_bulkAddResourcesMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "f525499329dae687dbffad3a0ce42267",
    "id": null,
    "metadata": {},
    "name": "bulkAddResourcesDialog_bulkAddResourcesMutation",
    "operationKind": "mutation",
    "text": "mutation bulkAddResourcesDialog_bulkAddResourcesMutation(\n  $input: BulkAddResourcesInput!\n) {\n  bulkAddResources(input: $input) {\n    clientMutationId\n    results {\n      rowIndex\n      createdResources {\n        id\n        name\n      }\n      failureReason\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cdfa223a019ab820fa6f235a11d9d3dc";

export default node;
