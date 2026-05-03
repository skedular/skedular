/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class XeroService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Xero OAuth start
     * @param organizationId
     * @param organizationCustomDomain
     * @returns void
     * @throws ApiError
     */
    public startXeroOAuth(
        organizationId?: string,
        organizationCustomDomain?: string,
    ): CancelablePromise<void> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/xero/oauth/start',
            query: {
                'organizationId': organizationId,
                'organizationCustomDomain': organizationCustomDomain,
            },
            errors: {
                302: `redirect to Xero consent page`,
            },
        });
    }
    /**
     * Xero OAuth callback
     * @param code
     * @param state
     * @returns void
     * @throws ApiError
     */
    public xeroOAuthCallback(
        code: string,
        state: string,
    ): CancelablePromise<void> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/xero/oauth/callback',
            query: {
                'code': code,
                'state': state,
            },
            errors: {
                302: `redirect back to organization marketplace setup`,
            },
        });
    }
}
