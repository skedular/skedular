/**
 * @generated SignedSource<<7b8922ab2c922a92687331112ef5af15>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddOrganizationBillingDetailsInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city: string;
  clientMutationId?: string | null | undefined;
  companyName?: string | null | undefined;
  country: string;
  email: string;
  formattedAddress?: string | null | undefined;
  id?: string | null | undefined;
  latitude?: number | null | undefined;
  longitude?: number | null | undefined;
  organizationId: string;
  osmId?: string | null | undefined;
  osmType?: string | null | undefined;
  placeId?: string | null | undefined;
  province?: string | null | undefined;
  suburb: string;
  zipcode: string;
};
export type organizationAdmin_addOrganizationBillingDetailsMutation$variables = {
  input: AddOrganizationBillingDetailsInput;
};
export type organizationAdmin_addOrganizationBillingDetailsMutation$data = {
  readonly addOrganizationBillingDetails: {
    readonly organization: {
      readonly billingDetails: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly companyName: string | null | undefined;
        readonly country: string;
        readonly email: string;
        readonly formattedAddress: string | null | undefined;
        readonly id: string;
        readonly latitude: number | null | undefined;
        readonly longitude: number | null | undefined;
        readonly osmId: string | null | undefined;
        readonly osmType: string | null | undefined;
        readonly placeId: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      } | null | undefined;
      readonly id: string;
    };
  };
};
export type organizationAdmin_addOrganizationBillingDetailsMutation$rawResponse = {
  readonly addOrganizationBillingDetails: {
    readonly organization: {
      readonly billingDetails: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly companyName: string | null | undefined;
        readonly country: string;
        readonly email: string;
        readonly formattedAddress: string | null | undefined;
        readonly id: string;
        readonly latitude: number | null | undefined;
        readonly longitude: number | null | undefined;
        readonly osmId: string | null | undefined;
        readonly osmType: string | null | undefined;
        readonly placeId: string | null | undefined;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      } | null | undefined;
      readonly id: string;
    };
  };
};
export type organizationAdmin_addOrganizationBillingDetailsMutation = {
  rawResponse: organizationAdmin_addOrganizationBillingDetailsMutation$rawResponse;
  response: organizationAdmin_addOrganizationBillingDetailsMutation$data;
  variables: organizationAdmin_addOrganizationBillingDetailsMutation$variables;
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
    "name": "addOrganizationBillingDetails",
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
    "name": "organizationAdmin_addOrganizationBillingDetailsMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_addOrganizationBillingDetailsMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "d0aeb352f5e829c84b976902d606c95f",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_addOrganizationBillingDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_addOrganizationBillingDetailsMutation(\n  $input: AddOrganizationBillingDetailsInput!\n) {\n  addOrganizationBillingDetails(input: $input) {\n    organization {\n      id\n      billingDetails {\n        id\n        companyName\n        email\n        osmType\n        osmId\n        placeId\n        longitude\n        latitude\n        formattedAddress\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3478718d3138b765eef40ae4c03afe15";

export default node;
