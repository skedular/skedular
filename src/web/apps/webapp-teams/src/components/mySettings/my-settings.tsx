import { CustomerAvatar } from '@/components/avatars';
import { SingleChoinceTimezone } from '@/components/forms';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { SingleChoiceUserPersonalInformationVisibility } from '@/components/user';
import type { mySettings_rootQuery } from '@/queries/__generated__/mySettings_rootQuery.graphql';
import type {
  CustomerDetailsPatchField,
  mySettings_updateCustomerDetailsMutation,
  PersonalInformationVisibility,
} from '@/queries/__generated__/mySettings_updateCustomerDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import { getCustomerFullName, getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import {
  CaptionIconTypography,
  defaultPadding,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  PageHeaderPanel,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<mySettings_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query mySettings_rootQuery {
    me {
      id
      email
      photoUrl
      designation
      title
      name
      givenName
      middleName
      familyName
      timezone
      phoneNumber
      personalInformationVisibility {
        type
        name
      }
    }
    ...singleChoiceUserPersonalInformationVisibility_query
  }
`;

type ProfileDetailsDetails = {
  designation: string | null;
  title: string | null;
  name: string | null;
  givenName: string | null;
  middleName: string | null;
  familyName: string | null;
  timezone: string;
  phoneNumber: string | null;
  personalInformationVisibility: string;
};

const profileDetailsSchema = object({
  designation: string().nullable(),
  title: string().nullable(),
  name: string().nullable(),
  givenName: string().nullable(),
  middleName: string().nullable(),
  familyName: string().nullable(),
  timezone: string().required('Timezone is required'),
  phoneNumber: string().nullable(),
  personalInformationVisibility: string().required('Personal Information Visibility is required'),
});

const formColumnSx = {
  width: '100%',
  maxWidth: 760,
};

const inlinePatchDebounceTimeout = 1000;

const profilePatchFields: Record<keyof ProfileDetailsDetails, CustomerDetailsPatchField> = {
  designation: 'DESIGNATION',
  title: 'TITLE',
  name: 'NAME',
  givenName: 'GIVEN_NAME',
  middleName: 'MIDDLE_NAME',
  familyName: 'FAMILY_NAME',
  timezone: 'TIMEZONE',
  phoneNumber: 'PHONE_NUMBER',
  personalInformationVisibility: 'PERSONAL_INFORMATION_VISIBILITY',
};

const getChangedProfileFields = (left: ProfileDetailsDetails, right: ProfileDetailsDetails) =>
  (Object.keys(profilePatchFields) as (keyof ProfileDetailsDetails)[]).filter((field) => left[field] !== right[field]).map((field) => profilePatchFields[field]);

const MySettings = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<mySettings_rootQuery>(RootQuery, queryReference);

  const [commitUpdateCustomerDetails] = useMutation<mySettings_updateCustomerDetailsMutation>(graphql`
    mutation mySettings_updateCustomerDetailsMutation($input: UpdateCustomerDetailsInput!) @raw_response_type {
      updateCustomerDetails(input: $input) {
        customer {
          id
          timezone
          designation
          title
          name
          givenName
          middleName
          familyName
          phoneNumber
          personalInformationVisibility {
            type
            name
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateProfileDetails = makeValidate(profileDetailsSchema);
  const requiredProfileDetailsFields = makeRequired(profileDetailsSchema);

  const me = rootData.me;
  const initialProfileValues = useMemo<ProfileDetailsDetails>(
    () => ({
      timezone: me.timezone ?? '',
      designation: me.designation ?? null,
      title: me.title ?? null,
      name: me.name ?? null,
      givenName: me.givenName ?? null,
      middleName: me.middleName ?? null,
      familyName: me.familyName ?? null,
      phoneNumber: me.phoneNumber ?? null,
      personalInformationVisibility: me.personalInformationVisibility.type,
    }),
    [me],
  );
  const draftProfileValues = useRef(initialProfileValues);
  const submittedProfileValues = useRef(initialProfileValues);

  const commitProfilePatch = useCallback(
    (fieldsToUpdate: CustomerDetailsPatchField[], values: ProfileDetailsDetails) => {
      if (fieldsToUpdate.length === 0 || !profileDetailsSchema.isValidSync(values)) {
        return;
      }

      const previousValues = submittedProfileValues.current;
      if (getChangedProfileFields(previousValues, values).length === 0) {
        return;
      }

      submittedProfileValues.current = values;

      commitUpdateCustomerDetails({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: me.id,
            fieldsToUpdate,
            ...values,
            personalInformationVisibility: values.personalInformationVisibility as PersonalInformationVisibility,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            submittedProfileValues.current = previousValues;
            themedToast(<NotificationContent content={`We couldn't update your profile details. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
            return;
          }

          themedToast(<NotificationContent content="Profile details saved." />, successNotificationOptions);
        },
        onError: (error) => {
          submittedProfileValues.current = previousValues;
          themedToast(<NotificationContent content={`We couldn't update your profile details. ${error.message}`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateCustomerDetails: {
            customer: {
              id: me.id,
              ...values,
              personalInformationVisibility: {
                type: values.personalInformationVisibility as PersonalInformationVisibility,
                name: '',
              },
            },
          },
        },
      });
    },
    [commitUpdateCustomerDetails, me.id, themedToast],
  );
  const debouncedCommitProfilePatch = useDebounceCallback(commitProfilePatch, inlinePatchDebounceTimeout);

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pt: { xs: 1, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn
        sx={{
          width: '100%',
          maxWidth: 1200,
          mx: 'auto',
          backgroundColor: 'transparent',
          gap: 2,
        }}
      >
        <PageHeaderPanel eyebrow="User settings" title="My Settings" description="Manage your profile details, contact information, timezone, and visibility settings.">
          <StackRow sx={{ alignItems: 'center', flexWrap: 'wrap', gap: 2 }}>
            <CustomerAvatar name={me} photo={{ url: me?.photoUrl }} size="large" />
            <StackColumn spacing={0.5}>
              <LeadIconTypography label={getCustomerFullName(me)} />
              <CaptionIconTypography label={me.email} />
            </StackColumn>
          </StackRow>
        </PageHeaderPanel>

        <Box
          sx={{
            borderRadius: 4,
            border: 1,
            borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
            bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
            boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
            overflow: 'hidden',
          }}
        >
          <Form
            onSubmit={() => undefined}
            initialValues={initialProfileValues}
            validate={validateProfileDetails}
            render={({ handleSubmit, values }) => {
              const formValues = values as ProfileDetailsDetails;
              const changedFields = getChangedProfileFields(draftProfileValues.current, formValues);
              if (changedFields.length > 0) {
                draftProfileValues.current = formValues;
                debouncedCommitProfilePatch(changedFields, formValues);
              }

              return (
                <FormStackColumn onSubmit={handleSubmit} sx={{ p: defaultPadding, ...formColumnSx }}>
                  <StackColumn spacing={2}>
                    <StackColumn spacing={0.5}>
                      <LeadIconTypography label="Profile" />
                      <SmallIconTypography label="Edit the details shown across your account and organizations." />
                    </StackColumn>

                    <Divider />

                    <FormFieldLabel label="Designation">
                      <TextField name="designation" required={requiredProfileDetailsFields.designation} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Title">
                      <TextField name="title" required={requiredProfileDetailsFields.title} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredProfileDetailsFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Given Name">
                      <TextField name="givenName" required={requiredProfileDetailsFields.givenName} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Middle Name">
                      <TextField name="middleName" required={requiredProfileDetailsFields.middleName} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Family Name">
                      <TextField name="familyName" required={requiredProfileDetailsFields.familyName} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Timezone">
                      <SingleChoinceTimezone name="timezone" required={requiredProfileDetailsFields.timezone} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Phone Number">
                      <TextField name="phoneNumber" required={requiredProfileDetailsFields.phoneNumber} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Personal Information Visibility" required={requiredProfileDetailsFields.personalInformationVisibility}>
                      <SingleChoiceUserPersonalInformationVisibility
                        rootDataRelay={rootData}
                        name="personalInformationVisibility"
                        required={requiredProfileDetailsFields.personalInformationVisibility}
                      />
                    </FormFieldLabel>
                  </StackColumn>
                </FormStackColumn>
              );
            }}
          />
        </Box>
      </StackColumn>
    </Box>
  );
};

const MemoMySettings = memo(MySettings);

type RelayProps = {
  onReloadRequired: () => void;
};

const MySettingsWithRelay = ({ onReloadRequired }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<mySettings_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoMySettings queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(MySettingsWithRelay);
