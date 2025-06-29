/**
 * @generated SignedSource<<ae18bde91d6e908a3012784338d344cd>>
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
        readonly uniqueId: string;
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
            "concreteType": "Customer_LocationDetails",
            "kind": "LinkedField",
            "name": "preferredLocations",
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
    "name": "locationCard_addCustomerPreferredLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationCard_addCustomerPreferredLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "23b8b2a1b1ff5d5536207462c129530f",
    "id": null,
    "metadata": {},
    "name": "locationCard_addCustomerPreferredLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationCard_addCustomerPreferredLocationMutation(\n  $input: AddCustomerPreferredLocationInput!\n) {\n  addCustomerPreferredLocation(input: $input) {\n    customer {\n      id\n      preferredLocations {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ea837a6e81e32f326efe3948b55910dd";

export default node;
