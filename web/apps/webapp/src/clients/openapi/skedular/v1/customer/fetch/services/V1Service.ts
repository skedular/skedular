/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { Version } from '../models/Version';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class V1Service {
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
            url: '/v1/customer/version',
        });
    }
    /**
     * republish customer
     * @param customerId
     * @returns any the status of customer event republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republish(
        customerId: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/customer/{customerId}/republish',
            path: {
                'customerId': customerId,
            },
        });
    }
    /**
     * republish all customers
     * @returns any the status of customer republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishAll(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/customer/republish-all',
        });
    }
}
