/**
 * @generated SignedSource<<9d054668d0dbf4665b369039923b7191>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddLocationPhysicalAddressInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city: string;
  clientMutationId?: string | null | undefined;
  country: string;
  id?: string | null | undefined;
  latitude?: any | null | undefined;
  locationId: string;
  longitude?: any | null | undefined;
  province?: string | null | undefined;
  suburb: string;
  zipcode: string;
};
export type organizationLocation_addLocationPhysicalAddressMutation$variables = {
  input: AddLocationPhysicalAddressInput;
};
export type organizationLocation_addLocationPhysicalAddressMutation$data = {
  readonly addLocationPhysicalAddress: {
    readonly location: {
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
export type organizationLocation_addLocationPhysicalAddressMutation$rawResponse = {
  readonly addLocationPhysicalAddress: {
    readonly location: {
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
export type organizationLocation_addLocationPhysicalAddressMutation = {
  rawResponse: organizationLocation_addLocationPhysicalAddressMutation$rawResponse;
  response: organizationLocation_addLocationPhysicalAddressMutation$data;
  variables: organizationLocation_addLocationPhysicalAddressMutation$variables;
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
    "concreteType": "LocationPayload",
    "kind": "LinkedField",
    "name": "addLocationPhysicalAddress",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationPhysicalAddressDetails",
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
    "name": "organizationLocation_addLocationPhysicalAddressMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_addLocationPhysicalAddressMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "9954bfd96d4c90cda33f22794cb9f1c1",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_addLocationPhysicalAddressMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_addLocationPhysicalAddressMutation(\n  $input: AddLocationPhysicalAddressInput!\n) {\n  addLocationPhysicalAddress(input: $input) {\n    location {\n      id\n      physicalAddress {\n        id\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f49e12ccaea1287431190d6d23f11ecc";

export default node;
