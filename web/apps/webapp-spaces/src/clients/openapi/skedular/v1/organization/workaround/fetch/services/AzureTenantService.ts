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
     * resync all azure tenants
     * @returns any the status of resyncing all azure tenants
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public reSyncAllAzureTenants(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/azure-tenant/resync-all-azure-tenants',
        });
    }
    /**
     * resync azure tenant
     * @param tenantId
     * @returns any the status of resyncing azure tenant
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public reSyncAzureTenant(
        tenantId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/azure-tenant/{tenantId}/resync',
            path: {
                'tenantId': tenantId,
            },
        });
    }
}
