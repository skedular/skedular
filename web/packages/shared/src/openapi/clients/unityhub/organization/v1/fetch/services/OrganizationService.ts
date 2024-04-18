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
            url: '/organization/api/v1/{organizationId}/republish',
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
            url: '/organization/api/v1/republish-all',
        });
    }
}
