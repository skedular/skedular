/**
 * @generated SignedSource<<4ad0695a83d1c9d4e0b749de807f1c48>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type customerEntitlementsStrip_query$data = {
  readonly myEntitlements: ReadonlyArray<{
    readonly availableQuantity: number;
    readonly expiresAt: any;
    readonly grantedQuantity: number;
    readonly id: string;
    readonly pricingId: string;
    readonly restrictions: {
      readonly availableDays: ReadonlyArray<DayOfWeek>;
      readonly productId: string;
    } | null | undefined;
    readonly status: EntitlementStatus;
  }>;
  readonly " $fragmentType": "customerEntitlementsStrip_query";
};
export type customerEntitlementsStrip_query$key = {
  readonly " $data"?: customerEntitlementsStrip_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"customerEntitlementsStrip_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "customerEntitlementsStrip_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "EntitlementDetails",
      "kind": "LinkedField",
      "name": "myEntitlements",
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
          "name": "pricingId",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "availableQuantity",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "grantedQuantity",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "expiresAt",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "status",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "EntitlementRestrictionsDetails",
          "kind": "LinkedField",
          "name": "restrictions",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "productId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "availableDays",
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

(node as any).hash = "031f6d4edb8746bea943cb07cf3e562b";

export default node;
