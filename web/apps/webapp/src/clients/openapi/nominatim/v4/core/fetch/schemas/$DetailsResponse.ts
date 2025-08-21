/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export const $DetailsResponse = {
    properties: {
        place_id: {
            type: 'number',
        },
        parent_place_id: {
            type: 'number',
        },
        osm_type: {
            type: 'OSMType',
        },
        osm_id: {
            type: 'number',
        },
        category: {
            type: 'string',
        },
        type: {
            type: 'string',
        },
        admin_level: {
            type: 'number',
        },
        localname: {
            type: 'string',
        },
        names: {
            properties: {
                name: {
                    type: 'string',
                },
            },
        },
        addresstags: {
            type: 'dictionary',
            contains: {
                properties: {
                },
            },
        },
        calculated_postcode: {
            type: 'string',
        },
        country_code: {
            type: 'string',
            pattern: '^[a-zA-Z]{2}$',
        },
        indexed_date: {
            type: 'string',
            format: 'date-time',
        },
        importance: {
            type: 'number',
        },
        calculated_importance: {
            type: 'number',
        },
        extratags: {
            type: 'dictionary',
            contains: {
                properties: {
                },
            },
        },
        rank_address: {
            type: 'number',
            minimum: 1,
        },
        rank_search: {
            type: 'number',
            minimum: 1,
        },
        isarea: {
            type: 'boolean',
        },
        centroid: {
            properties: {
                type: {
                    type: 'Enum',
                    isRequired: true,
                },
                coordinates: {
                    type: 'array',
                    contains: {
                        type: 'number',
                    },
                    isRequired: true,
                },
                bbox: {
                    type: 'array',
                    contains: {
                        type: 'number',
                    },
                },
            },
        },
        geometry: {
            type: 'one-of',
            contains: [{
                properties: {
                    type: {
                        type: 'Enum',
                        isRequired: true,
                    },
                    coordinates: {
                        type: 'array',
                        contains: {
                            type: 'number',
                        },
                        isRequired: true,
                    },
                    bbox: {
                        type: 'array',
                        contains: {
                            type: 'number',
                        },
                    },
                },
            }, {
                properties: {
                    type: {
                        type: 'Enum',
                        isRequired: true,
                    },
                    coordinates: {
                        type: 'array',
                        contains: {
                            type: 'array',
                            contains: {
                                type: 'number',
                            },
                        },
                        isRequired: true,
                    },
                    bbox: {
                        type: 'array',
                        contains: {
                            type: 'number',
                        },
                    },
                },
            }, {
                properties: {
                    type: {
                        type: 'Enum',
                        isRequired: true,
                    },
                    coordinates: {
                        type: 'array',
                        contains: {
                            type: 'array',
                            contains: {
                                type: 'array',
                                contains: {
                                    type: 'number',
                                },
                            },
                        },
                        isRequired: true,
                    },
                    bbox: {
                        type: 'array',
                        contains: {
                            type: 'number',
                        },
                    },
                },
            }, {
                properties: {
                    type: {
                        type: 'Enum',
                        isRequired: true,
                    },
                    coordinates: {
                        type: 'array',
                        contains: {
                            type: 'array',
                            contains: {
                                type: 'number',
                            },
                        },
                        isRequired: true,
                    },
                    bbox: {
                        type: 'array',
                        contains: {
                            type: 'number',
                        },
                    },
                },
            }, {
                properties: {
                    type: {
                        type: 'Enum',
                        isRequired: true,
                    },
                    coordinates: {
                        type: 'array',
                        contains: {
                            type: 'array',
                            contains: {
                                type: 'array',
                                contains: {
                                    type: 'number',
                                },
                            },
                        },
                        isRequired: true,
                    },
                    bbox: {
                        type: 'array',
                        contains: {
                            type: 'number',
                        },
                    },
                },
            }, {
                properties: {
                    type: {
                        type: 'Enum',
                        isRequired: true,
                    },
                    coordinates: {
                        type: 'array',
                        contains: {
                            type: 'array',
                            contains: {
                                type: 'array',
                                contains: {
                                    type: 'array',
                                    contains: {
                                        type: 'number',
                                    },
                                },
                            },
                        },
                        isRequired: true,
                    },
                    bbox: {
                        type: 'array',
                        contains: {
                            type: 'number',
                        },
                    },
                },
            }, {
                properties: {
                    type: {
                        type: 'Enum',
                        isRequired: true,
                    },
                    geometries: {
                        type: 'array',
                        contains: {
                            type: 'one-of',
                            contains: [{
                                properties: {
                                    type: {
                                        type: 'Enum',
                                        isRequired: true,
                                    },
                                    coordinates: {
                                        type: 'array',
                                        contains: {
                                            type: 'number',
                                        },
                                        isRequired: true,
                                    },
                                    bbox: {
                                        type: 'array',
                                        contains: {
                                            type: 'number',
                                        },
                                    },
                                },
                            }, {
                                properties: {
                                    type: {
                                        type: 'Enum',
                                        isRequired: true,
                                    },
                                    coordinates: {
                                        type: 'array',
                                        contains: {
                                            type: 'array',
                                            contains: {
                                                type: 'number',
                                            },
                                        },
                                        isRequired: true,
                                    },
                                    bbox: {
                                        type: 'array',
                                        contains: {
                                            type: 'number',
                                        },
                                    },
                                },
                            }, {
                                properties: {
                                    type: {
                                        type: 'Enum',
                                        isRequired: true,
                                    },
                                    coordinates: {
                                        type: 'array',
                                        contains: {
                                            type: 'array',
                                            contains: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                            },
                                        },
                                        isRequired: true,
                                    },
                                    bbox: {
                                        type: 'array',
                                        contains: {
                                            type: 'number',
                                        },
                                    },
                                },
                            }, {
                                properties: {
                                    type: {
                                        type: 'Enum',
                                        isRequired: true,
                                    },
                                    coordinates: {
                                        type: 'array',
                                        contains: {
                                            type: 'array',
                                            contains: {
                                                type: 'number',
                                            },
                                        },
                                        isRequired: true,
                                    },
                                    bbox: {
                                        type: 'array',
                                        contains: {
                                            type: 'number',
                                        },
                                    },
                                },
                            }, {
                                properties: {
                                    type: {
                                        type: 'Enum',
                                        isRequired: true,
                                    },
                                    coordinates: {
                                        type: 'array',
                                        contains: {
                                            type: 'array',
                                            contains: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                            },
                                        },
                                        isRequired: true,
                                    },
                                    bbox: {
                                        type: 'array',
                                        contains: {
                                            type: 'number',
                                        },
                                    },
                                },
                            }, {
                                properties: {
                                    type: {
                                        type: 'Enum',
                                        isRequired: true,
                                    },
                                    coordinates: {
                                        type: 'array',
                                        contains: {
                                            type: 'array',
                                            contains: {
                                                type: 'array',
                                                contains: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'number',
                                                    },
                                                },
                                            },
                                        },
                                        isRequired: true,
                                    },
                                    bbox: {
                                        type: 'array',
                                        contains: {
                                            type: 'number',
                                        },
                                    },
                                },
                            }],
                        },
                        isRequired: true,
                    },
                    bbox: {
                        type: 'array',
                        contains: {
                            type: 'number',
                        },
                    },
                },
            }, {
                properties: {
                    type: {
                        type: 'Enum',
                        isRequired: true,
                    },
                    id: {
                        type: 'one-of',
                        contains: [{
                            type: 'number',
                        }, {
                            type: 'string',
                        }],
                    },
                    properties: {
                        type: 'one-of',
                        contains: [{
                            type: 'null',
                        }, {
                            type: 'dictionary',
                            contains: {
                                properties: {
                                },
                            },
                        }],
                        isRequired: true,
                    },
                    geometry: {
                        type: 'one-of',
                        contains: [{
                            type: 'null',
                        }, {
                            properties: {
                                type: {
                                    type: 'Enum',
                                    isRequired: true,
                                },
                                coordinates: {
                                    type: 'array',
                                    contains: {
                                        type: 'number',
                                    },
                                    isRequired: true,
                                },
                                bbox: {
                                    type: 'array',
                                    contains: {
                                        type: 'number',
                                    },
                                },
                            },
                        }, {
                            properties: {
                                type: {
                                    type: 'Enum',
                                    isRequired: true,
                                },
                                coordinates: {
                                    type: 'array',
                                    contains: {
                                        type: 'array',
                                        contains: {
                                            type: 'number',
                                        },
                                    },
                                    isRequired: true,
                                },
                                bbox: {
                                    type: 'array',
                                    contains: {
                                        type: 'number',
                                    },
                                },
                            },
                        }, {
                            properties: {
                                type: {
                                    type: 'Enum',
                                    isRequired: true,
                                },
                                coordinates: {
                                    type: 'array',
                                    contains: {
                                        type: 'array',
                                        contains: {
                                            type: 'array',
                                            contains: {
                                                type: 'number',
                                            },
                                        },
                                    },
                                    isRequired: true,
                                },
                                bbox: {
                                    type: 'array',
                                    contains: {
                                        type: 'number',
                                    },
                                },
                            },
                        }, {
                            properties: {
                                type: {
                                    type: 'Enum',
                                    isRequired: true,
                                },
                                coordinates: {
                                    type: 'array',
                                    contains: {
                                        type: 'array',
                                        contains: {
                                            type: 'number',
                                        },
                                    },
                                    isRequired: true,
                                },
                                bbox: {
                                    type: 'array',
                                    contains: {
                                        type: 'number',
                                    },
                                },
                            },
                        }, {
                            properties: {
                                type: {
                                    type: 'Enum',
                                    isRequired: true,
                                },
                                coordinates: {
                                    type: 'array',
                                    contains: {
                                        type: 'array',
                                        contains: {
                                            type: 'array',
                                            contains: {
                                                type: 'number',
                                            },
                                        },
                                    },
                                    isRequired: true,
                                },
                                bbox: {
                                    type: 'array',
                                    contains: {
                                        type: 'number',
                                    },
                                },
                            },
                        }, {
                            properties: {
                                type: {
                                    type: 'Enum',
                                    isRequired: true,
                                },
                                coordinates: {
                                    type: 'array',
                                    contains: {
                                        type: 'array',
                                        contains: {
                                            type: 'array',
                                            contains: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                            },
                                        },
                                    },
                                    isRequired: true,
                                },
                                bbox: {
                                    type: 'array',
                                    contains: {
                                        type: 'number',
                                    },
                                },
                            },
                        }, {
                            properties: {
                                type: {
                                    type: 'Enum',
                                    isRequired: true,
                                },
                                geometries: {
                                    type: 'array',
                                    contains: {
                                        type: 'one-of',
                                        contains: [{
                                            properties: {
                                                type: {
                                                    type: 'Enum',
                                                    isRequired: true,
                                                },
                                                coordinates: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'number',
                                                    },
                                                    isRequired: true,
                                                },
                                                bbox: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'number',
                                                    },
                                                },
                                            },
                                        }, {
                                            properties: {
                                                type: {
                                                    type: 'Enum',
                                                    isRequired: true,
                                                },
                                                coordinates: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'array',
                                                        contains: {
                                                            type: 'number',
                                                        },
                                                    },
                                                    isRequired: true,
                                                },
                                                bbox: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'number',
                                                    },
                                                },
                                            },
                                        }, {
                                            properties: {
                                                type: {
                                                    type: 'Enum',
                                                    isRequired: true,
                                                },
                                                coordinates: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'array',
                                                        contains: {
                                                            type: 'array',
                                                            contains: {
                                                                type: 'number',
                                                            },
                                                        },
                                                    },
                                                    isRequired: true,
                                                },
                                                bbox: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'number',
                                                    },
                                                },
                                            },
                                        }, {
                                            properties: {
                                                type: {
                                                    type: 'Enum',
                                                    isRequired: true,
                                                },
                                                coordinates: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'array',
                                                        contains: {
                                                            type: 'number',
                                                        },
                                                    },
                                                    isRequired: true,
                                                },
                                                bbox: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'number',
                                                    },
                                                },
                                            },
                                        }, {
                                            properties: {
                                                type: {
                                                    type: 'Enum',
                                                    isRequired: true,
                                                },
                                                coordinates: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'array',
                                                        contains: {
                                                            type: 'array',
                                                            contains: {
                                                                type: 'number',
                                                            },
                                                        },
                                                    },
                                                    isRequired: true,
                                                },
                                                bbox: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'number',
                                                    },
                                                },
                                            },
                                        }, {
                                            properties: {
                                                type: {
                                                    type: 'Enum',
                                                    isRequired: true,
                                                },
                                                coordinates: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'array',
                                                        contains: {
                                                            type: 'array',
                                                            contains: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'number',
                                                                },
                                                            },
                                                        },
                                                    },
                                                    isRequired: true,
                                                },
                                                bbox: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'number',
                                                    },
                                                },
                                            },
                                        }],
                                    },
                                    isRequired: true,
                                },
                                bbox: {
                                    type: 'array',
                                    contains: {
                                        type: 'number',
                                    },
                                },
                            },
                        }],
                        isRequired: true,
                    },
                    bbox: {
                        type: 'array',
                        contains: {
                            type: 'number',
                        },
                    },
                },
            }, {
                properties: {
                    type: {
                        type: 'Enum',
                        isRequired: true,
                    },
                    features: {
                        type: 'array',
                        contains: {
                            properties: {
                                type: {
                                    type: 'Enum',
                                    isRequired: true,
                                },
                                id: {
                                    type: 'one-of',
                                    contains: [{
                                        type: 'number',
                                    }, {
                                        type: 'string',
                                    }],
                                },
                                properties: {
                                    type: 'one-of',
                                    contains: [{
                                        type: 'null',
                                    }, {
                                        type: 'dictionary',
                                        contains: {
                                            properties: {
                                            },
                                        },
                                    }],
                                    isRequired: true,
                                },
                                geometry: {
                                    type: 'one-of',
                                    contains: [{
                                        type: 'null',
                                    }, {
                                        properties: {
                                            type: {
                                                type: 'Enum',
                                                isRequired: true,
                                            },
                                            coordinates: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                                isRequired: true,
                                            },
                                            bbox: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                            },
                                        },
                                    }, {
                                        properties: {
                                            type: {
                                                type: 'Enum',
                                                isRequired: true,
                                            },
                                            coordinates: {
                                                type: 'array',
                                                contains: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'number',
                                                    },
                                                },
                                                isRequired: true,
                                            },
                                            bbox: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                            },
                                        },
                                    }, {
                                        properties: {
                                            type: {
                                                type: 'Enum',
                                                isRequired: true,
                                            },
                                            coordinates: {
                                                type: 'array',
                                                contains: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'array',
                                                        contains: {
                                                            type: 'number',
                                                        },
                                                    },
                                                },
                                                isRequired: true,
                                            },
                                            bbox: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                            },
                                        },
                                    }, {
                                        properties: {
                                            type: {
                                                type: 'Enum',
                                                isRequired: true,
                                            },
                                            coordinates: {
                                                type: 'array',
                                                contains: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'number',
                                                    },
                                                },
                                                isRequired: true,
                                            },
                                            bbox: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                            },
                                        },
                                    }, {
                                        properties: {
                                            type: {
                                                type: 'Enum',
                                                isRequired: true,
                                            },
                                            coordinates: {
                                                type: 'array',
                                                contains: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'array',
                                                        contains: {
                                                            type: 'number',
                                                        },
                                                    },
                                                },
                                                isRequired: true,
                                            },
                                            bbox: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                            },
                                        },
                                    }, {
                                        properties: {
                                            type: {
                                                type: 'Enum',
                                                isRequired: true,
                                            },
                                            coordinates: {
                                                type: 'array',
                                                contains: {
                                                    type: 'array',
                                                    contains: {
                                                        type: 'array',
                                                        contains: {
                                                            type: 'array',
                                                            contains: {
                                                                type: 'number',
                                                            },
                                                        },
                                                    },
                                                },
                                                isRequired: true,
                                            },
                                            bbox: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                            },
                                        },
                                    }, {
                                        properties: {
                                            type: {
                                                type: 'Enum',
                                                isRequired: true,
                                            },
                                            geometries: {
                                                type: 'array',
                                                contains: {
                                                    type: 'one-of',
                                                    contains: [{
                                                        properties: {
                                                            type: {
                                                                type: 'Enum',
                                                                isRequired: true,
                                                            },
                                                            coordinates: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'number',
                                                                },
                                                                isRequired: true,
                                                            },
                                                            bbox: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'number',
                                                                },
                                                            },
                                                        },
                                                    }, {
                                                        properties: {
                                                            type: {
                                                                type: 'Enum',
                                                                isRequired: true,
                                                            },
                                                            coordinates: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'array',
                                                                    contains: {
                                                                        type: 'number',
                                                                    },
                                                                },
                                                                isRequired: true,
                                                            },
                                                            bbox: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'number',
                                                                },
                                                            },
                                                        },
                                                    }, {
                                                        properties: {
                                                            type: {
                                                                type: 'Enum',
                                                                isRequired: true,
                                                            },
                                                            coordinates: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'array',
                                                                    contains: {
                                                                        type: 'array',
                                                                        contains: {
                                                                            type: 'number',
                                                                        },
                                                                    },
                                                                },
                                                                isRequired: true,
                                                            },
                                                            bbox: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'number',
                                                                },
                                                            },
                                                        },
                                                    }, {
                                                        properties: {
                                                            type: {
                                                                type: 'Enum',
                                                                isRequired: true,
                                                            },
                                                            coordinates: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'array',
                                                                    contains: {
                                                                        type: 'number',
                                                                    },
                                                                },
                                                                isRequired: true,
                                                            },
                                                            bbox: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'number',
                                                                },
                                                            },
                                                        },
                                                    }, {
                                                        properties: {
                                                            type: {
                                                                type: 'Enum',
                                                                isRequired: true,
                                                            },
                                                            coordinates: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'array',
                                                                    contains: {
                                                                        type: 'array',
                                                                        contains: {
                                                                            type: 'number',
                                                                        },
                                                                    },
                                                                },
                                                                isRequired: true,
                                                            },
                                                            bbox: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'number',
                                                                },
                                                            },
                                                        },
                                                    }, {
                                                        properties: {
                                                            type: {
                                                                type: 'Enum',
                                                                isRequired: true,
                                                            },
                                                            coordinates: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'array',
                                                                    contains: {
                                                                        type: 'array',
                                                                        contains: {
                                                                            type: 'array',
                                                                            contains: {
                                                                                type: 'number',
                                                                            },
                                                                        },
                                                                    },
                                                                },
                                                                isRequired: true,
                                                            },
                                                            bbox: {
                                                                type: 'array',
                                                                contains: {
                                                                    type: 'number',
                                                                },
                                                            },
                                                        },
                                                    }],
                                                },
                                                isRequired: true,
                                            },
                                            bbox: {
                                                type: 'array',
                                                contains: {
                                                    type: 'number',
                                                },
                                            },
                                        },
                                    }],
                                    isRequired: true,
                                },
                                bbox: {
                                    type: 'array',
                                    contains: {
                                        type: 'number',
                                    },
                                },
                            },
                        },
                        isRequired: true,
                    },
                    bbox: {
                        type: 'array',
                        contains: {
                            type: 'number',
                        },
                    },
                },
            }],
        },
        address: {
            type: 'array',
            contains: {
                properties: {
                    localname: {
                        type: 'string',
                    },
                    place_id: {
                        type: 'number',
                    },
                    osm_id: {
                        type: 'number',
                    },
                    osm_type: {
                        type: 'OSMType',
                    },
                    class: {
                        type: 'string',
                    },
                    type: {
                        type: 'string',
                    },
                    admin_level: {
                        type: 'number',
                    },
                    rank_address: {
                        type: 'number',
                        minimum: 1,
                    },
                    distance: {
                        type: 'number',
                    },
                    isaddress: {
                        type: 'boolean',
                    },
                },
            },
        },
    },
} as const;
