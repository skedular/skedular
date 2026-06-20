/**
 * @generated SignedSource<<d7f405bbde70ab58904127795887e82b>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type MarketplaceBookingPatchField = "CATEGORY" | "NOTES" | "PARTICIPANTS" | "%future added value";
export type UpdateMarketplaceBookingInput = {
  category?: BookingCategory | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  fieldsToUpdate: ReadonlyArray<MarketplaceBookingPatchField>;
  id: string;
  notes?: string | null | undefined;
  organizationCustomDomains?: ReadonlyArray<string> | null | undefined;
  organizationIds?: ReadonlyArray<string> | null | undefined;
  teamIds?: ReadonlyArray<string> | null | undefined;
};
export type editMarketplaceBooking_updateBookingMutation$variables = {
  input: UpdateMarketplaceBookingInput;
};
export type editMarketplaceBooking_updateBookingMutation$data = {
  readonly updateMarketplaceBooking: {
    readonly booking: {
      readonly bookingResources: ReadonlyArray<{
        readonly resource: {
          readonly color: string | null | undefined;
          readonly customTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly id: string;
          readonly name: string;
          readonly zones: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
        };
      }>;
      readonly category: {
        readonly category: BookingCategory;
        readonly name: string;
      };
      readonly from: any;
      readonly id: string;
      readonly involvedCustomers: ReadonlyArray<{
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly id: string;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      }>;
      readonly involvedLocations: ReadonlyArray<{
        readonly name: string;
      }>;
      readonly involvedOrganizations: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly involvedTeams: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly notes: string | null | undefined;
      readonly until: any;
    } | null | undefined;
  };
};
export type editMarketplaceBooking_updateBookingMutation$rawResponse = {
  readonly updateMarketplaceBooking: {
    readonly booking: {
      readonly bookingResources: ReadonlyArray<{
        readonly resource: {
          readonly color: string | null | undefined;
          readonly customTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly id: string;
          readonly name: string;
          readonly zones: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
        };
      }>;
      readonly category: {
        readonly category: BookingCategory;
        readonly name: string;
      };
      readonly from: any;
      readonly id: string;
      readonly involvedCustomers: ReadonlyArray<{
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly id: string;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      }>;
      readonly involvedLocations: ReadonlyArray<{
        readonly name: string;
      }>;
      readonly involvedOrganizations: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly involvedTeams: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly notes: string | null | undefined;
      readonly until: any;
    } | null | undefined;
  };
};
export type editMarketplaceBooking_updateBookingMutation = {
  rawResponse: editMarketplaceBooking_updateBookingMutation$rawResponse;
  response: editMarketplaceBooking_updateBookingMutation$data;
  variables: editMarketplaceBooking_updateBookingMutation$variables;
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
  (v1/*:: as any*/),
  (v2/*:: as any*/)
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v5 = [
  (v1/*:: as any*/),
  (v2/*:: as any*/),
  (v4/*:: as any*/)
],
v6 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "BookingPayload",
    "kind": "LinkedField",
    "name": "updateMarketplaceBooking",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
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
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "notes",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCategoryDetails",
            "kind": "LinkedField",
            "name": "category",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "category",
                "storageKey": null
              },
              (v2/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDetails",
            "kind": "LinkedField",
            "name": "involvedCustomers",
            "plural": true,
            "selections": [
              (v1/*:: as any*/),
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "givenName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "middleName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "familyName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "photoUrl",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationDetails",
            "kind": "LinkedField",
            "name": "involvedOrganizations",
            "plural": true,
            "selections": (v3/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "involvedLocations",
            "plural": true,
            "selections": [
              (v2/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamDetails",
            "kind": "LinkedField",
            "name": "involvedTeams",
            "plural": true,
            "selections": (v3/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingResourceDetails",
            "kind": "LinkedField",
            "name": "bookingResources",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "ResourceDetails",
                "kind": "LinkedField",
                "name": "resource",
                "plural": false,
                "selections": [
                  (v1/*:: as any*/),
                  (v2/*:: as any*/),
                  (v4/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "customTags",
                    "plural": true,
                    "selections": (v5/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "zones",
                    "plural": true,
                    "selections": (v5/*:: as any*/),
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editMarketplaceBooking_updateBookingMutation",
    "selections": (v6/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editMarketplaceBooking_updateBookingMutation",
    "selections": (v6/*:: as any*/)
  },
  "params": {
    "cacheID": "5c669824c7a0d28401e7dc4f1cd8eeae",
    "id": null,
    "metadata": {},
    "name": "editMarketplaceBooking_updateBookingMutation",
    "operationKind": "mutation",
    "text": "mutation editMarketplaceBooking_updateBookingMutation(\n  $input: UpdateMarketplaceBookingInput!\n) {\n  updateMarketplaceBooking(input: $input) {\n    booking {\n      id\n      from\n      until\n      notes\n      category {\n        category\n        name\n      }\n      involvedCustomers {\n        id\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n      involvedOrganizations {\n        id\n        name\n      }\n      involvedLocations {\n        name\n      }\n      involvedTeams {\n        id\n        name\n      }\n      bookingResources {\n        resource {\n          id\n          name\n          color\n          customTags {\n            id\n            name\n            color\n          }\n          zones {\n            id\n            name\n            color\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ee59bdd058d6b34cd85fd5c00af56edd";

export default node;
