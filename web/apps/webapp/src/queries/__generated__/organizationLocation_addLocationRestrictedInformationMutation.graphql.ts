/**
 * @generated SignedSource<<1a0590de1a74565f70a9efc2ebcc8b4e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type LocationRestrictedInformationCategory = "ACCESS" | "ACCESSIBILITY" | "AFTER_HOURS" | "CHECK_IN" | "CHECK_OUT" | "CLEANING" | "DELIVERIES" | "EQUIPMENT" | "EVACUATION" | "GUESTS" | "HOUSE_RULES" | "KITCHEN" | "MAINTENANCE" | "MEETING_ROOMS" | "NOISE" | "OTHER" | "PARKING" | "PETS" | "SECURITY" | "SMOKING" | "STORAGE" | "WASTE" | "WIFI" | "%future added value";
export type AddLocationRestrictedInformationInput = {
  active: boolean;
  category: LocationRestrictedInformationCategory;
  clientMutationId?: string | null | undefined;
  content: string;
  locationId: string;
  sortOrder: number;
  title: string;
};
export type organizationLocation_addLocationRestrictedInformationMutation$variables = {
  input: AddLocationRestrictedInformationInput;
};
export type organizationLocation_addLocationRestrictedInformationMutation$data = {
  readonly addLocationRestrictedInformation: {
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
export type organizationLocation_addLocationRestrictedInformationMutation = {
  response: organizationLocation_addLocationRestrictedInformationMutation$data;
  variables: organizationLocation_addLocationRestrictedInformationMutation$variables;
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
    "name": "addLocationRestrictedInformation",
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
    "name": "organizationLocation_addLocationRestrictedInformationMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_addLocationRestrictedInformationMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "d2d14e1f0f9229eadf4bd95bc6633236",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_addLocationRestrictedInformationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_addLocationRestrictedInformationMutation(\n  $input: AddLocationRestrictedInformationInput!\n) {\n  addLocationRestrictedInformation(input: $input) {\n    location {\n      id\n      restrictedInformation {\n        id\n        title\n        category\n        content\n        active\n        sortOrder\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "57066f66dfc5572119696229f4907706";

export default node;
