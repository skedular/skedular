/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class SsoService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * sso login
     * @param organizationId Unique identifier for the organization initiating SSO login.
     * @returns any sso login
     * @returns Error unexpected error
     * @throws ApiError
     */
    public ssoLogin(
        organizationId: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/organization/api/v1/{organizationId}/sso/login',
            path: {
                'organizationId': organizationId,
            },
        });
    }
    /**
     * sso acs
     * @returns any sso acs
     * @returns Error unexpected error
     * @throws ApiError
     */
    public ssoAcs(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/organization/api/v1/acs',
        });
    }
}
