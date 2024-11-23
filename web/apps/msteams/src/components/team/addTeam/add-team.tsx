import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { Loading } from '@repo/shared/components/loading';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationMemberSelector } from 'components/organization';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';
import type { addTeam_addTeamMutation } from './__generated__/addTeam_addTeamMutation.graphql';
import type { addTeam_completeTeamOnboardingMutation } from './__generated__/addTeam_completeTeamOnboardingMutation.graphql';
import type { addTeam_rootQuery } from './__generated__/addTeam_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<addTeam_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  onAdded: (id: string) => void;
  onCancelled: () => void;
  cancelButtonText?: string;
};

const RootQuery = graphql`
  query addTeam_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $bookingPeopleNameSearchText: String
    $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    me {
      id
    }
    organization(id: $organizationId) @include(if: $organizationExists) {
      id
      name
    }
    ...organizationMemberSelector_query
  }
`;

type TeamDetails = {
  name: string;
  about: string | null;
  timezone: string | null;
  organizationMemberIds: string[];
};

const teamSchema = object({
  name: string().min(3, 'Team name must be at least three charcters long.').required('Team name is required'),
  about: string().nullable(),
  timezone: string().nullable(),
  organizationMemberIds: array().nullable(),
});

const AddTeam = ({ queryReference, onReloadRequired, organizationId, onAdded, onCancelled, cancelButtonText }: Props) => {
  const rootData = usePreloadedQuery<addTeam_rootQuery>(RootQuery, queryReference);
  const [commitAddTeam] = useMutation<addTeam_addTeamMutation>(graphql`
    mutation addTeam_addTeamMutation($input: AddTeamInput!) @raw_response_type {
      addTeam(input: $input) {
        team {
          id
          name
          about
          timezone
        }
      }
    }
  `);

  const [commitCompleteTeamOnboarding] = useMutation<addTeam_completeTeamOnboardingMutation>(graphql`
    mutation addTeam_completeTeamOnboardingMutation($input: CompleteTeamOnboardingInput!) {
      completeTeamOnboarding(input: $input) {
        clientMutationId
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(teamSchema);
  const requiredFields = makeRequired(teamSchema);

  const handleCancelClick = () => {
    commitCompleteTeamOnboarding({
      variables: {
        input: {
          clientMutationId: nanoid(),
        },
      },
      onCompleted: () => {
        onCancelled();
        onReloadRequired();
      },
      onError: (_) => {
        onCancelled();
        onReloadRequired();
      },
    });
  };

  const handleTeamCreateClick = ({ name, about, timezone, organizationMemberIds }: TeamDetails) => {
    if (!rootData.me) {
      return;
    }

    const id = nanoid();
    const customerIds = !organizationId ? [rootData.me.id] : [];
    const toastId = themedToast(<NotificationContent content={`Adding team '${name}'...`} />, infoNotificationOptions);

    commitAddTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
          name,
          about,
          timezone,
          customerIds,
          organizationId,
          organizationMemberIds: [...new Set(organizationMemberIds)],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add new team '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        commitCompleteTeamOnboarding({
          variables: {
            input: {
              clientMutationId: nanoid(),
            },
          },
          onCompleted: (_, errors) => {
            if (errors && errors.length > 0) {
              toast.update(toastId, {
                ...errorNotificationOptions,
                render: <NotificationContent content={`Failed to complete team onboarding. Error: ${joinErrors(errors)}.`} />,
              });
            } else {
              toast.update(toastId, {
                ...successNotificationOptions,
                render: <NotificationContent content={`Team ${name} added.`} />,
              });

              onAdded(id);
              onReloadRequired();
            }
          },
          onError: (error) => {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to complete team onboarding. Error: ${error.message}.`} />,
            });
          },
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add new team '${name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addTeam: {
          team: {
            id,
            name,
            about,
            timezone,
          },
        },
      },
    });
  };

  if (!rootData.me) {
    return <></>;
  }

  return (
    <Paper elevation={24} sx={{ padding: 2 }}>
      <Form
        onSubmit={handleTeamCreateClick}
        initialValues={{
          name: '',
          about: null,
          organizationMemberIds: [],
        }}
        validate={validate}
        render={({ handleSubmit }) => (
          <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
            <TextField label="Name" name="name" required={requiredFields.name} />
            <TextField label="About" name="about" required={requiredFields.about} multiline={true} />
            <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />

            {organizationId && (
              <OrganizationMemberSelector
                rootDataRelay={rootData}
                organizationId={organizationId}
                name="organizationMemberIds"
                required={requiredFields.organizationMemberIds}
                multiple={true}
                useMemberId={true}
              />
            )}

            <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
              <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                {cancelButtonText ?? 'Cancel'}
              </Button>
              <Button color="primary" variant="contained" type="submit">
                Create
              </Button>
            </Stack>
          </Stack>
        )}
      />
    </Paper>
  );
};

const MemoAddTeam = memo(AddTeam);

type RelayProps = {
  organizationId: string;
  onReloadRequired: () => void;
  onAdded: (id: string) => void;
  onCancelled: () => void;
  cancelButtonText?: string;
};

const AddTeamWithRelay = ({ organizationId, onReloadRequired, onAdded, onCancelled, cancelButtonText }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addTeam_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId: organizationId ?? '',
        organizationExists: !!organizationId,
        organizationMemberSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

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
      <MemoAddTeam
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
        onAdded={onAdded}
        onCancelled={onCancelled}
        cancelButtonText={cancelButtonText}
      />
    </ErrorBoundary>
  );
};

export default memo(AddTeamWithRelay);
