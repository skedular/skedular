/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { RegenerateResourceAvailabilitySnapshotsInput } from '../models/RegenerateResourceAvailabilitySnapshotsInput';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class AnalyticsService {
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
    /**
     * regenerate resource availability snapshots for a location over a date range
     * @param locationId
     * @param requestBody
     * @returns any the status of regenerating resource availability snapshots
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateResourceAvailabilitySnapshots(
        locationId: string,
        requestBody: RegenerateResourceAvailabilitySnapshotsInput,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/location/analytics/{locationId}/regenerate-resource-availability-snapshots',
            path: {
                'locationId': locationId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
}
