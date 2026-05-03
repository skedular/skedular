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
     * verify organization ownership
     * @param organizationId
     * @param xApiKey API Key
     * @returns any the status of verifying organization ownership
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public verifyOrganizationOwnership(
        organizationId: string,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/{organizationId}/ownership/verify',
            path: {
                'organizationId': organizationId,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
}
