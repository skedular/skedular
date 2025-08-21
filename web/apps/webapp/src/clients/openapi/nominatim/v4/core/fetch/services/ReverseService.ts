/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OSMGeocodeJson } from '../models/OSMGeocodeJson';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class ReverseService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Generate an address from a coordinate given as latitude and longitude
     * The reverse geocoding API returns exactly one result or an error when the coordinate is in an area with no OSM data coverage. It does not exactly compute the address for the coordinate it receives. It works by finding the closest suitable OSM object and returning its address information. This may occasionally lead to unexpected results. First of all, Nominatim only includes OSM objects in its index that are suitable for searching. Small, unnamed paths for example are missing from the database and can therefore not be used for reverse geocoding either. The other issue to be aware of is that the closest OSM object may not always have a similar enough address to the coordinate you were requesting. For example, in dense city areas it may belong to a completely different street.
     * @param lat Latitude of a coordinate in WGS84 projection.
     * @param lon Longitude of a coordinate in WGS84 projection.
     * @param format Format of response. See [Place Output Formats](https://nominatim.org/release-docs/develop/api/Output/) for details on each format. If not specified, it is equal to `xml`.
     * @param jsonCallback Wrap JSON output in a callback function (JSONP) i.e. <string>(<json>). Only has an effect for JSON output formats.
     * @param addressdetails Include a breakdown of the address into elements. If not specified, it is equal to `1`.
     * @param extratags Include additional information in the result if available, e.g. wikipedia link, opening hours. If not specified, it is equal to `0`.
     * @param namedetails Include a full list of names for the result. These may include language variants, older names, references and brand. If not specified, it is equal to `0`.
     * @param acceptLanguage Preferred language order for showing search results. This may either be a simple comma-separated list of language codes or have the same format as the [Accept-Language HTTP header](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Accept-Language). If not specified, it is equal to the content of Accept-Language HTTP header (browsers send the currently chosen browser language, command-line tools usually don't send any Accept-Language header).
     * @param zoom Level of detail required for the address. This is a number that corresponds roughly to the zoom level used in XYZ tile sources in frameworks like Leaflet.js, Openlayers etc. In terms of address details the zoom levels are as follows: 3) country, 5) state, 8) county, 10) city, 12) town / borough, 13) village / suburb, 14) neighbourhood, 15) any settlement, 16) major streets, 17) major and minor streets, 18) building.
     * @param layer The layer filter allows to select places by themes, a comma-separated list of `address` (all places that make up an address: address points with house numbers, streets, inhabited places like suburbs, villages, cities, states, and administrative boundaries), `poi` (all points of interest like restaurants, shops, hotels but also less obvious features like recycling bins, guideposts or benches), `railway` (infrastructures like tracks), `natural` (feautures like rivers, lakes and mountains), `manmade` (catch-all for features not covered by the other layers).
     * @param polygonGeojson Output geometry of results as a GeoJSON.
     * @param polygonKml Output geometry of results as a KML.
     * @param polygonSvg Output geometry of results as a SVG.
     * @param polygonText Output geometry of results as a WKT.
     * @param polygonThreshold When one of the polygon_* outputs is chosen, return a simplified version of the output geometry. The parameter is the tolerance in degrees with which the geometry may differ from the original geometry. Topology is preserved in the result.
     * @param email If you are making large numbers of request please include an appropriate email address to identify your requests. See Nominatim's [Usage Policy](https://operations.osmfoundation.org/policies/nominatim/) for more details.
     * @param debug Output assorted developer debug information. Data on internals of Nominatim's Search Loop logic, and SQL queries. The output is (rough) HTML format. This overrides the specified machine readable format.
     * @returns OSMGeocodeJson OK
     * @throws ApiError
     */
    public reverse(
        lat?: number,
        lon?: number,
        format: 'xml' | 'json' | 'jsonv2' | 'geojson' | 'geocodejson' = 'geocodejson',
        jsonCallback?: string,
        addressdetails: number = 1,
        extratags?: number,
        namedetails?: number,
        acceptLanguage?: string,
        zoom: number = 18,
        layer?: string,
        polygonGeojson?: number,
        polygonKml?: number,
        polygonSvg?: number,
        polygonText?: number,
        polygonThreshold?: number,
        email?: string,
        debug?: number,
    ): CancelablePromise<OSMGeocodeJson> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/reverse',
            query: {
                'lat': lat,
                'lon': lon,
                'format': format,
                'json_callback': jsonCallback,
                'addressdetails': addressdetails,
                'extratags': extratags,
                'namedetails': namedetails,
                'accept-language': acceptLanguage,
                'zoom': zoom,
                'layer': layer,
                'polygon_geojson': polygonGeojson,
                'polygon_kml': polygonKml,
                'polygon_svg': polygonSvg,
                'polygon_text': polygonText,
                'polygon_threshold': polygonThreshold,
                'email': email,
                'debug': debug,
            },
        });
    }
}
