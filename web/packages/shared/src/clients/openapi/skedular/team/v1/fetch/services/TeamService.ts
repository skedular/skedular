/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Error } from '../models/Error';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class TeamService {
  constructor(public readonly httpRequest: BaseHttpRequest) {}
  /**
   * republish team
   * @param teamId
   * @returns any the status of team republishing
   * @returns Error unexpected error
   * @throws ApiError
   */
  public republish(teamId: string): CancelablePromise<any | Error> {
    return this.httpRequest.request({
      method: 'PUT',
      url: '/team/api/v1/{teamId}/republish',
      path: {
        teamId: teamId,
      },
    });
  }
  /**
   * republish all teams
   * @returns any the status of team republishing
   * @returns Error unexpected error
   * @throws ApiError
   */
  public republishAll(): CancelablePromise<any | Error> {
    return this.httpRequest.request({
      method: 'PUT',
      url: '/team/api/v1/republish-all',
    });
  }
}
