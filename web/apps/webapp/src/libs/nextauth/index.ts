import logger from '@repo/shared/libs/logger';
import { jwtDecode } from 'jwt-decode';
import type { AuthOptions } from 'next-auth';
import AzureADProvider from 'next-auth/providers/azure-ad';
import CognitoProvider from 'next-auth/providers/cognito';
import GoogleProvider from 'next-auth/providers/google';
import { v4 as uuidv4 } from 'uuid';

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

interface AzureEntraDecodedToken {
  tid: string; // Azure Teanant ID
}

const refreshCognitoTokens = async (token: TokenExtended): Promise<AccessToken> => {
  try {
    const formData = [
      `${encodeURIComponent('grant_type')}=${encodeURIComponent('refresh_token')}`,
      `${encodeURIComponent('client_id')}=${encodeURIComponent(process.env.COGNITO_CLIENT_ID)}`,
      `${encodeURIComponent('refresh_token')}=${encodeURIComponent(token.refreshToken)}`,
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
      refreshToken: responseJson.refresh_token,
    };
  } catch (exception) {
    logger.error({ exception }, 'failed to refresh access token');

    throw exception;
  }
};

const refreshGoogleTokens = async (token: TokenExtended): Promise<AccessToken> => {
  try {
    const formData = [
      `${encodeURIComponent('grant_type')}=${encodeURIComponent('refresh_token')}`,
      `${encodeURIComponent('client_id')}=${encodeURIComponent(process.env.GOOGLE_CLIENT_ID)}`,
      `${encodeURIComponent('refresh_token')}=${encodeURIComponent(token.refreshToken)}`,
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
      refreshToken: responseJson.refresh_token,
    };
  } catch (exception) {
    logger.error({ exception }, 'failed to refresh access token');

    throw exception;
  }
};

const refreshAzureEntraTokens = async (token: TokenExtended): Promise<AccessToken> => {
  try {
    const decodedAccessToken = jwtDecode<AzureEntraDecodedToken>(token.accessToken);
    const formData = [
      `${encodeURIComponent('grant_type')}=${encodeURIComponent('refresh_token')}`,
      `${encodeURIComponent('client_id')}=${encodeURIComponent(process.env.AZURE_AD_CLIENT_ID)}`,
      `${encodeURIComponent('client_secret')}=${encodeURIComponent(process.env.AZURE_AD_CLIENT_SECRET)}`,
      `${encodeURIComponent('refresh_token')}=${encodeURIComponent(token.refreshToken)}`,
    ];

    const buff = Buffer.from(`${process.env.GOOGLE_CLIENT_ID}:${process.env.GOOGLE_CLIENT_SECRET}`);

    const response = await fetch(`https://login.microsoftonline.com/${decodedAccessToken.tid}/oauth2/v2.0/token`, {
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
      refreshToken: responseJson.refresh_token,
    };
  } catch (exception) {
    logger.error({ exception }, 'failed to refresh access token');

    throw exception;
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
    AzureADProvider({
      clientId: process.env.AZURE_AD_CLIENT_ID,
      clientSecret: process.env.AZURE_AD_CLIENT_SECRET,
      authorization: {
        params: {
          scope: 'ProfilePhoto.Read.All email offline_access openid profile',
        },
      },
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

      const tokenExtended = token as any & TokenExtended;
      if (Date.now() < tokenExtended.accessTokenExpires) {
        return token;
      }

      if (!tokenExtended.refreshToken) {
        throw new TypeError('Missing refresh_token');
      }

      if (token.provider === 'cognito') {
        const response = await refreshCognitoTokens(tokenExtended);

        token.idToken = response.idToken;
        token.accessToken = response.accessToken;
        token.accessTokenExpires = response.accessTokenExpires;
        token.refreshToken = response.refreshToken;
        token.error = response.error;
      } else if (token.provider === 'google') {
        const response = await refreshGoogleTokens(tokenExtended);

        token.idToken = response.idToken;
        token.accessToken = response.accessToken;
        token.accessTokenExpires = response.accessTokenExpires;
        token.refreshToken = response.refreshToken;
        token.error = response.error;
      } else if (token.provider === 'azure-ad') {
        const response = await refreshAzureEntraTokens(tokenExtended);

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
