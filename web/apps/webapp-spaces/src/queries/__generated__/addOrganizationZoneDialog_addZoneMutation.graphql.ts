/**
 * @generated SignedSource<<6f52677ff50dfc60f8e1f7cf6a344d24>>
 * @lightSyntaxTransform
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
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type addOrganizationZoneDialog_addZoneMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddZoneInput;
};
export type addOrganizationZoneDialog_addZoneMutation$data = {
  readonly addZone: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  };
};
export type addOrganizationZoneDialog_addZoneMutation$rawResponse = {
  readonly addZone: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  };
};
export type addOrganizationZoneDialog_addZoneMutation = {
  rawResponse: addOrganizationZoneDialog_addZoneMutation$rawResponse;
  response: addOrganizationZoneDialog_addZoneMutation$data;
  variables: addOrganizationZoneDialog_addZoneMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addOrganizationZoneDialog_addZoneMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "addZone",
        "plural": false,
        "selections": [
          (v2/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "addOrganizationZoneDialog_addZoneMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "addZone",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
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
    "cacheID": "3f292625d88e7d2bff8c1f7c78ef717a",
    "id": null,
    "metadata": {},
    "name": "addOrganizationZoneDialog_addZoneMutation",
    "operationKind": "mutation",
    "text": "mutation addOrganizationZoneDialog_addZoneMutation(\n  $input: AddZoneInput!\n) {\n  addZone(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8c81a322d1ac6639e23c2b5bd26d514b";

export default node;
