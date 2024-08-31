import { app } from '@microsoft/teams-js';
import { useTeamsUserCredential } from '@microsoft/teamsfx-react';
import { MuiXLicense } from '@repo/shared/libs/mui';
import { Analytics } from '@vercel/analytics/react';
import { SpeedInsights } from '@vercel/speed-insights/react';
import Root from 'app';
import Install from 'app/install';
import Locations from 'app/locations';
import Notifications from 'app/notifications';
import Organization from 'app/organizations/organization';
import AddOrganizationLocation from 'app/organizations/organization/locations/add';
import OrganizationLocation from 'app/organizations/organization/locations/location';
import AddOrganizationTeam from 'app/organizations/organization/teams/add';
import OrganizationTeam from 'app/organizations/organization/teams/team';
import Settings from 'app/settings';
import Teams from 'app/teams';
import {
  ColorModeProvider,
  DatePickerLocalizationProvider,
  LogRocketProvider,
  RelayProvider,
  SnackbarProvider,
  TeamsFxContext,
  ThemeProvider,
} from 'libs/providers';
import { useEffect, useState } from 'react';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import './App.css';

const router = createBrowserRouter([
  {
    path: '/',
    element: <Root />,
  },
  {
    path: '/install',
    element: <Install />,
  },
  {
    path: '/organization/:organizationId',
    element: <Organization />,
  },
  {
    path: '/organization/:organizationId/location',
    element: <Locations />,
  },
  {
    path: '/organization/:organizationId/location/add',
    element: <AddOrganizationLocation />,
  },
  {
    path: '/organization/:organizationId/location/:locationId',
    element: <OrganizationLocation />,
  },
  {
    path: '/organization/:organizationId/team',
    element: <Teams />,
  },
  {
    path: '/organization/:organizationId/team/add',
    element: <AddOrganizationTeam />,
  },
  {
    path: '/organization/:organizationId/team/:teamId',
    element: <OrganizationTeam />,
  },
  {
    path: '/:organizationId/notification',
    element: <Notifications />,
  },
  {
    path: '/:organizationId/settings',
    element: <Settings />,
  },
]);

const App = () => {
  const [token, setToken] = useState<string | null>(null);
  const { loading, theme, themeString, teamsUserCredential } = useTeamsUserCredential({
    initiateLoginEndpoint: new URL('auth-start.html', process.env.REACT_APP_BASE_URL).href,
    clientId: process.env.REACT_APP_APPLICATION_REGISTRATION_ID!,
  });

  useEffect(() => {
    const appInitialize = async () => {
      if (!loading) {
        return;
      }

      await app.initialize();
      app.notifySuccess();

      if (!teamsUserCredential) {
        return;
      }

      try {
        const accessToken = await teamsUserCredential!.getToken([]);
        if (!accessToken) {
          throw new Error('Access token is null');
        }

        setToken(accessToken.token);
      } catch {
        try {
          await teamsUserCredential!.login([]);
          const accessToken = await teamsUserCredential!.getToken([]);
          if (!accessToken) {
            throw new Error('Access token is null');
          }

          setToken(accessToken.token);
        } catch (error) {
          console.log(error);
        }
      }
    };

    appInitialize();
  }, [loading, teamsUserCredential]);

  return (
    <>
      <TeamsFxContext.Provider value={{ theme, themeString, teamsUserCredential }}>
        <ColorModeProvider loadDefaultSystemMode={false}>
          <ThemeProvider mode={themeString === 'dark' ? 'dark' : 'light'}>
            <SnackbarProvider>
              <DatePickerLocalizationProvider>
                <LogRocketProvider logRocketAppId={process.env.REACT_APP_LOGROCKET_APP_ID!}>
                  <RelayProvider token={token}>
                    <RouterProvider router={router} />
                  </RelayProvider>
                </LogRocketProvider>
              </DatePickerLocalizationProvider>
            </SnackbarProvider>
          </ThemeProvider>
        </ColorModeProvider>
      </TeamsFxContext.Provider>
      <Analytics />
      <SpeedInsights />
      <MuiXLicense />
    </>
  );
};

export default App;
