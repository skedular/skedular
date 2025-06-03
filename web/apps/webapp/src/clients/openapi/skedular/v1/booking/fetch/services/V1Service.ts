/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { Version } from '../models/Version';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class V1Service {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * return API version
     * @returns Version the version of the API
     * @returns Error unexpected error
     * @throws ApiError
     */
    public getVersion(): CancelablePromise<Version | Error> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/booking/version',
        });
    }
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
            url: '/v1/booking/{bookingId}/republish',
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
            url: '/v1/booking/republish-all',
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
            url: '/v1/booking/republish-all-resources-slots',
        });
    }
    /**
     * republish resource slots
     * @param resourceId
     * @returns any the status of republishing resources slots
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishResourcesSlots(
        resourceId: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/booking/resources-slots/{resourceId}/republish',
            path: {
                'resourceId': resourceId,
            },
        });
    }
}
