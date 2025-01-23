/**
 * @generated SignedSource<<4a6dfc0b2e9ac54e34caaf7e23b449f0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerDefaultLocationInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type organizationLocations_removeCustomerDefaultLocationMutation$variables = {
  input: RemoveCustomerDefaultLocationInput;
};
export type organizationLocations_removeCustomerDefaultLocationMutation$data = {
  readonly removeCustomerDefaultLocation: {
    readonly customer: {
      readonly defaultLocations: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationLocations_removeCustomerDefaultLocationMutation = {
  response: organizationLocations_removeCustomerDefaultLocationMutation$data;
  variables: organizationLocations_removeCustomerDefaultLocationMutation$variables;
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
    "name": "removeCustomerDefaultLocation",
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
    "name": "organizationLocations_removeCustomerDefaultLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocations_removeCustomerDefaultLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "e31614763a610299d9091467161a40c3",
    "id": null,
    "metadata": {},
    "name": "organizationLocations_removeCustomerDefaultLocationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocations_removeCustomerDefaultLocationMutation(\n  $input: RemoveCustomerDefaultLocationInput!\n) {\n  removeCustomerDefaultLocation(input: $input) {\n    customer {\n      id\n      defaultLocations {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "748c8e115e2db0068f46b85ef69b6498";

export default node;
