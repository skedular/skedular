/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export const $Version = {
    properties: {
        major: {
            type: 'number',
            description: `major version`,
            isRequired: true,
            format: 'int32',
        },
        minor: {
            type: 'number',
            description: `minor version`,
            isRequired: true,
            format: 'int32',
        },
        build: {
            type: 'number',
            description: `build number`,
            isRequired: true,
            format: 'int32',
        },
        revision: {
            type: 'number',
            description: `revision`,
            isRequired: true,
            format: 'int32',
        },
    },
} as const;
