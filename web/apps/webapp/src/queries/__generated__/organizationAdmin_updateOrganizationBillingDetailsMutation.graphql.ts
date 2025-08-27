/**
 * @generated SignedSource<<9c8df7a8a34785a7d8ed97401de9194c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationBillingDetailsInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  companyName?: string | null | undefined;
  country: string;
  countryCode?: string | null | undefined;
  email: string;
  formattedAddress?: string | null | undefined;
  id: string;
  latitude?: number | null | undefined;
  longitude?: number | null | undefined;
  osmId?: string | null | undefined;
  osmType?: string | null | undefined;
  placeId?: string | null | undefined;
  province?: string | null | undefined;
  suburb?: string | null | undefined;
  zipcode: string;
};
export type organizationAdmin_updateOrganizationBillingDetailsMutation$variables = {
  input: UpdateOrganizationBillingDetailsInput;
};
export type organizationAdmin_updateOrganizationBillingDetailsMutation$data = {
  readonly updateOrganizationBillingDetails: {
    readonly organization: {
      readonly billingDetails: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string | null | undefined;
        readonly companyName: string | null | undefined;
        readonly country: string;
        readonly countryCode: string | null | undefined;
        readonly email: string;
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
      readonly id: string;
    };
  };
};
export type organizationAdmin_updateOrganizationBillingDetailsMutation$rawResponse = {
  readonly updateOrganizationBillingDetails: {
    readonly organization: {
      readonly billingDetails: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string | null | undefined;
        readonly companyName: string | null | undefined;
        readonly country: string;
        readonly countryCode: string | null | undefined;
        readonly email: string;
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
      readonly id: string;
    };
  };
};
export type organizationAdmin_updateOrganizationBillingDetailsMutation = {
  rawResponse: organizationAdmin_updateOrganizationBillingDetailsMutation$rawResponse;
  response: organizationAdmin_updateOrganizationBillingDetailsMutation$data;
  variables: organizationAdmin_updateOrganizationBillingDetailsMutation$variables;
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
v2 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "updateOrganizationBillingDetails",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationBillingDetails",
            "kind": "LinkedField",
            "name": "billingDetails",
            "plural": false,
            "selections": [
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "companyName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
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
    "name": "organizationAdmin_updateOrganizationBillingDetailsMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_updateOrganizationBillingDetailsMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "dc1654b3c8c3ecd791d493e2ca288d1e",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_updateOrganizationBillingDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_updateOrganizationBillingDetailsMutation(\n  $input: UpdateOrganizationBillingDetailsInput!\n) {\n  updateOrganizationBillingDetails(input: $input) {\n    organization {\n      id\n      billingDetails {\n        id\n        companyName\n        email\n        osmType\n        osmId\n        placeId\n        longitude\n        latitude\n        formattedAddress\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n        countryCode\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c6d673e9ad6aeb11a25c5f17eb3685f7";

export default node;
