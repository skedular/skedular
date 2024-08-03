/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class MsteamsService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * process bot messages sent by Azure Bot system
     * @returns any the message processing result
     * @returns Error unexpected error
     * @throws ApiError
     */
    public processBotMessage(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/msteams/api/v1/bot-message',
        });
    }
    /**
     * generate an admin consent Url for the given tenant
     * @param tenantId
     * @returns void
     * @throws ApiError
     */
    public adminConsent(
        tenantId: string,
    ): CancelablePromise<void> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/msteams/api/v1/adminconsent/{tenantId}',
            query: {
                'tenantId': tenantId,
            },
            errors: {
                302: `redirect status response code`,
            },
        });
    }
    /**
     * OnBoard a tenant
     * @param tenant
     * @param adminConsent
     * @param state
     * @param error
     * @param errorDescription
     * @returns any on boarding response
     * @throws ApiError
     */
    public onBoardTenant(
        tenant: string,
        adminConsent: boolean,
        state: string,
        error?: string,
        errorDescription?: string,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/msteams/api/v1/onboard-tenant',
            query: {
                'tenant': tenant,
                'admin_consent': adminConsent,
                'state': state,
                'error': error,
                'error_description': errorDescription,
            },
        });
    }
}
