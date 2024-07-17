import { TeamsFxContext } from 'libs/providers';
import { useContext } from 'react';

const Home = () => {
  const { themeString } = useContext(TeamsFxContext);
  return (
    <div className={themeString === 'default' ? 'light' : themeString === 'dark' ? 'dark' : 'contrast'}>
      <h1>Testing home page</h1>
    </div>
  );
};

export default Home;
