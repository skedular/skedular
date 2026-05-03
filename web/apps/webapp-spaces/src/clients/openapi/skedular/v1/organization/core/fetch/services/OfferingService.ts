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
