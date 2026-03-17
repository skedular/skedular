/**
 * @generated SignedSource<<1bb1074324a7d8835bf8c12da96100de>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrganizationType = "INDIVIDUAL" | "MARKETPLACE" | "PRIVATE" | "%future added value";
export type addPrivateLocation_rootQuery$variables = {
  organizationCustomDomain: string;
};
export type addPrivateLocation_rootQuery$data = {
  readonly emailsToShowLatestCapabilities: ReadonlyArray<string>;
  readonly me: {
    readonly emails: ReadonlyArray<string>;
  };
  readonly organization: {
    readonly type: {
      readonly type: OrganizationType;
    };
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceLocationType_query">;
};
export type addPrivateLocation_rootQuery = {
  response: addPrivateLocation_rootQuery$data;
  variables: addPrivateLocation_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "emailsToShowLatestCapabilities",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "emails",
  "storageKey": null
},
v3 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTypeDetails",
  "kind": "LinkedField",
  "name": "type",
  "plural": false,
  "selections": [
    (v4/*: any*/)
  ],
  "storageKey": null
},
v6 = {
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
    "name": "addPrivateLocation_rootQuery",
    "selections": [
      (v1/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v2/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v5/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceLocationType_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addPrivateLocation_rootQuery",
    "selections": [
      (v1/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v6/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          (v6/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationTypeDetails",
        "kind": "LinkedField",
        "name": "locationTypes",
        "plural": true,
        "selections": [
          (v4/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "8cf0ed69c8d4898d5ba9162b91031680",
    "id": null,
    "metadata": {},
    "name": "addPrivateLocation_rootQuery",
    "operationKind": "query",
    "text": "query addPrivateLocation_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  emailsToShowLatestCapabilities\n  me {\n    emails\n    id\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    type {\n      type\n    }\n    id\n  }\n  ...singleChoiceLocationType_query\n}\n\nfragment singleChoiceLocationType_query on Query {\n  locationTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "3da12aeedea0a5b602347626344b2ad8";

export default node;
