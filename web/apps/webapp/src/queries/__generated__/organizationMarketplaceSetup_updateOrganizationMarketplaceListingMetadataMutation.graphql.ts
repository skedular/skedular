/**
 * @generated SignedSource<<dc81ead5e63c481d93decd0d380dae65>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationMarketplaceListingMetadataInput = {
  clientMutationId?: string | null | undefined;
  id?: string | null | undefined;
  marketplaceListingMetadata: ListingMetadataInput;
  uniqueAlphanumericName?: string | null | undefined;
};
export type ListingMetadataInput = {
  about?: string | null | undefined;
  subTitle?: string | null | undefined;
  title?: string | null | undefined;
};
export type organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$variables = {
  input: UpdateOrganizationMarketplaceListingMetadataInput;
};
export type organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$data = {
  readonly updateOrganizationMarketplaceListingMetadata: {
    readonly organization: {
      readonly id: string;
      readonly marketplaceListingMetadata: {
        readonly about: string | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$rawResponse = {
  readonly updateOrganizationMarketplaceListingMetadata: {
    readonly organization: {
      readonly id: string;
      readonly marketplaceListingMetadata: {
        readonly about: string | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation = {
  rawResponse: organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$rawResponse;
  response: organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$data;
  variables: organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation$variables;
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
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "updateOrganizationMarketplaceListingMetadata",
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
            "concreteType": "ListingMetadata",
            "kind": "LinkedField",
            "name": "marketplaceListingMetadata",
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
    "name": "organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "49858ab73d9dccb8c2f35aac782d27d7",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation(\n  $input: UpdateOrganizationMarketplaceListingMetadataInput!\n) {\n  updateOrganizationMarketplaceListingMetadata(input: $input) {\n    organization {\n      id\n      marketplaceListingMetadata {\n        about\n        title\n        subTitle\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e16a8114747073e918fb888902575bf7";

export default node;
