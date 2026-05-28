/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export const $RegenerateResourceAvailabilitySnapshotsInput = {
    properties: {
        from: {
            type: 'string',
            description: `Start date (inclusive) for snapshot regeneration in UTC`,
            isRequired: true,
            format: 'date-time',
        },
        until: {
            type: 'string',
            description: `End date (inclusive) for snapshot regeneration in UTC`,
            isRequired: true,
            format: 'date-time',
        },
    },
} as const;
