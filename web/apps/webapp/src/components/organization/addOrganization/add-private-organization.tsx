import { BodyIconTypography, FormFieldLabel, FormStackColumn, HelperText, PushToRight, StackRow } from '@/components/commons';
import { AnalyticsIcon, CalendarIcon } from '@/components/icons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { OrganizationTermsOfUse } from '@/components/organization';
import { FeatureBox, LeftSidePanel, RightSidePanel, TwoSideVerticalWizard } from '@/components/wizard';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { addPrivateOrganization_addOrganizationMutation } from '@/queries/__generated__/addPrivateOrganization_addOrganizationMutation.graphql';
import type { addPrivateOrganization_query$key } from '@/queries/__generated__/addPrivateOrganization_query.graphql';
import GroupsIcon from '@mui/icons-material/Groups';
import LocationCityIcon from '@mui/icons-material/LocationCity';
import LockIcon from '@mui/icons-material/Lock';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { Checkboxes, makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { boolean, object, string } from 'yup';

type Props = {
  rootDataRelay: addPrivateOrganization_query$key;
  onReloadRequired: () => void;
  onAdded: (id: string) => void;
  onCancel?: () => void;
  cancelLabel?: string;
  createLabel?: string;
};

type OrganizationDetails = {
  uniqueAlphanumericName: string | null;
  isListable: boolean;
  name: string;
  about: string | null;
  website: string | null;
  agreedToTermsOfUse: boolean;
};

const organizationSchema = object({
  uniqueAlphanumericName: string().nullable(),
  isListable: boolean().required(),
  name: string().min(3, 'Organization name must be at least three characters long.').required('Organization name is required'),
  about: string().nullable(),
  website: string().nullable(),
  agreedToTermsOfUse: boolean().oneOf([true], 'Please accept the terms').required('Please accept the terms'),
});

const AddPrivateOrganization = ({ rootDataRelay, onReloadRequired, onAdded, onCancel, cancelLabel, createLabel }: Props) => {
  const rootData = useFragment<addPrivateOrganization_query$key>(
    graphql`
      fragment addPrivateOrganization_query on Query {
        me {
          emails
        }
        activeOrganizationTermsOfUse {
          id
        }
        ...organizationTermsOfUse_query
      }
    `,
    rootDataRelay,
  );

  const [commitAddOrganization] = useMutation<addPrivateOrganization_addOrganizationMutation>(graphql`
    mutation addPrivateOrganization_addOrganizationMutation($input: AddOrganizationInput!) @raw_response_type {
      addOrganization(input: $input) {
        organization {
          id
          uniqueAlphanumericName
          isListable
          name
          about
          website
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateOrganizationDetails = makeValidate(organizationSchema);
  const requiredFields = makeRequired(organizationSchema);

  const handleOrganizationAddClick = ({ uniqueAlphanumericName, isListable, name, about, website }: OrganizationDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding organization '${name}'...`} />, infoNotificationOptions);

    commitAddOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          uniqueAlphanumericName,
          isListable,
          name,
          about,
          website,
          type: 'PRIVATE',
          agreedToTermsOfUse: true,
          termsOfUseId: rootData.activeOrganizationTermsOfUse.id,
          industrySubCategoryIds: [],
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add new organization '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${name} added.`} />,
        });

        onAdded(response.addOrganization.organization.uniqueAlphanumericName!);
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add new organization '${name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addOrganization: {
          organization: {
            id,
            uniqueAlphanumericName,
            isListable,
            name,
            about,
            website,
          },
        },
      },
    });
  };

  return (
    <TwoSideVerticalWizard>
      <LeftSidePanel
        title="Manage Your Private Workspace with Full Control"
        description="Create a dedicated environment for your enterprise, manage teams, locations, and resources all in one place."
      >
        <FeatureBox
          icon={<LocationCityIcon sx={{ color: '#5C6BC0', fontSize: 40 }} />}
          title="Multi-location support"
          subtitle="Manage desks, meeting rooms, and zones across different offices."
        />
        <FeatureBox
          icon={<GroupsIcon sx={{ color: '#42A5F5', fontSize: 40 }} />}
          title="Team & role management"
          subtitle="Invite users, assign roles, and control access securely."
        />
        <FeatureBox
          icon={<CalendarIcon sx={{ color: '#66BB6A', fontSize: 40 }} />}
          title="Smart scheduling tools"
          subtitle="Enable frictionless booking of spaces with availability and conflict handling."
        />
        <FeatureBox
          icon={<AnalyticsIcon sx={{ color: '#FFA726', fontSize: 40 }} />}
          title="Workspace insights"
          subtitle="Understand usage patterns and optimize resource allocation."
        />
        <FeatureBox icon={<LockIcon sx={{ color: '#EF5350', fontSize: 40 }} />} title="Private & secure" subtitle="Your organization data is isolated and protected." />
      </LeftSidePanel>

      <RightSidePanel
        title="Set Up Your Organization"
        description="Tell us a bit about your company so we can tailor your workspace experience. We'll guide you through setting up locations, teams, and resources step by step."
      >
        <Form
          onSubmit={handleOrganizationAddClick}
          initialValues={{
            uniqueAlphanumericName: null,
            isListable: true,
            name: '',
            about: null,
            website: null,
          }}
          validate={validateOrganizationDetails}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <Divider />

              <FormFieldLabel label="Name" required={requiredFields.name}>
                <TextField
                  name="name"
                  required={requiredFields.name}
                  helperText={
                    <HelperText text="This will be used as the primary name for your organization across the platform. Choose a recognizable name your team will expect." />
                  }
                />
              </FormFieldLabel>

              {rootData.me.emails.some((item) => item.toLocaleLowerCase() === 'morteza.alizadeh@gmail.com' || item.toLocaleLowerCase() === 'leila.alavi78@gmail.com') && (
                <>
                  <FormFieldLabel label="Unique Name" required={requiredFields.uniqueAlphanumericName}>
                    <TextField name="uniqueAlphanumericName" required={requiredFields.uniqueAlphanumericName} />
                  </FormFieldLabel>

                  <FormFieldLabel label="" required={requiredFields.isListable}>
                    <Checkboxes name="isListable" required={requiredFields.isListable} data={{ label: 'Is listable?', value: true }} />
                  </FormFieldLabel>
                </>
              )}

              <FormFieldLabel label="About" required={requiredFields.about}>
                <TextField
                  name="about"
                  required={requiredFields.about}
                  multiline
                  rows={3}
                  helperText={
                    <HelperText text="Briefly describe what your organization does. This helps coworkers and team members understand your company's focus and purpose." />
                  }
                />
              </FormFieldLabel>

              <FormFieldLabel label="Website" required={requiredFields.website}>
                <TextField
                  name="website"
                  required={requiredFields.website}
                  helperText={<HelperText text="Provide your company's official website so members can learn more or verify your organization." />}
                />
              </FormFieldLabel>

              <FormFieldLabel label="" required={requiredFields.agreedToTermsOfUse}>
                <OrganizationTermsOfUse rootDataRelay={rootData} name="agreedToTermsOfUse" required={requiredFields.agreedToTermsOfUse} />
              </FormFieldLabel>

              <StackRow>
                <Button variant="contained" sx={defaultButtonStyle} onClick={onCancel}>
                  <BodyIconTypography label={cancelLabel ?? 'Cancel'} invertDefaultColor={paletteMode === 'dark'} />
                </Button>
                <PushToRight />
                <Button variant="contained" type="submit" sx={{ textTransform: 'none' }} color="primary">
                  <BodyIconTypography label={createLabel ?? 'Create'} invertDefaultColor={paletteMode === 'dark'} />
                </Button>
              </StackRow>
            </FormStackColumn>
          )}
        />
      </RightSidePanel>
    </TwoSideVerticalWizard>
  );
};

export default memo(AddPrivateOrganization);
