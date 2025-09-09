/**
 * @generated SignedSource<<cb58753c40cdea538a5582db2e1888b0>>
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
        readonly id: string;
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "ResourceDetails",
            "kind": "LinkedField",
            "name": "preferredResources",
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
    "name": "organizationLocation_addCustomerPreferredResourceMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_addCustomerPreferredResourceMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "4f06bc49745dd952579b3feb10d43186",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_addCustomerPreferredResourceMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_addCustomerPreferredResourceMutation(\n  $input: AddCustomerPreferredResourceInput!\n) {\n  addCustomerPreferredResource(input: $input) {\n    customer {\n      id\n      preferredResources {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a9fb94946c1208dc27c548862c883f65";

export default node;
