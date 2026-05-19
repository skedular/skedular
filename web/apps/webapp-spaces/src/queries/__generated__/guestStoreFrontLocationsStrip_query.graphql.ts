/**
 * @generated SignedSource<<7bc46c08c4dad0fe370c4c1d52677d39>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontLocationsStrip_query$data = {
  readonly marketplaceLocations: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly floorPlanCount: number;
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
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentType": "guestStoreFrontLocationsStrip_query";
};
export type guestStoreFrontLocationsStrip_query$key = {
  readonly " $data"?: guestStoreFrontLocationsStrip_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontLocationsStrip_query">;
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
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "guestStoreFrontLocationsStrip_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "organizationCustomDomain",
              "variableName": "organizationCustomDomain"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ConnectionOfLocationEdge",
      "kind": "LinkedField",
      "name": "marketplaceLocations",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "totalCount",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "LocationDetails",
              "kind": "LinkedField",
              "name": "node",
              "plural": false,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "id",
                  "storageKey": null
                },
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
                  "name": "timezone",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "floorPlanCount",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "LocationPhysicalAddressDetails",
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
                          "selections": (v0/*:: as any*/),
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OpeningHoursDetails",
                          "kind": "LinkedField",
                          "name": "tuesday",
                          "plural": false,
                          "selections": (v0/*:: as any*/),
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OpeningHoursDetails",
                          "kind": "LinkedField",
                          "name": "wednesday",
                          "plural": false,
                          "selections": (v0/*:: as any*/),
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OpeningHoursDetails",
                          "kind": "LinkedField",
                          "name": "thursday",
                          "plural": false,
                          "selections": (v0/*:: as any*/),
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OpeningHoursDetails",
                          "kind": "LinkedField",
                          "name": "friday",
                          "plural": false,
                          "selections": (v0/*:: as any*/),
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OpeningHoursDetails",
                          "kind": "LinkedField",
                          "name": "saturday",
                          "plural": false,
                          "selections": (v0/*:: as any*/),
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OpeningHoursDetails",
                          "kind": "LinkedField",
                          "name": "sunday",
                          "plural": false,
                          "selections": (v0/*:: as any*/),
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
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "f7e144161e26a8c86b853670b46917ae";

export default node;
