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
     * @returns any Sso Saml Acs
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public ssoSamlAcs(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/organization/sso/saml/acs',
        });
    }
}
