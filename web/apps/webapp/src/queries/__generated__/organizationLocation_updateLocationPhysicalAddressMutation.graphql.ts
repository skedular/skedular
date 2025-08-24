/**
 * @generated SignedSource<<91df934d05049b25d01475a8d0718c4a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateLocationPhysicalAddressInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city: string;
  clientMutationId?: string | null | undefined;
  country: string;
  formattedAddress?: string | null | undefined;
  id: string;
  latitude?: number | null | undefined;
  longitude?: number | null | undefined;
  osmId?: string | null | undefined;
  osmType?: string | null | undefined;
  placeId?: string | null | undefined;
  province?: string | null | undefined;
  suburb: string;
  zipcode: string;
};
export type organizationLocation_updateLocationPhysicalAddressMutation$variables = {
  input: UpdateLocationPhysicalAddressInput;
};
export type organizationLocation_updateLocationPhysicalAddressMutation$data = {
  readonly updateLocationPhysicalAddress: {
    readonly location: {
      readonly id: string;
      readonly physicalAddress: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly country: string;
        readonly formattedAddress: string | null | undefined;
        readonly id: string;
        readonly latitude: number | null | undefined;
        readonly longitude: number | null | undefined;
        readonly osmId: string | null | undefined;
        readonly osmType: string | null | undefined;
        readonly placeId: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      } | null | undefined;
    };
  };
};
export type organizationLocation_updateLocationPhysicalAddressMutation$rawResponse = {
  readonly updateLocationPhysicalAddress: {
    readonly location: {
      readonly id: string;
      readonly physicalAddress: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly country: string;
        readonly formattedAddress: string | null | undefined;
        readonly id: string;
        readonly latitude: number | null | undefined;
        readonly longitude: number | null | undefined;
        readonly osmId: string | null | undefined;
        readonly osmType: string | null | undefined;
        readonly placeId: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      } | null | undefined;
    };
  };
};
export type organizationLocation_updateLocationPhysicalAddressMutation = {
  rawResponse: organizationLocation_updateLocationPhysicalAddressMutation$rawResponse;
  response: organizationLocation_updateLocationPhysicalAddressMutation$data;
  variables: organizationLocation_updateLocationPhysicalAddressMutation$variables;
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
    "name": "updateLocationPhysicalAddress",
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
    "name": "organizationLocation_updateLocationPhysicalAddressMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_updateLocationPhysicalAddressMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "e07b4e2697d554a490f99095c77a4a17",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_updateLocationPhysicalAddressMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_updateLocationPhysicalAddressMutation(\n  $input: UpdateLocationPhysicalAddressInput!\n) {\n  updateLocationPhysicalAddress(input: $input) {\n    location {\n      id\n      physicalAddress {\n        id\n        osmType\n        osmId\n        placeId\n        longitude\n        latitude\n        formattedAddress\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d10b58e4b0ed17fa89b312136d48c39f";

export default node;
