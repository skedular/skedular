import type { organizationTeam_rootQuery } from '@/queries/__generated__/organizationTeam_rootQuery.graphql';
import type { organizationTeam_updateTeamMutation } from '@/queries/__generated__/organizationTeam_updateTeamMutation.graphql';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Toolbar from '@mui/material/Toolbar';
import {
  BodyIconTypography,
  FormStackColumn,
  LeadIconTypography,
  PushToRight,
  SectionIconTypography,
  SmallHeadingIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { coal, defaultPadding, maxScreenWidth, sandstone } from '@repo/shared/libs/theme';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useRef, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { object, string } from 'yup';
import { expandedDrawerWidthPx } from './commons';
import OrganizationTeamLeftSideNavigationMenuContent from './organization-team-left-side-navigation-menu-content';

type Props = {
  queryReference: PreloadedQuery<organizationTeam_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  teamId: string;
};

type TeamDetails = {
  name: string;
  about: string | null;
  timezone: string;
  primaryLocationId?: string;
};

const teamSchema = object({
  name: string().min(3, 'Team name must be at least three charcters long.').required('Team name is required'),
  about: string().nullable(),
  timezone: string().nullable(),
  primaryLocationId: string().nullable(),
});

const RootQuery = graphql`
  query organizationTeam_rootQuery($teamId: String!) {
    team(id: $teamId) {
      id
      name
      about
    }
  }
`;

const OrganizationTeam = ({ queryReference, organizationId, teamId }: Props) => {
  const rootData = usePreloadedQuery<organizationTeam_rootQuery>(RootQuery, queryReference);

  const [commitUpdateTeam] = useMutation<organizationTeam_updateTeamMutation>(graphql`
    mutation organizationTeam_updateTeamMutation($input: UpdateTeamInput!) @raw_response_type {
      updateTeam(input: $input) {
        team {
          id
          name
          about
          timezone
          organization {
            name
          }
          primaryLocation {
            uniqueId
            name
          }
          members {
            customer {
              uniqueId
            }
            organizationMember {
              uniqueId
            }
          }
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validate = makeValidate(teamSchema);
  const requiredFields = makeRequired(teamSchema);

  useEffect(() => {
    if (!section) {
      return;
    }

    const element = sectionRefs.current[section];
    if (!element) {
      return;
    }

    const appBarHeight = document.querySelector('.app-bar')?.clientHeight || 0;
    const elementTop = element.getBoundingClientRect().top + window.scrollY;
    window.scrollTo({
      top: elementTop - appBarHeight,
      behavior: 'smooth',
    });
  }, [section]);

  const handleDetailUpdateClick = () => {};

  const handleCancelClick = () => {
    router.back();
  };

  if (!rootData.team) {
    return null;
  }

  const team = rootData.team;

  return (
    <Box sx={{ display: 'flex' }}>
      <OrganizationTeamLeftSideNavigationMenuContent organizationId={organizationId} teamId={teamId} hideIcons />
      <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
        <AppBar position="sticky">
          <Toolbar
            sx={{
              backgroundColor: paletteMode === 'dark' ? sandstone : coal,
              borderBottom: paletteMode === 'dark' ? 1 : undefined,
              borderColor: (theme) => theme.palette.divider,
            }}
          >
            <SmallHeadingIconTypography label="Edit Team Information" invertDefaultColor />

            <PushToRight />
            <StackRow>
              <Button
                sx={{
                  border: 1,
                  borderColor: paletteMode === 'dark' ? coal : sandstone,
                  textTransform: 'none',
                  '&:hover': {
                    backgroundColor: 'inherit',
                  },
                }}
                variant="contained"
                color="inherit"
                onClick={handleCancelClick}
              >
                <SmallIconTypography label="Cancel" invertDefaultColor />
              </Button>

              <Button variant="contained" color="primary" sx={{ textTransform: 'none' }}>
                <SmallIconTypography label="Save & Exit" />
              </Button>
            </StackRow>
          </Toolbar>
        </AppBar>
        <StackColumn sx={{ maxWidth: maxScreenWidth }}>
          <StackColumn
            sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
            ref={(divElement) => {
              sectionRefs.current['setup'] = divElement;
            }}
          >
            <SectionIconTypography label="Team Setup" />
            <BodyIconTypography label="Edit your team name and details" />
            <Divider />
          </StackColumn>

          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
            <LeadIconTypography label="Team" />
            <Form
              onSubmit={handleDetailUpdateClick}
              initialValues={{
                name: team.name,
                about: team.about,
              }}
              validate={validate}
              render={({ handleSubmit }) => (
                <FormStackColumn onSubmit={handleSubmit}>
                  <TextField label="Name" name="name" required={requiredFields.name} />
                  <TextField label="About" name="about" required={requiredFields.about} multiline={true} />
                </FormStackColumn>
              )}
            />
          </StackColumn>

          <StackColumn
            sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
            ref={(divElement) => {
              sectionRefs.current['members'] = divElement;
            }}
          >
            <SectionIconTypography label="Team Members" />
            <BodyIconTypography label="Manage your team members" />
            <Divider />
          </StackColumn>
        </StackColumn>
      </Box>
    </Box>
  );
};

const MemoOrganizationTeam = memo(OrganizationTeam);

type RelayProps = {
  organizationId: string;
  teamId: string;
};

const OrganizationTeamWithRelay = ({ organizationId, teamId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationTeam_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        teamId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, teamId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationTeam queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} teamId={teamId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationTeamWithRelay);
