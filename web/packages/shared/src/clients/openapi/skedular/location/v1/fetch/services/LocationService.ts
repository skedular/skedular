/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class LocationService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * republish location
     * @param locationId
     * @returns any the status of location republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republish(
        locationId: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/location/api/v1/{locationId}/republish',
            path: {
                'locationId': locationId,
            },
        });
    }
    /**
     * republish all locations
     * @returns any the status of location republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishAll(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/location/api/v1/republish-all',
        });
    }
}
