/**
 * @generated SignedSource<<1c721a79ac070721fece3ba87b2cf2cd>>
 * @lightSyntaxTransform
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
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
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
  about?: string | null | undefined;
  includedFeatures?: ReadonlyArray<string> | null | undefined;
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
export type createUnifiedHostLocationMutation$variables = {
  input: AddLocationInput;
};
export type createUnifiedHostLocationMutation$data = {
  readonly addLocation: {
    readonly location: {
      readonly id: string;
      readonly name: string;
    };
  };
};
export type createUnifiedHostLocationMutation = {
  response: createUnifiedHostLocationMutation$data;
  variables: createUnifiedHostLocationMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "createUnifiedHostLocationMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "createUnifiedHostLocationMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "d24c35b8ccd3d04d80709e847962fa03",
    "id": null,
    "metadata": {},
    "name": "createUnifiedHostLocationMutation",
    "operationKind": "mutation",
    "text": "mutation createUnifiedHostLocationMutation(\n  $input: AddLocationInput!\n) {\n  addLocation(input: $input) {\n    location {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ae37a2053a1c4f2380de1d5f1d4feed1";

export default node;
