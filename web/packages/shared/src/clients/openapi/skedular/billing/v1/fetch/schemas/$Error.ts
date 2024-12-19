/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export const $Error = {
  properties: {
    code: {
      type: 'number',
      description: `Error code`,
      isRequired: true,
      format: 'int32',
    },
    message: {
      type: 'string',
      description: `Error message`,
      isRequired: true,
    },
  },
} as const;
