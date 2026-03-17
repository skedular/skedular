/**
 * @generated SignedSource<<7ceacad68369a1ef5185291ae17fd690>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type singleChoiceResourceType_query$data = {
  readonly organization: {
    readonly resourceTypes: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly id: string;
      readonly name: string;
    }>;
  } | null | undefined;
  readonly " $fragmentType": "singleChoiceResourceType_query";
};
export type singleChoiceResourceType_query$key = {
  readonly " $data"?: singleChoiceResourceType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceResourceType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceResourceType_query",
  "selections": [
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
          "args": null,
          "concreteType": "OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "resourceTypes",
          "plural": true,
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
              "name": "color",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "0562fe5d919fdeba8c4cfa17237dad6c";

export default node;
