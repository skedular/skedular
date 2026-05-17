/**
 * @generated SignedSource<<190d7720de12c3bdef7b1682570792ae>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type LocationRestrictedInformationCategory = "ACCESS" | "ACCESSIBILITY" | "AFTER_HOURS" | "CHECK_IN" | "CHECK_OUT" | "CLEANING" | "DELIVERIES" | "EQUIPMENT" | "EVACUATION" | "GUESTS" | "HOUSE_RULES" | "KITCHEN" | "MAINTENANCE" | "MEETING_ROOMS" | "NOISE" | "OTHER" | "PARKING" | "PETS" | "SECURITY" | "SMOKING" | "STORAGE" | "WASTE" | "WIFI" | "%future added value";
export type UpdateLocationRestrictedInformationInput = {
  active: boolean;
  category: LocationRestrictedInformationCategory;
  clientMutationId?: string | null | undefined;
  content: string;
  id: string;
  sortOrder: number;
  title: string;
};
export type organizationLocation_updateLocationRestrictedInformationMutation$variables = {
  input: UpdateLocationRestrictedInformationInput;
};
export type organizationLocation_updateLocationRestrictedInformationMutation$data = {
  readonly updateLocationRestrictedInformation: {
    readonly location: {
      readonly id: string;
      readonly restrictedInformation: ReadonlyArray<{
        readonly active: boolean;
        readonly category: LocationRestrictedInformationCategory;
        readonly content: string;
        readonly id: string;
        readonly sortOrder: number;
        readonly title: string;
      }>;
    };
  };
};
export type organizationLocation_updateLocationRestrictedInformationMutation = {
  response: organizationLocation_updateLocationRestrictedInformationMutation$data;
  variables: organizationLocation_updateLocationRestrictedInformationMutation$variables;
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
    "name": "updateLocationRestrictedInformation",
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
            "concreteType": "LocationRestrictedInformationDetails",
            "kind": "LinkedField",
            "name": "restrictedInformation",
            "plural": true,
            "selections": [
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "title",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "category",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "content",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "active",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "sortOrder",
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
    "name": "organizationLocation_updateLocationRestrictedInformationMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_updateLocationRestrictedInformationMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "f15358cc0bbd265f27d11aadfcc981e3",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_updateLocationRestrictedInformationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_updateLocationRestrictedInformationMutation(\n  $input: UpdateLocationRestrictedInformationInput!\n) {\n  updateLocationRestrictedInformation(input: $input) {\n    location {\n      id\n      restrictedInformation {\n        id\n        title\n        category\n        content\n        active\n        sortOrder\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5ec98259a275613f7623c0aad7b4453d";

export default node;
