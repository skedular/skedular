/**
 * @generated SignedSource<<6fa9131cbef419c1b4d008aa4c54779a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationBillingInfoQuery$variables = {
  organizationId: string;
};
export type organizationBillingInfoQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationBillingInfo_query">;
};
export type organizationBillingInfoQuery = {
  response: organizationBillingInfoQuery$data;
  variables: organizationBillingInfoQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationBillingInfoQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationBillingInfo_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationBillingInfoQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "organizationId"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
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
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "organizationId",
            "variableName": "organizationId"
          }
        ],
        "concreteType": "OrganizationBillingInfo",
        "kind": "LinkedField",
        "name": "organizationBillingInfo",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "organizationId",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "email",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "addressLine1",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "addressLine2",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "suburb",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "city",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "province",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "zipcode",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "country",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "83ee8d17a014884d93d3e7be7cdcff08",
    "id": null,
    "metadata": {},
    "name": "organizationBillingInfoQuery",
    "operationKind": "query",
    "text": "query organizationBillingInfoQuery(\n  $organizationId: String!\n) {\n  ...organizationBillingInfo_query\n}\n\nfragment organizationBillingInfo_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n  }\n  organizationBillingInfo(organizationId: $organizationId) {\n    organizationId\n    email\n    addressLine1\n    addressLine2\n    suburb\n    city\n    province\n    zipcode\n    country\n  }\n}\n"
  }
};
})();

(node as any).hash = "2820b61e72bec3d08ee52e63edc196f9";

export default node;
