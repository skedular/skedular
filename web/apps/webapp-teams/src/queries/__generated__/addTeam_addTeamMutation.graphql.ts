/**
 * @generated SignedSource<<7d6a198c4413e386d2702fb48eb3388f>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddTeamInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  id?: string | null | undefined;
  name: string;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
  organizationMemberIds: ReadonlyArray<string>;
  primaryLocationId?: string | null | undefined;
  timezone?: string | null | undefined;
};
export type CdnImageFileInput = {
  original?: CdnFileInput | null | undefined;
  thumbnail?: CdnFileInput | null | undefined;
};
export type CdnFileInput = {
  height?: number | null | undefined;
  url: string;
  width?: number | null | undefined;
};
export type addTeam_addTeamMutation$variables = {
  input: AddTeamInput;
};
export type addTeam_addTeamMutation$data = {
  readonly addTeam: {
    readonly team: {
      readonly about: string | null | undefined;
      readonly featureImages: ReadonlyArray<{
        readonly original: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
        readonly thumbnail: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
      }>;
      readonly id: string;
      readonly name: string;
      readonly timezone: string | null | undefined;
    };
  };
};
export type addTeam_addTeamMutation$rawResponse = {
  readonly addTeam: {
    readonly team: {
      readonly about: string | null | undefined;
      readonly featureImages: ReadonlyArray<{
        readonly original: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
        readonly thumbnail: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
      }>;
      readonly id: string;
      readonly name: string;
      readonly timezone: string | null | undefined;
    };
  };
};
export type addTeam_addTeamMutation = {
  rawResponse: addTeam_addTeamMutation$rawResponse;
  response: addTeam_addTeamMutation$data;
  variables: addTeam_addTeamMutation$variables;
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
    "args": null,
    "kind": "ScalarField",
    "name": "url",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "height",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "width",
    "storageKey": null
  }
],
v2 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "TeamPayload",
    "kind": "LinkedField",
    "name": "addTeam",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "TeamDetails",
        "kind": "LinkedField",
        "name": "team",
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
            "name": "name",
            "storageKey": null
          },
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
            "concreteType": "CdnImageFile",
            "kind": "LinkedField",
            "name": "featureImages",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "original",
                "plural": false,
                "selections": (v1/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v1/*:: as any*/),
                "storageKey": null
              }
            ],
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addTeam_addTeamMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "addTeam_addTeamMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "78a95b8a1ce3c14d8f669b73d6f74c15",
    "id": null,
    "metadata": {},
    "name": "addTeam_addTeamMutation",
    "operationKind": "mutation",
    "text": "mutation addTeam_addTeamMutation(\n  $input: AddTeamInput!\n) {\n  addTeam(input: $input) {\n    team {\n      id\n      name\n      about\n      timezone\n      featureImages {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "605536afc9b695b7c2a18b316dc607ca";

export default node;
