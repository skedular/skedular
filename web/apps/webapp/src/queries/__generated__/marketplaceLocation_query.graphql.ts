/**
 * @generated SignedSource<<8ce0f019f06beb6987e2a16b647eb00e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type ProductPricingCadence = "DAILY_V1" | "FIVE_MONTHS_V1" | "FOUR_MONTHS_V1" | "HALF_DAY_V1" | "MONTHLY_V1" | "NOT_SET" | "ONE_TIME_V1" | "PER15_MINUTE_V1" | "PER30_MINUTE_V1" | "PER_HOUR_V1" | "PER_MINUTE_V1" | "QUARTERLY_V1" | "SIX_MONTHS_V1" | "TWO_MONTHS_V1" | "WEEKLY_V1" | "YEARLY_V1" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type marketplaceLocation_query$data = {
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly location: {
    readonly amenities: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
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
      readonly peopleCapacity: {
        readonly from: string;
        readonly to: string;
      } | null | undefined;
      readonly relatedImageLinks: ReadonlyArray<string> | null | undefined;
      readonly website: string | null | undefined;
    } | null | undefined;
    readonly featureImages: ReadonlyArray<{
      readonly original: {
        readonly height: number | null | undefined;
        readonly url: string;
        readonly width: number | null | undefined;
      } | null | undefined;
    }>;
    readonly id: string;
    readonly listingMetadata: {
      readonly about: string | null | undefined;
      readonly includedFeatures: ReadonlyArray<string> | null | undefined;
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
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
    readonly organization: {
      readonly uniqueAlphanumericName: string | null | undefined;
    };
    readonly physicalAddress: {
      readonly latitude: number | null | undefined;
      readonly longitude: number | null | undefined;
      readonly multilinesFormattedAddress: string | null | undefined;
    } | null | undefined;
    readonly products: ReadonlyArray<{
      readonly amenities: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly currency: {
        readonly type: Currency;
      };
      readonly featureImages: ReadonlyArray<{
        readonly original: {
          readonly url: string;
        } | null | undefined;
      }>;
      readonly id: string;
      readonly listingMetadata: {
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly pricingOptions: ReadonlyArray<{
        readonly cadence: ProductPricingCadence;
        readonly id: string;
        readonly index: number;
        readonly isTaxInclusive: boolean;
        readonly listingMetadata: {
          readonly title: string | null | undefined;
        };
        readonly price: any;
      }>;
    }>;
    readonly timezone: string | null | undefined;
  } | null | undefined;
  readonly productPricingCadences: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductPricingCadence;
  }>;
  readonly " $fragmentType": "marketplaceLocation_query";
};
export type marketplaceLocation_query$key = {
  readonly " $data"?: marketplaceLocation_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocation_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  (v0/*: any*/),
  (v1/*: any*/)
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "amenities",
  "plural": true,
  "selections": [
    (v3/*: any*/),
    (v1/*: any*/)
  ],
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "url",
  "storageKey": null
},
v9 = [
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
  (v7/*: any*/),
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
  "name": "marketplaceLocation_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ProductPricingCadenceDetails",
      "kind": "LinkedField",
      "name": "productPricingCadences",
      "plural": true,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currencies",
      "plural": true,
      "selections": (v2/*: any*/),
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
        (v3/*: any*/),
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "uniqueAlphanumericName",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "ListingMetadata",
          "kind": "LinkedField",
          "name": "listingMetadata",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "about",
              "storageKey": null
            },
            (v4/*: any*/),
            (v5/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "includedFeatures",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "timezone",
          "storageKey": null
        },
        (v6/*: any*/),
        {
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
                (v7/*: any*/),
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
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "CdnImageFile",
          "kind": "LinkedField",
          "name": "featureImages",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "CdnFile",
              "kind": "LinkedField",
              "name": "original",
              "plural": false,
              "selections": [
                (v8/*: any*/),
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
              "storageKey": null
            }
          ],
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
              "name": "longitude",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "latitude",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "multilinesFormattedAddress",
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
                  "selections": (v9/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "tuesday",
                  "plural": false,
                  "selections": (v9/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "wednesday",
                  "plural": false,
                  "selections": (v9/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "thursday",
                  "plural": false,
                  "selections": (v9/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "friday",
                  "plural": false,
                  "selections": (v9/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "saturday",
                  "plural": false,
                  "selections": (v9/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "sunday",
                  "plural": false,
                  "selections": (v9/*: any*/),
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
          "concreteType": "ProductDetails",
          "kind": "LinkedField",
          "name": "products",
          "plural": true,
          "selections": [
            (v3/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "ListingMetadata",
              "kind": "LinkedField",
              "name": "listingMetadata",
              "plural": false,
              "selections": [
                (v4/*: any*/),
                (v5/*: any*/)
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "CdnImageFile",
              "kind": "LinkedField",
              "name": "featureImages",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "CdnFile",
                  "kind": "LinkedField",
                  "name": "original",
                  "plural": false,
                  "selections": [
                    (v8/*: any*/)
                  ],
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "CurrencyDetails",
              "kind": "LinkedField",
              "name": "currency",
              "plural": false,
              "selections": [
                (v0/*: any*/)
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "ProductPricing",
              "kind": "LinkedField",
              "name": "pricingOptions",
              "plural": true,
              "selections": [
                (v3/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "index",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "ListingMetadata",
                  "kind": "LinkedField",
                  "name": "listingMetadata",
                  "plural": false,
                  "selections": [
                    (v4/*: any*/)
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "cadence",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "price",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "isTaxInclusive",
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            (v6/*: any*/)
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

(node as any).hash = "f127500bf6454d53bd424232c547d0be";

export default node;
