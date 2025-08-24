import type { BaseHttpRequest } from '../core/BaseHttpRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { AddressJsonV2 } from '../models/json-v2';

export class SearchService {
  constructor(public readonly httpRequest: BaseHttpRequest) {}
  /**
   * Look up a location from a textual description or address
   * The search API allows you to look up a location from a textual description or address. Nominatim supports structured and free-form search queries. The search query may also contain special phrases which are translated into specific OpenStreetMap (OSM) tags. This can be used to narrow down the kind of objects to be returned.
   * @param q Free-form query string to search for. Free-form queries are processed first left-to-right and then right-to-left if that fails. Commas are optional, but improve performance by reducing the complexity of the search. The free-form may also contain special phrases to describe the type of place to be returned or a coordinate to search close to a position.
   * @param limit Limit the maximum number of returned results. Nominatim may decide to return less results than given, if additional results do not sufficiently match the query. If not specified, it is equal to `10`.
   * @returns AddressJsonV2[] OK
   * @throws ApiError
   */
  public searchFreeForm(q: string, options?: { limit?: number }): CancelablePromise<AddressJsonV2[]> {
    return this.httpRequest.request({
      method: 'GET',
      url: '/search',
      query: {
        q: q,
        format: 'jsonv2',
        limit: options?.limit,
        addressdetails: 1,
        dedupe: 1,
      },
    });
  }
}
