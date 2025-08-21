/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OSMType } from './OSMType';
export type DetailsResponse = {
    place_id?: number;
    parent_place_id?: number;
    osm_type?: OSMType;
    osm_id?: number;
    category?: string;
    type?: string;
    admin_level?: number;
    localname?: string;
    names?: {
        name?: string;
    };
    addresstags?: Record<string, any>;
    calculated_postcode?: string;
    country_code?: string;
    indexed_date?: string;
    importance?: number;
    calculated_importance?: number;
    extratags?: Record<string, any>;
    rank_address?: number;
    rank_search?: number;
    isarea?: boolean;
    centroid?: {
        type: DetailsResponse.type;
        coordinates: Array<number>;
        bbox?: Array<number>;
    };
    geometry?: ({
        type: DetailsResponse.type;
        coordinates: Array<number>;
        bbox?: Array<number>;
    } | {
        type: DetailsResponse.type;
        coordinates: Array<Array<number>>;
        bbox?: Array<number>;
    } | {
        type: DetailsResponse.type;
        coordinates: Array<Array<Array<number>>>;
        bbox?: Array<number>;
    } | {
        type: DetailsResponse.type;
        coordinates: Array<Array<Array<Array<number>>>>;
        bbox?: Array<number>;
    } | {
        type: DetailsResponse.type;
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
        type: DetailsResponse.type;
        id?: (number | string);
        properties: (null | Record<string, any>);
        geometry: (null | {
            type: DetailsResponse.type;
            coordinates: Array<number>;
            bbox?: Array<number>;
        } | {
            type: DetailsResponse.type;
            coordinates: Array<Array<number>>;
            bbox?: Array<number>;
        } | {
            type: DetailsResponse.type;
            coordinates: Array<Array<Array<number>>>;
            bbox?: Array<number>;
        } | {
            type: DetailsResponse.type;
            coordinates: Array<Array<Array<Array<number>>>>;
            bbox?: Array<number>;
        } | {
            type: DetailsResponse.type;
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
        type: DetailsResponse.type;
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
    });
    address?: Array<{
        localname?: string;
        place_id?: number;
        osm_id?: number;
        osm_type?: OSMType;
        class?: string;
        type?: string;
        admin_level?: number;
        rank_address?: number;
        distance?: number;
        isaddress?: boolean;
    }>;
};
export namespace DetailsResponse {
    export enum type {
        POINT = 'Point',
    }
}

