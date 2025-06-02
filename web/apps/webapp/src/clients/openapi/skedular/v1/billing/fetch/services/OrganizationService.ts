/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class OrganizationService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * republish organization billing info
     * @param organizationId
     * @returns any the status of organization republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishOrganizationBillingInfo(
        organizationId: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/billing/republish-organization-billing-info/{organizationId}',
            path: {
                'organizationId': organizationId,
            },
        });
    }
    /**
     * republish all organizations billing info
     * @returns any the status of organization republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishAllOrganizationsBillingInfo(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/billing/republish-all-organizations-billing-info',
        });
    }
}
