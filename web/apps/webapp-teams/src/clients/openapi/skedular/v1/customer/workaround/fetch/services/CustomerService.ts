/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class CustomerService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * republish customer
     * @param customerId
     * @returns any the status of customer event republishing
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republish(
        customerId: string,
    ): CancelablePromise<any | ProblemDetails> {
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
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republishAll(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/customer/republish-all',
        });
    }
}
