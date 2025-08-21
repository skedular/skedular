/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OSMGeocodeJson } from '../models/OSMGeocodeJson';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class LookupService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Query the address and other details of one or multiple OSM objects like node, way or relation
     * The lookup API allows to query the address and other details of one or multiple OSM objects like node, way or relation.
     * @param osmIds Comma-separated list of OSM ids each prefixed with its type, one of node (N), way (W) or relation (R). Up to 50 ids can be queried at the same time.
     * @param format Format of response. See [Place Output Formats](https://nominatim.org/release-docs/develop/api/Output/) for details on each format. If not specified, it is equal to `jsonv2`.
     * @param jsonCallback Wrap JSON output in a callback function (JSONP) i.e. <string>(<json>). Only has an effect for JSON output formats.
     * @param addressdetails Include a breakdown of the address into elements. If not specified, it is equal to `0`.
     * @param extratags Include additional information in the result if available, e.g. wikipedia link, opening hours. If not specified, it is equal to `0`.
     * @param namedetails Include a full list of names for the result. These may include language variants, older names, references and brand. If not specified, it is equal to `0`.
     * @param acceptLanguage Preferred language order for showing search results. This may either be a simple comma-separated list of language codes or have the same format as the [Accept-Language HTTP header](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Accept-Language). If not specified, it is equal to the content of Accept-Language HTTP header (browsers send the currently chosen browser language, command-line tools usually don't send any Accept-Language header).
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
    public lookup(
        osmIds: string,
        format: 'xml' | 'json' | 'jsonv2' | 'geojson' | 'geocodejson' = 'geocodejson',
        jsonCallback?: string,
        addressdetails: number = 1,
        extratags?: number,
        namedetails?: number,
        acceptLanguage?: string,
        polygonGeojson?: number,
        polygonKml?: number,
        polygonSvg?: number,
        polygonText?: number,
        polygonThreshold?: number,
        email?: string,
        debug?: number,
    ): CancelablePromise<Array<OSMGeocodeJson>> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/lookup',
            query: {
                'osm_ids': osmIds,
                'format': format,
                'json_callback': jsonCallback,
                'addressdetails': addressdetails,
                'extratags': extratags,
                'namedetails': namedetails,
                'accept-language': acceptLanguage,
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
