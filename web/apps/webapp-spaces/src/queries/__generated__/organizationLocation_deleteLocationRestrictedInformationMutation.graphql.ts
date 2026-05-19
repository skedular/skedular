/**
 * @generated SignedSource<<b02710dae66d1b440d9a34da81823009>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type LocationRestrictedInformationCategory = "ACCESS" | "ACCESSIBILITY" | "AFTER_HOURS" | "CHECK_IN" | "CHECK_OUT" | "CLEANING" | "DELIVERIES" | "EQUIPMENT" | "EVACUATION" | "GUESTS" | "HOUSE_RULES" | "KITCHEN" | "MAINTENANCE" | "MEETING_ROOMS" | "NOISE" | "OTHER" | "PARKING" | "PETS" | "SECURITY" | "SMOKING" | "STORAGE" | "WASTE" | "WIFI" | "%future added value";
export type DeleteLocationRestrictedInformationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationLocation_deleteLocationRestrictedInformationMutation$variables = {
  input: DeleteLocationRestrictedInformationInput;
};
export type organizationLocation_deleteLocationRestrictedInformationMutation$data = {
  readonly deleteLocationRestrictedInformation: {
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
export type organizationLocation_deleteLocationRestrictedInformationMutation = {
  response: organizationLocation_deleteLocationRestrictedInformationMutation$data;
  variables: organizationLocation_deleteLocationRestrictedInformationMutation$variables;
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
    "name": "deleteLocationRestrictedInformation",
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
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationRestrictedInformationDetails",
            "kind": "LinkedField",
            "name": "restrictedInformation",
            "plural": true,
            "selections": [
              (v1/*:: as any*/),
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocation_deleteLocationRestrictedInformationMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationLocation_deleteLocationRestrictedInformationMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "eab1e1e37e711d3ce954899cf49e2439",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_deleteLocationRestrictedInformationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_deleteLocationRestrictedInformationMutation(\n  $input: DeleteLocationRestrictedInformationInput!\n) {\n  deleteLocationRestrictedInformation(input: $input) {\n    location {\n      id\n      restrictedInformation {\n        id\n        title\n        category\n        content\n        active\n        sortOrder\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "fcb1a6370cbd507ffb1e7fa91e9f7b14";

export default node;
