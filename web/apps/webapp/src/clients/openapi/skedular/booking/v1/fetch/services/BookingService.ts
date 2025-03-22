/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class BookingService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * republish booking
     * @param bookingId
     * @returns any the status of booking republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republish(
        bookingId: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/booking/api/v1/{bookingId}/republish',
            path: {
                'bookingId': bookingId,
            },
        });
    }
    /**
     * republish all bookings
     * @returns any the status of republishing all bookings
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishAll(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/booking/api/v1/republish-all',
        });
    }
    /**
     * republish all resource slots
     * @returns any the status of republishing all resources slots
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishAllResourcesSlots(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/booking/api/v1/republish-all-resources-slots',
        });
    }
}
