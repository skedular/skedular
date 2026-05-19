/**
 * @generated SignedSource<<2ceea8be7fae1fdca9a51685b8ab6937>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ClaimLocationOwnershipInput = {
  clientMutationId?: string | null | undefined;
  id?: string | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
  uniqueClaimCode: string;
};
export type claimLocationOwnershipDialog_claimLocationOwnershipMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: ClaimLocationOwnershipInput;
};
export type claimLocationOwnershipDialog_claimLocationOwnershipMutation$data = {
  readonly claimLocationOwnership: {
    readonly location: {
      readonly canDelete: boolean;
      readonly canModify: boolean;
      readonly customTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly id: string;
      readonly name: string;
      readonly organization: {
        readonly customDomain: string | null | undefined;
      };
      readonly physicalAddress: {
        readonly formattedAddress: string | null | undefined;
      } | null | undefined;
      readonly resources: {
        readonly totalCount: number;
      };
      readonly zones: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
    };
  };
};
export type claimLocationOwnershipDialog_claimLocationOwnershipMutation = {
  response: claimLocationOwnershipDialog_claimLocationOwnershipMutation$data;
  variables: claimLocationOwnershipDialog_claimLocationOwnershipMutation$variables;
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
v4 = [
  (v2/*:: as any*/),
  (v3/*:: as any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
],
v5 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "customTags",
  "plural": true,
  "selections": (v4/*:: as any*/),
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "zones",
  "plural": true,
  "selections": (v4/*:: as any*/),
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "ConnectionOfResourceEdge",
  "kind": "LinkedField",
  "name": "resources",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "totalCount",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "formattedAddress",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canDelete",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "customDomain",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "claimLocationOwnershipDialog_claimLocationOwnershipMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationPayload",
        "kind": "LinkedField",
        "name": "claimLocationOwnership",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": [
              (v2/*:: as any*/),
              (v3/*:: as any*/),
              (v5/*:: as any*/),
              (v6/*:: as any*/),
              (v7/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationPhysicalAddressDetails",
                "kind": "LinkedField",
                "name": "physicalAddress",
                "plural": false,
                "selections": [
                  (v8/*:: as any*/)
                ],
                "storageKey": null
              },
              (v9/*:: as any*/),
              (v10/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationDetails",
                "kind": "LinkedField",
                "name": "organization",
                "plural": false,
                "selections": [
                  (v11/*:: as any*/)
                ],
                "storageKey": null
              }
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "claimLocationOwnershipDialog_claimLocationOwnershipMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationPayload",
        "kind": "LinkedField",
        "name": "claimLocationOwnership",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": [
              (v2/*:: as any*/),
              (v3/*:: as any*/),
              (v5/*:: as any*/),
              (v6/*:: as any*/),
              (v7/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationPhysicalAddressDetails",
                "kind": "LinkedField",
                "name": "physicalAddress",
                "plural": false,
                "selections": [
                  (v8/*:: as any*/),
                  (v2/*:: as any*/)
                ],
                "storageKey": null
              },
              (v9/*:: as any*/),
              (v10/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationDetails",
                "kind": "LinkedField",
                "name": "organization",
                "plural": false,
                "selections": [
                  (v11/*:: as any*/),
                  (v2/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "filters": null,
            "handle": "appendNode",
            "key": "",
            "kind": "LinkedHandle",
            "name": "location",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "LocationDetails"
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "864d8674faf82b9ba38b2d9bcab49cec",
    "id": null,
    "metadata": {},
    "name": "claimLocationOwnershipDialog_claimLocationOwnershipMutation",
    "operationKind": "mutation",
    "text": "mutation claimLocationOwnershipDialog_claimLocationOwnershipMutation(\n  $input: ClaimLocationOwnershipInput!\n) {\n  claimLocationOwnership(input: $input) {\n    location {\n      id\n      name\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n      resources {\n        totalCount\n      }\n      physicalAddress {\n        formattedAddress\n        id\n      }\n      canModify\n      canDelete\n      organization {\n        customDomain\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c98237234ddc5dc6b6f2336453589041";

export default node;
