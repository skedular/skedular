/**
 * @generated SignedSource<<479f41184327f064e2e1b74e0b112c7b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type PersonalInformationVisibility = "REDACTED" | "VISIBLE" | "%future added value";
export type mySettings_rootQuery$variables = Record<PropertyKey, never>;
export type mySettings_rootQuery$data = {
  readonly me: {
    readonly designation: string | null | undefined;
    readonly email: string | null | undefined;
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly id: string;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly personalInformationVisibility: {
      readonly name: string;
      readonly type: PersonalInformationVisibility;
    };
    readonly phoneNumber: string | null | undefined;
    readonly photoUrl: string | null | undefined;
    readonly timezone: string | null | undefined;
    readonly title: string | null | undefined;
  };
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceUserPersonalInformationVisibility_query">;
};
export type mySettings_rootQuery = {
  response: mySettings_rootQuery$data;
  variables: mySettings_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v1 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v0/*: any*/)
],
v2 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
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
      "name": "email",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "photoUrl",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "designation",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "title",
      "storageKey": null
    },
    (v0/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "givenName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "middleName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "familyName",
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
      "kind": "ScalarField",
      "name": "phoneNumber",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "PersonalInformationVisibilityDetails",
      "kind": "LinkedField",
      "name": "personalInformationVisibility",
      "plural": false,
      "selections": (v1/*: any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "mySettings_rootQuery",
    "selections": [
      (v2/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceUserPersonalInformationVisibility_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "mySettings_rootQuery",
    "selections": [
      (v2/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "PersonalInformationVisibilityDetails",
        "kind": "LinkedField",
        "name": "personalInformationVisibilityTypes",
        "plural": true,
        "selections": (v1/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "76fad3ea16295f2bf4fe0a1deab74b0d",
    "id": null,
    "metadata": {},
    "name": "mySettings_rootQuery",
    "operationKind": "query",
    "text": "query mySettings_rootQuery {\n  me {\n    id\n    email\n    photoUrl\n    designation\n    title\n    name\n    givenName\n    middleName\n    familyName\n    timezone\n    phoneNumber\n    personalInformationVisibility {\n      type\n      name\n    }\n  }\n  ...singleChoiceUserPersonalInformationVisibility_query\n}\n\nfragment singleChoiceUserPersonalInformationVisibility_query on Query {\n  personalInformationVisibilityTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "f0fddbae53764517a63b2ebb5ef593d1";

export default node;
