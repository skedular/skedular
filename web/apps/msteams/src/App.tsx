import { FluentProvider, teamsDarkTheme, teamsHighContrastTheme, teamsLightTheme, tokens } from '@fluentui/react-components';
import { app } from '@microsoft/teams-js';
import { useTeamsUserCredential } from '@microsoft/teamsfx-react';
import Home from 'app';
import { TeamsFxContext } from 'libs/providers';
import { useEffect } from 'react';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import './App.css';

const router = createBrowserRouter([
  {
    path: '/',
    element: <Home />,
  },
]);

const App = () => {
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

      let consentNeeded = false;

      try {
        let x = process.env.REACT_APP_BASE_URL;
        let y = process.env.REACT_APP_APPLICATION_REGISTRATION_ID;
      
        console.log(x)
        console.log(y)
      
        await teamsUserCredential!.getToken(['User.Read', 'User.ReadBasic.All']);
      } catch (error) {
        consentNeeded = true;
        try {
          await teamsUserCredential!.login(['User.Read', 'User.ReadBasic.All']);
        } catch (error1) {
          console.log(error1)
        }
      }
    };
    appInitialize();
  }, [loading, teamsUserCredential]);

  return (
    <TeamsFxContext.Provider value={{ theme, themeString, teamsUserCredential }}>
      <FluentProvider
        theme={
          themeString === 'dark'
            ? teamsDarkTheme
            : themeString === 'contrast'
              ? teamsHighContrastTheme
              : {
                  ...teamsLightTheme,
                  colorNeutralBackground3: '#eeeeee',
                }
        }
        style={{ background: tokens.colorNeutralBackground3 }}
      >
        <RouterProvider router={router} />
      </FluentProvider>
    </TeamsFxContext.Provider>
  );
};

export default App;
