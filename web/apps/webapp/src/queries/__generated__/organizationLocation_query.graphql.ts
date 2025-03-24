/**
 * @generated SignedSource<<cf7f8ab79ce8fec4600fd4892893b9c5>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationLocation_query$data = {
  readonly location: {
    readonly about: string | null | undefined;
    readonly id: string;
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
  } | null | undefined;
  readonly me: {
    readonly id: string;
    readonly preferredResources: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
  } | null | undefined;
  readonly openingHoursMinutesStep: number;
  readonly " $fragmentSpreads": FragmentRefs<"customTagSelector_allCustomTags_query" | "weekOpeningHours_query" | "zoneSelector_allZones_query">;
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
v1 = [
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
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "uniqueId",
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
                  "selections": (v1/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "tuesday",
                  "plural": false,
                  "selections": (v1/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "wednesday",
                  "plural": false,
                  "selections": (v1/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "thursday",
                  "plural": false,
                  "selections": (v1/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "friday",
                  "plural": false,
                  "selections": (v1/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "saturday",
                  "plural": false,
                  "selections": (v1/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "sunday",
                  "plural": false,
                  "selections": (v1/*: any*/),
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

(node as any).hash = "c93b9e2283ae27d250d39f407cfee707";

export default node;
