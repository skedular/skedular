/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { DetailsResponse } from '../models/DetailsResponse';
import type { OSMType } from '../models/OSMType';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class DebugService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Show all details about a single place saved in the database
     * This API endpoint is meant for visual inspection of the data in the database, mainly together with [Nominatim UI](https://github.com/osm-search/nominatim-ui/). The parameters of the endpoint and the output may change occasionally between versions of Nominatim. Do not rely on the output in scripts or applications.
     * @param osmtype The type is required and it is one of node (N), way (W) or relation (R).
     * @param osmid The id is required and it must be a number.
     * @param _class The class parameter is optional and allows to distinguish between entries, when the corresponding OSM object has more than one main tag. For example, when a place is tagged with tourism=hotel and amenity=restaurant, there will be two place entries in Nominatim, one for a restaurant, one for a hotel. You need to specify class=tourism or class=amentity to get exactly the one you want. If there are multiple places in the database but the class parameter is left out, then one of the places will be chosen at random and displayed.
     * @param placeId Place IDs are assigned sequentially during Nominatim data import. The ID for a place is different between Nominatim installation (servers) and changes when data gets reimported. Therefore it cannot be used as a permanent id and shouldn't be used in bug reports.
     * @param format Format of response. See [Place Output Formats](https://nominatim.org/release-docs/develop/api/Output/) for details on each format. If not specified, it is equal to `json`.
     * @param jsonCallback Wrap JSON output in a callback function (JSONP) i.e. <string>(<json>). Only has an effect for JSON output formats.
     * @param addressdetails Include a breakdown of the address into elements. If not specified, it is equal to `0`.
     * @param keywords Include a list of name keywords and address keywords in the result. If not specified, it is equal to `0`.
     * @param linkedplaces Include details of places that are linked with this one. Places get linked together when they are different forms of the same physical object. Nominatim links two kinds of objects together: place nodes get linked with the corresponding administrative boundaries. Waterway relations get linked together with their members. If not specified, it is equal to `1`.
     * @param hierarchy Include details of places lower in the address hierarchy. If not specified, it is equal to `0`.
     * @param groupHierarchy The output of the address hierarchy will be grouped by type. If not specified, it is equal to `0`.
     * @param polygonGeojson Include geometry of result as a GeoJSON. If not specified, it is equal to `0`.
     * @param acceptLanguage Preferred language order for showing search results. This may either be a simple comma-separated list of language codes or have the same format as the [Accept-Language HTTP header](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Accept-Language). If not specified, it is equal to the content of Accept-Language HTTP header (browsers send the currently chosen browser language, command-line tools usually don't send any Accept-Language header).
     * @returns DetailsResponse OK
     * @throws ApiError
     */
    public details(
        osmtype: OSMType,
        osmid: number,
        _class?: string,
        placeId?: number,
        format: 'json' = 'json',
        jsonCallback?: string,
        addressdetails: number = 1,
        keywords?: number,
        linkedplaces: number = 1,
        hierarchy?: number,
        groupHierarchy?: number,
        polygonGeojson?: number,
        acceptLanguage?: string,
    ): CancelablePromise<DetailsResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/details',
            query: {
                'osmtype': osmtype,
                'osmid': osmid,
                'class': _class,
                'place_id': placeId,
                'format': format,
                'json_callback': jsonCallback,
                'addressdetails': addressdetails,
                'keywords': keywords,
                'linkedplaces': linkedplaces,
                'hierarchy': hierarchy,
                'group_hierarchy': groupHierarchy,
                'polygon_geojson': polygonGeojson,
                'accept-language': acceptLanguage,
            },
            errors: {
                404: `Not found`,
            },
        });
    }
    /**
     * List objects that have been deleted in OSM but are held back in Nominatim in case the deletion was accidental
     * Redirect to https://nominatim.openstreetmap.org/ui/deletable.html.
     * @returns void
     * @throws ApiError
     */
    public deletable(): CancelablePromise<void> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/deletable',
            errors: {
                302: `Redirect`,
            },
        });
    }
    /**
     * List of broken polygons detected by Nominatim
     * Redirect to https://nominatim.openstreetmap.org/ui/polygons.html.
     * @returns void
     * @throws ApiError
     */
    public polygons(): CancelablePromise<void> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/polygons',
            errors: {
                302: `Redirect`,
            },
        });
    }
}
