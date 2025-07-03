/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class SsoService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * sso acs
     * @returns any sso acs
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public ssoAcs(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/organization/acs',
        });
    }
}
