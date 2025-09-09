/**
 * @generated SignedSource<<84e07856cb2727907ad6a91627f3f1a9>>
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
        readonly id: string;
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
    "name": "organizationLocation_removeCustomerPreferredResourceMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_removeCustomerPreferredResourceMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "fce7033f2db5b0d97d63d9fdc1ba963f",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_removeCustomerPreferredResourceMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_removeCustomerPreferredResourceMutation(\n  $input: RemoveCustomerPreferredResourceInput!\n) {\n  removeCustomerPreferredResource(input: $input) {\n    customer {\n      id\n      preferredResources {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "75d445386afc025984c9f332fccb93b0";

export default node;
