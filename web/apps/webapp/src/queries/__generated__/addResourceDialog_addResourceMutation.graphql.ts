/**
 * @generated SignedSource<<a1228f7787d9d0a008a7fe0b7377878f>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddResourceInput = {
  capacity: number;
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  customTagIds: ReadonlyArray<string>;
  id?: string | null | undefined;
  inactive: boolean;
  locationId: string;
  name: string;
  organizationResourceTypeId: string;
  productTagIds: ReadonlyArray<string>;
  requireBookingApproval: boolean;
  zoneIds: ReadonlyArray<string>;
};
export type addResourceDialog_addResourceMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddResourceInput;
};
export type addResourceDialog_addResourceMutation$data = {
  readonly addResource: {
    readonly resource: {
      readonly capacity: number;
      readonly color: string | null | undefined;
      readonly customTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly id: string;
      readonly inactive: boolean;
      readonly name: string;
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly requireBookingApproval: boolean;
      readonly resourceType: {
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      };
      readonly zones: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
    };
  };
};
export type addResourceDialog_addResourceMutation$rawResponse = {
  readonly addResource: {
    readonly resource: {
      readonly capacity: number;
      readonly color: string | null | undefined;
      readonly customTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly id: string;
      readonly inactive: boolean;
      readonly name: string;
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly requireBookingApproval: boolean;
      readonly resourceType: {
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      };
      readonly zones: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
    };
  };
};
export type addResourceDialog_addResourceMutation = {
  rawResponse: addResourceDialog_addResourceMutation$rawResponse;
  response: addResourceDialog_addResourceMutation$data;
  variables: addResourceDialog_addResourceMutation$variables;
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
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v5 = [
  (v2/*:: as any*/),
  (v3/*:: as any*/),
  (v4/*:: as any*/)
],
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "ResourceDetails",
  "kind": "LinkedField",
  "name": "resource",
  "plural": false,
  "selections": [
    (v2/*:: as any*/),
    (v3/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "inactive",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "requireBookingApproval",
      "storageKey": null
    },
    (v4/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "capacity",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "customTags",
      "plural": true,
      "selections": (v5/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "zones",
      "plural": true,
      "selections": (v5/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "productTags",
      "plural": true,
      "selections": (v5/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "resourceType",
      "plural": false,
      "selections": (v5/*:: as any*/),
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
    "name": "addResourceDialog_addResourceMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "ResourcePayload",
        "kind": "LinkedField",
        "name": "addResource",
        "plural": false,
        "selections": [
          (v6/*:: as any*/)
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
    "name": "addResourceDialog_addResourceMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "ResourcePayload",
        "kind": "LinkedField",
        "name": "addResource",
        "plural": false,
        "selections": [
          (v6/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "filters": null,
            "handle": "appendNode",
            "key": "",
            "kind": "LinkedHandle",
            "name": "resource",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "ResourceDetails"
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "d9da8605f3e14434bbfb5b2b229f17b3",
    "id": null,
    "metadata": {},
    "name": "addResourceDialog_addResourceMutation",
    "operationKind": "mutation",
    "text": "mutation addResourceDialog_addResourceMutation(\n  $input: AddResourceInput!\n) {\n  addResource(input: $input) {\n    resource {\n      id\n      name\n      inactive\n      requireBookingApproval\n      color\n      capacity\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n      productTags {\n        id\n        name\n        color\n      }\n      resourceType {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "864bfc88c7a53582968e1024a333f1bf";

export default node;
