/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class BillingService {
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
            url: '/billing/api/v1/republish-organization-billing-info/{organizationId}',
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
            url: '/billing/api/v1/republish-all-organizations-billing-info',
        });
    }
}
