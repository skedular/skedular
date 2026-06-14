/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Currency } from './Currency';
export type SetEnterpriseOfferingRequest = {
    /**
     * Fixed monthly price for the negotiated enterprise offering in minor currency units.
     */
    fixedPrice: number;
    currency: Currency;
    /**
     * Maximum monthly users allowed for the negotiated enterprise offering.
     */
    purchasedUserCapacity: number;
    /**
     * Maximum monthly locations allowed for the negotiated enterprise offering.
     */
    purchasedLocationCapacity: number;
    /**
     * Maximum monthly teams allowed for the negotiated enterprise offering.
     */
    purchasedTeamCapacity: number;
};

