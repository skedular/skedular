/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class LocationService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
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
