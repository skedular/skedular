/**
 * @generated SignedSource<<a278d50f9e2ec74bf2788f078909821f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type editResource_query$data = {
  readonly location: {
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
  } | null | undefined;
  readonly organization: {
    readonly type: {
      readonly type: OrganizationType;
    };
  } | null | undefined;
  readonly resource: {
    readonly availableHours: {
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
    } | null | undefined;
    readonly capacity: number;
    readonly color: string | null | undefined;
    readonly customTags: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly id: string;
      readonly name: string;
    }>;
    readonly id: string;
    readonly inactive: boolean;
    readonly isAvailableHoursOverridden: boolean;
    readonly name: string;
    readonly productTags: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly id: string;
      readonly name: string;
    }>;
    readonly requireBookingApproval: boolean;
    readonly resourceType: {
      readonly color: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
    readonly zones: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly id: string;
      readonly name: string;
    }>;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesCustomTags_query" | "multipleChoicesProductTags_query" | "multipleChoicesZones_query" | "singleChoiceResourceType_query" | "weekOpeningHours_query">;
  readonly " $fragmentType": "editResource_query";
};
export type editResource_query$key = {
  readonly " $data"?: editResource_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editResource_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
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
v1 = [
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
        "selections": (v0/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "tuesday",
        "plural": false,
        "selections": (v0/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "wednesday",
        "plural": false,
        "selections": (v0/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "thursday",
        "plural": false,
        "selections": (v0/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "friday",
        "plural": false,
        "selections": (v0/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "saturday",
        "plural": false,
        "selections": (v0/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OpeningHoursDetails",
        "kind": "LinkedField",
        "name": "sunday",
        "plural": false,
        "selections": (v0/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
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
  "name": "color",
  "storageKey": null
},
v5 = [
  (v2/*: any*/),
  (v3/*: any*/),
  (v4/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    },
    {
      "kind": "RootArgument",
      "name": "resourceId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "editResource_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "uniqueAlphanumericName",
          "variableName": "organizationUniqueAlphanumericName"
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
        {
          "alias": null,
          "args": null,
          "concreteType": "OpeningHours",
          "kind": "LinkedField",
          "name": "openingHours",
          "plural": false,
          "selections": (v1/*: any*/),
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
          "variableName": "resourceId"
        }
      ],
      "concreteType": "ResourceDetails",
      "kind": "LinkedField",
      "name": "resource",
      "plural": false,
      "selections": [
        (v2/*: any*/),
        (v3/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "inactive",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "requireBookingApproval",
          "storageKey": null
        },
        (v4/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "capacity",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "customTags",
          "plural": true,
          "selections": (v5/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "zones",
          "plural": true,
          "selections": (v5/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "productTags",
          "plural": true,
          "selections": (v5/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "resourceType",
          "plural": false,
          "selections": (v5/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "isAvailableHoursOverridden",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OpeningHours",
          "kind": "LinkedField",
          "name": "availableHours",
          "plural": false,
          "selections": (v1/*: any*/),
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceResourceType_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesCustomTags_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesZones_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesProductTags_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "weekOpeningHours_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "b23963bf37167c02bd9ca954886cc999";

export default node;
