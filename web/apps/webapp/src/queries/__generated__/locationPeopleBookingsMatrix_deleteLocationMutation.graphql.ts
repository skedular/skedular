/**
 * @generated SignedSource<<ffaba30e781a29feb5558f4886a9381f>>
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
export type locationPeopleBookingsMatrix_deleteLocationMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteLocationInput;
};
export type locationPeopleBookingsMatrix_deleteLocationMutation$data = {
  readonly deleteLocation: {
    readonly location: {
      readonly id: string;
    };
  } | null | undefined;
};
export type locationPeopleBookingsMatrix_deleteLocationMutation = {
  response: locationPeopleBookingsMatrix_deleteLocationMutation$data;
  variables: locationPeopleBookingsMatrix_deleteLocationMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "locationPeopleBookingsMatrix_deleteLocationMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationPeopleBookingsMatrix_deleteLocationMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "50b4727b7bf2324a2e2f12e02a4e3aba",
    "id": null,
    "metadata": {},
    "name": "locationPeopleBookingsMatrix_deleteLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationPeopleBookingsMatrix_deleteLocationMutation(\n  $input: DeleteLocationInput!\n) {\n  deleteLocation(input: $input) {\n    location {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d1df0e1ef211f5ed1e96de655f40a279";

export default node;
