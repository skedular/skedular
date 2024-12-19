/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class OrganizationService {
  constructor(public readonly httpRequest: BaseHttpRequest) {}
  /**
   * add organization payment method
   * @param setupIntent
   * @param setupIntentClientSecret
   * @param redirectStatus
   * @returns any the readiness status
   * @returns Error unexpected error
   * @throws ApiError
   */
  public addOrganizationPaymentMethod(setupIntent: string, setupIntentClientSecret: string, redirectStatus: string): CancelablePromise<any | Error> {
    return this.httpRequest.request({
      method: 'GET',
      url: '/payment/api/v1/organization/add-payment-method',
      query: {
        setup_intent: setupIntent,
        setup_intent_client_secret: setupIntentClientSecret,
        redirect_status: redirectStatus,
      },
    });
  }
}
