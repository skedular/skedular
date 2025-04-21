/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class OrganizationStripeConnectAccountsService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * return OrganizationStripeConnectAccount onboarding refresh URL
     * @param id
     * @returns any should never be returned
     * @returns Error unexpected error
     * @throws ApiError
     */
    public refreshOrganizationStripeConnectAccountOnboarding(
        id: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/payment/api/v1/organization-stripe-connect-account/{id}/refresh-onboarding-url',
            path: {
                'id': id,
            },
            errors: {
                302: `redirect to OrganizationStripeConnectAccounts new onboarding URL`,
            },
        });
    }
    /**
     * complete OrganizationStripeConnectAccount onboarding
     * @param xStripeSignature Stripe webhook signature
     * @param requestBody raw JSON string
     * @returns any the status of OrganizationStripeConnectAccounts onboarding completed
     * @returns Error unexpected error
     * @throws ApiError
     */
    public organizationStripeConnectAccountOnboardingCompleted(
        xStripeSignature?: string,
        requestBody?: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/payment/api/v1/organization-stripe-connect-account/onboarding-completed',
            headers: {
                'x-stripe-signature': xStripeSignature,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * republish all OrganizationStripeConnectAccounts
     * @returns any the status of republishing all OrganizationStripeConnectAccounts
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishAllOrganizationStripeConnectAccounts(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/payment/api/v1/organization-stripe-connect-account/republish-all',
        });
    }
}
