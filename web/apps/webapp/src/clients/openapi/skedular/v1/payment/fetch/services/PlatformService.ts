/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class PlatformService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Stripe Platform Account Webhook
     * @param stripeSignature Stripe webhook signature
     * @returns any the status of processing the Stripe Platform Account event
     * @returns Error unexpected error
     * @throws ApiError
     */
    public processStripePlatformAccountEvent(
        stripeSignature?: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/payment/stripe/platform/account/webhook',
            headers: {
                'Stripe-Signature': stripeSignature,
            },
        });
    }
}
