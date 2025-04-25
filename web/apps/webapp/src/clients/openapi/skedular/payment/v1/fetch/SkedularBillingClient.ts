/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BaseHttpRequest } from './core/BaseHttpRequest';
import type { OpenAPIConfig } from './core/OpenAPI';
import { FetchHttpRequest } from './core/FetchHttpRequest';
import { ConnectService } from './services/ConnectService';
import { CustomerService } from './services/CustomerService';
import { OnboardingService } from './services/OnboardingService';
import { OrganizationService } from './services/OrganizationService';
import { OrganizationStripeConnectAccountsService } from './services/OrganizationStripeConnectAccountsService';
import { PaymentService } from './services/PaymentService';
import { PaymentMethodService } from './services/PaymentMethodService';
import { PlatformService } from './services/PlatformService';
import { StripeService } from './services/StripeService';
import { WebhookService } from './services/WebhookService';
type HttpRequestConstructor = new (config: OpenAPIConfig) => BaseHttpRequest;
export class SkedularBillingClient {
    public readonly connect: ConnectService;
    public readonly customer: CustomerService;
    public readonly onboarding: OnboardingService;
    public readonly organization: OrganizationService;
    public readonly organizationStripeConnectAccounts: OrganizationStripeConnectAccountsService;
    public readonly payment: PaymentService;
    public readonly paymentMethod: PaymentMethodService;
    public readonly platform: PlatformService;
    public readonly stripe: StripeService;
    public readonly webhook: WebhookService;
    public readonly request: BaseHttpRequest;
    constructor(config?: Partial<OpenAPIConfig>, HttpRequest: HttpRequestConstructor = FetchHttpRequest) {
        this.request = new HttpRequest({
            BASE: config?.BASE ?? 'https://api.skedular.app',
            VERSION: config?.VERSION ?? '1.0.0',
            WITH_CREDENTIALS: config?.WITH_CREDENTIALS ?? false,
            CREDENTIALS: config?.CREDENTIALS ?? 'include',
            TOKEN: config?.TOKEN,
            USERNAME: config?.USERNAME,
            PASSWORD: config?.PASSWORD,
            HEADERS: config?.HEADERS,
            ENCODE_PATH: config?.ENCODE_PATH,
        });
        this.connect = new ConnectService(this.request);
        this.customer = new CustomerService(this.request);
        this.onboarding = new OnboardingService(this.request);
        this.organization = new OrganizationService(this.request);
        this.organizationStripeConnectAccounts = new OrganizationStripeConnectAccountsService(this.request);
        this.payment = new PaymentService(this.request);
        this.paymentMethod = new PaymentMethodService(this.request);
        this.platform = new PlatformService(this.request);
        this.stripe = new StripeService(this.request);
        this.webhook = new WebhookService(this.request);
    }
}

