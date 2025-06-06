/**
 * @generated SignedSource<<ccaade25a08f48e1156f67346faa2521>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeactivateProductsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationMarketplaceSetup_deactivateProductsMutation$variables = {
  input: DeactivateProductsInput;
};
export type organizationMarketplaceSetup_deactivateProductsMutation$data = {
  readonly deactivateProducts: {
    readonly products: ReadonlyArray<{
      readonly bookAllLocationResources: boolean;
      readonly description: string | null | undefined;
      readonly featureImages: ReadonlyArray<{
        readonly id: string;
        readonly url: string;
      }>;
      readonly id: string;
      readonly inactive: boolean;
      readonly maxBookingSpreadDays: number | null | undefined;
      readonly maxDurationMinutes: number | null | undefined;
      readonly minDurationMinutes: number | null | undefined;
      readonly name: string;
      readonly numberOfResourcesToBook: number;
      readonly organization: {
        readonly uniqueId: string;
      };
      readonly priceToDisplay: string;
      readonly priceUnit: {
        readonly name: string;
      };
      readonly recurrenceWindowDays: number;
      readonly requireConsecutiveDays: boolean;
    }>;
  } | null | undefined;
};
export type organizationMarketplaceSetup_deactivateProductsMutation = {
  response: organizationMarketplaceSetup_deactivateProductsMutation$data;
  variables: organizationMarketplaceSetup_deactivateProductsMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "ProductsPayload",
    "kind": "LinkedField",
    "name": "deactivateProducts",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "products",
        "plural": true,
        "selections": [
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "inactive",
            "storageKey": null
          },
          (v2/*: any*/),
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
            "name": "priceToDisplay",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "PriceUnitDetails",
            "kind": "LinkedField",
            "name": "priceUnit",
            "plural": false,
            "selections": [
              (v2/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "numberOfResourcesToBook",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "minDurationMinutes",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "maxDurationMinutes",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "bookAllLocationResources",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "recurrenceWindowDays",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "requireConsecutiveDays",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "maxBookingSpreadDays",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Marketplace_OrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "FeatureImageDetails",
            "kind": "LinkedField",
            "name": "featureImages",
            "plural": true,
            "selections": [
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "url",
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMarketplaceSetup_deactivateProductsMutation",
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_deactivateProductsMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "d609108ab279d14ab8b502f0a4bc2d23",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_deactivateProductsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_deactivateProductsMutation(\n  $input: DeactivateProductsInput!\n) {\n  deactivateProducts(input: $input) {\n    products {\n      id\n      inactive\n      name\n      description\n      priceToDisplay\n      priceUnit {\n        name\n      }\n      numberOfResourcesToBook\n      minDurationMinutes\n      maxDurationMinutes\n      bookAllLocationResources\n      recurrenceWindowDays\n      requireConsecutiveDays\n      maxBookingSpreadDays\n      organization {\n        uniqueId\n      }\n      featureImages {\n        id\n        url\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "49317a20992b0b2b2b054fed094af101";

export default node;
