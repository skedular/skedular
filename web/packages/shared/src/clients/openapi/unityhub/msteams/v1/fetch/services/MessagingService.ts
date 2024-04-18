/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class MessagingService {
  constructor(public readonly httpRequest: BaseHttpRequest) {}
  /**
   * process bot messages sent by Azure Bot system
   * @returns any the message processing result
   * @returns Error unexpected error
   * @throws ApiError
   */
  public processBotMessage(): CancelablePromise<any | Error> {
    return this.httpRequest.request({
      method: 'POST',
      url: '/msteams/api/v1/bot-message',
    });
  }
}
