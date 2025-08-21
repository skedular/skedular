/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export const $OSMGeocodeJson = {
    type: 'all-of',
    contains: [{
        type: 'all-of',
        description: `GeocodeJSON is an extension of the GeoJSON format and it is an attempt to create a standard for handling geocoding results.`,
        contains: [{
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
        }, {
            description: `GeocodeJSON extension of GeoJSON Feature Collection`,
            properties: {
                type: {
                    description: `REQUIRED. GeocodeJSON result is a FeatureCollection.`,
                    properties: {
                    },
                },
                geocoding: {
                    description: `REQUIRED. Namespace.`,
                    properties: {
                        version: {
                            type: 'string',
                            description: `A semver.org compliant version number. Describes the version of the GeocodeJSON spec that is implemented by this instance.`,
                            isRequired: true,
                            maxLength: 256,
                            minLength: 5,
                            pattern: '^(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)(?:-((?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+([0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?$',
                        },
                        licence: {
                            type: 'string',
                            description: `OPTIONAL. The licence of the data. In case of multiple sources, and then multiple licences, can be an object with one key by source.`,
                        },
                        attribution: {
                            type: 'string',
                            description: `OPTIONAL. The attribution of the data. In case of multiple sources, and then multiple attributions, can be an object with one key by source.`,
                        },
                        query: {
                            type: 'string',
                            description: `OPTIONAL. The query that has been issued to trigger the search.`,
                        },
                    },
                    isRequired: true,
                },
            },
        }, {
            description: `GeocodeJSON extension of GeoJSON Feature`,
            properties: {
                features: {
                    type: 'array',
                    contains: {
                        description: `OPTIONAL. An array of feature objects.`,
                        properties: {
                            type: {
                                description: `REQUIRED. As per GeoJSON spec.`,
                                properties: {
                                },
                            },
                            properties: {
                                description: `REQUIRED. As per GeoJSON spec.`,
                                properties: {
                                    geocoding: {
                                        description: `REQUIRED. Namespace.`,
                                        properties: {
                                            type: {
                                                type: 'string',
                                                description: `REQUIRED. One of house, street, locality, city, region, country.`,
                                                isRequired: true,
                                            },
                                            accuracy: {
                                                type: 'number',
                                                description: `OPTIONAL. Result accuracy, in meters.`,
                                            },
                                            label: {
                                                type: 'string',
                                                description: `RECOMMENDED. Suggested label for the result.`,
                                            },
                                            name: {
                                                type: 'string',
                                                description: `OPTIONAL. Name of the place.`,
                                            },
                                            housenumber: {
                                                type: 'string',
                                                description: `OPTIONAL. Housenumber of the place.`,
                                            },
                                            street: {
                                                type: 'string',
                                                description: `OPTIONAL. Street of the place.`,
                                            },
                                            locality: {
                                                type: 'string',
                                                description: `OPTIONAL. Locality of the place.`,
                                            },
                                            postcode: {
                                                type: 'string',
                                                description: `OPTIONAL. Postcode of the place.`,
                                            },
                                            city: {
                                                type: 'string',
                                                description: `OPTIONAL. City of the place.`,
                                            },
                                            district: {
                                                type: 'string',
                                                description: `OPTIONAL. District of the place.`,
                                            },
                                            county: {
                                                type: 'string',
                                                description: `OPTIONAL. County of the place.`,
                                            },
                                            state: {
                                                type: 'string',
                                                description: `OPTIONAL. State of the place.`,
                                            },
                                            country: {
                                                type: 'string',
                                                description: `OPTIONAL. Country of the place.`,
                                            },
                                            admin: {
                                                type: 'dictionary',
                                                contains: {
                                                    properties: {
                                                    },
                                                },
                                            },
                                            geohash: {
                                                type: 'string',
                                                description: `OPTIONAL. Geohash encoding of coordinates (see http://geohash.org/site/tips.html).`,
                                                pattern: '^[0123456789bcdefghjkmnpqrstuvwxyz]+(:.+)?$',
                                            },
                                        },
                                        isRequired: true,
                                    },
                                },
                            },
                        },
                    },
                },
            },
        }],
    }, {
        properties: {
            features: {
                type: 'array',
                contains: {
                    properties: {
                        properties: {
                            properties: {
                                geocoding: {
                                    properties: {
                                        osm_type: {
                                            type: 'string',
                                        },
                                        osm_id: {
                                            type: 'number',
                                        },
                                        osm_key: {
                                            type: 'string',
                                        },
                                        osm_value: {
                                            type: 'string',
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            },
        },
    }],
} as const;
