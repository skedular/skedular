/**
 * @generated SignedSource<<291ea450f40d65bb971de09147cf36e5>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ActivateProductsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationMarketplaceSetup_activateProductsMutation$variables = {
  input: ActivateProductsInput;
};
export type organizationMarketplaceSetup_activateProductsMutation$data = {
  readonly activateProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
      readonly inactive: boolean;
    }>;
  };
};
export type organizationMarketplaceSetup_activateProductsMutation$rawResponse = {
  readonly activateProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
      readonly inactive: boolean;
    }>;
  };
};
export type organizationMarketplaceSetup_activateProductsMutation = {
  rawResponse: organizationMarketplaceSetup_activateProductsMutation$rawResponse;
  response: organizationMarketplaceSetup_activateProductsMutation$data;
  variables: organizationMarketplaceSetup_activateProductsMutation$variables;
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
    "name": "activateProducts",
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
    "name": "organizationMarketplaceSetup_activateProductsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_activateProductsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "be4d56502b2958c80ae2ee4bdf23becb",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_activateProductsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_activateProductsMutation(\n  $input: ActivateProductsInput!\n) {\n  activateProducts(input: $input) {\n    products {\n      id\n      inactive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "73faf906f76cfb58fd5482c5467458be";

export default node;
