/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { StatusResponse } from '../models/StatusResponse';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class StatusService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Report on the state of the service and database
     * Report on the state of the service and database. Useful for checking if the service is up and running. The JSON output also reports when the database was last updated.
     * @param format Selects the output format. See [Status Output Formats](https://nominatim.org/release-docs/develop/api/Status/#output) for details on each format. If not specified, it is equal to `text`.
     * @returns StatusResponse If `format` is `json` always returns a HTTP code 200, when the status call could be executed.
     * @throws ApiError
     */
    public status(
        format: 'text' | 'json' = 'json',
    ): CancelablePromise<StatusResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/status',
            query: {
                'format': format,
            },
        });
    }
}
