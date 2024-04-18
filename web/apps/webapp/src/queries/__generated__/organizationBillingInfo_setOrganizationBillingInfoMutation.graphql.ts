/**
 * @generated SignedSource<<06b5b424d95853aeb2bb065dca8d48a2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type SetOrganizationBillingInfoInput = {
  addressLine1?: string | null | undefined;
  addressLine2?: string | null | undefined;
  city?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  country?: string | null | undefined;
  email?: string | null | undefined;
  organizationId: string;
  province?: string | null | undefined;
  suburb?: string | null | undefined;
  zipcode?: string | null | undefined;
};
export type organizationBillingInfo_setOrganizationBillingInfoMutation$variables = {
  input: SetOrganizationBillingInfoInput;
};
export type organizationBillingInfo_setOrganizationBillingInfoMutation$data = {
  readonly setOrganizationBillingInfo: {
    readonly organizationBillingInfo: {
      readonly addressLine1: string | null | undefined;
      readonly addressLine2: string | null | undefined;
      readonly city: string | null | undefined;
      readonly country: string | null | undefined;
      readonly email: string | null | undefined;
      readonly organizationId: string;
      readonly province: string | null | undefined;
      readonly suburb: string | null | undefined;
      readonly zipcode: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationBillingInfo_setOrganizationBillingInfoMutation$rawResponse = {
  readonly setOrganizationBillingInfo: {
    readonly organizationBillingInfo: {
      readonly addressLine1: string | null | undefined;
      readonly addressLine2: string | null | undefined;
      readonly city: string | null | undefined;
      readonly country: string | null | undefined;
      readonly email: string | null | undefined;
      readonly organizationId: string;
      readonly province: string | null | undefined;
      readonly suburb: string | null | undefined;
      readonly zipcode: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationBillingInfo_setOrganizationBillingInfoMutation = {
  rawResponse: organizationBillingInfo_setOrganizationBillingInfoMutation$rawResponse;
  response: organizationBillingInfo_setOrganizationBillingInfoMutation$data;
  variables: organizationBillingInfo_setOrganizationBillingInfoMutation$variables;
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
    "concreteType": "OrganizationBillingInfoPayload",
    "kind": "LinkedField",
    "name": "setOrganizationBillingInfo",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationBillingInfo",
        "kind": "LinkedField",
        "name": "organizationBillingInfo",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "organizationId",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationBillingInfo_setOrganizationBillingInfoMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationBillingInfo_setOrganizationBillingInfoMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "0f48f3d790fe9d206f632e89a153bbb9",
    "id": null,
    "metadata": {},
    "name": "organizationBillingInfo_setOrganizationBillingInfoMutation",
    "operationKind": "mutation",
    "text": "mutation organizationBillingInfo_setOrganizationBillingInfoMutation(\n  $input: SetOrganizationBillingInfoInput!\n) {\n  setOrganizationBillingInfo(input: $input) {\n    organizationBillingInfo {\n      organizationId\n      email\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a8dae293cf846deaef2add4117c30f94";

export default node;
