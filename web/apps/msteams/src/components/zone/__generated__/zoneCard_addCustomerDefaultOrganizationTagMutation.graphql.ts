/**
 * @generated SignedSource<<f72e3e51515c0678ec04a619884dee2e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerDefaultOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  organizationTagId: string;
};
export type zoneCard_addCustomerDefaultOrganizationTagMutation$variables = {
  input: AddCustomerDefaultOrganizationTagInput;
};
export type zoneCard_addCustomerDefaultOrganizationTagMutation$data = {
  readonly addCustomerDefaultOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type zoneCard_addCustomerDefaultOrganizationTagMutation = {
  response: zoneCard_addCustomerDefaultOrganizationTagMutation$data;
  variables: zoneCard_addCustomerDefaultOrganizationTagMutation$variables;
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
    "name": "addCustomerDefaultOrganizationTag",
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
            "concreteType": "CustomerOrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredZones",
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
    "name": "zoneCard_addCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "zoneCard_addCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "b80745a146a39ead72dd186cbf0d57e0",
    "id": null,
    "metadata": {},
    "name": "zoneCard_addCustomerDefaultOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation zoneCard_addCustomerDefaultOrganizationTagMutation(\n  $input: AddCustomerDefaultOrganizationTagInput!\n) {\n  addCustomerDefaultOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "03c3bc0b78c1cf4634790fd0cf89c592";

export default node;
