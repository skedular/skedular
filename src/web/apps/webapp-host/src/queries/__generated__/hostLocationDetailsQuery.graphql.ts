/**
 * @generated SignedSource<<2522428931a430315a090bcc479ed56d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ProductPricingCadence = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "HALF_DAY" | "MONTHLY" | "NOT_SET" | "ONE_TIME" | "PER15_MINUTES" | "PER30_MINUTES" | "PER_HOUR" | "PER_MINUTE" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
export type hostLocationDetailsQuery$variables = {
  locationId: string;
};
export type hostLocationDetailsQuery$data = {
  readonly location: {
    readonly extraMetadata: {
      readonly contactDetails: {
        readonly contactEmails: ReadonlyArray<string> | null | undefined;
        readonly contactPhones: ReadonlyArray<string> | null | undefined;
      } | null | undefined;
      readonly peopleCapacity: {
        readonly from: string;
        readonly to: string;
      } | null | undefined;
    } | null | undefined;
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly customDomain: string | null | undefined;
    };
    readonly physicalAddress: {
      readonly latitude: number | null | undefined;
      readonly longitude: number | null | undefined;
      readonly multilinesFormattedAddress: string | null | undefined;
    } | null | undefined;
    readonly products: ReadonlyArray<{
      readonly currency: {
        readonly name: string;
      };
      readonly id: string;
      readonly inactive: boolean;
      readonly listingMetadata: {
        readonly title: string | null | undefined;
      };
      readonly pricingOptions: ReadonlyArray<{
        readonly bookingCadence: ProductPricingCadence;
        readonly price: any;
      }>;
    }>;
    readonly timezone: string | null | undefined;
    readonly type: {
      readonly name: string;
    };
  } | null | undefined;
};
export type hostLocationDetailsQuery = {
  response: hostLocationDetailsQuery$data;
  variables: hostLocationDetailsQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "locationId"
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
  "name": "timezone",
  "storageKey": null
},
v5 = [
  (v3/*:: as any*/)
],
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "LocationTypeDetails",
  "kind": "LinkedField",
  "name": "type",
  "plural": false,
  "selections": (v5/*:: as any*/),
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "customDomain",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "multilinesFormattedAddress",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "latitude",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "longitude",
  "storageKey": null
},
v11 = {
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
      "concreteType": "PeopleCapacity",
      "kind": "LinkedField",
      "name": "peopleCapacity",
      "plural": false,
      "selections": [
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
          "name": "to",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
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
    }
  ],
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "concreteType": "ProductDetails",
  "kind": "LinkedField",
  "name": "products",
  "plural": true,
  "selections": [
    (v2/*:: as any*/),
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
      "concreteType": "ListingMetadata",
      "kind": "LinkedField",
      "name": "listingMetadata",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "title",
          "storageKey": null
        }
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
          "name": "bookingCadence",
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
      "selections": (v5/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "hostLocationDetailsQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v6/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              (v7/*:: as any*/)
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
              (v8/*:: as any*/),
              (v9/*:: as any*/),
              (v10/*:: as any*/)
            ],
            "storageKey": null
          },
          (v11/*:: as any*/),
          (v12/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "hostLocationDetailsQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v6/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              (v7/*:: as any*/),
              (v2/*:: as any*/)
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
              (v8/*:: as any*/),
              (v9/*:: as any*/),
              (v10/*:: as any*/),
              (v2/*:: as any*/)
            ],
            "storageKey": null
          },
          (v11/*:: as any*/),
          (v12/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "0e9080c1c41a1d0500f9a6a2bde97eac",
    "id": null,
    "metadata": {},
    "name": "hostLocationDetailsQuery",
    "operationKind": "query",
    "text": "query hostLocationDetailsQuery(\n  $locationId: String!\n) {\n  location(id: $locationId) {\n    id\n    name\n    timezone\n    type {\n      name\n    }\n    organization {\n      customDomain\n      id\n    }\n    physicalAddress {\n      multilinesFormattedAddress\n      latitude\n      longitude\n      id\n    }\n    extraMetadata {\n      peopleCapacity {\n        from\n        to\n      }\n      contactDetails {\n        contactEmails\n        contactPhones\n      }\n    }\n    products {\n      id\n      inactive\n      listingMetadata {\n        title\n      }\n      pricingOptions {\n        price\n        bookingCadence\n      }\n      currency {\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "469c6a132845fcd4eb234851e1fca7da";

export default node;
