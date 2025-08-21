/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BaseHttpRequest } from './core/BaseHttpRequest';
import type { OpenAPIConfig } from './core/OpenAPI';
import { FetchHttpRequest } from './core/FetchHttpRequest';
import { DebugService } from './services/DebugService';
import { LookupService } from './services/LookupService';
import { ReverseService } from './services/ReverseService';
import { SearchService } from './services/SearchService';
import { StatusService } from './services/StatusService';
type HttpRequestConstructor = new (config: OpenAPIConfig) => BaseHttpRequest;
export class NominatimV4Client {
    public readonly debug: DebugService;
    public readonly lookup: LookupService;
    public readonly reverse: ReverseService;
    public readonly search: SearchService;
    public readonly status: StatusService;
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
        this.debug = new DebugService(this.request);
        this.lookup = new LookupService(this.request);
        this.reverse = new ReverseService(this.request);
        this.search = new SearchService(this.request);
        this.status = new StatusService(this.request);
    }
}

