/**
 * @generated SignedSource<<2f49f1ad854abfb30eeca5afcc1c2ed4>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddOrganizationPhysicalAddressInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city: string;
  clientMutationId?: string | null | undefined;
  country: string;
  id?: string | null | undefined;
  latitude?: any | null | undefined;
  longitude?: any | null | undefined;
  organizationId: string;
  province?: string | null | undefined;
  suburb: string;
  zipcode: string;
};
export type organizationAdmin_addOrganizationPhysicalAddressMutation$variables = {
  input: AddOrganizationPhysicalAddressInput;
};
export type organizationAdmin_addOrganizationPhysicalAddressMutation$data = {
  readonly addOrganizationPhysicalAddress: {
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
export type organizationAdmin_addOrganizationPhysicalAddressMutation$rawResponse = {
  readonly addOrganizationPhysicalAddress: {
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
export type organizationAdmin_addOrganizationPhysicalAddressMutation = {
  rawResponse: organizationAdmin_addOrganizationPhysicalAddressMutation$rawResponse;
  response: organizationAdmin_addOrganizationPhysicalAddressMutation$data;
  variables: organizationAdmin_addOrganizationPhysicalAddressMutation$variables;
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
    "name": "addOrganizationPhysicalAddress",
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
    "name": "organizationAdmin_addOrganizationPhysicalAddressMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_addOrganizationPhysicalAddressMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "7d7621ce2c23c1eacc1ca0f292ff5e6b",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_addOrganizationPhysicalAddressMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_addOrganizationPhysicalAddressMutation(\n  $input: AddOrganizationPhysicalAddressInput!\n) {\n  addOrganizationPhysicalAddress(input: $input) {\n    organization {\n      id\n      physicalAddress {\n        id\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e42985c461e189360bd78116ff8d342f";

export default node;
