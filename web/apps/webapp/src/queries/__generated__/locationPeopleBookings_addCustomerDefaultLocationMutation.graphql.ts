/**
 * @generated SignedSource<<041aba8212ee61e282f217570b431e3e>>
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
export type locationPeopleBookings_addCustomerDefaultLocationMutation$variables = {
  input: AddCustomerDefaultLocationInput;
};
export type locationPeopleBookings_addCustomerDefaultLocationMutation$data = {
  readonly addCustomerDefaultLocation: {
    readonly customer: {
      readonly defaultLocations: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
    };
  } | null | undefined;
};
export type locationPeopleBookings_addCustomerDefaultLocationMutation = {
  response: locationPeopleBookings_addCustomerDefaultLocationMutation$data;
  variables: locationPeopleBookings_addCustomerDefaultLocationMutation$variables;
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
    "name": "locationPeopleBookings_addCustomerDefaultLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationPeopleBookings_addCustomerDefaultLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "43a3196a096a49f5bb91c66b493b4d72",
    "id": null,
    "metadata": {},
    "name": "locationPeopleBookings_addCustomerDefaultLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationPeopleBookings_addCustomerDefaultLocationMutation(\n  $input: AddCustomerDefaultLocationInput!\n) {\n  addCustomerDefaultLocation(input: $input) {\n    customer {\n      id\n      defaultLocations {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cfe555ab7df467927768f320b25ea857";

export default node;
