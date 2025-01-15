/**
 * @generated SignedSource<<de792990075d6af0b86b21c471b191a7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddZoneInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  description?: string | null | undefined;
  id?: string | null | undefined;
  name: string;
  organizationId: string;
};
export type newZoneDialog_addZoneMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddZoneInput;
};
export type newZoneDialog_addZoneMutation$data = {
  readonly addZone: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type newZoneDialog_addZoneMutation$rawResponse = {
  readonly addZone: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type newZoneDialog_addZoneMutation = {
  rawResponse: newZoneDialog_addZoneMutation$rawResponse;
  response: newZoneDialog_addZoneMutation$data;
  variables: newZoneDialog_addZoneMutation$variables;
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
    "name": "newZoneDialog_addZoneMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "addZone",
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
    "name": "newZoneDialog_addZoneMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "addZone",
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
    "cacheID": "eb04211bcecafb8f8dbdb82b4eec3ef9",
    "id": null,
    "metadata": {},
    "name": "newZoneDialog_addZoneMutation",
    "operationKind": "mutation",
    "text": "mutation newZoneDialog_addZoneMutation(\n  $input: AddZoneInput!\n) {\n  addZone(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c1840817b9beff13c099f61be07678ff";

export default node;
