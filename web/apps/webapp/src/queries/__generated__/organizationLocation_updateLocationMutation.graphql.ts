/**
 * @generated SignedSource<<9088a1922cc24fd020b943d2dffd5816>>
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
  locationTagIds: ReadonlyArray<string>;
  name: string;
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
      readonly locationTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly name: string;
      readonly openingHours: {
        readonly weekOpeningHours: {
          readonly friday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly monday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly saturday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly sunday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly thursday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly tuesday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly wednesday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
        };
      };
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
      readonly locationTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly name: string;
      readonly openingHours: {
        readonly weekOpeningHours: {
          readonly friday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly monday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly saturday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly sunday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly thursday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly tuesday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
          readonly wednesday: {
            readonly closed: boolean;
            readonly from: string | null | undefined;
            readonly openAllDay: boolean;
            readonly until: string | null | undefined;
          };
        };
      };
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
    "args": null,
    "kind": "ScalarField",
    "name": "closed",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "openAllDay",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "from",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "until",
    "storageKey": null
  }
],
v3 = [
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
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OpeningHours",
            "kind": "LinkedField",
            "name": "openingHours",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "WeekOpeningHours",
                "kind": "LinkedField",
                "name": "weekOpeningHours",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "monday",
                    "plural": false,
                    "selections": (v2/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "tuesday",
                    "plural": false,
                    "selections": (v2/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "wednesday",
                    "plural": false,
                    "selections": (v2/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "thursday",
                    "plural": false,
                    "selections": (v2/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "friday",
                    "plural": false,
                    "selections": (v2/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "saturday",
                    "plural": false,
                    "selections": (v2/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "sunday",
                    "plural": false,
                    "selections": (v2/*: any*/),
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
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_updateLocationMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "9eb01dcb5f9975ef9a4e8bb8c5803550",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_updateLocationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_updateLocationMutation(\n  $input: UpdateLocationInput!\n) {\n  updateLocation(input: $input) {\n    location {\n      id\n      name\n      about\n      timezone\n      physicalAddress {\n        formattedAddress\n      }\n      locationTags {\n        uniqueId\n        name\n        color\n      }\n      openingHours {\n        weekOpeningHours {\n          monday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          tuesday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          wednesday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          thursday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          friday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          saturday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          sunday {\n            closed\n            openAllDay\n            from\n            until\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2b540befcd9df6646bead55987d80403";

export default node;
