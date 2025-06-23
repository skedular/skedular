/**
 * @generated SignedSource<<9dd950eb3f72a98ca549d425803e588e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteFloorPlanInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type floorPlanCard_deleteFloorPlanMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteFloorPlanInput;
};
export type floorPlanCard_deleteFloorPlanMutation$data = {
  readonly deleteFloorPlan: {
    readonly floorPlan: {
      readonly id: string;
    };
  };
};
export type floorPlanCard_deleteFloorPlanMutation = {
  response: floorPlanCard_deleteFloorPlanMutation$data;
  variables: floorPlanCard_deleteFloorPlanMutation$variables;
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
    "name": "floorPlanCard_deleteFloorPlanMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "FloorPlanPayload",
        "kind": "LinkedField",
        "name": "deleteFloorPlan",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "FloorPlanDetails",
            "kind": "LinkedField",
            "name": "floorPlan",
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
    "name": "floorPlanCard_deleteFloorPlanMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "FloorPlanPayload",
        "kind": "LinkedField",
        "name": "deleteFloorPlan",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "FloorPlanDetails",
            "kind": "LinkedField",
            "name": "floorPlan",
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
    "cacheID": "d63200171de28d2a67825278365c0971",
    "id": null,
    "metadata": {},
    "name": "floorPlanCard_deleteFloorPlanMutation",
    "operationKind": "mutation",
    "text": "mutation floorPlanCard_deleteFloorPlanMutation(\n  $input: DeleteFloorPlanInput!\n) {\n  deleteFloorPlan(input: $input) {\n    floorPlan {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "11bd4cddc92847877d1f366dc6f647e8";

export default node;
