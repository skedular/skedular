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
        original: {
            type: 'cdnFile',
            isRequired: true,
        },
        thumbnail: {
            type: 'cdnFile',
            isNullable: true,
        },
    },
} as const;
