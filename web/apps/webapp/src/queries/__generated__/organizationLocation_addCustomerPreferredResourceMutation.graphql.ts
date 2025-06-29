/**
 * @generated SignedSource<<d279a26c1c63e52925fb289eaec8d69c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerPreferredResourceInput = {
  clientMutationId?: string | null | undefined;
  resourceId: string;
};
export type organizationLocation_addCustomerPreferredResourceMutation$variables = {
  input: AddCustomerPreferredResourceInput;
};
export type organizationLocation_addCustomerPreferredResourceMutation$data = {
  readonly addCustomerPreferredResource: {
    readonly customer: {
      readonly id: string;
      readonly preferredResources: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  };
};
export type organizationLocation_addCustomerPreferredResourceMutation = {
  response: organizationLocation_addCustomerPreferredResourceMutation$data;
  variables: organizationLocation_addCustomerPreferredResourceMutation$variables;
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
    "name": "addCustomerPreferredResource",
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
    "name": "organizationLocation_addCustomerPreferredResourceMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_addCustomerPreferredResourceMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "b5c02903c00cfdcfd36a992f7bd9d4fb",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_addCustomerPreferredResourceMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_addCustomerPreferredResourceMutation(\n  $input: AddCustomerPreferredResourceInput!\n) {\n  addCustomerPreferredResource(input: $input) {\n    customer {\n      id\n      preferredResources {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1e2f58b5851a8fd3c8803a448b6deb65";

export default node;
