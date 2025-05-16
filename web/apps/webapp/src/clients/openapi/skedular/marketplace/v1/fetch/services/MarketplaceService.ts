/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class MarketplaceService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * republish organization products
     * @param organizationId
     * @returns any the status of all organization products republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishAllOrganizationProducts(
        organizationId: string,
    ): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/marketplace/{organizationId}/products/republish-all',
            path: {
                'organizationId': organizationId,
            },
        });
    }
    /**
     * republish all products
     * @returns any the status of all products republishing
     * @returns Error unexpected error
     * @throws ApiError
     */
    public republishAllProducts(): CancelablePromise<any | Error> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/v1/marketplace/products/republish-all',
        });
    }
}
