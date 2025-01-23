/**
 * @generated SignedSource<<84b1612eef5bccd2b92e03d13f58ce9f>>
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
export type organizationLocations_addCustomerDefaultLocationMutation$variables = {
  input: AddCustomerDefaultLocationInput;
};
export type organizationLocations_addCustomerDefaultLocationMutation$data = {
  readonly addCustomerDefaultLocation: {
    readonly customer: {
      readonly defaultLocations: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationLocations_addCustomerDefaultLocationMutation = {
  response: organizationLocations_addCustomerDefaultLocationMutation$data;
  variables: organizationLocations_addCustomerDefaultLocationMutation$variables;
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
    "name": "organizationLocations_addCustomerDefaultLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocations_addCustomerDefaultLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "f010ca3ac4e5a48dd57ee29ed6742ab3",
    "id": null,
    "metadata": {},
    "name": "organizationLocations_addCustomerDefaultLocationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocations_addCustomerDefaultLocationMutation(\n  $input: AddCustomerDefaultLocationInput!\n) {\n  addCustomerDefaultLocation(input: $input) {\n    customer {\n      id\n      defaultLocations {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "64e05aac738f7f938c1f3a5c1b31a19d";

export default node;
