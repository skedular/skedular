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
     * republish booking
     * @param bookingId
     * @returns any the status of booking republishing
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republish(
        bookingId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/booking/{bookingId}/republish',
            path: {
                'bookingId': bookingId,
            },
        });
    }
    /**
     * republish all bookings
     * @returns any the status of republishing all bookings
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republishAll(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/booking/republish-all',
        });
    }
    /**
     * generate all locations resources slots
     * @returns any the status of republishing all location resources slots
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public generateAllLocationsResourcesSlots(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/booking/generate-all-resources-slots',
        });
    }
    /**
     * generate location's resources slots
     * @param locationId
     * @returns any the status of republishing locations resources slots
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public generateLocationResourcesSlots(
        locationId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/booking/resources-slots/{locationId}/generate-all-resources-slots',
            path: {
                'locationId': locationId,
            },
        });
    }
    /**
     * generate organization arrears invoices
     * @param organizationId
     * @returns any the status of generating organization arrears invoices
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public generateOrganizationArrearsInvoices(
        organizationId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/booking/organizations/{organizationId}/generate-arrears-invoices',
            path: {
                'organizationId': organizationId,
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
}
