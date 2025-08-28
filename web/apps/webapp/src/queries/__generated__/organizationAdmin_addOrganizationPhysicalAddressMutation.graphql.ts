/**
 * @generated SignedSource<<28f66f6b72c28d1b73cada55d5f046a0>>
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
  city?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  country: string;
  countryCode?: string | null | undefined;
  formattedAddress?: string | null | undefined;
  id?: string | null | undefined;
  latitude?: number | null | undefined;
  longitude?: number | null | undefined;
  organizationId?: string | null | undefined;
  organizationUniqueAlphanumericName?: string | null | undefined;
  osmId?: string | null | undefined;
  osmType?: string | null | undefined;
  placeId?: string | null | undefined;
  province?: string | null | undefined;
  suburb?: string | null | undefined;
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
        readonly city: string | null | undefined;
        readonly country: string;
        readonly countryCode: string | null | undefined;
        readonly formattedAddress: string | null | undefined;
        readonly id: string;
        readonly latitude: number | null | undefined;
        readonly longitude: number | null | undefined;
        readonly osmId: string | null | undefined;
        readonly osmType: string | null | undefined;
        readonly placeId: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string | null | undefined;
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
        readonly city: string | null | undefined;
        readonly country: string;
        readonly countryCode: string | null | undefined;
        readonly formattedAddress: string | null | undefined;
        readonly id: string;
        readonly latitude: number | null | undefined;
        readonly longitude: number | null | undefined;
        readonly osmId: string | null | undefined;
        readonly osmType: string | null | undefined;
        readonly placeId: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string | null | undefined;
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
                "name": "osmType",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "osmId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "placeId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "longitude",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "latitude",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "formattedAddress",
                "storageKey": null
              },
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
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "countryCode",
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
    "cacheID": "84c2ef943300108fd694f1accec132b5",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_addOrganizationPhysicalAddressMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_addOrganizationPhysicalAddressMutation(\n  $input: AddOrganizationPhysicalAddressInput!\n) {\n  addOrganizationPhysicalAddress(input: $input) {\n    organization {\n      id\n      physicalAddress {\n        id\n        osmType\n        osmId\n        placeId\n        longitude\n        latitude\n        formattedAddress\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n        countryCode\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ae54b57972d29ba3f3927fc143c72805";

export default node;
