/**
 * @generated SignedSource<<316bae83aea79a124192df8e34fb7d5f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateLocationInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  id: string;
  name: string;
  organizationId?: string | null | undefined;
  timezone?: string | null | undefined;
};
export type locationAboutTab_updateLocationMutation$variables = {
  input: UpdateLocationInput;
};
export type locationAboutTab_updateLocationMutation$data = {
  readonly updateLocation: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type locationAboutTab_updateLocationMutation$rawResponse = {
  readonly updateLocation: {
    readonly location: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type locationAboutTab_updateLocationMutation = {
  rawResponse: locationAboutTab_updateLocationMutation$rawResponse;
  response: locationAboutTab_updateLocationMutation$data;
  variables: locationAboutTab_updateLocationMutation$variables;
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
    "name": "updateLocation",
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
    "name": "locationAboutTab_updateLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationAboutTab_updateLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "8cf146f1dd967bdf984fe6cf36636433",
    "id": null,
    "metadata": {},
    "name": "locationAboutTab_updateLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationAboutTab_updateLocationMutation(\n  $input: UpdateLocationInput!\n) {\n  updateLocation(input: $input) {\n    location {\n      id\n      name\n      about\n      timezone\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4c264c8678adafc9e67d1a010e3a69ff";

export default node;
