/**
 * @generated SignedSource<<d5ad6f9fd04f90765ec2a21099e3b159>>
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
export type organizationLocations_addCustomerPreferredLocationMutation$variables = {
  input: AddCustomerPreferredLocationInput;
};
export type organizationLocations_addCustomerPreferredLocationMutation$data = {
  readonly addCustomerPreferredLocation: {
    readonly customer: {
      readonly id: string;
      readonly preferredLocations: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationLocations_addCustomerPreferredLocationMutation = {
  response: organizationLocations_addCustomerPreferredLocationMutation$data;
  variables: organizationLocations_addCustomerPreferredLocationMutation$variables;
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
    "name": "organizationLocations_addCustomerPreferredLocationMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocations_addCustomerPreferredLocationMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "cbb9476f90d85c63e188ee97039dc706",
    "id": null,
    "metadata": {},
    "name": "organizationLocations_addCustomerPreferredLocationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocations_addCustomerPreferredLocationMutation(\n  $input: AddCustomerPreferredLocationInput!\n) {\n  addCustomerPreferredLocation(input: $input) {\n    customer {\n      id\n      preferredLocations {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "34e2f8c5c1d2c897aebae292647ae37c";

export default node;
