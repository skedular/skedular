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
