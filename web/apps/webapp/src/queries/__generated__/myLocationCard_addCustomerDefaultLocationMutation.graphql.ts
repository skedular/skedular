/**
 * @generated SignedSource<<f39b423698e12908feef7ac77ceee65b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerDefaultLocationInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type myLocationCard_addCustomerDefaultLocationMutation$variables = {
  input: AddCustomerDefaultLocationInput;
};
export type myLocationCard_addCustomerDefaultLocationMutation$data = {
  readonly addCustomerDefaultLocation: {
    readonly customer: {
      readonly defaultLocations: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
    };
  } | null | undefined;
};
export type myLocationCard_addCustomerDefaultLocationMutation = {
  response: myLocationCard_addCustomerDefaultLocationMutation$data;
  variables: myLocationCard_addCustomerDefaultLocationMutation$variables;
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
    "concreteType": "CustomerPayload",
    "kind": "LinkedField",
    "name": "addCustomerDefaultLocation",
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
            "concreteType": "CustomerLocationDetails",
            "kind": "LinkedField",
            "name": "defaultLocations",
            "plural": true,
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
    "name": "myLocationCard_addCustomerDefaultLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myLocationCard_addCustomerDefaultLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "bd9da0fa1ed9ccd1ef61f180b0cee0f7",
    "id": null,
    "metadata": {},
    "name": "myLocationCard_addCustomerDefaultLocationMutation",
    "operationKind": "mutation",
    "text": "mutation myLocationCard_addCustomerDefaultLocationMutation(\n  $input: AddCustomerDefaultLocationInput!\n) {\n  addCustomerDefaultLocation(input: $input) {\n    customer {\n      id\n      defaultLocations {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "17cbe235f519593ae69df60f406c683b";

export default node;
