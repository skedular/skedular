/**
 * @generated SignedSource<<985bb1f373199a2f6f4c615e06c83bd9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerFavouriteLocationInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type marketplaceLocationCard_addCustomerFavouriteLocationMutation$variables = {
  input: AddCustomerFavouriteLocationInput;
};
export type marketplaceLocationCard_addCustomerFavouriteLocationMutation$data = {
  readonly addCustomerFavouriteLocation: {
    readonly customer: {
      readonly favouriteLocations: ReadonlyArray<{
        readonly id: string;
      }>;
      readonly id: string;
    };
  };
};
export type marketplaceLocationCard_addCustomerFavouriteLocationMutation = {
  response: marketplaceLocationCard_addCustomerFavouriteLocationMutation$data;
  variables: marketplaceLocationCard_addCustomerFavouriteLocationMutation$variables;
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
v2 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "CustomerPayload",
    "kind": "LinkedField",
    "name": "addCustomerFavouriteLocation",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "customer",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "favouriteLocations",
            "plural": true,
            "selections": [
              (v1/*: any*/)
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
    "name": "marketplaceLocationCard_addCustomerFavouriteLocationMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "marketplaceLocationCard_addCustomerFavouriteLocationMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "0d5e708ddd0ba8892dd4c59dedb3a4b5",
    "id": null,
    "metadata": {},
    "name": "marketplaceLocationCard_addCustomerFavouriteLocationMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceLocationCard_addCustomerFavouriteLocationMutation(\n  $input: AddCustomerFavouriteLocationInput!\n) {\n  addCustomerFavouriteLocation(input: $input) {\n    customer {\n      id\n      favouriteLocations {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3e0b961b9026b21e70f02967d07245a5";

export default node;
