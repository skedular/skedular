/**
 * @generated SignedSource<<8c0014a4f238ae0c7b561991f4e44c53>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationType = "HOST" | "MARKETPLACE" | "PRIVATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type LocationCard_location$data = {
  readonly id: string;
  readonly name: string;
  readonly organization: {
    readonly id: string;
    readonly type: {
      readonly name: string;
      readonly type: OrganizationType;
    };
  };
  readonly " $fragmentType": "LocationCard_location";
};
export type LocationCard_location$key = {
  readonly " $data"?: LocationCard_location$data;
  readonly " $fragmentSpreads": FragmentRefs<"LocationCard_location">;
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
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "LocationCard_location",
  "selections": [
    (v0/*:: as any*/),
    (v1/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        (v0/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTypeDetails",
          "kind": "LinkedField",
          "name": "type",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "type",
              "storageKey": null
            },
            (v1/*:: as any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "LocationDetails",
  "abstractKey": null
};
})();

(node as any).hash = "9e5d33b37cae3d0a54fe30e0df9ee75e";

export default node;
