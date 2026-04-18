/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class WebhookService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
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
