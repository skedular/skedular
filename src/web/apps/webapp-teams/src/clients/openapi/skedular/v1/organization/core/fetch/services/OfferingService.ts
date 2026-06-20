/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OfferingCode } from '../models/OfferingCode';
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class OfferingService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * change organization offering by Id
     * @param organizationId
     * @param offeringCode
     * @param xApiKey API Key
     * @returns any the status of changing organization offering
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public changeOrganizationOfferingById(
        organizationId: string,
        offeringCode: OfferingCode,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/{organizationId}/changeOrganizationOfferingById/{offeringCode}',
            path: {
                'organizationId': organizationId,
                'offeringCode': offeringCode,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
    /**
     * change organization offering by custom domain
     * @param customDomain
     * @param offeringCode
     * @param xApiKey API Key
     * @returns any the status of changing organization offering
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public changeOrganizationOfferingByCustomDomain(
        customDomain: string,
        offeringCode: OfferingCode,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/{customDomain}/changeOrganizationOfferingByCustomDomain/{offeringCode}',
            path: {
                'customDomain': customDomain,
                'offeringCode': offeringCode,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
}
