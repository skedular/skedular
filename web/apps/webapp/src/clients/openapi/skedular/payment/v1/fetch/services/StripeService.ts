/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class StripeService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Stripe Webhook
     * @param stripeSignature Stripe webhook signature
     * @returns any the status of processing the Stripe event
     * @returns Error unexpected error
     * @throws ApiError
     */
    public processStripeEvent(
        stripeSignature?: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/payment/api/v1/stripe/webhook',
            headers: {
                'Stripe-Signature': stripeSignature,
            },
        });
    }
}
