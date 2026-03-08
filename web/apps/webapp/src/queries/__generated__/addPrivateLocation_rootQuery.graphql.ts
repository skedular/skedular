/**
 * @generated SignedSource<<39b047f060489e26315bcf5edafd798e>>
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
  organizationUniqueAlphanumericName: string;
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
    "name": "organizationUniqueAlphanumericName"
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
    "name": "uniqueAlphanumericName",
    "variableName": "organizationUniqueAlphanumericName"
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
    "cacheID": "1ac4dfc2dc27cc258b24a7be599c1ec9",
    "id": null,
    "metadata": {},
    "name": "addPrivateLocation_rootQuery",
    "operationKind": "query",
    "text": "query addPrivateLocation_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n) {\n  emailsToShowLatestCapabilities\n  me {\n    emails\n    id\n  }\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    type {\n      type\n    }\n    id\n  }\n  ...singleChoiceLocationType_query\n}\n\nfragment singleChoiceLocationType_query on Query {\n  locationTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "13ba51fabb2377576ebdde181c3aaab4";

export default node;
