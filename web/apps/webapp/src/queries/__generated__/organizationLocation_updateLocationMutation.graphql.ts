/**
 * @generated SignedSource<<532cbc27b8f0a79da0476e7f305db2dc>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type LocationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type UpdateLocationInput = {
  clientMutationId?: string | null | undefined;
  extraMetadata?: LocationExtraMetadataInput | null | undefined;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  id: string;
  listingMetadata?: ListingMetadataInput | null | undefined;
  name: string;
  tagIds: ReadonlyArray<string>;
  timezone?: string | null | undefined;
  type: LocationType;
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
  about?: string | null | undefined;
  includedFeatures?: ReadonlyArray<string> | null | undefined;
  subTitle?: string | null | undefined;
  title?: string | null | undefined;
};
export type organizationLocation_updateLocationMutation$variables = {
  input: UpdateLocationInput;
};
export type organizationLocation_updateLocationMutation$data = {
  readonly updateLocation: {
    readonly location: {
      readonly amenities: ReadonlyArray<{
        readonly color: string | null | undefined;
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
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly name: string;
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
export type organizationLocation_updateLocationMutation$rawResponse = {
  readonly updateLocation: {
    readonly location: {
      readonly amenities: ReadonlyArray<{
        readonly color: string | null | undefined;
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
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly name: string;
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
v5 = [
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
                "name": "title",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "subTitle",
                "storageKey": null
              },
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
            "selections": (v4/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "amenities",
            "plural": true,
            "selections": (v4/*: any*/),
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
    "selections": (v5/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_updateLocationMutation",
    "selections": (v5/*: any*/)
  },
  "params": {
    "cacheID": "ceee86b08804b87446188bb90912d79d",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_updateLocationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_updateLocationMutation(\n  $input: UpdateLocationInput!\n) {\n  updateLocation(input: $input) {\n    location {\n      id\n      name\n      listingMetadata {\n        title\n        subTitle\n        includedFeatures\n      }\n      timezone\n      type {\n        type\n        name\n      }\n      extraMetadata {\n        contactDetails {\n          contactPeople\n          contactEmails\n          contactPhones\n        }\n        areaRange {\n          fromInSqm\n          toInSqm\n        }\n        peopleCapacity {\n          from\n          to\n        }\n        website\n        relatedImageLinks\n        relatedVideoLinks\n        otherLinks\n      }\n      featureImages {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n      spaceTypes {\n        id\n        name\n        color\n      }\n      amenities {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9306ac146999dfcf70d079c83806180c";

export default node;
