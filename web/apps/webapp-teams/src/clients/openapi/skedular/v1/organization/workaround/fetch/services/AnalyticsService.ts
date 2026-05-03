/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class AnalyticsService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * regenerate all organizations daily analytics
     * @returns any the status of regenerating all organizations daily analytics
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateAllDailyAnalytics(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/analytics/regenerate-all-daily-analytics',
        });
    }
    /**
     * regenerate organization daily analytics
     * @param organizationId
     * @returns any the status of regenerating organization daily analytics
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateDailyAnalytics(
        organizationId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/analytics/{organizationId}/regenerate-daily-analytics',
            path: {
                'organizationId': organizationId,
            },
        });
    }
}
