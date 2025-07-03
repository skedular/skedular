/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { Version } from '../models/Version';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class TeamService {
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
            url: '/v1/team/version',
        });
    }
    /**
     * republish team
     * @param teamId
     * @returns any the status of team republishing
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republish(
        teamId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/team/{teamId}/republish',
            path: {
                'teamId': teamId,
            },
        });
    }
    /**
     * republish all teams
     * @returns any the status of team republishing
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republishAll(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/team/republish-all',
        });
    }
}
