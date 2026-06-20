/**
 * @generated SignedSource<<bc4bcb693ba1d68a172e287a8b3e1e79>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationUser_rootQuery$variables = {
  customerId: string;
  organizationCustomDomain: string;
};
export type pageOrganizationUser_rootQuery$data = {
  readonly customer: {
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationUser_query">;
};
export type pageOrganizationUser_rootQuery = {
  response: pageOrganizationUser_rootQuery$data;
  variables: pageOrganizationUser_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customerId"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v2 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "customerId"
  }
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v8 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v3/*:: as any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationUser_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v2/*:: as any*/),
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "customer",
        "plural": false,
        "selections": [
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v6/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationUser_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*:: as any*/),
      (v0/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationUser_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v2/*:: as any*/),
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "customer",
        "plural": false,
        "selections": [
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
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
            "selections": (v8/*:: as any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v7/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "customDomain",
            "variableName": "organizationCustomDomain"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "fields": [
                  {
                    "kind": "Variable",
                    "name": "customerId",
                    "variableName": "customerId"
                  }
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "ConnectionOfOrganizationMemberEdge",
            "kind": "LinkedField",
            "name": "members",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "totalCount",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationMemberEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationMemberDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v7/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationMemberStatusDetails",
                        "kind": "LinkedField",
                        "name": "status",
                        "plural": false,
                        "selections": (v8/*:: as any*/),
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "kind": "ClientExtension",
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "__id",
                    "storageKey": null
                  }
                ]
              }
            ],
            "storageKey": null
          },
          (v7/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "PersonalInformationVisibilityDetails",
        "kind": "LinkedField",
        "name": "personalInformationVisibilityTypes",
        "plural": true,
        "selections": (v8/*:: as any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "5e7c2de6f0dde0c5cc8fe263562a932e",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationUser_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationUser_rootQuery(\n  $organizationCustomDomain: String!\n  $customerId: String!\n) {\n  customer(id: $customerId) {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n  ...organizationUser_query\n}\n\nfragment organizationUserLeftSideNavigationMenuContent_query on Query {\n  me {\n    id\n  }\n}\n\nfragment organizationUser_query on Query {\n  me {\n    id\n  }\n  customer(id: $customerId) {\n    id\n    email\n    photoUrl\n    designation\n    title\n    name\n    givenName\n    middleName\n    familyName\n    timezone\n    phoneNumber\n    personalInformationVisibility {\n      type\n      name\n    }\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    members(where: {customerId: $customerId}) {\n      totalCount\n      edges {\n        node {\n          id\n          status {\n            type\n            name\n          }\n        }\n      }\n    }\n    id\n  }\n  ...organizationUserLeftSideNavigationMenuContent_query\n  ...singleChoiceUserPersonalInformationVisibility_query\n}\n\nfragment singleChoiceUserPersonalInformationVisibility_query on Query {\n  personalInformationVisibilityTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "9b9670a011c33d701436c0a1a5b3a125";

export default node;
