/**
 * @generated SignedSource<<a72555e0869e7cfcbdf9258a187f8ff7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateDeskTypeInput = {
  clientMutationId?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
};
export type deskTypeCard_updateDeskTypeMutation$variables = {
  input: UpdateDeskTypeInput;
};
export type deskTypeCard_updateDeskTypeMutation$data = {
  readonly updateDeskType: {
    readonly organizationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type deskTypeCard_updateDeskTypeMutation = {
  response: deskTypeCard_updateDeskTypeMutation$data;
  variables: deskTypeCard_updateDeskTypeMutation$variables;
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
    "concreteType": "OrganizationTagPayload",
    "kind": "LinkedField",
    "name": "updateDeskType",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTagDetails",
        "kind": "LinkedField",
        "name": "organizationTag",
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
            "kind": "ScalarField",
            "name": "name",
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
    "name": "deskTypeCard_updateDeskTypeMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "deskTypeCard_updateDeskTypeMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "9d87fbea307bb53d71a02cc17f8d12b1",
    "id": null,
    "metadata": {},
    "name": "deskTypeCard_updateDeskTypeMutation",
    "operationKind": "mutation",
    "text": "mutation deskTypeCard_updateDeskTypeMutation(\n  $input: UpdateDeskTypeInput!\n) {\n  updateDeskType(input: $input) {\n    organizationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1e19ef8c209d71d3bcc9154ecc307ffe";

export default node;
