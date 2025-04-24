/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class PaymentService {
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
    public addOrganizationPaymentMethod(
        setupIntent: string,
        setupIntentClientSecret: string,
        redirectStatus: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/payment/api/v1/organization/add-payment-method',
            query: {
                'setup_intent': setupIntent,
                'setup_intent_client_secret': setupIntentClientSecret,
                'redirect_status': redirectStatus,
            },
        });
    }
    /**
     * return OrganizationStripeConnectAccount onboarding refresh URL
     * @param code
     * @returns any should never be returned
     * @returns Error unexpected error
     * @throws ApiError
     */
    public refreshOrganizationStripeConnectAccountOnboarding(
        code: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/payment/api/v1/organization-stripe-connect-account/refresh-onboarding-url',
            query: {
                'code': code,
            },
            errors: {
                302: `redirect to OrganizationStripeConnectAccounts new onboarding URL`,
            },
        });
    }
    /**
     * Stripe Platform Account Webhook
     * @param stripeSignature Stripe webhook signature
     * @returns any the status of processing the Stripe Platform Account event
     * @returns Error unexpected error
     * @throws ApiError
     */
    public processStripePlatformAccountEvent(
        stripeSignature?: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/payment/api/v1/stripe/platform/account/webhook',
            headers: {
                'Stripe-Signature': stripeSignature,
            },
        });
    }
    /**
     * Stripe Connect Account Webhook
     * @param stripeSignature Stripe webhook signature
     * @returns any the status of processing the Stripe Connect Account event
     * @returns Error unexpected error
     * @throws ApiError
     */
    public processStripeConnectAccountEvent(
        stripeSignature?: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/payment/api/v1/stripe/connect/account/webhook',
            headers: {
                'Stripe-Signature': stripeSignature,
            },
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
