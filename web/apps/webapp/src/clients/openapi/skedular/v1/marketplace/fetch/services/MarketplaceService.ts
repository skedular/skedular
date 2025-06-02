/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { FileUploadResponse } from '../models/FileUploadResponse';
import type { Version } from '../models/Version';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class MarketplaceService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * return API version
     * @returns Version the version of the API
     * @returns Error unexpected error
     * @throws ApiError
     */
    public getVersion(): CancelablePromise<Version | Error> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/marketplace/version',
        });
    }
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
    /**
     * Upload file
     * @param formData
     * @returns FileUploadResponse the response of uploading file
     * @returns Error unexpected error
     * @throws ApiError
     */
    public uploadFile(
        formData: {
            /**
             * The file to upload
             */
            file?: Blob;
        },
    ): CancelablePromise<FileUploadResponse | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/marketplace/uploadFile',
            formData: formData,
            mediaType: 'multipart/form-data',
        });
    }
}
