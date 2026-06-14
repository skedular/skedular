/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export const $SetEnterpriseOfferingRequest = {
    properties: {
        fixedPrice: {
            type: 'number',
            description: `Fixed monthly price for the negotiated enterprise offering in minor currency units.`,
            isRequired: true,
            format: 'int32',
        },
        currency: {
            type: 'Currency',
            isRequired: true,
        },
        purchasedUserCapacity: {
            type: 'number',
            description: `Maximum monthly users allowed for the negotiated enterprise offering.`,
            isRequired: true,
            format: 'int32',
            minimum: 1,
        },
        purchasedLocationCapacity: {
            type: 'number',
            description: `Maximum monthly locations allowed for the negotiated enterprise offering.`,
            isRequired: true,
            format: 'int32',
            minimum: 1,
        },
        purchasedTeamCapacity: {
            type: 'number',
            description: `Maximum monthly teams allowed for the negotiated enterprise offering.`,
            isRequired: true,
            format: 'int32',
            minimum: 1,
        },
    },
} as const;
