/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class OwnershipService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * verify organization ownership by id
     * @param organizationId
     * @param xApiKey API Key
     * @returns any the status of verifying organization ownership
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public verifyOrganizationOwnershipById(
        organizationId: string,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/ownership/verifyById/{organizationId}',
            path: {
                'organizationId': organizationId,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
    /**
     * verify organization ownership by custom domain
     * @param customDomain
     * @param xApiKey API Key
     * @returns any the status of verifying organization ownership
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public verifyOrganizationOwnershipByCustomDomain(
        customDomain: string,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/ownership/verifyByCustomDomain/{customDomain}',
            path: {
                'customDomain': customDomain,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
}
