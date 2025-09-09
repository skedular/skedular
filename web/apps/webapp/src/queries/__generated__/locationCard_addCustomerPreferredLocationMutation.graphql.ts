/**
 * @generated SignedSource<<53bfb2afcfba2735ed9d1ca4698b20b1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerPreferredLocationInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type locationCard_addCustomerPreferredLocationMutation$variables = {
  input: AddCustomerPreferredLocationInput;
};
export type locationCard_addCustomerPreferredLocationMutation$data = {
  readonly addCustomerPreferredLocation: {
    readonly customer: {
      readonly id: string;
      readonly preferredLocations: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type locationCard_addCustomerPreferredLocationMutation = {
  response: locationCard_addCustomerPreferredLocationMutation$data;
  variables: locationCard_addCustomerPreferredLocationMutation$variables;
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
    "name": "addCustomerPreferredLocation",
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
            "name": "preferredLocations",
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
    "name": "locationCard_addCustomerPreferredLocationMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationCard_addCustomerPreferredLocationMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "bfa5c731e5515f21ac870f95bfde5e9f",
    "id": null,
    "metadata": {},
    "name": "locationCard_addCustomerPreferredLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationCard_addCustomerPreferredLocationMutation(\n  $input: AddCustomerPreferredLocationInput!\n) {\n  addCustomerPreferredLocation(input: $input) {\n    customer {\n      id\n      preferredLocations {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9779b683e1d3425ae7d0b089b9a6c694";

export default node;
