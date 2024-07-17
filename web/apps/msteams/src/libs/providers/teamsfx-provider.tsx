import { Theme } from '@fluentui/react-components';
import { TeamsUserCredential } from '@microsoft/teamsfx';
import { createContext } from 'react';

const TeamsFxContext = createContext<{
  theme?: Theme;
  themeString: string;
  teamsUserCredential?: TeamsUserCredential;
}>({
  theme: undefined,
  themeString: '',
  teamsUserCredential: undefined,
});

export default TeamsFxContext;
