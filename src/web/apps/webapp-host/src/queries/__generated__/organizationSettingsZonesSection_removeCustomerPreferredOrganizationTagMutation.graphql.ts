/**
 * @generated SignedSource<<2e0221e770cf76804125a075c01fb884>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPreferredOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  organizationTagId: string;
};
export type organizationSettingsZonesSection_removeCustomerPreferredOrganizationTagMutation$variables = {
  input: RemoveCustomerPreferredOrganizationTagInput;
};
export type organizationSettingsZonesSection_removeCustomerPreferredOrganizationTagMutation$data = {
  readonly removeCustomerPreferredOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationSettingsZonesSection_removeCustomerPreferredOrganizationTagMutation = {
  response: organizationSettingsZonesSection_removeCustomerPreferredOrganizationTagMutation$data;
  variables: organizationSettingsZonesSection_removeCustomerPreferredOrganizationTagMutation$variables;
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
    "name": "removeCustomerPreferredOrganizationTag",
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
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredZones",
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
    "name": "organizationSettingsZonesSection_removeCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsZonesSection_removeCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "22bddf2006a9d4c13f74ebcd6a70effc",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsZonesSection_removeCustomerPreferredOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsZonesSection_removeCustomerPreferredOrganizationTagMutation(\n  $input: RemoveCustomerPreferredOrganizationTagInput!\n) {\n  removeCustomerPreferredOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "47e5f2dd7d943dade819bbda328451b4";

export default node;
