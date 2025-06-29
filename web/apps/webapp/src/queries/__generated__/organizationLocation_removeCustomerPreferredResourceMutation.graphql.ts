/**
 * @generated SignedSource<<1ac2943a58d540778ee7f1faf19e716e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPreferredResourceInput = {
  clientMutationId?: string | null | undefined;
  resourceId: string;
};
export type organizationLocation_removeCustomerPreferredResourceMutation$variables = {
  input: RemoveCustomerPreferredResourceInput;
};
export type organizationLocation_removeCustomerPreferredResourceMutation$data = {
  readonly removeCustomerPreferredResource: {
    readonly customer: {
      readonly id: string;
      readonly preferredResources: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  };
};
export type organizationLocation_removeCustomerPreferredResourceMutation = {
  response: organizationLocation_removeCustomerPreferredResourceMutation$data;
  variables: organizationLocation_removeCustomerPreferredResourceMutation$variables;
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
    "name": "removeCustomerPreferredResource",
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
            "concreteType": "CustomerResourceDetails",
            "kind": "LinkedField",
            "name": "preferredResources",
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
    "name": "organizationLocation_removeCustomerPreferredResourceMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_removeCustomerPreferredResourceMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "a42a493d133e8e260bed241d5d8f6582",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_removeCustomerPreferredResourceMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_removeCustomerPreferredResourceMutation(\n  $input: RemoveCustomerPreferredResourceInput!\n) {\n  removeCustomerPreferredResource(input: $input) {\n    customer {\n      id\n      preferredResources {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "12abd6a9fb9adcf70aa5154148df98bb";

export default node;
