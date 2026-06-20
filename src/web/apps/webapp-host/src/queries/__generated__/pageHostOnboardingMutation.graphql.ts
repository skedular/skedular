/**
 * @generated SignedSource<<3d5a2f491bcfb68ff737c9fff49b58b9>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type OrganizationType = "HOST" | "MARKETPLACE" | "PRIVATE" | "%future added value";
export type AddOrganizationInput = {
  agreedToTermsOfUse: boolean;
  billingCycle: OrganizationBillingCycle;
  clientMutationId?: string | null | undefined;
  contactEmail?: string | null | undefined;
  contactPhone?: string | null | undefined;
  customDomain?: string | null | undefined;
  customerFacingTermsAndConditionsUrl?: string | null | undefined;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  id?: string | null | undefined;
  industrySubCategoryIds: ReadonlyArray<string>;
  invoiceDueInDays: number;
  logoUrl?: string | null | undefined;
  marketplaceListingMetadata?: ListingMetadataInput | null | undefined;
  name: string;
  refundNotificationEmails: ReadonlyArray<string>;
  termsOfUseId: string;
  type: OrganizationType;
  website?: string | null | undefined;
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
export type pageHostOnboardingMutation$variables = {
  input: AddOrganizationInput;
};
export type pageHostOnboardingMutation$data = {
  readonly addOrganization: {
    readonly organization: {
      readonly customDomain: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  };
};
export type pageHostOnboardingMutation = {
  response: pageHostOnboardingMutation$data;
  variables: pageHostOnboardingMutation$variables;
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
    "name": "addOrganization",
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
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "customDomain",
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
    "name": "pageHostOnboardingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageHostOnboardingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "5cbe5e94e255850876f947b1ea8e9ffb",
    "id": null,
    "metadata": {},
    "name": "pageHostOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation pageHostOnboardingMutation(\n  $input: AddOrganizationInput!\n) {\n  addOrganization(input: $input) {\n    organization {\n      id\n      name\n      customDomain\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8234a3f96ac5900f303b96d8d505f6b9";

export default node;
