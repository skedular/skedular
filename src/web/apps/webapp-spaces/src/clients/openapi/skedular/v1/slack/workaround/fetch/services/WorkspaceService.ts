/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class WorkspaceService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * resync all slack workspace
     * @returns any the status of resyncing all slack workspaces
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public reSyncAllSlackWorkspaces(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/slack/workspace/resync-all-workspaces',
        });
    }
    /**
     * resync slack workspace
     * @param workspaceId
     * @returns any the status of resyncing slack workspace
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public reSyncSlackWorkspace(
        workspaceId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/slack/workspace/{workspaceId}/resync',
            path: {
                'workspaceId': workspaceId,
            },
        });
    }
}
