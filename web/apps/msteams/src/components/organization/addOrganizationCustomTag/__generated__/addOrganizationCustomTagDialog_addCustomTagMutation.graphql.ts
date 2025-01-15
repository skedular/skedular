/**
 * @generated SignedSource<<b0781f67d590562421a48bd36067ca27>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomTagInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  description?: string | null | undefined;
  id?: string | null | undefined;
  name: string;
  organizationId: string;
};
export type addOrganizationCustomTagDialog_addCustomTagMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddCustomTagInput;
};
export type addOrganizationCustomTagDialog_addCustomTagMutation$data = {
  readonly addCustomTag: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type addOrganizationCustomTagDialog_addCustomTagMutation$rawResponse = {
  readonly addCustomTag: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type addOrganizationCustomTagDialog_addCustomTagMutation = {
  rawResponse: addOrganizationCustomTagDialog_addCustomTagMutation$rawResponse;
  response: addOrganizationCustomTagDialog_addCustomTagMutation$data;
  variables: addOrganizationCustomTagDialog_addCustomTagMutation$variables;
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
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "description",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "color",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addOrganizationCustomTagDialog_addCustomTagMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "addCustomTag",
        "plural": false,
        "selections": [
          (v2/*: any*/)
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
    "name": "addOrganizationCustomTagDialog_addCustomTagMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "addCustomTag",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "filters": null,
            "handle": "appendNode",
            "key": "",
            "kind": "LinkedHandle",
            "name": "organizationTag",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "OrganizationTagDetails"
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "348290ce5ec7aa4f063dae26daea7627",
    "id": null,
    "metadata": {},
    "name": "addOrganizationCustomTagDialog_addCustomTagMutation",
    "operationKind": "mutation",
    "text": "mutation addOrganizationCustomTagDialog_addCustomTagMutation(\n  $input: AddCustomTagInput!\n) {\n  addCustomTag(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "dd3403091a6e3755293b836c2f992c93";

export default node;
