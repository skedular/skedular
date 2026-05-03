/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class WebhookService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
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
