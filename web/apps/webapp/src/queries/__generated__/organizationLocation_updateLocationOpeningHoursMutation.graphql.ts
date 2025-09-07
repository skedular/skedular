/**
 * @generated SignedSource<<8dfa52e6e45d4261b5908e3e745e784a>>
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
      readonly extraMetadata: {
        readonly areaRange: {
          readonly fromInSqm: string;
          readonly toInSqm: string;
        } | null | undefined;
        readonly contactDetails: {
          readonly contactEmails: ReadonlyArray<string> | null | undefined;
          readonly contactPeople: ReadonlyArray<string> | null | undefined;
          readonly contactPhones: ReadonlyArray<string> | null | undefined;
        } | null | undefined;
        readonly otherLinks: ReadonlyArray<string> | null | undefined;
        readonly peopleCapacity: {
          readonly from: string;
          readonly to: string;
        } | null | undefined;
        readonly relatedImageLinks: ReadonlyArray<string> | null | undefined;
        readonly relatedVideoLinks: ReadonlyArray<string> | null | undefined;
        readonly website: string | null | undefined;
      } | null | undefined;
      readonly id: string;
      readonly locationSpaceTypes: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
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
        readonly city: string | null | undefined;
        readonly country: string;
        readonly countryCode: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string | null | undefined;
        readonly zipcode: string;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  };
};
export type organizationLocation_updateLocationOpeningHoursMutation$rawResponse = {
  readonly updateLocationOpeningHours: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly extraMetadata: {
        readonly areaRange: {
          readonly fromInSqm: string;
          readonly toInSqm: string;
        } | null | undefined;
        readonly contactDetails: {
          readonly contactEmails: ReadonlyArray<string> | null | undefined;
          readonly contactPeople: ReadonlyArray<string> | null | undefined;
          readonly contactPhones: ReadonlyArray<string> | null | undefined;
        } | null | undefined;
        readonly otherLinks: ReadonlyArray<string> | null | undefined;
        readonly peopleCapacity: {
          readonly from: string;
          readonly to: string;
        } | null | undefined;
        readonly relatedImageLinks: ReadonlyArray<string> | null | undefined;
        readonly relatedVideoLinks: ReadonlyArray<string> | null | undefined;
        readonly website: string | null | undefined;
      } | null | undefined;
      readonly id: string;
      readonly locationSpaceTypes: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
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
        readonly city: string | null | undefined;
        readonly country: string;
        readonly countryCode: string | null | undefined;
        readonly id: string;
        readonly province: string | null | undefined;
        readonly suburb: string | null | undefined;
        readonly zipcode: string;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  };
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
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "about",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "timezone",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "LocationExtraMetadata",
  "kind": "LinkedField",
  "name": "extraMetadata",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ContactDetails",
      "kind": "LinkedField",
      "name": "contactDetails",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "contactPeople",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "contactEmails",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "contactPhones",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "AreaRange",
      "kind": "LinkedField",
      "name": "areaRange",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "fromInSqm",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "toInSqm",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "PeopleCapacity",
      "kind": "LinkedField",
      "name": "peopleCapacity",
      "plural": false,
      "selections": [
        (v6/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "to",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "website",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "relatedImageLinks",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "relatedVideoLinks",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "otherLinks",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "addressLine1",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "addressLine2",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "suburb",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "city",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "province",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "zipcode",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "country",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "countryCode",
  "storageKey": null
},
v16 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v3/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
],
v17 = {
  "alias": null,
  "args": null,
  "concreteType": "Location_OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "locationTags",
  "plural": true,
  "selections": (v16/*: any*/),
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "concreteType": "Location_OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "locationSpaceTypes",
  "plural": true,
  "selections": (v16/*: any*/),
  "storageKey": null
},
v19 = [
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
  (v6/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "until",
    "storageKey": null
  }
],
v20 = {
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
          "selections": (v19/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OpeningHoursDetails",
          "kind": "LinkedField",
          "name": "tuesday",
          "plural": false,
          "selections": (v19/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OpeningHoursDetails",
          "kind": "LinkedField",
          "name": "wednesday",
          "plural": false,
          "selections": (v19/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OpeningHoursDetails",
          "kind": "LinkedField",
          "name": "thursday",
          "plural": false,
          "selections": (v19/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OpeningHoursDetails",
          "kind": "LinkedField",
          "name": "friday",
          "plural": false,
          "selections": (v19/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OpeningHoursDetails",
          "kind": "LinkedField",
          "name": "saturday",
          "plural": false,
          "selections": (v19/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OpeningHoursDetails",
          "kind": "LinkedField",
          "name": "sunday",
          "plural": false,
          "selections": (v19/*: any*/),
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocation_updateLocationOpeningHoursMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/),
              (v3/*: any*/),
              (v4/*: any*/),
              (v5/*: any*/),
              (v7/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationPhysicalAddressDetails",
                "kind": "LinkedField",
                "name": "physicalAddress",
                "plural": false,
                "selections": [
                  (v8/*: any*/),
                  (v9/*: any*/),
                  (v10/*: any*/),
                  (v11/*: any*/),
                  (v12/*: any*/),
                  (v13/*: any*/),
                  (v14/*: any*/),
                  (v15/*: any*/)
                ],
                "storageKey": null
              },
              (v17/*: any*/),
              (v18/*: any*/),
              (v20/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_updateLocationOpeningHoursMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/),
              (v3/*: any*/),
              (v4/*: any*/),
              (v5/*: any*/),
              (v7/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationPhysicalAddressDetails",
                "kind": "LinkedField",
                "name": "physicalAddress",
                "plural": false,
                "selections": [
                  (v8/*: any*/),
                  (v9/*: any*/),
                  (v10/*: any*/),
                  (v11/*: any*/),
                  (v12/*: any*/),
                  (v13/*: any*/),
                  (v14/*: any*/),
                  (v15/*: any*/),
                  (v2/*: any*/)
                ],
                "storageKey": null
              },
              (v17/*: any*/),
              (v18/*: any*/),
              (v20/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "63fe12a4f61e7479de2e093a7cb10f24",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_updateLocationOpeningHoursMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_updateLocationOpeningHoursMutation(\n  $input: UpdateLocationOpeningHoursInput!\n) {\n  updateLocationOpeningHours(input: $input) {\n    location {\n      id\n      name\n      about\n      timezone\n      extraMetadata {\n        contactDetails {\n          contactPeople\n          contactEmails\n          contactPhones\n        }\n        areaRange {\n          fromInSqm\n          toInSqm\n        }\n        peopleCapacity {\n          from\n          to\n        }\n        website\n        relatedImageLinks\n        relatedVideoLinks\n        otherLinks\n      }\n      physicalAddress {\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n        countryCode\n        id\n      }\n      locationTags {\n        uniqueId\n        name\n        color\n      }\n      locationSpaceTypes {\n        uniqueId\n        name\n        color\n      }\n      openingHours {\n        weekOpeningHours {\n          monday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          tuesday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          wednesday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          thursday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          friday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          saturday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          sunday {\n            closed\n            openAllDay\n            from\n            until\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5d77adee28e4062da652240b3c3dbccb";

export default node;
