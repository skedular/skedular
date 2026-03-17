/**
 * @generated SignedSource<<2fcade932aa80011a2611c39acfc2b61>>
 * @lightSyntaxTransform
 * @nogrep
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
      readonly hasFutureBooking: boolean;
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
  (v2/*: any*/),
  (v3/*: any*/),
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
  "selections": (v4/*: any*/),
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "zones",
  "plural": true,
  "selections": (v4/*: any*/),
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
  "name": "hasFutureBooking",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canDelete",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "customDomain",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "claimLocationOwnershipDialog_claimLocationOwnershipMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/),
              (v3/*: any*/),
              (v5/*: any*/),
              (v6/*: any*/),
              (v7/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationPhysicalAddressDetails",
                "kind": "LinkedField",
                "name": "physicalAddress",
                "plural": false,
                "selections": [
                  (v8/*: any*/)
                ],
                "storageKey": null
              },
              (v9/*: any*/),
              (v10/*: any*/),
              (v11/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationDetails",
                "kind": "LinkedField",
                "name": "organization",
                "plural": false,
                "selections": [
                  (v12/*: any*/)
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "claimLocationOwnershipDialog_claimLocationOwnershipMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/),
              (v3/*: any*/),
              (v5/*: any*/),
              (v6/*: any*/),
              (v7/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationPhysicalAddressDetails",
                "kind": "LinkedField",
                "name": "physicalAddress",
                "plural": false,
                "selections": [
                  (v8/*: any*/),
                  (v2/*: any*/)
                ],
                "storageKey": null
              },
              (v9/*: any*/),
              (v10/*: any*/),
              (v11/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationDetails",
                "kind": "LinkedField",
                "name": "organization",
                "plural": false,
                "selections": [
                  (v12/*: any*/),
                  (v2/*: any*/)
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
    "cacheID": "bcf27578a0f957f51b299ee23ffe4475",
    "id": null,
    "metadata": {},
    "name": "claimLocationOwnershipDialog_claimLocationOwnershipMutation",
    "operationKind": "mutation",
    "text": "mutation claimLocationOwnershipDialog_claimLocationOwnershipMutation(\n  $input: ClaimLocationOwnershipInput!\n) {\n  claimLocationOwnership(input: $input) {\n    location {\n      id\n      name\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n      resources {\n        totalCount\n      }\n      physicalAddress {\n        formattedAddress\n        id\n      }\n      hasFutureBooking\n      canModify\n      canDelete\n      organization {\n        customDomain\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8028649323f0788ca937e89bc701b199";

export default node;
