/**
 * @generated SignedSource<<a2f7dc0cdd920bd08eeb9382edbe2bc4>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateLocationOpeningHoursInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  weekOpeningHours: WeekOpeningHoursInput;
};
export type WeekOpeningHoursInput = {
  friday: OpeningHoursDetailsInput;
  monday: OpeningHoursDetailsInput;
  saturday: OpeningHoursDetailsInput;
  sunday: OpeningHoursDetailsInput;
  thursday: OpeningHoursDetailsInput;
  tuesday: OpeningHoursDetailsInput;
  wednesday: OpeningHoursDetailsInput;
};
export type OpeningHoursDetailsInput = {
  closed: boolean;
  from?: string | null | undefined;
  openAllDay: boolean;
  until?: string | null | undefined;
};
export type organizationLocation_updateLocationOpeningHoursMutation$variables = {
  input: UpdateLocationOpeningHoursInput;
};
export type organizationLocation_updateLocationOpeningHoursMutation$data = {
  readonly updateLocationOpeningHours: {
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
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly country: string;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      };
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationLocation_updateLocationOpeningHoursMutation$rawResponse = {
  readonly updateLocationOpeningHours: {
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
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly country: string;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      };
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationLocation_updateLocationOpeningHoursMutation = {
  rawResponse: organizationLocation_updateLocationOpeningHoursMutation$rawResponse;
  response: organizationLocation_updateLocationOpeningHoursMutation$data;
  variables: organizationLocation_updateLocationOpeningHoursMutation$variables;
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
    "name": "updateLocationOpeningHours",
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
    "name": "organizationLocation_updateLocationOpeningHoursMutation",
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_updateLocationOpeningHoursMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "d98bd46784b54f73c152abe2fb9776b9",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_updateLocationOpeningHoursMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_updateLocationOpeningHoursMutation(\n  $input: UpdateLocationOpeningHoursInput!\n) {\n  updateLocationOpeningHours(input: $input) {\n    location {\n      id\n      name\n      about\n      timezone\n      physicalAddress {\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n      }\n      locationTags {\n        uniqueId\n        name\n        color\n      }\n      openingHours {\n        weekOpeningHours {\n          monday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          tuesday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          wednesday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          thursday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          friday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          saturday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          sunday {\n            closed\n            openAllDay\n            from\n            until\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "abbe69925889ffc9fe06d8fbca2bb4a9";

export default node;
