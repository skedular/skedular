/**
 * @generated SignedSource<<e2615f8e7569add2aca3328e824a16e7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type locationAboutTab_rootQuery$variables = {
  locationId: string;
};
export type locationAboutTab_rootQuery$data = {
  readonly location: {
    readonly about: string | null | undefined;
    readonly canModify: boolean;
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly name: string;
    } | null | undefined;
    readonly physicalAddress: {
      readonly formattedAddress: string | null | undefined;
    } | null | undefined;
    readonly timezone: string | null | undefined;
  } | null | undefined;
};
export type locationAboutTab_rootQuery = {
  response: locationAboutTab_rootQuery$data;
  variables: locationAboutTab_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "locationId"
      }
    ],
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
      (v1/*: any*/),
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
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationAddressDetails",
        "kind": "LinkedField",
        "name": "physicalAddress",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "formattedAddress",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationOrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v1/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "canModify",
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
    "name": "locationAboutTab_rootQuery",
    "selections": (v2/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationAboutTab_rootQuery",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "dc96ff56e7c65eb588e4259474bda886",
    "id": null,
    "metadata": {},
    "name": "locationAboutTab_rootQuery",
    "operationKind": "query",
    "text": "query locationAboutTab_rootQuery(\n  $locationId: String!\n) {\n  location(id: $locationId) {\n    id\n    name\n    about\n    timezone\n    physicalAddress {\n      formattedAddress\n    }\n    organization {\n      name\n    }\n    canModify\n  }\n}\n"
  }
};
})();

(node as any).hash = "693f01a7340084aecd19a274a260614d";

export default node;
