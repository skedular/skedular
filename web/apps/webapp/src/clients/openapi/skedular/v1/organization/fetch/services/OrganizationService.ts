/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { Version } from '../models/Version';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class OrganizationService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * return API version
     * @returns Version the version of the API
     * @returns Error unexpected error
     * @throws ApiError
     */
    public getVersion(): CancelablePromise<Version | Error> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/version',
        });
    }
    /**
     * republish organization
     * @param organizationId
     * @returns any the status of organization republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republish(
        organizationId: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/{organizationId}/republish',
            path: {
                'organizationId': organizationId,
            },
        });
    }
    /**
     * republish all organizations
     * @returns any the status of organization republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishAll(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/republish-all',
        });
    }
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
     * sso acs
     * @returns any sso acs
     * @returns Error unexpected error
     * @throws ApiError
     */
    public ssoAcs(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/organization/acs',
        });
    }
}
