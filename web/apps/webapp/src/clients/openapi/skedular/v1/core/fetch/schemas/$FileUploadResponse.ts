/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export const $FileUploadResponse = {
    properties: {
        id: {
            type: 'string',
            isRequired: true,
        },
        cdnUrl: {
            type: 'string',
            isRequired: true,
        },
        contentType: {
            type: 'string',
            isNullable: true,
        },
        width: {
            type: 'number',
            isNullable: true,
            format: 'int32',
        },
        height: {
            type: 'number',
            isNullable: true,
            format: 'int32',
        },
    },
} as const;
