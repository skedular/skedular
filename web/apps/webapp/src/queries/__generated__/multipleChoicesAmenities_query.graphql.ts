/**
 * @generated SignedSource<<14b0b1364564ed1cec948c34fd05dfc2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type multipleChoicesAmenities_query$data = {
  readonly organization: {
    readonly amenities: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly id: string;
      readonly name: string;
    }>;
  } | null | undefined;
  readonly " $fragmentType": "multipleChoicesAmenities_query";
};
export type multipleChoicesAmenities_query$key = {
  readonly " $data"?: multipleChoicesAmenities_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesAmenities_query">;
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
  "name": "multipleChoicesAmenities_query",
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
          "name": "amenities",
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

(node as any).hash = "51a6324e9fbd7e85659b6fedccd4fce5";

export default node;
