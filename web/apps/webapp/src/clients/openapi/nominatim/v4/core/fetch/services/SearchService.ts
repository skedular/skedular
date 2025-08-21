/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OSMGeocodeJson } from '../models/OSMGeocodeJson';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class SearchService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Look up a location from a textual description or address
     * The search API allows you to look up a location from a textual description or address. Nominatim supports structured and free-form search queries. The search query may also contain special phrases which are translated into specific OpenStreetMap (OSM) tags. This can be used to narrow down the kind of objects to be returned.
     * @param q Free-form query string to search for. Free-form queries are processed first left-to-right and then right-to-left if that fails. Commas are optional, but improve performance by reducing the complexity of the search. The free-form may also contain special phrases to describe the type of place to be returned or a coordinate to search close to a position.
     * @param amenity Name and/or type of POI. Do not combine with `q=<query>` parameter.
     * @param street Name of street with optional housenumber. Do not combine with `q=<query>` parameter.
     * @param city Name of city. Do not combine with `q=<query>` parameter.
     * @param county Name of county. Do not combine with `q=<query>` parameter.
     * @param state Name of state. Do not combine with `q=<query>` parameter.
     * @param country Name of country. Do not combine with `q=<query>` parameter.
     * @param postalcode Postal code. Do not combine with `q=<query>` parameter.
     * @param format Format of response. See [Place Output Formats](https://nominatim.org/release-docs/develop/api/Output/) for details on each format. If not specified, it is equal to `jsonv2`.
     * @param jsonCallback Wrap JSON output in a callback function (JSONP) i.e. <string>(<json>). Only has an effect for JSON output formats.
     * @param limit Limit the maximum number of returned results. Nominatim may decide to return less results than given, if additional results do not sufficiently match the query. If not specified, it is equal to `10`.
     * @param addressdetails Include a breakdown of the address into elements. If not specified, it is equal to `0`.
     * @param extratags Include additional information in the result if available, e.g. wikipedia link, opening hours. If not specified, it is equal to `0`.
     * @param namedetails Include a full list of names for the result. These may include language variants, older names, references and brand. If not specified, it is equal to `0`.
     * @param acceptLanguage Preferred language order for showing search results. This may either be a simple comma-separated list of language codes or have the same format as the [Accept-Language HTTP header](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Accept-Language). If not specified, it is equal to the content of Accept-Language HTTP header (browsers send the currently chosen browser language, command-line tools usually don't send any Accept-Language header).
     * @param countrycodes Limit search results to one or more countries. The country code must be the [ISO 3166-1alpha2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2) code of the country, e.g. gb for the United Kingdom, de for Germany. Each place in Nominatim is assigned to one country code based on OSM country boundaries. In rare cases a place may not be in any country at all, for example, when it is in international waters. These places are also excluded when the filter is set.
     * @param layer The layer filter allows to select places by themes, a comma-separated list of `address` (all places that make up an address: address points with house numbers, streets, inhabited places like suburbs, villages, cities, states, and administrative boundaries), `poi` (all points of interest like restaurants, shops, hotels but also less obvious features like recycling bins, guideposts or benches), `railway` (infrastructures like tracks), `natural` (feautures like rivers, lakes and mountains), `manmade` (catch-all for features not covered by the other layers).
     * @param featureType The featureType allows to have a more fine-grained selection for places from the address layer. Results can be restricted to places that make up the 'state', 'country' or 'city' part of an address. A featureType of settlement selects any human inhabited feature from 'state' down to 'neighbourhood'. If not specified, results are automatically restricted to the address layer.
     * @param excludePlaceIds If you do not want certain OSM objects to appear in the search result, give a comma separated list of the `place_ids` you want to skip. This can be used to retrieve additional search results. For example, if a previous query only returned a few results, then including those here would cause the search to return other, less accurate, matches (if possible).
     * @param viewbox The preferred area to find search results. Any two corner points of the box are accepted as long as they span a real box. In `viewbox=<x1>,<y1>,<x2>,<y2>` x is longitude, y is latitude.
     * @param bounded When a viewbox is given, restrict the result to items contained within that viewbox (see above). When `viewbox` and `bounded=1` are given, an amenity only search is allowed. Give the special keyword for the amenity in square brackets, e.g. `[pub]` and a selection of objects of this type is returned. There is no guarantee that the result is complete.
     * @param polygonGeojson Output geometry of results as a GeoJSON.
     * @param polygonKml Output geometry of results as a KML.
     * @param polygonSvg Output geometry of results as a SVG.
     * @param polygonText Output geometry of results as a WKT.
     * @param polygonThreshold When one of the polygon_* outputs is chosen, return a simplified version of the output geometry. The parameter is the tolerance in degrees with which the geometry may differ from the original geometry. Topology is preserved in the result.
     * @param email If you are making large numbers of request please include an appropriate email address to identify your requests. See Nominatim's [Usage Policy](https://operations.osmfoundation.org/policies/nominatim/) for more details.
     * @param dedupe Sometimes you have several objects in OSM identifying the same place or object in reality. The simplest case is a street being split into many different OSM ways due to different characteristics. Nominatim will attempt to detect such duplicates and only return one match unless this parameter is set to 0. If not specified, it is equal to `0`.
     * @param debug Output assorted developer debug information. Data on internals of Nominatim's Search Loop logic, and SQL queries. The output is (rough) HTML format. This overrides the specified machine readable format. If not specified, it is equal to `0`.
     * @returns OSMGeocodeJson OK
     * @throws ApiError
     */
    public search(
        q?: string,
        amenity?: string,
        street?: string,
        city?: string,
        county?: string,
        state?: string,
        country?: string,
        postalcode?: string,
        format: 'xml' | 'json' | 'jsonv2' | 'geojson' | 'geocodejson' = 'geocodejson',
        jsonCallback?: string,
        limit?: number,
        addressdetails: number = 1,
        extratags?: number,
        namedetails?: number,
        acceptLanguage?: string,
        countrycodes?: string,
        layer?: string,
        featureType?: 'country' | 'state' | 'city' | 'settlement',
        excludePlaceIds?: string,
        viewbox?: string,
        bounded?: number,
        polygonGeojson?: number,
        polygonKml?: number,
        polygonSvg?: number,
        polygonText?: number,
        polygonThreshold?: number,
        email?: string,
        dedupe: number = 1,
        debug?: number,
    ): CancelablePromise<OSMGeocodeJson> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/search',
            query: {
                'q': q,
                'amenity': amenity,
                'street': street,
                'city': city,
                'county': county,
                'state': state,
                'country': country,
                'postalcode': postalcode,
                'format': format,
                'json_callback': jsonCallback,
                'limit': limit,
                'addressdetails': addressdetails,
                'extratags': extratags,
                'namedetails': namedetails,
                'accept-language': acceptLanguage,
                'countrycodes': countrycodes,
                'layer': layer,
                'featureType': featureType,
                'exclude_place_ids': excludePlaceIds,
                'viewbox': viewbox,
                'bounded': bounded,
                'polygon_geojson': polygonGeojson,
                'polygon_kml': polygonKml,
                'polygon_svg': polygonSvg,
                'polygon_text': polygonText,
                'polygon_threshold': polygonThreshold,
                'email': email,
                'dedupe': dedupe,
                'debug': debug,
            },
        });
    }
}
