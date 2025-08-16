/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { Version } from '../models/Version';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class V1Service {
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
            url: '/v1/location/version',
        });
    }
    /**
     * republish location
     * @param locationId
     * @returns any the status of location republishing
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republish(
        locationId: string,
    ): CancelablePromise<any | ProblemDetails> {
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
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republishAll(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/location/republish-all',
        });
    }
    /**
     * regenerate all locations daily analytics
     * @returns any the status of regenerating all locations daily analytics
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateAllDailyAnalytics(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/location/analytics/regenerate-all-daily-analytics',
        });
    }
    /**
     * regenerate location daily analytics
     * @param locationId
     * @returns any the status of regenerating location daily analytics
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateDailyAnalytics(
        locationId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/location/analytics/{locationId}/regenerate-daily-analytics',
            path: {
                'locationId': locationId,
            },
        });
    }
}
