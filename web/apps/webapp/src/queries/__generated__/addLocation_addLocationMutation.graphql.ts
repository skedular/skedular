/**
 * @generated SignedSource<<176601f95d708bc6bb0d3c1e78dfbf3e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddLocationInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  id?: string | null | undefined;
  locationTagIds: ReadonlyArray<string>;
  name: string;
  organizationId: string;
  physicalAddress?: LocationAddressDetailsInput | null | undefined;
  timezone?: string | null | undefined;
};
export type LocationAddressDetailsInput = {
  addressLine1?: string | null | undefined;
  addressLine2?: string | null | undefined;
  city?: string | null | undefined;
  country?: string | null | undefined;
  formattedAddress?: string | null | undefined;
  province?: string | null | undefined;
  suburb?: string | null | undefined;
  zipcode?: string | null | undefined;
};
export type addLocation_addLocationMutation$variables = {
  input: AddLocationInput;
};
export type addLocation_addLocationMutation$data = {
  readonly addLocation: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly locationTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly name: string;
      readonly physicalAddress: {
        readonly formattedAddress: string | null | undefined;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type addLocation_addLocationMutation$rawResponse = {
  readonly addLocation: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly locationTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly name: string;
      readonly physicalAddress: {
        readonly formattedAddress: string | null | undefined;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type addLocation_addLocationMutation = {
  rawResponse: addLocation_addLocationMutation$rawResponse;
  response: addLocation_addLocationMutation$data;
  variables: addLocation_addLocationMutation$variables;
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
  "name": "name",
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
    "name": "addLocation",
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "about",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "timezone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "formattedAddress",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Location_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "locationTags",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
                "storageKey": null
              },
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "color",
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
    "name": "addLocation_addLocationMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addLocation_addLocationMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "403b145808e05e0a5735bfc2c9846a47",
    "id": null,
    "metadata": {},
    "name": "addLocation_addLocationMutation",
    "operationKind": "mutation",
    "text": "mutation addLocation_addLocationMutation(\n  $input: AddLocationInput!\n) {\n  addLocation(input: $input) {\n    location {\n      id\n      name\n      about\n      timezone\n      physicalAddress {\n        formattedAddress\n      }\n      locationTags {\n        uniqueId\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "35a4b1987e4ef49ff75eec5242f93628";

export default node;
