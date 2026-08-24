/**
 * @generated SignedSource<<796e11f9eec6a3dd445333516a34e323>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerPreferredOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  organizationTagId: string;
};
export type organizationSettingsZonesSection_addCustomerPreferredOrganizationTagMutation$variables = {
  input: AddCustomerPreferredOrganizationTagInput;
};
export type organizationSettingsZonesSection_addCustomerPreferredOrganizationTagMutation$data = {
  readonly addCustomerPreferredOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationSettingsZonesSection_addCustomerPreferredOrganizationTagMutation = {
  response: organizationSettingsZonesSection_addCustomerPreferredOrganizationTagMutation$data;
  variables: organizationSettingsZonesSection_addCustomerPreferredOrganizationTagMutation$variables;
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
    "name": "addCustomerPreferredOrganizationTag",
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
    "name": "organizationSettingsZonesSection_addCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsZonesSection_addCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "1cb434c137ff6d4adfe766656bfd5963",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsZonesSection_addCustomerPreferredOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsZonesSection_addCustomerPreferredOrganizationTagMutation(\n  $input: AddCustomerPreferredOrganizationTagInput!\n) {\n  addCustomerPreferredOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "445fdfaed5de0e60d2cd2ae4f08c8989";

export default node;
