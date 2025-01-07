import type { locationAboutTab_rootQuery } from '@/queries/__generated__/locationAboutTab_rootQuery.graphql';
import type { locationAboutTab_updateLocationMutation } from '@/queries/__generated__/locationAboutTab_updateLocationMutation.graphql';
import { FormFieldLabel, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
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
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<locationAboutTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
};

const RootQuery = graphql`
  query locationAboutTab_rootQuery($locationId: String!) {
    location(id: $locationId) {
      id
      name
      about
      timezone
      physicalAddress {
        formattedAddress
      }
      organization {
        name
      }
      canModify
    }
  }
`;

type LocationDetails = {
  name: string;
  about: string | null;
  timezone: string;
  physicalAddress: string;
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
  physicalAddress: string().nullable(),
});

const LocationAboutTab = ({ queryReference, organizationId }: Props) => {
  const rootData = usePreloadedQuery<locationAboutTab_rootQuery>(RootQuery, queryReference);
  const [commitUpdateLocation] = useMutation<locationAboutTab_updateLocationMutation>(graphql`
    mutation locationAboutTab_updateLocationMutation($input: UpdateLocationInput!) @raw_response_type {
      updateLocation(input: $input) {
        location {
          id
          name
          about
          timezone
          physicalAddress {
            formattedAddress
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);

  const handleLocationUpdateClick = ({ name, about, timezone, physicalAddress }: LocationDetails) => {
    if (!rootData.location) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating location '${rootData.location.name}'...`} />, infoNotificationOptions);

    commitUpdateLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.location.id,
          name,
          about,
          timezone,
          organizationId,
          physicalAddress: {
            formattedAddress: physicalAddress,
          },
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update location '${rootData.location?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location ${name} details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update location '${rootData.location?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateLocation: {
          location: {
            id: rootData.location.id,
            name,
            about,
            timezone,
            physicalAddress: {
              formattedAddress: physicalAddress,
            },
          },
        },
      },
    });
  };

  if (!rootData.location) {
    return null;
  }

  const location = rootData.location;

  return (
    <Form
      onSubmit={handleLocationUpdateClick}
      initialValues={{
        name: location.name,
        about: location.about,
        timezone: location.timezone,
        physicalAddress: location.physicalAddress?.formattedAddress,
      }}
      validate={validate}
      render={({ handleSubmit }) => (
        <FormStackColumn onSubmit={handleSubmit}>
          <FormFieldLabel label="Name">
            <TextField name="name" required={requiredFields.name} />
          </FormFieldLabel>

          <FormFieldLabel label="About">
            <TextField name="about" required={requiredFields.about} multiline rows={3} />
          </FormFieldLabel>

          <FormFieldLabel label="Timezone">
            <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
          </FormFieldLabel>

          <FormFieldLabel label="Physical Address">
            <TextField name="physicalAddress" required={requiredFields.physicalAddress} multiline rows={5} />
          </FormFieldLabel>

          <TwoButtonsDialogActions primaryLabel="Update" hideSecondary />
        </FormStackColumn>
      )}
    />
  );
};

const MemoLocationAboutTab = memo(LocationAboutTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
};

const LocationAboutTabWithRelay = ({ onReloadRequired, organizationId, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationAboutTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId]);

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
      <MemoLocationAboutTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationAboutTabWithRelay);
