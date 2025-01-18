/**
 * @generated SignedSource<<86213e6788e677a15758985e9e48913c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationOfferingTab_rootQuery$variables = {
  organizationId: string;
};
export type organizationOfferingTab_rootQuery$data = {
  readonly organization: {
    readonly availableOfferings: ReadonlyArray<{
      readonly code: string;
    }>;
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAvailableOfferings_query" | "organizationOffering_query">;
};
export type organizationOfferingTab_rootQuery = {
  response: organizationOfferingTab_rootQuery$data;
  variables: organizationOfferingTab_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationId"
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
  "name": "code",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "unitPrice",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationFeatureSetDetails",
  "kind": "LinkedField",
  "name": "featureSet",
  "plural": true,
  "selections": [
    (v4/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "description",
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
    "name": "organizationOfferingTab_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationOfferingDetails",
            "kind": "LinkedField",
            "name": "availableOfferings",
            "plural": true,
            "selections": [
              (v3/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationOffering_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAvailableOfferings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationOfferingTab_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationOfferingDetails",
            "kind": "LinkedField",
            "name": "availableOfferings",
            "plural": true,
            "selections": [
              (v3/*: any*/),
              (v4/*: any*/),
              (v5/*: any*/),
              (v6/*: any*/)
            ],
            "storageKey": null
          },
          (v4/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationActiveOfferingDetails",
            "kind": "LinkedField",
            "name": "activeOffering",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              (v4/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "start",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "end",
                "storageKey": null
              },
              (v5/*: any*/),
              (v6/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "free",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "hasAttachedPaymentMethod",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "7b0dec4164de2f90e7a791053d9546b1",
    "id": null,
    "metadata": {},
    "name": "organizationOfferingTab_rootQuery",
    "operationKind": "query",
    "text": "query organizationOfferingTab_rootQuery(\n  $organizationId: String!\n) {\n  organization(id: $organizationId) {\n    id\n    availableOfferings {\n      code\n    }\n  }\n  ...organizationOffering_query\n  ...organizationAvailableOfferings_query\n}\n\nfragment organizationAvailableOfferings_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    hasAttachedPaymentMethod\n    availableOfferings {\n      code\n      name\n      unitPrice\n      featureSet {\n        name\n        description\n      }\n    }\n  }\n}\n\nfragment organizationOffering_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    activeOffering {\n      id\n      name\n      start\n      end\n      unitPrice\n      featureSet {\n        name\n        description\n      }\n      free\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4d491e8ca977552ba03414bd01f2f774";

export default node;
