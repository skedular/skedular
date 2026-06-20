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
}
