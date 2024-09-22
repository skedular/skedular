/**
 * @generated SignedSource<<8f8b9d463b47d6c83817d9938fb5f3f2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamMemberCard_TeamMemberDetails$data = {
  readonly customer: {
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  };
  readonly id: string;
  readonly organizationMember: {
    readonly customer: {
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
      readonly photoUrl: string | null | undefined;
    };
  } | null | undefined;
  readonly " $fragmentType": "teamMemberCard_TeamMemberDetails";
};
export type teamMemberCard_TeamMemberDetails$key = {
  readonly " $data"?: teamMemberCard_TeamMemberDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamMemberCard_TeamMemberDetails">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamCustomerDetails",
  "kind": "LinkedField",
  "name": "customer",
  "plural": false,
  "selections": [
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
      "name": "photoUrl",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "teamMemberCard_TeamMemberDetails",
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
      "concreteType": "TeamOrganizationMemberDetails",
      "kind": "LinkedField",
      "name": "organizationMember",
      "plural": false,
      "selections": [
        (v0/*: any*/)
      ],
      "storageKey": null
    }
  ],
  "type": "TeamMemberDetails",
  "abstractKey": null
};
})();

(node as any).hash = "e852307225387212bf6563dcb288b1f8";

export default node;
