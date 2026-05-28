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
     * slack installation callback
     * @param code
     * @param state
     * @returns any the response
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public callback(
        code: string,
        state?: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/slack/callback',
            query: {
                'code': code,
                'state': state,
            },
        });
    }
}
