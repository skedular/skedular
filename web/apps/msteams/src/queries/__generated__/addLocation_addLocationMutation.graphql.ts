/**
 * @generated SignedSource<<f913e61d70a520ecab61da526d1084ca>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type AddLocationInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  id?: string | null | undefined;
  name: string;
  organizationId?: string | null | undefined;
  timezone?: string | null | undefined;
};
export type addLocation_addLocationMutation$variables = {
  input: AddLocationInput;
};
export type addLocation_addLocationMutation$data = {
  readonly addLocation: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type addLocation_addLocationMutation$rawResponse = {
  readonly addLocation: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type addLocation_addLocationMutation = {
  rawResponse: addLocation_addLocationMutation$rawResponse;
  response: addLocation_addLocationMutation$data;
  variables: addLocation_addLocationMutation$variables;
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
            "name": "timezone",
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
    "name": "addLocation_addLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addLocation_addLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "50e1d4faeafd8c97babd72e4c8129bee",
    "id": null,
    "metadata": {},
    "name": "addLocation_addLocationMutation",
    "operationKind": "mutation",
    "text": "mutation addLocation_addLocationMutation(\n  $input: AddLocationInput!\n) {\n  addLocation(input: $input) {\n    location {\n      id\n      name\n      about\n      timezone\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "521f39a921d6d6827549d9aed581ca1e";

export default node;
