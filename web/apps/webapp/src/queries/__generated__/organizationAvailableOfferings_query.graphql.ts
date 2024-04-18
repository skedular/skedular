/**
 * @generated SignedSource<<4103e101a7c082d8f0e2bda62141c0da>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationAvailableOfferings_query$data = {
  readonly organization: {
    readonly availableOfferings: ReadonlyArray<{
      readonly code: string;
      readonly featureSet: ReadonlyArray<{
        readonly description: string;
        readonly name: string;
      }>;
      readonly name: string;
      readonly unitPrice: number;
    }>;
    readonly hasAttachedPaymentMethod: boolean;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentType": "organizationAvailableOfferings_query";
};
export type organizationAvailableOfferings_query$key = {
  readonly " $data"?: organizationAvailableOfferings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAvailableOfferings_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
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
  "name": "organizationAvailableOfferings_query",
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
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        },
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "hasAttachedPaymentMethod",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationAvailableOfferingDetails",
          "kind": "LinkedField",
          "name": "availableOfferings",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "code",
              "storageKey": null
            },
            (v0/*: any*/),
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
                (v0/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "description",
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
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "81d8721c5fd1b9c94bf66648ce6e8a9d";

export default node;
