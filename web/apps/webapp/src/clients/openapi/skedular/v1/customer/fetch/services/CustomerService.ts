/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { Version } from '../models/Version';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class CustomerService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * return API version
     * @returns Version the version of the API
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public getVersion(): CancelablePromise<Version | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/customer/version',
        });
    }
    /**
     * raise graphql change
     * @param topicName
     * @param id
     * @param xApiKey API Key
     * @returns any the result of raising the graphql change
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public raiseGraphqlChange(
        topicName: string,
        id: string,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/customer/raiseGraphqlChange/{topicName}/{id}',
            path: {
                'topicName': topicName,
                'id': id,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
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
    /**
     * add customer payment method
     * @param setupIntent
     * @param setupIntentClientSecret
     * @param redirectStatus
     * @returns any the readiness status
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public addCustomerPaymentMethod(
        setupIntent: string,
        setupIntentClientSecret: string,
        redirectStatus: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/customer/add-payment-method',
            query: {
                'setup_intent': setupIntent,
                'setup_intent_client_secret': setupIntentClientSecret,
                'redirect_status': redirectStatus,
            },
        });
    }
}
