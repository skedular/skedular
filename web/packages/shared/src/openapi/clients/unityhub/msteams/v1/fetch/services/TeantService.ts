/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class TeantService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * generate a temporary authorization code
     * @returns void
     * @throws ApiError
     */
    public generateTemporaryAuthorizationCode(): CancelablePromise<void> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/msteams/api/v1/generate-temporary-authorization-code',
            errors: {
                302: `redirect status response code`,
            },
        });
    }
}
