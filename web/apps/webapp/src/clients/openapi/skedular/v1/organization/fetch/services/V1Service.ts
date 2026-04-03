/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProblemDetails } from '../models/ProblemDetails';
import type { Version } from '../models/Version';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class V1Service {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * return API version
     * @returns Version the version of the API
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public getVersion(): CancelablePromise<Version | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/version',
        });
    }
    /**
     * raise graphql change
     * @param topicName
     * @param id
     * @param xApiKey API Key
     * @returns any the result of raising the graphql change
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public raiseGraphqlChange(
        topicName: string,
        id: string,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/raiseGraphqlChange/{topicName}/{id}',
            path: {
                'topicName': topicName,
                'id': id,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
    /**
     * republish organization
     * @param organizationId
     * @returns any the status of organization republishing
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republish(
        organizationId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/{organizationId}/republish',
            path: {
                'organizationId': organizationId,
            },
        });
    }
    /**
     * republish all organizations
     * @returns any the status of organization republishing
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public republishAll(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/republish-all',
        });
    }
    /**
     * regenerate all offerings
     * @returns any the status of regenrating all offerings
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateAllOfferings(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/regenerate-all-offerings',
        });
    }
    /**
     * rerun all offerings workflows
     * @returns any the status of running all offerings workflows
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public rerunAllOfferingsWorkflows(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/rerun-all-offerings-workflows',
        });
    }
    /**
     * change organization offering
     * @param organizationId
     * @param offeringCode
     * @param xApiKey API Key
     * @returns any the status of changing organization offering
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public changeOrganizationOffering(
        organizationId: string,
        offeringCode: string,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/{organizationId}/offering/{offeringCode}',
            path: {
                'organizationId': organizationId,
                'offeringCode': offeringCode,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
    /**
     * verify organization ownership
     * @param organizationId
     * @param xApiKey API Key
     * @returns any the status of verifying organization ownership
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public verifyOrganizationOwnership(
        organizationId: string,
        xApiKey: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/{organizationId}/ownership/verify',
            path: {
                'organizationId': organizationId,
            },
            headers: {
                'X-API-Key': xApiKey,
            },
        });
    }
    /**
     * generate an admin consent Url for the given tenant
     * @returns void
     * @throws ApiError
     */
    public azureTenantAdminConsentUrl(): CancelablePromise<void> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/azure-tenant-admin-consent-url',
            errors: {
                302: `redirect status response code`,
            },
        });
    }
    /**
     * OnBoard a tenant
     * @param tenant
     * @param adminConsent
     * @param state
     * @param error
     * @param errorDescription
     * @returns any onboarding response
     * @throws ApiError
     */
    public onboardAzureTenant(
        tenant: string,
        adminConsent: boolean,
        state: string,
        error?: string,
        errorDescription?: string,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/onboard-azure-tenant',
            query: {
                'tenant': tenant,
                'admin_consent': adminConsent,
                'state': state,
                'error': error,
                'error_description': errorDescription,
            },
        });
    }
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
    /**
     * add payment method
     * @param setupIntent
     * @param setupIntentClientSecret
     * @param redirectStatus
     * @returns any the readiness status
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public addPaymentMethod(
        setupIntent: string,
        setupIntentClientSecret: string,
        redirectStatus: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/add-payment-method',
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
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public refreshOrganizationStripeConnectAccountOnboarding(
        code: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/organization-stripe-connect-account/refresh-onboarding-url',
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
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public processStripePlatformAccountEvent(
        stripeSignature?: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/organization/stripe/platform/account/webhook',
            headers: {
                'Stripe-Signature': stripeSignature,
            },
        });
    }
    /**
     * Stripe Connect Account Webhook
     * @param stripeSignature Stripe webhook signature
     * @returns any the status of processing the Stripe Connect Account event
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public processStripeConnectAccountEvent(
        stripeSignature?: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/organization/stripe/connect/account/webhook',
            headers: {
                'Stripe-Signature': stripeSignature,
            },
        });
    }
    /**
     * Stripe Connect Account OAuth Callback
     * @param code An authorization code you can use in the next call to get an access token for your user. This can only be used once and expires in 5 minutes.
     * @param scope read_write or read_only, depending what you passed on the initial GET request.
     * @param state The value of the state parameter you provided on the initial GET request.
     * @returns any the status of processing the Stripe Connect Account event
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public stripeConnectAccountOAuthCallback(
        code: string,
        scope: string,
        state: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/stripe/connect/account/oauth/callback',
            query: {
                'code': code,
                'scope': scope,
                'state': state,
            },
        });
    }
    /**
     * Xero OAuth start
     * @param organizationId
     * @param organizationCustomDomain
     * @returns void
     * @throws ApiError
     */
    public startXeroOAuth(
        organizationId?: string,
        organizationCustomDomain?: string,
    ): CancelablePromise<void> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/xero/oauth/start',
            query: {
                'organizationId': organizationId,
                'organizationCustomDomain': organizationCustomDomain,
            },
            errors: {
                302: `redirect to Xero consent page`,
            },
        });
    }
    /**
     * Xero OAuth callback
     * @param code
     * @param state
     * @returns void
     * @throws ApiError
     */
    public xeroOAuthCallback(
        code: string,
        state: string,
    ): CancelablePromise<void> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/organization/xero/oauth/callback',
            query: {
                'code': code,
                'state': state,
            },
            errors: {
                302: `redirect back to organization marketplace setup`,
            },
        });
    }
    /**
     * resync all azure tenants
     * @returns any the status of resyncing all azure tenants
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public reSyncAllAzureTenants(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/azure-tenant/resync-all-azure-tenants',
        });
    }
    /**
     * resync azure tenant
     * @param tenantId
     * @returns any the status of resyncing azure tenant
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public reSyncAzureTenant(
        tenantId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/azure-tenant/{tenantId}/resync',
            path: {
                'tenantId': tenantId,
            },
        });
    }
    /**
     * regenerate all organizations daily analytics
     * @returns any the status of regenerating all organizations daily analytics
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateAllDailyAnalytics(): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/analytics/regenerate-all-daily-analytics',
        });
    }
    /**
     * regenerate organization daily analytics
     * @param organizationId
     * @returns any the status of regenerating organization daily analytics
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public regenerateDailyAnalytics(
        organizationId: string,
    ): CancelablePromise<any | ProblemDetails> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/organization/analytics/{organizationId}/regenerate-daily-analytics',
            path: {
                'organizationId': organizationId,
            },
        });
    }
}
