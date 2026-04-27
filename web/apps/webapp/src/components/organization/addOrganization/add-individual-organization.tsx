import { BodyIconTypography, FormFieldLabel, FormStackColumn, HelperText, StackColumn } from '@skedular/ui';
import { AnalyticsIcon, CalendarIcon } from '@/components/icons';
import { ListingMetadata, listingMetadataSchemaShape } from '@/components/listingMetadata';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { OrganizationTermsOfUse } from '@/components/organization';
import { PaletteModeContext } from '@skedular/shared';
import { defaultButtonStyle } from '@skedular/ui';
import { getRelayErrorMessage } from '@skedular/shared';
import { EditorActionBar, SettingsSectionCard, SetupFeatureCard, SetupSplitLayout } from '@skedular/ui';
import type { addIndividualOrganization_addOrganizationMutation } from '@/queries/__generated__/addIndividualOrganization_addOrganizationMutation.graphql';
import type { addIndividualOrganization_query$key } from '@/queries/__generated__/addIndividualOrganization_query.graphql';
import GroupsIcon from '@mui/icons-material/Groups';
import LocationCityIcon from '@mui/icons-material/LocationCity';
import LockIcon from '@mui/icons-material/Lock';
import Button from '@mui/material/Button';
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
          refundNotificationEmails: [],
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
    <SetupSplitLayout
      asideTitle="Share your space, list and host easily"
      asideDescription="Built for individual hosts and small space owners who want to publish a place and manage bookings simply."
      asideChildren={
        <>
          <SetupFeatureCard
            icon={<LocationCityIcon sx={{ color: '#5C6BC0', fontSize: 40 }} />}
            title="Create a listing"
            description="Add photos, a description, amenities and house rules so guests know what to expect."
          />
          <SetupFeatureCard
            icon={<CalendarIcon sx={{ color: '#66BB6A', fontSize: 40 }} />}
            title="Manage availability & bookings"
            description="Set your availability, block dates, and accept or decline guest bookings with an easy calendar."
          />
          <SetupFeatureCard
            icon={<GroupsIcon sx={{ color: '#42A5F5', fontSize: 40 }} />}
            title="Guest communication"
            description="Message guests, share check in instructions, and coordinate stays from one place."
          />
          <SetupFeatureCard
            icon={<AnalyticsIcon sx={{ color: '#FFA726', fontSize: 40 }} />}
            title="Booking insights"
            description="See simple stats about days booked and enquiries to help you optimise availability and pricing."
          />
          <SetupFeatureCard
            icon={<LockIcon sx={{ color: '#EF5350', fontSize: 40 }} />}
            title="Secure hosting"
            description="Control who can book and protect your space with booking rules and verification."
          />
        </>
      }
      mainTitle="Create your hosting profile"
      mainDescription="These details represent you as the host. We’ll use them for payouts, invoices, and to connect future listings under one profile."
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
            <StackColumn>
              <SettingsSectionCard title="Host Identity" description="Set the core host profile details that will represent you across billing, payouts, and future listings.">
                <StackColumn>
                  <FormFieldLabel label="Name" required={requiredFields.name}>
                    <TextField
                      name="name"
                      required={requiredFields.name}
                      helperText={
                        <HelperText text="Use the name of your hosting profile, such as your full name or hosting brand. This is for billing and payouts, not the title of a specific listing." />
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
                        <HelperText text="Introduce yourself as a host. Share your hosting style or the kinds of stays you offer. Individual listings can add detailed descriptions later." />
                      ),
                    }}
                    requiredFields={requiredFields}
                  />
                </StackColumn>
              </SettingsSectionCard>

              <SettingsSectionCard title="Public Links" description="Add the links and terms customers should see before they book or contact you.">
                <StackColumn>
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
                </StackColumn>
              </SettingsSectionCard>

              <SettingsSectionCard title="Acceptance" description="Confirm the platform terms before the hosting profile is created.">
                <FormFieldLabel label="" required={requiredFields.agreedToTermsOfUse}>
                  <OrganizationTermsOfUse rootDataRelay={rootData} name="agreedToTermsOfUse" required={requiredFields.agreedToTermsOfUse} />
                </FormFieldLabel>
              </SettingsSectionCard>

              <EditorActionBar
                secondaryActions={
                  <Button variant="contained" sx={defaultButtonStyle} onClick={onCancel}>
                    <BodyIconTypography label={cancelLabel ?? 'Cancel'} invertDefaultColor={paletteMode === 'dark'} />
                  </Button>
                }
                primaryAction={
                  <Button variant="contained" type="submit" sx={{ textTransform: 'none' }} color="primary">
                    <BodyIconTypography label={createLabel ?? 'Create'} invertDefaultColor={paletteMode === 'dark'} />
                  </Button>
                }
              />
            </StackColumn>
          </FormStackColumn>
        )}
      />
    </SetupSplitLayout>
  );
};

export default memo(AddIndividualOrganization);
