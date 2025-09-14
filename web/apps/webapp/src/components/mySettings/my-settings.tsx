import { CustomerAvatar } from '@/components/avatars';
import { AppBarWithStackColumn, CaptionIconTypography, FormFieldLabel, FormStackColumn, GridContainer, LeadIconTypography, StackColumn, StackRow } from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { getRootLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { SingleChoiceUserPersonalInformationVisibility } from '@/components/user';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getCustomerFullName, joinErrors } from '@/libs/utils';
import type { mySettings_rootQuery } from '@/queries/__generated__/mySettings_rootQuery.graphql';
import type { mySettings_updateCustomerDetailsMutation, PersonalInformationVisibility } from '@/queries/__generated__/mySettings_updateCustomerDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
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

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateProfileDetails = makeValidate(profileDetailsSchema);
  const requiredProfileDetailsFields = makeRequired(profileDetailsSchema);

  const handleCloseClick = () => {
    router.push(getRootLink(integratedPlatrform));
  };

  const handleProfileDetailUpdateClick = ({
    timezone,
    designation,
    title,
    name,
    givenName,
    middleName,
    familyName,
    phoneNumber,
    personalInformationVisibility,
  }: ProfileDetailsDetails) => {
    const toastId = themedToast(<NotificationContent content={`Updating user profile details'...`} />, infoNotificationOptions);

    commitUpdateCustomerDetails({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: rootData.me.id,
          timezone,
          designation,
          title,
          name,
          givenName,
          middleName,
          familyName,
          phoneNumber,
          personalInformationVisibility: personalInformationVisibility as PersonalInformationVisibility,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update user profile details. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`User profile details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update user profile details. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateCustomerDetails: {
          customer: {
            id: rootData.me.id,
            timezone,
            designation,
            title,
            name,
            givenName,
            middleName,
            familyName,
            phoneNumber,
            personalInformationVisibility: {
              type: personalInformationVisibility as PersonalInformationVisibility,
              name: '',
            },
          },
        },
      },
    });
  };

  const me = rootData.me;

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit My Settings">
          <Form
            onSubmit={handleProfileDetailUpdateClick}
            initialValues={{
              timezone: me.timezone ?? '',
              designation: me.designation,
              title: me.title,
              name: me.name,
              givenName: me.givenName,
              middleName: me.middleName,
              familyName: me.familyName,
              phoneNumber: me.phoneNumber,
              personalInformationVisibility: me.personalInformationVisibility.type,
            }}
            validate={validateProfileDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <GridContainer sx={{ justifyContent: 'space-between' }}>
                    <Grid>
                      <StackRow>
                        <CustomerAvatar name={me} photo={{ url: me?.photoUrl }} size="large" />
                        <StackColumn spacing={-0.5}>
                          <LeadIconTypography label={getCustomerFullName(me)} />
                          <CaptionIconTypography label={me.email} />
                        </StackColumn>
                      </StackRow>
                    </Grid>

                    <Grid></Grid>
                  </GridContainer>
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
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

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <StackRow>
                    <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                      Update
                    </Button>
                  </StackRow>
                </StackColumn>
              </FormStackColumn>
            )}
          />
        </AppBarWithStackColumn>
      </Box>
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
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoMySettings queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(MySettingsWithRelay);
