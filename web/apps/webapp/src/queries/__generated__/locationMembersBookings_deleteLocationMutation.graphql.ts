/**
 * @generated SignedSource<<77a53fe0afc1f1d85d372cbd80f81bdc>>
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
export type locationMembersBookings_deleteLocationMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteLocationInput;
};
export type locationMembersBookings_deleteLocationMutation$data = {
  readonly deleteLocation: {
    readonly location: {
      readonly id: string;
    };
  } | null | undefined;
};
export type locationMembersBookings_deleteLocationMutation = {
  response: locationMembersBookings_deleteLocationMutation$data;
  variables: locationMembersBookings_deleteLocationMutation$variables;
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
    "name": "locationMembersBookings_deleteLocationMutation",
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
    "name": "locationMembersBookings_deleteLocationMutation",
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
    "cacheID": "461d91bfef99f98aa46f9a6e90cfc81e",
    "id": null,
    "metadata": {},
    "name": "locationMembersBookings_deleteLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationMembersBookings_deleteLocationMutation(\n  $input: DeleteLocationInput!\n) {\n  deleteLocation(input: $input) {\n    location {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "dccc727d12570d94ff3102b31b34fbff";

export default node;
