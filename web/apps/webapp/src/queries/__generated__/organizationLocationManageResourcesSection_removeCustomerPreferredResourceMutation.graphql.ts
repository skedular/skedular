/**
 * @generated SignedSource<<e637427817a4535a8868bcf4725fca26>>
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
export type organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation$variables = {
  input: RemoveCustomerPreferredResourceInput;
};
export type organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation$data = {
  readonly removeCustomerPreferredResource: {
    readonly customer: {
      readonly id: string;
      readonly preferredResources: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation = {
  response: organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation$data;
  variables: organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation$variables;
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
    "name": "organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "5d7611f9c5d2f11d7bda6d5562b5d0b7",
    "id": null,
    "metadata": {},
    "name": "organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation(\n  $input: RemoveCustomerPreferredResourceInput!\n) {\n  removeCustomerPreferredResource(input: $input) {\n    customer {\n      id\n      preferredResources {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "dd32359d46168f60c81ce9858c215276";

export default node;
