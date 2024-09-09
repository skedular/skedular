/**
 * @generated SignedSource<<bc9e154cf24dc96f3f112bfeffa635c5>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  id: string;
  industrySubCategoryIds: ReadonlyArray<string>;
  name: string;
  website?: string | null | undefined;
};
export type organizationAboutTab_updateOrganizationMutation$variables = {
  input: UpdateOrganizationInput;
};
export type organizationAboutTab_updateOrganizationMutation$data = {
  readonly updateOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly industrySubCategories: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly name: string;
      readonly website: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationAboutTab_updateOrganizationMutation$rawResponse = {
  readonly updateOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly industrySubCategories: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly name: string;
      readonly website: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationAboutTab_updateOrganizationMutation = {
  rawResponse: organizationAboutTab_updateOrganizationMutation$rawResponse;
  response: organizationAboutTab_updateOrganizationMutation$data;
  variables: organizationAboutTab_updateOrganizationMutation$variables;
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
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "updateOrganization",
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
          (v2/*: any*/),
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
            "name": "website",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
            "kind": "LinkedField",
            "name": "industrySubCategories",
            "plural": true,
            "selections": [
              (v1/*: any*/),
              (v2/*: any*/)
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
    "name": "organizationAboutTab_updateOrganizationMutation",
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAboutTab_updateOrganizationMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "2ebe6bee106ace0fd39ac4c28da977d3",
    "id": null,
    "metadata": {},
    "name": "organizationAboutTab_updateOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAboutTab_updateOrganizationMutation(\n  $input: UpdateOrganizationInput!\n) {\n  updateOrganization(input: $input) {\n    organization {\n      id\n      name\n      about\n      website\n      industrySubCategories {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2913ef3dcbe2cc7f2ef88e9ca635bb27";

export default node;
