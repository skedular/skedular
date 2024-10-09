import { memo } from 'react';
import { v4 as uuidv4 } from 'uuid';
import { encodeBase64, getPublicSiteUrl } from '../../libs/utils';

const parameterizedUrl =
  'https://slack.com/oauth/v2/authorize?scope=app_mentions:read,channels:join,channels:manage,channels:read,chat:write,team:read,users:read,users:read.email,users.profile:read&user_scope=users.profile:read,users.profile:write&state=$SLACK_STATE$&redirect_uri=$SLACK_REDIRECT_URL$&client_id=$SLACK_CLIENT_ID$';

const parameterizedHtml = `
<a href="$URL$" style="align-items:center;color:#fff;background-color:#4A154B;border:0;border-radius:44px;display:inline-flex;font-family:Lato, sans-serif;font-size:14px;font-weight:600;height:44px;justify-content:center;text-decoration:none;width:204px"><svg xmlns="http://www.w3.org/2000/svg" style="height:16px;width:16px;margin-right:12px" viewBox="0 0 122.8 122.8"><path d="M25.8 77.6c0 7.1-5.8 12.9-12.9 12.9S0 84.7 0 77.6s5.8-12.9 12.9-12.9h12.9v12.9zm6.5 0c0-7.1 5.8-12.9 12.9-12.9s12.9 5.8 12.9 12.9v32.3c0 7.1-5.8 12.9-12.9 12.9s-12.9-5.8-12.9-12.9V77.6z" fill="#e01e5a"></path><path d="M45.2 25.8c-7.1 0-12.9-5.8-12.9-12.9S38.1 0 45.2 0s12.9 5.8 12.9 12.9v12.9H45.2zm0 6.5c7.1 0 12.9 5.8 12.9 12.9s-5.8 12.9-12.9 12.9H12.9C5.8 58.1 0 52.3 0 45.2s5.8-12.9 12.9-12.9h32.3z" fill="#36c5f0"></path><path d="M97 45.2c0-7.1 5.8-12.9 12.9-12.9s12.9 5.8 12.9 12.9-5.8 12.9-12.9 12.9H97V45.2zm-6.5 0c0 7.1-5.8 12.9-12.9 12.9s-12.9-5.8-12.9-12.9V12.9C64.7 5.8 70.5 0 77.6 0s12.9 5.8 12.9 12.9v32.3z" fill="#2eb67d"></path><path d="M77.6 97c7.1 0 12.9 5.8 12.9 12.9s-5.8 12.9-12.9 12.9-12.9-5.8-12.9-12.9V97h12.9zm0-6.5c-7.1 0-12.9-5.8-12.9-12.9s5.8-12.9 12.9-12.9h32.3c7.1 0 12.9 5.8 12.9 12.9s-5.8 12.9-12.9 12.9H77.6z" fill="#ecb22e"></path></svg>Add to Slack</a>`;

interface State {
  correlationId: string;
}

const SlackButton = () => {
  const state: State = {
    correlationId: uuidv4(),
  };

  const originalUrl = parameterizedUrl
    .replace('$SLACK_CLIENT_ID$', process.env.NEXT_PUBLIC_SLACK_CLIENT_ID)
    .replace('$SLACK_STATE$', encodeBase64(JSON.stringify(state)))
    .replace('$SLACK_REDIRECT_URL$', encodeURIComponent(`${getPublicSiteUrl()}/api/slack/v1/callback`));

  const [baseUrl, queryString] = originalUrl.split('?');
  const params = new URLSearchParams(queryString);
  const encodedParams = Array.from(params).map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
  const encodedUrl = `${baseUrl}?${encodedParams.join('&')}`;
  const html = parameterizedHtml.replace('$URL$', encodedUrl);

  return <div dangerouslySetInnerHTML={{ __html: html }} />;
};

export default memo(SlackButton);
