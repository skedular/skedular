/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BaseHttpRequest } from './core/BaseHttpRequest';
import type { OpenAPIConfig } from './core/OpenAPI';
import { FetchHttpRequest } from './core/FetchHttpRequest';
import { BookingService } from './services/BookingService';
import { ConnectService } from './services/ConnectService';
import { PlatformService } from './services/PlatformService';
import { StripeService } from './services/StripeService';
import { V1Service } from './services/V1Service';
import { WebhookService } from './services/WebhookService';
type HttpRequestConstructor = new (config: OpenAPIConfig) => BaseHttpRequest;
export class SkedularBookingStripeWebhookV1Client {
    public readonly booking: BookingService;
    public readonly connect: ConnectService;
    public readonly platform: PlatformService;
    public readonly stripe: StripeService;
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
        this.booking = new BookingService(this.request);
        this.connect = new ConnectService(this.request);
        this.platform = new PlatformService(this.request);
        this.stripe = new StripeService(this.request);
        this.v1 = new V1Service(this.request);
        this.webhook = new WebhookService(this.request);
    }
}

