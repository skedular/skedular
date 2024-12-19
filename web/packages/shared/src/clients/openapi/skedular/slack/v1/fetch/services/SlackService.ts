/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class SlackService {
  constructor(public readonly httpRequest: BaseHttpRequest) {}
  /**
   * slack installation callback
   * @param code
   * @param state
   * @returns any the response
   * @returns Error unexpected error
   * @throws ApiError
   */
  public callback(code: string, state?: string): CancelablePromise<any | Error> {
    return this.httpRequest.request({
      method: 'GET',
      url: '/slack/api/v1/callback',
      query: {
        code: code,
        state: state,
      },
    });
  }
}
