/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class CustomerService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
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
            url: '/customer/api/v1/{customerId}/republish',
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
            url: '/customer/api/v1/republish-all',
        });
    }
}
