/**
 * @generated SignedSource<<633a6dfcef21cac47056b4f8737b6e4a>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type TeamPatchField = "ABOUT" | "FEATURE_IMAGES" | "NAME" | "PRIMARY_LOCATION" | "TIMEZONE" | "%future added value";
export type UpdateTeamInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  fieldsToUpdate: ReadonlyArray<TeamPatchField>;
  id: string;
  name: string;
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
export type organizationTeam_updateTeamMutation$variables = {
  input: UpdateTeamInput;
};
export type organizationTeam_updateTeamMutation$data = {
  readonly updateTeam: {
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
      readonly primaryLocation: {
        readonly id: string;
        readonly name: string;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  };
};
export type organizationTeam_updateTeamMutation$rawResponse = {
  readonly updateTeam: {
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
      readonly primaryLocation: {
        readonly id: string;
        readonly name: string;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  };
};
export type organizationTeam_updateTeamMutation = {
  rawResponse: organizationTeam_updateTeamMutation$rawResponse;
  response: organizationTeam_updateTeamMutation$data;
  variables: organizationTeam_updateTeamMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
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
v4 = [
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
    "name": "updateTeam",
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
          (v1/*:: as any*/),
          (v2/*:: as any*/),
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
                "selections": (v3/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v3/*:: as any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "primaryLocation",
            "plural": false,
            "selections": [
              (v1/*:: as any*/),
              (v2/*:: as any*/)
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
    "name": "organizationTeam_updateTeamMutation",
    "selections": (v4/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationTeam_updateTeamMutation",
    "selections": (v4/*:: as any*/)
  },
  "params": {
    "cacheID": "e6dbdfaf5f0c85f66798cae1442ca77d",
    "id": null,
    "metadata": {},
    "name": "organizationTeam_updateTeamMutation",
    "operationKind": "mutation",
    "text": "mutation organizationTeam_updateTeamMutation(\n  $input: UpdateTeamInput!\n) {\n  updateTeam(input: $input) {\n    team {\n      id\n      name\n      about\n      timezone\n      featureImages {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n      primaryLocation {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "fef27b8bf28b617b18134a46522f40f0";

export default node;
