import { FormFieldLabel, FormStackColumnWithSaveCancelExitAppBar } from '@repo/shared/components/commons';
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
import { SingleChoiceLocation } from 'components/location/locationSelector';
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
  onCancel: () => void;
  cancelButtonText?: string;
};

const RootQuery = graphql`
  query addTeam_rootQuery(
    $organizationId: String!
    $bookingPeopleNameSearchText: String
    $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    me {
      id
    }
    organization(id: $organizationId) {
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

const AddTeam = ({ queryReference, onReloadRequired, organizationId, onAdded, onCancel, cancelButtonText }: Props) => {
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
        onCancel();
        onReloadRequired();
      },
      onError: (_) => {
        onCancel();
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
    <Form
      onSubmit={handleTeamCreateClick}
      initialValues={{
        name: '',
        about: null,
        organizationMemberIds: [],
      }}
      validate={validate}
      render={({ handleSubmit }) => (
        <FormStackColumnWithSaveCancelExitAppBar onSubmit={handleSubmit} onCancel={handleCancelClick} label="Add Team">
          <FormFieldLabel label="Name">
            <TextField name="name" required={requiredFields.name} />
          </FormFieldLabel>

          <FormFieldLabel label="About">
            <TextField name="about" required={requiredFields.about} multiline={true} />
          </FormFieldLabel>

          <FormFieldLabel label="Timezone">
            <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
          </FormFieldLabel>

          <FormFieldLabel label="Primary Location">
            <SingleChoiceLocation rootDataRelay={rootData} id="primaryLocationId" required={requiredFields.primaryLocationId} />
          </FormFieldLabel>

          <FormFieldLabel label="Organization Member">
            <OrganizationMemberSelector
              rootDataRelay={rootData}
              name="organizationMemberIds"
              required={requiredFields.organizationMemberIds}
              multiple={true}
              useMemberId={true}
            />
          </FormFieldLabel>
        </FormStackColumnWithSaveCancelExitAppBar>
      )}
    />
  );
};

const MemoAddTeam = memo(AddTeam);

type RelayProps = {
  organizationId: string;
  onReloadRequired: () => void;
  onAdded: (id: string) => void;
  onCancel: () => void;
  cancelButtonText?: string;
};

const AddTeamWithRelay = ({ organizationId, onReloadRequired, onAdded, onCancel, cancelButtonText }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addTeam_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
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
        onCancel={onCancel}
        cancelButtonText={cancelButtonText}
      />
    </ErrorBoundary>
  );
};

export default memo(AddTeamWithRelay);
