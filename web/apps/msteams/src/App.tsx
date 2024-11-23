import { app } from '@microsoft/teams-js';
import { useTeamsUserCredential } from '@microsoft/teamsfx-react';
import CssBaseline from '@mui/material/CssBaseline';
import { MuiXLicense } from '@repo/shared/libs/mui';
import { DatePickerLocalizationProvider, GlobalReloadIdProvider, ThemeProvider } from '@repo/shared/libs/providers';
import { Analytics } from '@vercel/analytics/react';
import { SpeedInsights } from '@vercel/speed-insights/react';
import { Home } from 'app/';
import { Install } from 'app/install';
import { Locations } from 'app/locations';
import { Notifications } from 'app/notifications';
import { Organization } from 'app/organizations/organization';
import { AddOrganizationLocation } from 'app/organizations/organization/locations/add';
import { OrganizationLocation } from 'app/organizations/organization/locations/location';
import { AddOrganizationTeam } from 'app/organizations/organization/teams/add';
import { OrganizationTeam } from 'app/organizations/organization/teams/team';
import { Settings } from 'app/settings';
import { StartInstall } from 'app/start-install';
import { Teams } from 'app/teams';
import { LogRocketProvider, RelayProvider, TeamsFxContext } from 'libs/providers';
import { useEffect, useState } from 'react';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './App.css';

const router = createBrowserRouter([
  {
    path: '/',
    element: <Home />,
  },
  {
    path: '/install',
    element: <Install />,
  },
  {
    path: '/start-install',
    element: <StartInstall />,
  },
  {
    path: '/organizations/:organizationId',
    element: <Organization />,
  },
  {
    path: '/organizations/:organizationId/locations',
    element: <Locations />,
  },
  {
    path: '/organizations/:organizationId/locations/add',
    element: <AddOrganizationLocation />,
  },
  {
    path: '/organizations/:organizationId/locations/:locationId',
    element: <OrganizationLocation />,
  },
  {
    path: '/organizations/:organizationId/teams',
    element: <Teams />,
  },
  {
    path: '/organizations/:organizationId/teams/add',
    element: <AddOrganizationTeam />,
  },
  {
    path: '/organizations/:organizationId/teams/:teamId',
    element: <OrganizationTeam />,
  },
  {
    path: '/:organizationId/notifications',
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
        <GlobalReloadIdProvider>
          <ThemeProvider mode={themeString === 'dark' ? 'dark' : 'light'}>
            <CssBaseline />
            <DatePickerLocalizationProvider>
              <RelayProvider token={token}>
                <RouterProvider router={router} />
              </RelayProvider>
            </DatePickerLocalizationProvider>
          </ThemeProvider>
        </GlobalReloadIdProvider>
      </TeamsFxContext.Provider>
      <Analytics />
      <SpeedInsights />
      <MuiXLicense />
      <LogRocketProvider logRocketAppId={process.env.REACT_APP_LOGROCKET_APP_ID!} />
      <ToastContainer position="top-right" pauseOnFocusLoss pauseOnHover hideProgressBar={false} draggable rtl={false} />
    </>
  );
};

export default App;
