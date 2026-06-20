/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class V1Service {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
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
}
