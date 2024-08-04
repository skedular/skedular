/**
 * @generated SignedSource<<80be2946ff219abd0952100964f77680>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Query } from 'relay-runtime';
export type appHome_rootQuery$variables = Record<PropertyKey, never>;
export type appHome_rootQuery$data = {
  readonly msTeamsVersion: {
    readonly major: number;
  };
};
export type appHome_rootQuery = {
  response: appHome_rootQuery$data;
  variables: appHome_rootQuery$variables;
};

const node: ConcreteRequest = (function () {
  var v0 = [
    {
      alias: null,
      args: null,
      concreteType: 'Version',
      kind: 'LinkedField',
      name: 'msTeamsVersion',
      plural: false,
      selections: [
        {
          alias: null,
          args: null,
          kind: 'ScalarField',
          name: 'major',
          storageKey: null,
        },
      ],
      storageKey: null,
    },
  ];
  return {
    fragment: {
      argumentDefinitions: [],
      kind: 'Fragment',
      metadata: null,
      name: 'appHome_rootQuery',
      selections: v0 /*: any*/,
      type: 'Query',
      abstractKey: null,
    },
    kind: 'Request',
    operation: {
      argumentDefinitions: [],
      kind: 'Operation',
      name: 'appHome_rootQuery',
      selections: v0 /*: any*/,
    },
    params: {
      cacheID: 'f83f8cffeef072c160f7f7ff89bd35f9',
      id: null,
      metadata: {},
      name: 'appHome_rootQuery',
      operationKind: 'query',
      text: 'query appHome_rootQuery {\n  msTeamsVersion {\n    major\n  }\n}\n',
    },
  };
})();

(node as any).hash = 'f4a6ed250d7894018a23fae82d71ab92';

export default node;
