import { aqua, flame, sunbeam, violet } from '@skedular/ui';
import type { PayloadError } from 'relay-runtime';

export type ErrorWithGraphQlSource = {
  message?: string | null;
  source?: {
    errors?: PayloadError[] | null;
  } | null;
};

export type RelayErrorLike = ErrorWithGraphQlSource | PayloadError[];

export const secondaryColors = [violet, aqua, sunbeam, flame];

const normalizeRelayErrorMessage = (message: string | null | undefined) => {
  const trimmedMessage = message?.trim();
  if (!trimmedMessage) {
    return null;
  }

  if (trimmedMessage === 'See the error `source` property for more information..') {
    return null;
  }

  return trimmedMessage;
};

const getRelayMessagesFromSingleError = (error: ErrorWithGraphQlSource) => {
  const graphQlMessages = (error.source?.errors ?? []).map((item) => normalizeRelayErrorMessage(item.message)).filter((item): item is string => !!item);

  if (graphQlMessages.length > 0) {
    return graphQlMessages;
  }

  const fallbackMessage = normalizeRelayErrorMessage(error.message);
  return fallbackMessage ? [fallbackMessage] : [];
};

export const getRelayErrorMessage = (error: RelayErrorLike) => {
  if (Array.isArray(error)) {
    const messages = error.flatMap((item) => getRelayMessagesFromSingleError(item));
    return Array.from(new Set(messages)).join('\n');
  }

  const messages = getRelayMessagesFromSingleError(error);
  if (messages.length > 0) {
    return Array.from(new Set(messages)).join('\n');
  }

  return 'Unknown error';
};

export const stringToColor = (string: string) => {
  let hash = 0;

  for (let i = 0; i < string.length; i++) {
    hash = string.charCodeAt(i) + ((hash << 5) - hash);
  }

  const index = Math.abs(hash) % secondaryColors.length;
  return secondaryColors[index];
};
