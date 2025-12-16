/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { Version } from '../models/Version';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class MsteamsService {
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
            url: '/v1/msteams/version',
        });
    }
    /**
     * raise graphql change
     * @param topicName
     * @param id
     * @param xApiKey API Key
     * @returns any the result of raising the graphql change
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public raiseGraphqlChange(
        topicName: string,
        id: string,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/msteams/raiseGraphqlChange/{topicName}/{id}',
            path: {
                'topicName': topicName,
                'id': id,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
    /**
     * resync all MsTeams
     * @returns any the status of resyncing all MsTeams
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public reSyncAllMsTeams(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/msteams/azure-tenant/resync-all-msteams',
        });
    }
    /**
     * resync MsTeams
     * @param tenantId
     * @returns any the status of resyncing MsTeams
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public reSyncMsTeams(
        tenantId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/msteams/azure-tenant/{tenantId}/resync',
            path: {
                'tenantId': tenantId,
            },
        });
    }
}
