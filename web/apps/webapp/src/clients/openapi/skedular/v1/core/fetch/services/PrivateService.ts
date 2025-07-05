/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { FileUploadResponse } from '../models/FileUploadResponse';
import type { ProblemDetails } from '../models/ProblemDetails';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class PrivateService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Upload file with private access
     * @param formData
     * @returns FileUploadResponse the response of uploading file
     * @returns ProblemDetails unexpected error
     * @throws ApiError
     */
    public uploadPrivateAccessFile(
        formData: {
            /**
             * The file to upload
             */
            file?: Blob;
        },
    ): CancelablePromise<FileUploadResponse | ProblemDetails> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/v1/core/uploadPrivateAccessFile',
            formData: formData,
            mediaType: 'multipart/form-data',
        });
    }
    /**
     * Serve static file from local private storage
     * Returns a file stored in the local private storage by filename
     * @param filename Name of the file to return
     * @returns binary File successfully returned
     * @throws ApiError
     */
    public getPrivateFile(
        filename: string,
    ): CancelablePromise<Blob> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/v1/core/private/{filename}',
            path: {
                'filename': filename,
            },
            errors: {
                404: `File not found`,
            },
        });
    }
}
