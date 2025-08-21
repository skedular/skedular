/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export type OSMGeocodeJson = ((({
    type: OSMGeocodeJson.type;
    coordinates: Array<number>;
    bbox?: Array<number>;
} | {
    type: OSMGeocodeJson.type;
    coordinates: Array<Array<number>>;
    bbox?: Array<number>;
} | {
    type: OSMGeocodeJson.type;
    coordinates: Array<Array<Array<number>>>;
    bbox?: Array<number>;
} | {
    type: OSMGeocodeJson.type;
    coordinates: Array<Array<Array<Array<number>>>>;
    bbox?: Array<number>;
} | {
    type: OSMGeocodeJson.type;
    geometries: Array<({
        type: 'Point';
        coordinates: Array<number>;
        bbox?: Array<number>;
    } | {
        type: 'LineString';
        coordinates: Array<Array<number>>;
        bbox?: Array<number>;
    } | {
        type: 'Polygon';
        coordinates: Array<Array<Array<number>>>;
        bbox?: Array<number>;
    } | {
        type: 'MultiPoint';
        coordinates: Array<Array<number>>;
        bbox?: Array<number>;
    } | {
        type: 'MultiLineString';
        coordinates: Array<Array<Array<number>>>;
        bbox?: Array<number>;
    } | {
        type: 'MultiPolygon';
        coordinates: Array<Array<Array<Array<number>>>>;
        bbox?: Array<number>;
    })>;
    bbox?: Array<number>;
} | {
    type: OSMGeocodeJson.type;
    id?: (number | string);
    properties: (null | Record<string, any>);
    geometry: (null | {
        type: OSMGeocodeJson.type;
        coordinates: Array<number>;
        bbox?: Array<number>;
    } | {
        type: OSMGeocodeJson.type;
        coordinates: Array<Array<number>>;
        bbox?: Array<number>;
    } | {
        type: OSMGeocodeJson.type;
        coordinates: Array<Array<Array<number>>>;
        bbox?: Array<number>;
    } | {
        type: OSMGeocodeJson.type;
        coordinates: Array<Array<Array<Array<number>>>>;
        bbox?: Array<number>;
    } | {
        type: OSMGeocodeJson.type;
        geometries: Array<({
            type: 'Point';
            coordinates: Array<number>;
            bbox?: Array<number>;
        } | {
            type: 'LineString';
            coordinates: Array<Array<number>>;
            bbox?: Array<number>;
        } | {
            type: 'Polygon';
            coordinates: Array<Array<Array<number>>>;
            bbox?: Array<number>;
        } | {
            type: 'MultiPoint';
            coordinates: Array<Array<number>>;
            bbox?: Array<number>;
        } | {
            type: 'MultiLineString';
            coordinates: Array<Array<Array<number>>>;
            bbox?: Array<number>;
        } | {
            type: 'MultiPolygon';
            coordinates: Array<Array<Array<Array<number>>>>;
            bbox?: Array<number>;
        })>;
        bbox?: Array<number>;
    });
    bbox?: Array<number>;
} | {
    type: OSMGeocodeJson.type;
    features: Array<{
        type: 'Feature';
        id?: (number | string);
        properties: (null | Record<string, any>);
        geometry: (null | {
            type: 'Point';
            coordinates: Array<number>;
            bbox?: Array<number>;
        } | {
            type: 'LineString';
            coordinates: Array<Array<number>>;
            bbox?: Array<number>;
        } | {
            type: 'Polygon';
            coordinates: Array<Array<Array<number>>>;
            bbox?: Array<number>;
        } | {
            type: 'MultiPoint';
            coordinates: Array<Array<number>>;
            bbox?: Array<number>;
        } | {
            type: 'MultiLineString';
            coordinates: Array<Array<Array<number>>>;
            bbox?: Array<number>;
        } | {
            type: 'MultiPolygon';
            coordinates: Array<Array<Array<Array<number>>>>;
            bbox?: Array<number>;
        } | {
            type: 'GeometryCollection';
            geometries: Array<({
                type: 'Point';
                coordinates: Array<number>;
                bbox?: Array<number>;
            } | {
                type: 'LineString';
                coordinates: Array<Array<number>>;
                bbox?: Array<number>;
            } | {
                type: 'Polygon';
                coordinates: Array<Array<Array<number>>>;
                bbox?: Array<number>;
            } | {
                type: 'MultiPoint';
                coordinates: Array<Array<number>>;
                bbox?: Array<number>;
            } | {
                type: 'MultiLineString';
                coordinates: Array<Array<Array<number>>>;
                bbox?: Array<number>;
            } | {
                type: 'MultiPolygon';
                coordinates: Array<Array<Array<Array<number>>>>;
                bbox?: Array<number>;
            })>;
            bbox?: Array<number>;
        });
        bbox?: Array<number>;
    }>;
    bbox?: Array<number>;
}) & {
    /**
     * REQUIRED. GeocodeJSON result is a FeatureCollection.
     */
    type?: any;
    /**
     * REQUIRED. Namespace.
     */
    geocoding: {
        /**
         * A semver.org compliant version number. Describes the version of the GeocodeJSON spec that is implemented by this instance.
         */
        version: string;
        /**
         * OPTIONAL. The licence of the data. In case of multiple sources, and then multiple licences, can be an object with one key by source.
         */
        licence?: string;
        /**
         * OPTIONAL. The attribution of the data. In case of multiple sources, and then multiple attributions, can be an object with one key by source.
         */
        attribution?: string;
        /**
         * OPTIONAL. The query that has been issued to trigger the search.
         */
        query?: string;
    };
} & {
    /**
     * REQUIRED. As per GeoJSON spec.
     */
    features?: Array<{
        /**
         * REQUIRED. As per GeoJSON spec.
         */
        type?: any;
        /**
         * REQUIRED. As per GeoJSON spec.
         */
        properties?: {
            /**
             * REQUIRED. Namespace.
             */
            geocoding: {
                /**
                 * REQUIRED. One of house, street, locality, city, region, country.
                 */
                type: string;
                /**
                 * OPTIONAL. Result accuracy, in meters.
                 */
                accuracy?: number;
                /**
                 * RECOMMENDED. Suggested label for the result.
                 */
                label?: string;
                /**
                 * OPTIONAL. Name of the place.
                 */
                name?: string;
                /**
                 * OPTIONAL. Housenumber of the place.
                 */
                housenumber?: string;
                /**
                 * OPTIONAL. Street of the place.
                 */
                street?: string;
                /**
                 * OPTIONAL. Locality of the place.
                 */
                locality?: string;
                /**
                 * OPTIONAL. Postcode of the place.
                 */
                postcode?: string;
                /**
                 * OPTIONAL. City of the place.
                 */
                city?: string;
                /**
                 * OPTIONAL. District of the place.
                 */
                district?: string;
                /**
                 * OPTIONAL. County of the place.
                 */
                county?: string;
                /**
                 * OPTIONAL. State of the place.
                 */
                state?: string;
                /**
                 * OPTIONAL. Country of the place.
                 */
                country?: string;
                /**
                 * OPTIONAL. Administratives boundaries the feature is included in, as defined in http://wiki.osm.org/wiki/Key:admin_level#admin_level.
                 */
                admin?: Record<string, any>;
                /**
                 * OPTIONAL. Geohash encoding of coordinates (see http://geohash.org/site/tips.html).
                 */
                geohash?: string;
            };
        };
    }>;
}) & {
    features?: Array<{
        properties?: {
            geocoding?: {
                osm_type?: string;
                osm_id?: number;
                osm_key?: string;
                osm_value?: string;
            };
        };
    }>;
});
export namespace OSMGeocodeJson {
    export enum type {
        POINT = 'Point',
    }
}

