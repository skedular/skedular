/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class AzureTenantService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
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
