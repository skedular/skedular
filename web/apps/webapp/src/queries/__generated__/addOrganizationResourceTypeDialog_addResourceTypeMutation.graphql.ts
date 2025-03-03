/**
 * @generated SignedSource<<9dccfda0e045a2702932ded7b9f1a530>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationResourceTypeSystemType = "Desk" | "Room" | "%future added value";
export type AddResourceTypeInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  description?: string | null | undefined;
  id?: string | null | undefined;
  name: string;
  organizationId: string;
};
export type addOrganizationResourceTypeDialog_addResourceTypeMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddResourceTypeInput;
};
export type addOrganizationResourceTypeDialog_addResourceTypeMutation$data = {
  readonly addResourceType: {
    readonly organizationResourceType: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly systemType: OrganizationResourceTypeSystemType | null | undefined;
    };
  } | null | undefined;
};
export type addOrganizationResourceTypeDialog_addResourceTypeMutation$rawResponse = {
  readonly addResourceType: {
    readonly organizationResourceType: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly systemType: OrganizationResourceTypeSystemType | null | undefined;
    };
  } | null | undefined;
};
export type addOrganizationResourceTypeDialog_addResourceTypeMutation = {
  rawResponse: addOrganizationResourceTypeDialog_addResourceTypeMutation$rawResponse;
  response: addOrganizationResourceTypeDialog_addResourceTypeMutation$data;
  variables: addOrganizationResourceTypeDialog_addResourceTypeMutation$variables;
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
  "concreteType": "OrganizationResourceTypeDetails",
  "kind": "LinkedField",
  "name": "organizationResourceType",
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
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "systemType",
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
    "name": "addOrganizationResourceTypeDialog_addResourceTypeMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationResourceTypePayload",
        "kind": "LinkedField",
        "name": "addResourceType",
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
    "name": "addOrganizationResourceTypeDialog_addResourceTypeMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationResourceTypePayload",
        "kind": "LinkedField",
        "name": "addResourceType",
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
            "name": "organizationResourceType",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "OrganizationResourceTypeDetails"
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "e7c38e0b82079e09ee25b3ad6ac5912d",
    "id": null,
    "metadata": {},
    "name": "addOrganizationResourceTypeDialog_addResourceTypeMutation",
    "operationKind": "mutation",
    "text": "mutation addOrganizationResourceTypeDialog_addResourceTypeMutation(\n  $input: AddResourceTypeInput!\n) {\n  addResourceType(input: $input) {\n    organizationResourceType {\n      id\n      name\n      description\n      color\n      systemType\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b3658a81055e16160ba1c5a57f123d60";

export default node;
