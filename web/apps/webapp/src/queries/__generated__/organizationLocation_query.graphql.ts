/**
 * @generated SignedSource<<78f75eac227b8bf59eeb93bb4a7b664f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationLocation_query$data = {
  readonly location: {
    readonly about: string | null | undefined;
    readonly contactEmail: string | null | undefined;
    readonly contactPhone: string | null | undefined;
    readonly floorPlans: ReadonlyArray<{
      readonly floorLevel: number;
      readonly floorName: string | null | undefined;
      readonly height: number;
      readonly id: string;
      readonly imagePath: string;
      readonly isActive: boolean;
      readonly name: string;
      readonly resourcePositions: ReadonlyArray<{
        readonly height: number;
        readonly id: string;
        readonly metadata: string | null | undefined;
        readonly resource: {
          readonly id: string;
          readonly name: string;
        };
        readonly shape: string | null | undefined;
        readonly width: number;
        readonly x: number;
        readonly y: number;
      }>;
      readonly thumbnailPath: string | null | undefined;
      readonly width: number;
    }>;
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
  } | null | undefined;
  readonly me: {
    readonly id: string;
    readonly preferredResources: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
  } | null | undefined;
  readonly openingHoursMinutesStep: number;
  readonly organization: {
    readonly type: {
      readonly type: OrganizationType;
    };
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"customTagSelector_allCustomTags_query" | "multipleChoicesLocationTags_query" | "weekOpeningHours_query" | "zoneSelector_allZones_query">;
  readonly " $fragmentType": "organizationLocation_query";
};
export type organizationLocation_query$key = {
  readonly " $data"?: organizationLocation_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationLocation_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "height",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "width",
  "storageKey": null
},
v5 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "url",
    "storageKey": null
  },
  (v3/*: any*/),
  (v4/*: any*/)
],
v6 = [
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
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationLocation_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "CustomerResourceDetails",
          "kind": "LinkedField",
          "name": "preferredResources",
          "plural": true,
          "selections": [
            (v1/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTypeDetails",
          "kind": "LinkedField",
          "name": "type",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "type",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "locationId"
        }
      ],
      "concreteType": "LocationDetails",
      "kind": "LinkedField",
      "name": "location",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        (v2/*: any*/),
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
              "selections": (v5/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "CdnFile",
              "kind": "LinkedField",
              "name": "thumbnail",
              "plural": false,
              "selections": (v5/*: any*/),
              "storageKey": null
            }
          ],
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
            (v1/*: any*/),
            (v2/*: any*/),
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
                  "selections": (v6/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "tuesday",
                  "plural": false,
                  "selections": (v6/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "wednesday",
                  "plural": false,
                  "selections": (v6/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "thursday",
                  "plural": false,
                  "selections": (v6/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "friday",
                  "plural": false,
                  "selections": (v6/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "saturday",
                  "plural": false,
                  "selections": (v6/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "sunday",
                  "plural": false,
                  "selections": (v6/*: any*/),
                  "storageKey": null
                }
              ],
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "FloorPlanDetails",
          "kind": "LinkedField",
          "name": "floorPlans",
          "plural": true,
          "selections": [
            (v0/*: any*/),
            (v2/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "floorLevel",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "floorName",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "imagePath",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "thumbnailPath",
              "storageKey": null
            },
            (v4/*: any*/),
            (v3/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "isActive",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "ResourcePosition",
              "kind": "LinkedField",
              "name": "resourcePositions",
              "plural": true,
              "selections": [
                (v0/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "x",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "y",
                  "storageKey": null
                },
                (v4/*: any*/),
                (v3/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "shape",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "metadata",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "ResourceDetails",
                  "kind": "LinkedField",
                  "name": "resource",
                  "plural": false,
                  "selections": [
                    (v0/*: any*/),
                    (v2/*: any*/)
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
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "openingHoursMinutesStep",
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesLocationTags_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "weekOpeningHours_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "customTagSelector_allCustomTags_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "zoneSelector_allZones_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "bb929112b20e8579f21f40419acaad71";

export default node;
