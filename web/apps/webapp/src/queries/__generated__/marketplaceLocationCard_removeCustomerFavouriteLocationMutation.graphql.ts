/**
 * @generated SignedSource<<500a76f8f5cd71272fd41d066f788ae8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerFavouriteLocationInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type marketplaceLocationCard_removeCustomerFavouriteLocationMutation$variables = {
  input: RemoveCustomerFavouriteLocationInput;
};
export type marketplaceLocationCard_removeCustomerFavouriteLocationMutation$data = {
  readonly removeCustomerFavouriteLocation: {
    readonly customer: {
      readonly favouriteLocations: ReadonlyArray<{
        readonly id: string;
      }>;
      readonly id: string;
    };
  };
};
export type marketplaceLocationCard_removeCustomerFavouriteLocationMutation = {
  response: marketplaceLocationCard_removeCustomerFavouriteLocationMutation$data;
  variables: marketplaceLocationCard_removeCustomerFavouriteLocationMutation$variables;
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
    "name": "removeCustomerFavouriteLocation",
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
    "name": "marketplaceLocationCard_removeCustomerFavouriteLocationMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "marketplaceLocationCard_removeCustomerFavouriteLocationMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "700054d73200280713bdf60c1e3f65a5",
    "id": null,
    "metadata": {},
    "name": "marketplaceLocationCard_removeCustomerFavouriteLocationMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceLocationCard_removeCustomerFavouriteLocationMutation(\n  $input: RemoveCustomerFavouriteLocationInput!\n) {\n  removeCustomerFavouriteLocation(input: $input) {\n    customer {\n      id\n      favouriteLocations {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "bf0b21eda980c3a1fb3107d861bb82ca";

export default node;
