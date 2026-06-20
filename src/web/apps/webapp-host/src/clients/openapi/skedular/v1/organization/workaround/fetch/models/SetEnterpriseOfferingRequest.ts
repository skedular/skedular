/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Currency } from './Currency';
import type { OfferingCode } from './OfferingCode';
export type SetEnterpriseOfferingRequest = {
    organizationId?: string | null;
    customDomain?: string | null;
    /**
     * The organization offering code to assign.
     */
    offeringCode: OfferingCode;
    /**
     * Fixed monthly price for the negotiated offering in minor currency units.
     */
    fixedPrice: number;
    currency: Currency;
    /**
     * Optional maximum monthly users allowed for the negotiated offering. Omit for the offering default.
     */
    purchasedUserCapacity?: number | null;
    /**
     * Optional maximum monthly locations allowed for the negotiated offering. Omit for the offering default.
     */
    purchasedLocationCapacity?: number | null;
    /**
     * Optional maximum monthly teams allowed for the negotiated offering. Omit for the offering default.
     */
    purchasedTeamCapacity?: number | null;
    /**
     * Optional monthly booking instance quota for Spaces offerings. Omit for the offering default.
     */
    monthlyBookingInstanceQuota?: number | null;
    /**
     * Optional discount percentage to apply while billing this offering. It is copied to renewed offering periods until changed.
     */
    discountPercentage?: number;
};

