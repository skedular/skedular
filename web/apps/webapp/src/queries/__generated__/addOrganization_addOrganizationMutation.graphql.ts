/**
 * @generated SignedSource<<60746c8363e04979f3e13d372ca525c2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddOrganizationInput = {
  about?: string | null | undefined;
  agreedToTermsOfUse: boolean;
  clientMutationId?: string | null | undefined;
  id?: string | null | undefined;
  industrySubCategoryIds: ReadonlyArray<string>;
  name: string;
  termsOfUseId: string;
  website?: string | null | undefined;
};
export type addOrganization_addOrganizationMutation$variables = {
  input: AddOrganizationInput;
};
export type addOrganization_addOrganizationMutation$data = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly website: string | null | undefined;
    };
  } | null | undefined;
};
export type addOrganization_addOrganizationMutation$rawResponse = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly website: string | null | undefined;
    };
  } | null | undefined;
};
export type addOrganization_addOrganizationMutation = {
  rawResponse: addOrganization_addOrganizationMutation$rawResponse;
  response: addOrganization_addOrganizationMutation$data;
  variables: addOrganization_addOrganizationMutation$variables;
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
            "name": "about",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "website",
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
    "name": "addOrganization_addOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addOrganization_addOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "1c9dc7e2e2b04bb460b4e2d2b2b47e77",
    "id": null,
    "metadata": {},
    "name": "addOrganization_addOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation addOrganization_addOrganizationMutation(\n  $input: AddOrganizationInput!\n) {\n  addOrganization(input: $input) {\n    organization {\n      id\n      name\n      about\n      website\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5a6413bdfba5da80b030744650f6fc21";

export default node;
