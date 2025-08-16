/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class AzureTenantService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * generate an admin consent Url for the given tenant
     * @returns void
     * @throws ApiError
     */
    public azureTenantAdminConsentUrl(): CancelablePromise<void> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/azure-tenant-admin-consent-url',
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
     * @returns any onboarding response
     * @throws ApiError
     */
    public onboardAzureTenant(
        tenant: string,
        adminConsent: boolean,
        state: string,
        error?: string,
        errorDescription?: string,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/onboard-azure-tenant',
            query: {
                'tenant': tenant,
                'admin_consent': adminConsent,
                'state': state,
                'error': error,
                'error_description': errorDescription,
            },
        });
    }
    /**
     * resync all azure tenants
     * @returns any the status of resyncing all azure tenants
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public reSyncAllAzureTenants(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/azure-tenant/resync-all-azure-tenants',
        });
    }
    /**
     * resync azure tenant
     * @param tenantId
     * @returns any the status of resyncing azure tenant
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public reSyncAzureTenant(
        tenantId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/azure-tenant/{tenantId}/resync',
            path: {
                'tenantId': tenantId,
            },
        });
    }
}
