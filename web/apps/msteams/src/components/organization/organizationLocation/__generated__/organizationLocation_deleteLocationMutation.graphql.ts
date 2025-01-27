/**
 * @generated SignedSource<<7b2c8342269ccee43b0cd42a171fb240>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteLocationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationLocation_deleteLocationMutation$variables = {
  input: DeleteLocationInput;
};
export type organizationLocation_deleteLocationMutation$data = {
  readonly deleteLocation: {
    readonly location: {
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationLocation_deleteLocationMutation = {
  response: organizationLocation_deleteLocationMutation$data;
  variables: organizationLocation_deleteLocationMutation$variables;
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
    "name": "deleteLocation",
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
    "name": "organizationLocation_deleteLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_deleteLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "826969540e324742a2c0d77308b7658c",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_deleteLocationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_deleteLocationMutation(\n  $input: DeleteLocationInput!\n) {\n  deleteLocation(input: $input) {\n    location {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cb8b488df07f709d9e2ae3fd93c45b1a";

export default node;
