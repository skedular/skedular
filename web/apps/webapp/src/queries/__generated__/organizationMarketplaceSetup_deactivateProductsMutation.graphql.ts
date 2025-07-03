/**
 * @generated SignedSource<<fc14eee7e50cc093246a30aada5572e9>>
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
      readonly id: string;
      readonly inactive: boolean;
    }>;
  };
};
export type organizationMarketplaceSetup_deactivateProductsMutation$rawResponse = {
  readonly deactivateProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
      readonly inactive: boolean;
    }>;
  };
};
export type organizationMarketplaceSetup_deactivateProductsMutation = {
  rawResponse: organizationMarketplaceSetup_deactivateProductsMutation$rawResponse;
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
v1 = [
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
            "name": "inactive",
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
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_deactivateProductsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "111bd7eb799c22df45f112bac286bfb2",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_deactivateProductsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_deactivateProductsMutation(\n  $input: DeactivateProductsInput!\n) {\n  deactivateProducts(input: $input) {\n    products {\n      id\n      inactive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5d3f5d8eb0f96d03221696c7c320d30f";

export default node;
