import { SingleChoiceLocation } from '@/components/location/locationSelector';
import { OrganizationMemberSelector } from '@/components/organization';
import type { addTeam_addTeamMutation } from '@/queries/__generated__/addTeam_addTeamMutation.graphql';
import type { addTeam_completeTeamOnboardingMutation } from '@/queries/__generated__/addTeam_completeTeamOnboardingMutation.graphql';
import type { addTeam_rootQuery } from '@/queries/__generated__/addTeam_rootQuery.graphql';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import { FormStackColumn, StackRow } from '@repo/shared/components/commons';
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
import { maxScreenWidth } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<addTeam_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
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
    ...singleChoiceLocation_locations_query
  }
`;

type TeamDetails = {
  name: string;
  about: string | null;
  timezone: string | null;
  organizationMemberIds: string[];
  primaryLocationId?: string;
};

const teamSchema = object({
  name: string().min(3, 'Team name must be at least three charcters long.').required('Team name is required'),
  about: string().nullable(),
  timezone: string().nullable(),
  organizationMemberIds: array().nullable(),
  primaryLocationId: string().nullable(),
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

  const handleTeamCreateClick = ({ name, about, timezone, organizationMemberIds, primaryLocationId }: TeamDetails) => {
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
          primaryLocationId,
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
    <Paper sx={{ padding: 2, maxWidth: maxScreenWidth }}>
      <Form
        onSubmit={handleTeamCreateClick}
        initialValues={{
          name: '',
          about: null,
          organizationMemberIds: [],
          primaryLocationId: null,
        }}
        validate={validate}
        render={({ handleSubmit }) => (
          <FormStackColumn onSubmit={handleSubmit}>
            <TextField label="Name" name="name" required={requiredFields.name} />
            <TextField label="About" name="about" required={requiredFields.about} multiline={true} />
            <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />

            {organizationId && (
              <SingleChoiceLocation
                rootDataRelay={rootData}
                id="primaryLocationId"
                required={requiredFields.primaryLocationId}
                label="Primary Location"
              />
            )}

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

            <StackRow sx={{ justifyContent: 'flex-end' }}>
              <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                {cancelButtonText ?? 'Cancel'}
              </Button>
              <Button color="primary" variant="contained" type="submit">
                Create
              </Button>
            </StackRow>
          </FormStackColumn>
        )}
      />
    </Paper>
  );
};

const MemoAddTeam = memo(AddTeam);

type RelayProps = {
  organizationId?: string;
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

      onReloadRequired();
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
