/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BaseHttpRequest } from './core/BaseHttpRequest';
import type { OpenAPIConfig } from './core/OpenAPI';
import { FetchHttpRequest } from './core/FetchHttpRequest';
import { OnboardingService } from './services/OnboardingService';
import { OrganizationService } from './services/OrganizationService';
import { OrganizationStripeConnectAccountsService } from './services/OrganizationStripeConnectAccountsService';
import { PaymentService } from './services/PaymentService';
import { PaymentMethodService } from './services/PaymentMethodService';
type HttpRequestConstructor = new (config: OpenAPIConfig) => BaseHttpRequest;
export class SkedularBillingClient {
    public readonly onboarding: OnboardingService;
    public readonly organization: OrganizationService;
    public readonly organizationStripeConnectAccounts: OrganizationStripeConnectAccountsService;
    public readonly payment: PaymentService;
    public readonly paymentMethod: PaymentMethodService;
    public readonly request: BaseHttpRequest;
    constructor(config?: Partial<OpenAPIConfig>, HttpRequest: HttpRequestConstructor = FetchHttpRequest) {
        this.request = new HttpRequest({
            BASE: config?.BASE ?? '',
            VERSION: config?.VERSION ?? '1.0.0',
            WITH_CREDENTIALS: config?.WITH_CREDENTIALS ?? false,
            CREDENTIALS: config?.CREDENTIALS ?? 'include',
            TOKEN: config?.TOKEN,
            USERNAME: config?.USERNAME,
            PASSWORD: config?.PASSWORD,
            HEADERS: config?.HEADERS,
            ENCODE_PATH: config?.ENCODE_PATH,
        });
        this.onboarding = new OnboardingService(this.request);
        this.organization = new OrganizationService(this.request);
        this.organizationStripeConnectAccounts = new OrganizationStripeConnectAccountsService(this.request);
        this.payment = new PaymentService(this.request);
        this.paymentMethod = new PaymentMethodService(this.request);
    }
}

