/**
 * @generated SignedSource<<79cfb879da9cc1233a69fab830e75337>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationPhysicalAddressInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city: string;
  clientMutationId?: string | null | undefined;
  country: string;
  id: string;
  latitude?: any | null | undefined;
  longitude?: any | null | undefined;
  province?: string | null | undefined;
  suburb: string;
  zipcode: string;
};
export type organizationAdmin_updateOrganizationPhysicalAddressMutation$variables = {
  input: UpdateOrganizationPhysicalAddressInput;
};
export type organizationAdmin_updateOrganizationPhysicalAddressMutation$data = {
  readonly updateOrganizationPhysicalAddress: {
    readonly organization: {
      readonly id: string;
      readonly physicalAddress: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly country: string;
        readonly id: string;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      } | null | undefined;
    };
  };
};
export type organizationAdmin_updateOrganizationPhysicalAddressMutation$rawResponse = {
  readonly updateOrganizationPhysicalAddress: {
    readonly organization: {
      readonly id: string;
      readonly physicalAddress: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly country: string;
        readonly id: string;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      } | null | undefined;
    };
  };
};
export type organizationAdmin_updateOrganizationPhysicalAddressMutation = {
  rawResponse: organizationAdmin_updateOrganizationPhysicalAddressMutation$rawResponse;
  response: organizationAdmin_updateOrganizationPhysicalAddressMutation$data;
  variables: organizationAdmin_updateOrganizationPhysicalAddressMutation$variables;
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
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "updateOrganizationPhysicalAddress",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationPhysicalAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "addressLine1",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "addressLine2",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "suburb",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "city",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "province",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "zipcode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "country",
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
    "name": "organizationAdmin_updateOrganizationPhysicalAddressMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_updateOrganizationPhysicalAddressMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "57cbb5597bd0e30427d3f7aa72be738c",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_updateOrganizationPhysicalAddressMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_updateOrganizationPhysicalAddressMutation(\n  $input: UpdateOrganizationPhysicalAddressInput!\n) {\n  updateOrganizationPhysicalAddress(input: $input) {\n    organization {\n      id\n      physicalAddress {\n        id\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9f2a4b5ec35e5bc9b2ab30c508c634e8";

export default node;
