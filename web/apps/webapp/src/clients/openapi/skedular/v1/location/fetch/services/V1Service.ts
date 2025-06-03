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
            url: '/v1/location/version',
        });
    }
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
            url: '/v1/location/{locationId}/republish',
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
            url: '/v1/location/republish-all',
        });
    }
}
