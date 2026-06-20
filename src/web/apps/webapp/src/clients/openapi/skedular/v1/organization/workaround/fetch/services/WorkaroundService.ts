/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { SetEnterpriseOfferingRequest } from '../models/SetEnterpriseOfferingRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class WorkaroundService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * republish organization
     * @param customDomain
     * @returns any the status of organization republishing
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republish(
        customDomain: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/{customDomain}/republish',
            path: {
                'customDomain': customDomain,
            },
        });
    }
    /**
     * republish all organizations
     * @returns any the status of organization republishing
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republishAll(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/republish-all',
        });
    }
    /**
     * regenerate all offerings
     * @returns any the status of regenrating all offerings
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateAllOfferings(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/regenerate-all-offerings',
        });
    }
    /**
     * rerun all offerings workflows
     * @returns any the status of running all offerings workflows
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public rerunAllOfferingsWorkflows(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/rerun-all-offerings-workflows',
        });
    }
    /**
     * set negotiated organization offering
     * @param xApiKey API Key
     * @param requestBody
     * @returns any the status of setting the negotiated organization offering
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public setEnterpriseOffering(
        xApiKey: string,
        requestBody: SetEnterpriseOfferingRequest,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/enterprise-offering',
            headers: {
                'X-API-Key': xApiKey,
            },
            body: requestBody,
            mediaType: 'application/json',
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
    /**
     * regenerate all organizations daily analytics
     * @returns any the status of regenerating all organizations daily analytics
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateAllDailyAnalytics(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/analytics/regenerate-all-daily-analytics',
        });
    }
    /**
     * regenerate organization daily analytics
     * @param customDomain
     * @returns any the status of regenerating organization daily analytics
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateDailyAnalytics(
        customDomain: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/analytics/{customDomain}/regenerate-daily-analytics',
            path: {
                'customDomain': customDomain,
            },
        });
    }
}
