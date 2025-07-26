/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BaseHttpRequest } from './core/BaseHttpRequest';
import type { OpenAPIConfig } from './core/OpenAPI';
import { FetchHttpRequest } from './core/FetchHttpRequest';
import { AzureService } from './services/AzureService';
import { ConnectService } from './services/ConnectService';
import { OauthService } from './services/OauthService';
import { OfferingService } from './services/OfferingService';
import { OnboardingService } from './services/OnboardingService';
import { OrganizationService } from './services/OrganizationService';
import { OrganizationStripeConnectAccountsService } from './services/OrganizationStripeConnectAccountsService';
import { PaymentMethodService } from './services/PaymentMethodService';
import { PlatformService } from './services/PlatformService';
import { SamlService } from './services/SamlService';
import { SsoService } from './services/SsoService';
import { StripeService } from './services/StripeService';
import { TenantService } from './services/TenantService';
import { V1Service } from './services/V1Service';
import { WebhookService } from './services/WebhookService';
type HttpRequestConstructor = new (config: OpenAPIConfig) => BaseHttpRequest;
export class SkedularOrganizationV1Client {
    public readonly azure: AzureService;
    public readonly connect: ConnectService;
    public readonly oauth: OauthService;
    public readonly offering: OfferingService;
    public readonly onboarding: OnboardingService;
    public readonly organization: OrganizationService;
    public readonly organizationStripeConnectAccounts: OrganizationStripeConnectAccountsService;
    public readonly paymentMethod: PaymentMethodService;
    public readonly platform: PlatformService;
    public readonly saml: SamlService;
    public readonly sso: SsoService;
    public readonly stripe: StripeService;
    public readonly tenant: TenantService;
    public readonly v1: V1Service;
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
        this.azure = new AzureService(this.request);
        this.connect = new ConnectService(this.request);
        this.oauth = new OauthService(this.request);
        this.offering = new OfferingService(this.request);
        this.onboarding = new OnboardingService(this.request);
        this.organization = new OrganizationService(this.request);
        this.organizationStripeConnectAccounts = new OrganizationStripeConnectAccountsService(this.request);
        this.paymentMethod = new PaymentMethodService(this.request);
        this.platform = new PlatformService(this.request);
        this.saml = new SamlService(this.request);
        this.sso = new SsoService(this.request);
        this.stripe = new StripeService(this.request);
        this.tenant = new TenantService(this.request);
        this.v1 = new V1Service(this.request);
        this.webhook = new WebhookService(this.request);
    }
}

