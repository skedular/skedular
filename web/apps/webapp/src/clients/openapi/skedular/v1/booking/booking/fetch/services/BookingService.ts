/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { Version } from '../models/Version';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class BookingService {
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
            url: '/v1/booking/version',
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
            url: '/v1/booking/raiseGraphqlChange/{topicName}/{id}',
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
     * Stripe Platform Account Webhook
     * @param stripeSignature Stripe webhook signature
     * @returns any the status of processing the Stripe Platform Account event
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public processStripePlatformAccountEvent(
        stripeSignature?: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/booking/stripe/platform/account/webhook',
            headers: {
                'Stripe-Signature': stripeSignature,
            },
        });
    }
    /**
     * Stripe Connect Account Webhook
     * @param stripeSignature Stripe webhook signature
     * @returns any the status of processing the Stripe Connect Account event
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public processStripeConnectAccountEvent(
        stripeSignature?: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/booking/stripe/connect/account/webhook',
            headers: {
                'Stripe-Signature': stripeSignature,
            },
        });
    }
    /**
     * Xero Webhook
     * @param xXeroSignature Xero webhook signature
     * @returns any the status of processing the Xero webhook event
     * @throws ApiError
     */
    public processXeroWebhookEvent(
        xXeroSignature?: string,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/booking/xero/webhook',
            headers: {
                'x-xero-signature': xXeroSignature,
            },
            errors: {
                401: `invalid Xero webhook signature`,
            },
        });
    }
}
