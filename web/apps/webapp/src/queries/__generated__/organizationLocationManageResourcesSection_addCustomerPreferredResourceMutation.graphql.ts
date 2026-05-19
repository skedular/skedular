/**
 * @generated SignedSource<<72677514d1ac50d9cbbd65a09a24e576>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerPreferredResourceInput = {
  clientMutationId?: string | null | undefined;
  resourceId: string;
};
export type organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation$variables = {
  input: AddCustomerPreferredResourceInput;
};
export type organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation$data = {
  readonly addCustomerPreferredResource: {
    readonly customer: {
      readonly id: string;
      readonly preferredResources: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation = {
  response: organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation$data;
  variables: organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation$variables;
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
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "ResourceDetails",
            "kind": "LinkedField",
            "name": "preferredResources",
            "plural": true,
            "selections": [
              (v1/*:: as any*/)
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "27687942dab066a4f95a1d3c81a5525e",
    "id": null,
    "metadata": {},
    "name": "organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation(\n  $input: AddCustomerPreferredResourceInput!\n) {\n  addCustomerPreferredResource(input: $input) {\n    customer {\n      id\n      preferredResources {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2aa796973aad66b427dcf2e47fa48c49";

export default node;
