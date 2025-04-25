/**
 * @generated SignedSource<<e143f97197c0fbfcf029bf4ddbb6db9d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type myDetails_rootQuery$variables = Record<PropertyKey, never>;
export type myDetails_rootQuery$data = {
  readonly me: {
    readonly designation: string | null | undefined;
    readonly email: string | null | undefined;
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly id: string;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly phoneNumber: string | null | undefined;
    readonly photoUrl: string | null | undefined;
    readonly timezone: string | null | undefined;
    readonly title: string | null | undefined;
  } | null | undefined;
  readonly myBillingContactDetails: {
    readonly addressLine1: string | null | undefined;
    readonly addressLine2: string | null | undefined;
    readonly city: string | null | undefined;
    readonly companyName: string | null | undefined;
    readonly country: string | null | undefined;
    readonly email: string | null | undefined;
    readonly id: string;
    readonly province: string | null | undefined;
    readonly suburb: string | null | undefined;
    readonly zipcode: string | null | undefined;
  };
};
export type myDetails_rootQuery = {
  response: myDetails_rootQuery$data;
  variables: myDetails_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "email",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "CustomerDetails",
    "kind": "LinkedField",
    "name": "me",
    "plural": false,
    "selections": [
      (v0/*: any*/),
      (v1/*: any*/),
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
      }
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "CustomerBillingContactDetails",
    "kind": "LinkedField",
    "name": "myBillingContactDetails",
    "plural": false,
    "selections": [
      (v0/*: any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "companyName",
        "storageKey": null
      },
      (v1/*: any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "addressLine1",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "addressLine2",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "suburb",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "city",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "province",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "zipcode",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "country",
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "myDetails_rootQuery",
    "selections": (v2/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "myDetails_rootQuery",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "f64814df486207be007032aa85261212",
    "id": null,
    "metadata": {},
    "name": "myDetails_rootQuery",
    "operationKind": "query",
    "text": "query myDetails_rootQuery {\n  me {\n    id\n    email\n    photoUrl\n    designation\n    title\n    name\n    givenName\n    middleName\n    familyName\n    timezone\n    phoneNumber\n  }\n  myBillingContactDetails {\n    id\n    companyName\n    email\n    addressLine1\n    addressLine2\n    suburb\n    city\n    province\n    zipcode\n    country\n  }\n}\n"
  }
};
})();

(node as any).hash = "9771b84cb97f0b1673781e84ba260f91";

export default node;
