import { app } from '@microsoft/teams-js';
import { useTeamsUserCredential } from '@microsoft/teamsfx-react';
import Home from 'app';
import Settings from 'app/settings';
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
    element: <Home />,
  },
  {
    path: '/settings',
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
  );
};

export default App;
