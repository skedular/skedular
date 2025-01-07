/**
 * @generated SignedSource<<18d57bb157b9c287ad1848900bbadaae>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateLocationInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  id: string;
  name: string;
  organizationId?: string | null | undefined;
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
export type organizationLocation_updateLocationMutation$variables = {
  input: UpdateLocationInput;
};
export type organizationLocation_updateLocationMutation$data = {
  readonly updateLocation: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly physicalAddress: {
        readonly formattedAddress: string | null | undefined;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationLocation_updateLocationMutation$rawResponse = {
  readonly updateLocation: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly physicalAddress: {
        readonly formattedAddress: string | null | undefined;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationLocation_updateLocationMutation = {
  rawResponse: organizationLocation_updateLocationMutation$rawResponse;
  response: organizationLocation_updateLocationMutation$data;
  variables: organizationLocation_updateLocationMutation$variables;
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
    "concreteType": "LocationPayload",
    "kind": "LinkedField",
    "name": "updateLocation",
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          },
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
    "name": "organizationLocation_updateLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_updateLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "c1ed5f8490b8dff70bda26228edaedeb",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_updateLocationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_updateLocationMutation(\n  $input: UpdateLocationInput!\n) {\n  updateLocation(input: $input) {\n    location {\n      id\n      name\n      about\n      timezone\n      physicalAddress {\n        formattedAddress\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7b505f82747f2aa13f7cb1ec00529385";

export default node;
