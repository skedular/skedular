/**
 * @generated SignedSource<<928a62b625f5eebcbf554c5db20cd862>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveOrganizationSsoSettingsInput = {
  clientMutationId?: string | null | undefined;
  organizationId: string;
};
export type organizationAdmin_removeOrganizationSsoSettingsMutation$variables = {
  input: RemoveOrganizationSsoSettingsInput;
};
export type organizationAdmin_removeOrganizationSsoSettingsMutation$data = {
  readonly removeOrganizationSsoSettings: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type organizationAdmin_removeOrganizationSsoSettingsMutation = {
  response: organizationAdmin_removeOrganizationSsoSettingsMutation$data;
  variables: organizationAdmin_removeOrganizationSsoSettingsMutation$variables;
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
    "concreteType": "UpdateOrganizationSsoSettingsPayload",
    "kind": "LinkedField",
    "name": "removeOrganizationSsoSettings",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "clientMutationId",
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
    "name": "organizationAdmin_removeOrganizationSsoSettingsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_removeOrganizationSsoSettingsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "bbac3b05e4642bd3a570aee5a107e076",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_removeOrganizationSsoSettingsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_removeOrganizationSsoSettingsMutation(\n  $input: RemoveOrganizationSsoSettingsInput!\n) {\n  removeOrganizationSsoSettings(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "268c6730ed213420180541b91fba57fa";

export default node;
