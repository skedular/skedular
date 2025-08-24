import type { BaseHttpRequest } from './core/BaseHttpRequest';
import { FetchHttpRequest } from './core/FetchHttpRequest';
import type { OpenAPIConfig } from './core/OpenAPI';
import { SearchService } from './services/SearchService';

type HttpRequestConstructor = new (config: OpenAPIConfig) => BaseHttpRequest;

export class NominatimV4Client {
  public readonly search: SearchService;
  public readonly request: BaseHttpRequest;
  constructor(config?: Partial<OpenAPIConfig>, HttpRequest: HttpRequestConstructor = FetchHttpRequest) {
    this.request = new HttpRequest({
      BASE: config?.BASE ?? 'https://nominatim.openstreetmap.org',
      VERSION: config?.VERSION ?? '4.3.2',
      WITH_CREDENTIALS: config?.WITH_CREDENTIALS ?? false,
      CREDENTIALS: config?.CREDENTIALS ?? 'include',
      TOKEN: config?.TOKEN,
      USERNAME: config?.USERNAME,
      PASSWORD: config?.PASSWORD,
      HEADERS: config?.HEADERS,
      ENCODE_PATH: config?.ENCODE_PATH,
    });
    this.search = new SearchService(this.request);
  }
}
