/**
 * @generated SignedSource<<c2311a9b9223f9aa30c4ad5371fc5139>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationOffering_query$data = {
  readonly organization: {
    readonly id: string;
    readonly name: string;
    readonly offering: {
      readonly end: any;
      readonly featureSet: ReadonlyArray<{
        readonly description: string;
        readonly name: string;
      }>;
      readonly free: boolean;
      readonly id: string;
      readonly name: string;
      readonly start: any;
      readonly unitPrice: number;
    };
  } | null | undefined;
  readonly " $fragmentType": "organizationOffering_query";
};
export type organizationOffering_query$key = {
  readonly " $data"?: organizationOffering_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationOffering_query">;
};

const node: ReaderFragment = (function(){
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
  "name": "name",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationOffering_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationOfferingDetails",
          "kind": "LinkedField",
          "name": "offering",
          "plural": false,
          "selections": [
            (v0/*: any*/),
            (v1/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "start",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "end",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "unitPrice",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationFeatureSetDetails",
              "kind": "LinkedField",
              "name": "featureSet",
              "plural": true,
              "selections": [
                (v1/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "description",
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "free",
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
})();

(node as any).hash = "2b3b8591a608825785a7f5da1ae1fb2a";

export default node;
