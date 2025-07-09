/**
 * @generated SignedSource<<06e7bb938e8ab5666ba5bd8bdbefe292>>
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
  contactEmail?: string | null | undefined;
  contactPhone?: string | null | undefined;
  id: string;
  locationTagIds: ReadonlyArray<string>;
  name: string;
  primaryFeatureImage?: CdnImageFileInput | null | undefined;
  timezone?: string | null | undefined;
};
export type CdnImageFileInput = {
  original?: CdnFileInput | null | undefined;
  thumbnail?: CdnFileInput | null | undefined;
};
export type CdnFileInput = {
  height?: number | null | undefined;
  url: string;
  width?: number | null | undefined;
};
export type organizationLocation_updateLocationMutation$variables = {
  input: UpdateLocationInput;
};
export type organizationLocation_updateLocationMutation$data = {
  readonly updateLocation: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly contactEmail: string | null | undefined;
      readonly contactPhone: string | null | undefined;
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
      readonly primaryFeatureImage: {
        readonly original: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
        readonly thumbnail: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  };
};
export type organizationLocation_updateLocationMutation$rawResponse = {
  readonly updateLocation: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly contactEmail: string | null | undefined;
      readonly contactPhone: string | null | undefined;
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
      readonly primaryFeatureImage: {
        readonly original: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
        readonly thumbnail: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  };
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
    "name": "url",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "height",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "width",
    "storageKey": null
  }
],
v3 = [
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
v4 = [
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
            "kind": "ScalarField",
            "name": "contactEmail",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "contactPhone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CdnImageFile",
            "kind": "LinkedField",
            "name": "primaryFeatureImage",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "original",
                "plural": false,
                "selections": (v2/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v2/*: any*/),
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
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "tuesday",
                    "plural": false,
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "wednesday",
                    "plural": false,
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "thursday",
                    "plural": false,
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "friday",
                    "plural": false,
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "saturday",
                    "plural": false,
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "sunday",
                    "plural": false,
                    "selections": (v3/*: any*/),
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
    "selections": (v4/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_updateLocationMutation",
    "selections": (v4/*: any*/)
  },
  "params": {
    "cacheID": "ecffb2d6c5043585770fae1f66aecac2",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_updateLocationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_updateLocationMutation(\n  $input: UpdateLocationInput!\n) {\n  updateLocation(input: $input) {\n    location {\n      id\n      name\n      about\n      timezone\n      contactEmail\n      contactPhone\n      primaryFeatureImage {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n      locationTags {\n        uniqueId\n        name\n        color\n      }\n      openingHours {\n        weekOpeningHours {\n          monday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          tuesday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          wednesday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          thursday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          friday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          saturday {\n            closed\n            openAllDay\n            from\n            until\n          }\n          sunday {\n            closed\n            openAllDay\n            from\n            until\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a942dd74139ab337adb27085f7bf3623";

export default node;
