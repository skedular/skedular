/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { FileUploadResponse } from '../models/FileUploadResponse';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class PublicService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Upload file with public access
     * @param formData
     * @returns FileUploadResponse the response of uploading file
     * @returns Error unexpected error
     * @throws ApiError
     */
    public uploadPublicAccessFile(
        formData: {
            /**
             * The file to upload
             */
            file?: Blob;
        },
    ): CancelablePromise<FileUploadResponse | Error> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/core/uploadPublicAccessFile',
            formData: formData,
            mediaType: 'multipart/form-data',
        });
    }
    /**
     * Serve static file from local CDN
     * Returns a file stored in the local CDN by filename
     * @param filename Name of the file to return
     * @returns binary File successfully returned
     * @throws ApiError
     */
    public getPublicCdnFile(
        filename: string,
    ): CancelablePromise<Blob> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/core/cdn/{filename}',
            path: {
                'filename': filename,
            },
            errors: {
                404: `File not found`,
            },
        });
    }
}
