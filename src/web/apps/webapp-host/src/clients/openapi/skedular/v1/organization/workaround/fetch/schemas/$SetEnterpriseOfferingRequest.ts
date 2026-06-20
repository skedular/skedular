/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export const $SetEnterpriseOfferingRequest = {
    properties: {
        organizationId: {
            type: 'string',
            isNullable: true,
        },
        customDomain: {
            type: 'string',
            isNullable: true,
        },
        offeringCode: {
            type: 'OfferingCode',
            description: `The organization offering code to assign.`,
            isRequired: true,
        },
        fixedPrice: {
            type: 'number',
            description: `Fixed monthly price for the negotiated offering in minor currency units.`,
            isRequired: true,
            format: 'int32',
        },
        currency: {
            type: 'Currency',
            isRequired: true,
        },
        purchasedUserCapacity: {
            type: 'number',
            description: `Optional maximum monthly users allowed for the negotiated offering. Omit for the offering default.`,
            isNullable: true,
            format: 'int32',
            minimum: 1,
        },
        purchasedLocationCapacity: {
            type: 'number',
            description: `Optional maximum monthly locations allowed for the negotiated offering. Omit for the offering default.`,
            isNullable: true,
            format: 'int32',
            minimum: 1,
        },
        purchasedTeamCapacity: {
            type: 'number',
            description: `Optional maximum monthly teams allowed for the negotiated offering. Omit for the offering default.`,
            isNullable: true,
            format: 'int32',
            minimum: 1,
        },
        monthlyBookingInstanceQuota: {
            type: 'number',
            description: `Optional monthly booking instance quota for Spaces offerings. Omit for the offering default.`,
            isNullable: true,
            format: 'int32',
            minimum: 1,
        },
        discountPercentage: {
            type: 'number',
            description: `Optional discount percentage to apply while billing this offering. It is copied to renewed offering periods until changed.`,
            format: 'int32',
            maximum: 100,
        },
    },
} as const;
