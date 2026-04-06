import { BodyIconTypography, FormFieldLabel, FormStackColumn, HelperText, PushToRight, StackRow } from '@/components/commons';
import { AnalyticsIcon, CalendarIcon } from '@/components/icons';
import { ListingMetadata, listingMetadataSchemaShape } from '@/components/listingMetadata';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { OrganizationTermsOfUse } from '@/components/organization';
import { FeatureBox, LeftSidePanel, RightSidePanel, TwoSideVerticalWizard } from '@/components/wizard';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle } from '@/libs/theme';
import { getRelayErrorMessage } from '@/libs/utils';
import type { addIndividualOrganization_addOrganizationMutation } from '@/queries/__generated__/addIndividualOrganization_addOrganizationMutation.graphql';
import type { addIndividualOrganization_query$key } from '@/queries/__generated__/addIndividualOrganization_query.graphql';
import GroupsIcon from '@mui/icons-material/Groups';
import LocationCityIcon from '@mui/icons-material/LocationCity';
import LockIcon from '@mui/icons-material/Lock';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { boolean, object, string } from 'yup';

type Props = {
  rootDataRelay: addIndividualOrganization_query$key;
  onReloadRequired: () => void;
  onAdded: (id: string, customDomain: string) => void;
  onCancel?: () => void;
  cancelLabel?: string;
  createLabel?: string;
};

type OrganizationDetails = {
  customDomain: string | null;
  name: string;
  about: string | null;
  website: string | null;
  customerFacingTermsAndConditionsUrl: string | null;
  agreedToTermsOfUse: boolean;
};

const organizationSchema = object({
  customDomain: string().nullable(),
  name: string().min(3, 'Organization name must be at least three characters long.').required('Organization name is required'),
  ...listingMetadataSchemaShape,
  website: string().url('Website must be a valid Url').nullable(),
  customerFacingTermsAndConditionsUrl: string().url('Terms and Conditions must be a valid Url').nullable(),
  agreedToTermsOfUse: boolean().oneOf([true], 'Please accept the terms').required('Please accept the terms'),
});

const AddIndividualOrganization = ({ rootDataRelay, onReloadRequired, onAdded, onCancel, cancelLabel, createLabel }: Props) => {
  const rootData = useFragment<addIndividualOrganization_query$key>(
    graphql`
      fragment addIndividualOrganization_query on Query {
        emailsToShowLatestCapabilities
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

  const [commitAddOrganization] = useMutation<addIndividualOrganization_addOrganizationMutation>(graphql`
    mutation addIndividualOrganization_addOrganizationMutation($input: AddOrganizationInput!) @raw_response_type {
      addOrganization(input: $input) {
        organization {
          id
          customDomain
          name
          listingMetadata {
            about
            title
            subTitle
            includedFeatures
          }
          website
          customerFacingTermsAndConditionsUrl
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateOrganizationDetails = makeValidate(organizationSchema);
  const requiredFields = makeRequired(organizationSchema);

  const handleOrganizationAddClick = ({ customDomain, name, about, website, customerFacingTermsAndConditionsUrl }: OrganizationDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding organization '${name}'...`} />, infoNotificationOptions);

    commitAddOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          customDomain,
          name,
          listingMetadata: {
            about: about ?? '',
            title: '',
            subTitle: '',
            includedFeatures: [],
          },
          website,
          customerFacingTermsAndConditionsUrl,
          type: 'INDIVIDUAL',
          agreedToTermsOfUse: true,
          termsOfUseId: rootData.activeOrganizationTermsOfUse.id,
          industrySubCategoryIds: [],
          billingCycle: 'MONTHLY',
          invoiceDueInDays: 7,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add new organization '${name}'. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${name} added.`} />,
        });

        onAdded(response.addOrganization.organization.id, response.addOrganization.organization.customDomain!);
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
            customDomain,
            name,
            listingMetadata: {
              about: about ?? '',
              title: '',
              subTitle: '',
              includedFeatures: [],
            },
            website,
            customerFacingTermsAndConditionsUrl,
          },
        },
      },
    });
  };

  return (
    <TwoSideVerticalWizard>
      <LeftSidePanel
        title="Share your space, list and host easily"
        description="Built for individual hosts and small space owners who want to list a place (think home share). Create a listing, publish availability, and accept guest bookings quickly and simply."
      >
        <FeatureBox
          icon={<LocationCityIcon sx={{ color: '#5C6BC0', fontSize: 40 }} />}
          title="Create a listing"
          subtitle="Add photos, a description, amenities and house rules so guests know what to expect."
        />
        <FeatureBox
          icon={<CalendarIcon sx={{ color: '#66BB6A', fontSize: 40 }} />}
          title="Manage availability & bookings"
          subtitle="Set your availability, block dates, and accept or decline guest bookings with an easy calendar."
        />
        <FeatureBox
          icon={<GroupsIcon sx={{ color: '#42A5F5', fontSize: 40 }} />}
          title="Guest communication"
          subtitle="Message guests, share check in instructions, and coordinate stays from one place."
        />
        <FeatureBox
          icon={<AnalyticsIcon sx={{ color: '#FFA726', fontSize: 40 }} />}
          title="Booking insights"
          subtitle="See simple stats about days booked and enquiries to help you optimise availability and pricing."
        />
        <FeatureBox
          icon={<LockIcon sx={{ color: '#EF5350', fontSize: 40 }} />}
          title="Secure hosting"
          subtitle="Control who can book and protect your space with booking rules and verification."
        />
      </LeftSidePanel>

      <RightSidePanel
        title="Create your hosting profile"
        description="These details represent you as the host. We’ll use them for payouts, invoices, and to connect your future listings under one profile."
      >
        <Form
          onSubmit={handleOrganizationAddClick}
          initialValues={{
            customDomain: null,
            name: '',
            about: null,
            website: null,
            customerFacingTermsAndConditionsUrl: null,
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
                    <HelperText text="Use the name of your hosting profile (e.g. your full name or hosting brand). This is for billing and payouts, not the title of any specific space you list." />
                  }
                />
              </FormFieldLabel>

              {rootData.me.emails.some((item) => !!rootData.emailsToShowLatestCapabilities.find((email) => email.toLocaleLowerCase() === item.toLocaleLowerCase())) && (
                <FormFieldLabel label="Custom Domain" required={requiredFields.customDomain}>
                  <TextField name="customDomain" required={requiredFields.customDomain} />
                </FormFieldLabel>
              )}

              <ListingMetadata
                fields={['about']}
                helperTexts={{
                  about: (
                    <HelperText text="Introduce yourself as a host. Share your hosting style or the types of stays you offer. Individual listings can include their own detailed descriptions later." />
                  ),
                }}
                requiredFields={requiredFields}
              />

              <FormFieldLabel label="Website" required={requiredFields.website}>
                <TextField
                  name="website"
                  required={requiredFields.website}
                  helperText={
                    <HelperText text="Link to a personal site or social profile that represents you as a host. You’ll add location-specific links for each listing separately." />
                  }
                />
              </FormFieldLabel>

              <FormFieldLabel label="Terms and Conditions" required={requiredFields.customerFacingTermsAndConditionsUrl}>
                <TextField
                  name="customerFacingTermsAndConditionsUrl"
                  required={requiredFields.customerFacingTermsAndConditionsUrl}
                  helperText={<HelperText text="Provide the URL to your customer-facing terms and conditions." />}
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

export default memo(AddIndividualOrganization);
