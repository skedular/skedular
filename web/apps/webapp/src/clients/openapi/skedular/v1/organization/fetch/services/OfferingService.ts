/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class OfferingService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
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
     * change organization offering
     * @param organizationId
     * @param offeringCode
     * @param xApiKey API Key
     * @returns any the status of changing organization offering
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public changeOrganizationOffering(
        organizationId: string,
        offeringCode: string,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/{organizationId}/offering/{offeringCode}',
            path: {
                'organizationId': organizationId,
                'offeringCode': offeringCode,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
}
