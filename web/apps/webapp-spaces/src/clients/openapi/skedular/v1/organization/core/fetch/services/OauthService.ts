/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class OauthService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Stripe Connect Account OAuth Callback
     * @param code An authorization code you can use in the next call to get an access token for your user. This can only be used once and expires in 5 minutes.
     * @param scope read_write or read_only, depending what you passed on the initial GET request.
     * @param state The value of the state parameter you provided on the initial GET request.
     * @returns any the status of processing the Stripe Connect Account event
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public stripeConnectAccountOAuthCallback(
        code: string,
        scope: string,
        state: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/stripe/connect/account/oauth/callback',
            query: {
                'code': code,
                'scope': scope,
                'state': state,
            },
        });
    }
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
