/**
 * @generated SignedSource<<848fc5d950e5ef7dacdd827968a69cac>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationLocation_desks_query$data = {
  readonly desks: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly deactivated: boolean;
        readonly deskTypes: ReadonlyArray<{
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly id: string;
        readonly name: string;
        readonly requireBookingApproval: boolean;
        readonly zones: ReadonlyArray<{
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "organizationLocation_desks_query";
};
export type organizationLocation_desks_query$key = {
  readonly " $data"?: organizationLocation_desks_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationLocation_desks_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  "desks"
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v1/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "count"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "cursor"
    },
    {
      "kind": "RootArgument",
      "name": "deskDeskTypeIds"
    },
    {
      "kind": "RootArgument",
      "name": "deskNameSearchText"
    },
    {
      "kind": "RootArgument",
      "name": "deskZoneIds"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "connection": [
      {
        "count": "count",
        "cursor": "cursor",
        "direction": "forward",
        "path": (v0/*: any*/)
      }
    ],
    "refetch": {
      "connection": {
        "forward": {
          "count": "count",
          "cursor": "cursor"
        },
        "backward": null,
        "path": (v0/*: any*/)
      },
      "fragmentPathInResult": [],
      "operation": require('./organizationLocation_desks_refetchableFragment.graphql')
    }
  },
  "name": "organizationLocation_desks_query",
  "selections": [
    {
      "alias": "desks",
      "args": [
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "deskTypeIds",
              "variableName": "deskDeskTypeIds"
            },
            {
              "kind": "Variable",
              "name": "locationId",
              "variableName": "locationId"
            },
            {
              "kind": "Variable",
              "name": "nameContains",
              "variableName": "deskNameSearchText"
            },
            {
              "kind": "Variable",
              "name": "zoneIds",
              "variableName": "deskZoneIds"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "DeskConnection",
      "kind": "LinkedField",
      "name": "__organizationLocation_desks_connection",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "totalCount",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "DeskEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "DeskDetails",
              "kind": "LinkedField",
              "name": "node",
              "plural": false,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "id",
                  "storageKey": null
                },
                (v1/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "deactivated",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "requireBookingApproval",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Organization_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "deskTypes",
                  "plural": true,
                  "selections": (v2/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Organization_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "zones",
                  "plural": true,
                  "selections": (v2/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "__typename",
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "cursor",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "PageInfo",
          "kind": "LinkedField",
          "name": "pageInfo",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "endCursor",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "hasNextPage",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "kind": "ClientExtension",
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "__id",
              "storageKey": null
            }
          ]
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "7ed0f3e42f93eeb4de4aeb4bb5b56bbb";

export default node;
