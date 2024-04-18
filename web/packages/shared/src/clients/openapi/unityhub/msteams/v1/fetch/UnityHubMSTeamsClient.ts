/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */

import type { BaseHttpRequest } from './core/BaseHttpRequest';
import type { OpenAPIConfig } from './core/OpenAPI';
import { FetchHttpRequest } from './core/FetchHttpRequest';
import { GenerateATemporaryAuthorizationCodeService } from './services/GenerateATemporaryAuthorizationCodeService';
import { MessagingService } from './services/MessagingService';
import { MsteamsService } from './services/MsteamsService';
import { OnboardATenantService } from './services/OnboardATenantService';
import { TeantService } from './services/TeantService';
type HttpRequestConstructor = new (config: OpenAPIConfig) => BaseHttpRequest;
export class UnityHubMSTeamsClient {
  public readonly generateATemporaryAuthorizationCode: GenerateATemporaryAuthorizationCodeService;
  public readonly messaging: MessagingService;
  public readonly msteams: MsteamsService;
  public readonly onboardATenant: OnboardATenantService;
  public readonly teant: TeantService;
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
    this.generateATemporaryAuthorizationCode = new GenerateATemporaryAuthorizationCodeService(this.request);
    this.messaging = new MessagingService(this.request);
    this.msteams = new MsteamsService(this.request);
    this.onboardATenant = new OnboardATenantService(this.request);
    this.teant = new TeantService(this.request);
  }
}
