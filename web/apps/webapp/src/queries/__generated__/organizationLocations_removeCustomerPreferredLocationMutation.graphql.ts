/**
 * @generated SignedSource<<7bab296e79d912a37b646b5c42473c2a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPreferredLocationInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type organizationLocations_removeCustomerPreferredLocationMutation$variables = {
  input: RemoveCustomerPreferredLocationInput;
};
export type organizationLocations_removeCustomerPreferredLocationMutation$data = {
  readonly removeCustomerPreferredLocation: {
    readonly customer: {
      readonly id: string;
      readonly preferredLocations: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationLocations_removeCustomerPreferredLocationMutation = {
  response: organizationLocations_removeCustomerPreferredLocationMutation$data;
  variables: organizationLocations_removeCustomerPreferredLocationMutation$variables;
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
    "name": "removeCustomerPreferredLocation",
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
    "name": "organizationLocations_removeCustomerPreferredLocationMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocations_removeCustomerPreferredLocationMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "6fb492f02a3e3065db6c2e43cb8a7254",
    "id": null,
    "metadata": {},
    "name": "organizationLocations_removeCustomerPreferredLocationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocations_removeCustomerPreferredLocationMutation(\n  $input: RemoveCustomerPreferredLocationInput!\n) {\n  removeCustomerPreferredLocation(input: $input) {\n    customer {\n      id\n      preferredLocations {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f6c7e8220470e0c5ff8a571b476014d5";

export default node;
