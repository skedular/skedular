import logger from '../logger';
import { v4 as uuidv4 } from 'uuid';
import type { AuthOptions } from 'next-auth';
import CognitoProvider from 'next-auth/providers/cognito';
import GoogleProvider from 'next-auth/providers/google';

export interface TokenExtended {
  accessToken: string;
  accessTokenExpires: number;
  refreshToken: string;
  error?: string;
}

type AccessToken = {
  idToken: string;
  accessToken: string;
  accessTokenExpires: number;
  refreshToken: string;
  error?: string;
};

const refreshCognitoTokens = async (refreshToken?: string): Promise<AccessToken> => {
  try {
    const formData = [
      `${encodeURIComponent('grant_type')}=${encodeURIComponent('refresh_token')}`,
      `${encodeURIComponent('client_id')}=${encodeURIComponent(process.env.COGNITO_CLIENT_ID)}`,
      `${encodeURIComponent('refresh_token')}=${encodeURIComponent(refreshToken ?? '')}`,
    ];

    const buff = Buffer.from(`${process.env.COGNITO_CLIENT_ID}:${process.env.COGNITO_CLIENT_SECRET}`);

    const response = await fetch(`${process.env.COGNITO_DOMAIN}/oauth2/token`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8',
        Authorization: `Basic ${buff.toString('base64')}`,
      },
      body: formData.join('&'),
    });

    const responseJson = await response.json();

    if (!response.ok) {
      throw responseJson;
    }

    return {
      idToken: responseJson.id_token,
      accessToken: responseJson.access_token,
      accessTokenExpires: Date.now() + responseJson.expires_in * 1000,
      refreshToken: responseJson.refresh_token ?? refreshToken,
    };
  } catch (exception) {
    logger.error({ exception }, 'failed to refresh access token');

    return {
      idToken: '',
      accessToken: '',
      accessTokenExpires: 0,
      refreshToken: '',
      error: 'RefreshAccessTokenError',
    };
  }
};

const refreshGoogleTokens = async (refreshToken?: string): Promise<AccessToken> => {
  try {
    const formData = [
      `${encodeURIComponent('grant_type')}=${encodeURIComponent('refresh_token')}`,
      `${encodeURIComponent('client_id')}=${encodeURIComponent(process.env.GOOGLE_CLIENT_ID)}`,
      `${encodeURIComponent('refresh_token')}=${encodeURIComponent(refreshToken ?? '')}`,
    ];

    const buff = Buffer.from(`${process.env.GOOGLE_CLIENT_ID}:${process.env.GOOGLE_CLIENT_SECRET}`);

    const response = await fetch('https://oauth2.googleapis.com/token', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8',
        Authorization: `Basic ${buff.toString('base64')}`,
      },
      body: formData.join('&'),
    });

    const responseJson = await response.json();

    if (!response.ok) {
      throw responseJson;
    }

    return {
      idToken: responseJson.id_token,
      accessToken: responseJson.access_token,
      accessTokenExpires: Date.now() + responseJson.expires_in * 1000,
      refreshToken: responseJson.refresh_token ?? refreshToken,
    };
  } catch (exception) {
    logger.error({ exception }, 'failed to refresh access token');

    return {
      idToken: '',
      accessToken: '',
      accessTokenExpires: 0,
      refreshToken: '',
      error: 'RefreshAccessTokenError',
    };
  }
};

const authOptions: AuthOptions = {
  providers: [
    GoogleProvider({
      clientId: process.env.GOOGLE_CLIENT_ID,
      clientSecret: process.env.GOOGLE_CLIENT_SECRET,
      authorization: {
        params: {
          prompt: 'consent',
          access_type: 'offline',
          response_type: 'code',
        },
      },
    }),
    CognitoProvider({
      clientId: process.env.COGNITO_CLIENT_ID,
      clientSecret: process.env.COGNITO_CLIENT_SECRET,
      issuer: process.env.COGNITO_ISSUER,
      checks: ['nonce'],
    }),
  ],
  callbacks: {
    async signIn({ user, profile }) {
      const corelationId = uuidv4();

      logger.info({ corelationId, user, profile }, 'User is trying to sign in...');

      if (!profile || !profile.sub) {
        logger.error({ corelationId, user, profile }, 'Either profile or sub must be provided.');

        return false;
      }

      return true;
    },
    async jwt({ token, account }) {
      if (account) {
        token.provider = account.provider;
        token.idToken = account.id_token;
        token.accessToken = account.access_token;
        token.accessTokenExpires = (account.expires_at ?? 0) * 1000;
        token.refreshToken = account.refresh_token;

        return token;
      }

      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const tokenExtended = token as any & TokenExtended;
      if (Date.now() < tokenExtended.accessTokenExpires) {
        return token;
      }

      if (token.provider === 'cognito') {
        const response = await refreshCognitoTokens(tokenExtended.refreshToken);

        token.idToken = response.idToken;
        token.accessToken = response.accessToken;
        token.accessTokenExpires = response.accessTokenExpires;
        token.refreshToken = response.refreshToken;
        token.error = response.error;
      } else if (token.provider === 'google') {
        const response = await refreshGoogleTokens(tokenExtended.refreshToken);

        token.idToken = response.idToken;
        token.accessToken = response.accessToken;
        token.accessTokenExpires = response.accessTokenExpires;
        token.refreshToken = response.refreshToken;
        token.error = response.error;
      } else {
        throw new Error(`Provider type: ${token.provider} not supported`);
      }

      return token;
    },
    async session({ session, token }) {
      return Object.assign({}, session, {
        error: token.error,
      });
    },
  },
};

export { authOptions };
