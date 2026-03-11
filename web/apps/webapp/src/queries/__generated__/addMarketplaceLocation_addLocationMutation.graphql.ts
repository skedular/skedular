/**
 * @generated SignedSource<<ff7c07e1c25a8f20e24dce0e9dfae9b9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type LocationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type AddLocationInput = {
  clientMutationId?: string | null | undefined;
  extraMetadata?: LocationExtraMetadataInput | null | undefined;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  id?: string | null | undefined;
  listingMetadata?: ListingMetadataInput | null | undefined;
  name: string;
  organizationId?: string | null | undefined;
  organizationUniqueAlphanumericName?: string | null | undefined;
  physicalAddress?: LocationPhysicalAddressInput | null | undefined;
  tagIds: ReadonlyArray<string>;
  timezone?: string | null | undefined;
  type: LocationType;
  weekOpeningHours?: WeekOpeningHoursInput | null | undefined;
};
export type LocationExtraMetadataInput = {
  areaRange?: AreaRangeInput | null | undefined;
  contactDetails?: ContactDetailsInput | null | undefined;
  otherLinks?: ReadonlyArray<string> | null | undefined;
  peopleCapacity?: PeopleCapacityInput | null | undefined;
  relatedImageLinks?: ReadonlyArray<string> | null | undefined;
  relatedVideoLinks?: ReadonlyArray<string> | null | undefined;
  website?: string | null | undefined;
};
export type AreaRangeInput = {
  fromInSqm: string;
  toInSqm: string;
};
export type ContactDetailsInput = {
  contactEmails?: ReadonlyArray<string> | null | undefined;
  contactPeople?: ReadonlyArray<string> | null | undefined;
  contactPhones?: ReadonlyArray<string> | null | undefined;
};
export type PeopleCapacityInput = {
  from: string;
  to: string;
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
export type ListingMetadataInput = {
  about: string;
  subTitle?: string | null | undefined;
  title?: string | null | undefined;
};
export type LocationPhysicalAddressInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city?: string | null | undefined;
  country: string;
  countryCode?: string | null | undefined;
  formattedAddress?: string | null | undefined;
  latitude?: number | null | undefined;
  longitude?: number | null | undefined;
  osmId?: string | null | undefined;
  osmType?: string | null | undefined;
  placeId?: string | null | undefined;
  province?: string | null | undefined;
  suburb?: string | null | undefined;
  zipcode: string;
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
export type addMarketplaceLocation_addLocationMutation$variables = {
  input: AddLocationInput;
};
export type addMarketplaceLocation_addLocationMutation$data = {
  readonly addLocation: {
    readonly location: {
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
      readonly featureImages: ReadonlyArray<{
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
      }>;
      readonly id: string;
      readonly listingMetadata: {
        readonly about: string;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly name: string;
      readonly physicalAddress: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string | null | undefined;
        readonly country: string;
        readonly countryCode: string | null | undefined;
        readonly formattedAddress: string | null | undefined;
        readonly id: string;
        readonly latitude: number | null | undefined;
        readonly longitude: number | null | undefined;
        readonly osmId: string | null | undefined;
        readonly osmType: string | null | undefined;
        readonly placeId: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string | null | undefined;
        readonly zipcode: string;
      } | null | undefined;
      readonly spaceTypes: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly timezone: string | null | undefined;
      readonly type: {
        readonly name: string;
        readonly type: LocationType;
      };
    };
  };
};
export type addMarketplaceLocation_addLocationMutation$rawResponse = {
  readonly addLocation: {
    readonly location: {
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
      readonly featureImages: ReadonlyArray<{
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
      }>;
      readonly id: string;
      readonly listingMetadata: {
        readonly about: string;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly name: string;
      readonly physicalAddress: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string | null | undefined;
        readonly country: string;
        readonly countryCode: string | null | undefined;
        readonly formattedAddress: string | null | undefined;
        readonly id: string;
        readonly latitude: number | null | undefined;
        readonly longitude: number | null | undefined;
        readonly osmId: string | null | undefined;
        readonly osmType: string | null | undefined;
        readonly placeId: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string | null | undefined;
        readonly zipcode: string;
      } | null | undefined;
      readonly spaceTypes: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly timezone: string | null | undefined;
      readonly type: {
        readonly name: string;
        readonly type: LocationType;
      };
    };
  };
};
export type addMarketplaceLocation_addLocationMutation = {
  rawResponse: addMarketplaceLocation_addLocationMutation$rawResponse;
  response: addMarketplaceLocation_addLocationMutation$data;
  variables: addMarketplaceLocation_addLocationMutation$variables;
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
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
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
    "name": "addLocation",
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
          (v1/*: any*/),
          (v2/*: any*/),
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
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "title",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "subTitle",
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
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationTypeDetails",
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
              },
              (v2/*: any*/)
            ],
            "storageKey": null
          },
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
                "selections": (v3/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v3/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "spaceTypes",
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
            "concreteType": "LocationPhysicalAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "osmType",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "osmId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "placeId",
                "storageKey": null
              },
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
                "name": "formattedAddress",
                "storageKey": null
              },
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
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "countryCode",
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
    "name": "addMarketplaceLocation_addLocationMutation",
    "selections": (v4/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addMarketplaceLocation_addLocationMutation",
    "selections": (v4/*: any*/)
  },
  "params": {
    "cacheID": "84c09644f61727be5628e2491f6142dc",
    "id": null,
    "metadata": {},
    "name": "addMarketplaceLocation_addLocationMutation",
    "operationKind": "mutation",
    "text": "mutation addMarketplaceLocation_addLocationMutation(\n  $input: AddLocationInput!\n) {\n  addLocation(input: $input) {\n    location {\n      id\n      name\n      listingMetadata {\n        about\n        title\n        subTitle\n      }\n      timezone\n      type {\n        type\n        name\n      }\n      extraMetadata {\n        contactDetails {\n          contactPeople\n          contactEmails\n          contactPhones\n        }\n        areaRange {\n          fromInSqm\n          toInSqm\n        }\n        peopleCapacity {\n          from\n          to\n        }\n        website\n        relatedImageLinks\n        relatedVideoLinks\n        otherLinks\n      }\n      featureImages {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n      spaceTypes {\n        id\n        name\n        color\n      }\n      physicalAddress {\n        id\n        osmType\n        osmId\n        placeId\n        longitude\n        latitude\n        formattedAddress\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n        countryCode\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6ce0b61416967072f9a51c3e7099de2c";

export default node;
