import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, HelperText, StackColumn, StackRow } from '@skedular/ui';
import { AnalyticsIcon, CalendarIcon, DeleteIcon } from '@/components/icons';
import { ListingMetadata, listingMetadataSchemaShape } from '@/components/listingMetadata';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { OrganizationTermsOfUse } from '@/components/organization';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@skedular/shared';
import { defaultButtonStyle } from '@skedular/ui';
import { getRelayErrorMessage } from '@skedular/shared';
import type { addPrivateOrganization_addOrganizationMutation } from '@/queries/__generated__/addPrivateOrganization_addOrganizationMutation.graphql';
import type { addPrivateOrganization_query$key } from '@/queries/__generated__/addPrivateOrganization_query.graphql';
import GroupsIcon from '@mui/icons-material/Groups';
import LocationCityIcon from '@mui/icons-material/LocationCity';
import LockIcon from '@mui/icons-material/Lock';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import IconButton from '@mui/material/IconButton';
import { EditorActionBar, SettingsSectionCard, SetupFeatureCard, SetupSplitLayout } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { boolean, object, string } from 'yup';

type Props = {
  rootDataRelay: addPrivateOrganization_query$key;
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

const AddPrivateOrganization = ({ rootDataRelay, onReloadRequired, onAdded, onCancel, cancelLabel, createLabel }: Props) => {
  const rootData = useFragment<addPrivateOrganization_query$key>(
    graphql`
      fragment addPrivateOrganization_query on Query {
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

  const [commitAddOrganization] = useMutation<addPrivateOrganization_addOrganizationMutation>(graphql`
    mutation addPrivateOrganization_addOrganizationMutation($input: AddOrganizationInput!) @raw_response_type {
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
          featureImages {
            original {
              url
              height
              width
            }
            thumbnail {
              url
              height
              width
            }
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateOrganizationDetails = makeValidate(organizationSchema);
  const requiredFields = makeRequired(organizationSchema);

  const [featureImages, setFeatureImages] = useState<FileUploadResponse[]>([]);
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(null);

  const handleOrganizationAddClick = ({ customDomain, name, about, website, customerFacingTermsAndConditionsUrl }: OrganizationDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding organization '${name}'...`} />, infoNotificationOptions);
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

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
          type: 'PRIVATE',
          agreedToTermsOfUse: true,
          termsOfUseId: rootData.activeOrganizationTermsOfUse.id,
          industrySubCategoryIds: [],
          refundNotificationEmails: [],
          featureImages: finalFeatureImages,
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
            featureImages: finalFeatureImages,
          },
        },
      },
    });
  };

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setFeatureImages((prev) => [response, ...prev]);
    setPrimaryFeatureImage((prevPrimary) => prevPrimary ?? response);
  };

  const handleRemoveFeatureImage = (image: FileUploadResponse) => {
    setFeatureImages((prev) => {
      const next = prev.filter((item) => item.original?.url !== image.original?.url);

      if (primaryFeatureImage?.original?.url === image.original?.url) {
        setPrimaryFeatureImage(next[0] ?? null);
      }

      return next;
    });
  };

  const handleSetPrimaryFeatureImage = (image: FileUploadResponse) => {
    setPrimaryFeatureImage(image);
    setFeatureImages((prev) => [image, ...prev.filter((item) => item.original?.url !== image.original?.url)]);
  };

  return (
    <SetupSplitLayout
      asideTitle="Manage Your Private Workspace with Full Control"
      asideDescription="Create a dedicated environment for your enterprise, then manage teams, locations, and resources in one place."
      asideChildren={
        <>
          <SetupFeatureCard
            icon={<LocationCityIcon sx={{ color: '#5C6BC0', fontSize: 40 }} />}
            title="Multi-location support"
            description="Manage desks, meeting rooms, and zones across different offices."
          />
          <SetupFeatureCard
            icon={<GroupsIcon sx={{ color: '#42A5F5', fontSize: 40 }} />}
            title="Team & role management"
            description="Invite users, assign roles, and control access securely."
          />
          <SetupFeatureCard
            icon={<CalendarIcon sx={{ color: '#66BB6A', fontSize: 40 }} />}
            title="Smart scheduling tools"
            description="Enable frictionless booking of spaces with availability and conflict handling."
          />
          <SetupFeatureCard
            icon={<AnalyticsIcon sx={{ color: '#FFA726', fontSize: 40 }} />}
            title="Workspace insights"
            description="Understand usage patterns and optimize resource allocation."
          />
          <SetupFeatureCard icon={<LockIcon sx={{ color: '#EF5350', fontSize: 40 }} />} title="Private & secure" description="Your organization data is isolated and protected." />
        </>
      }
      mainTitle="Set Up Your Organization"
      mainDescription="Tell us a bit about your company so we can tailor the workspace experience before you start adding locations, teams, and resources."
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
              <SettingsSectionCard
                title="Brand and Identity"
                description="Set the name, domain, and profile copy that will represent the organization across the private workspace."
              >
                <StackColumn>
                  <FormFieldLabel label="Feature Images">
                    <StackColumn>
                      <Box
                        sx={{
                          display: 'grid',
                          gridTemplateColumns: { xs: 'repeat(auto-fill, minmax(140px, 1fr))', sm: 'repeat(auto-fill, minmax(180px, 1fr))' },
                          gap: 2,
                        }}
                      >
                        {featureImages.map((image, index) => (
                          <Box
                            key={index}
                            sx={{
                              position: 'relative',
                              borderRadius: 2,
                              overflow: 'hidden',
                              border: 1,
                              borderColor: 'divider',
                              backgroundColor: paletteMode === 'dark' ? 'grey.900' : 'grey.50',
                            }}
                          >
                            {/* eslint-disable-next-line @next/next/no-img-element */}
                            <img src={image.original?.url ?? image.thumbnail?.url ?? ''} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                            <StackRow sx={{ position: 'absolute', top: 8, right: 8 }}>
                              <IconButton size="small" aria-label="Remove feature image" onClick={() => handleRemoveFeatureImage(image)}>
                                <DeleteIcon fontSize="small" />
                              </IconButton>
                            </StackRow>
                            <StackRow sx={{ position: 'absolute', left: 8, bottom: 8 }}>
                              {primaryFeatureImage?.original?.url === image.original?.url ? (
                                <Chip size="small" color="success" label="Cover image" />
                              ) : (
                                <Button variant="contained" size="small" onClick={() => handleSetPrimaryFeatureImage(image)} sx={{ textTransform: 'none' }}>
                                  Make cover
                                </Button>
                              )}
                            </StackRow>
                          </Box>
                        ))}
                      </Box>

                      <ImageFileUploaderWithCropper
                        onUploadCompleted={handleFeatureImageUploadCompleted}
                        helperText="Upload a high-quality image that represents the organization. This will be used in dashboards and admin surfaces to visually identify the workspace."
                      />
                    </StackColumn>
                  </FormFieldLabel>

                  <FormFieldLabel label="Name" required={requiredFields.name}>
                    <TextField
                      name="name"
                      required={requiredFields.name}
                      helperText={
                        <HelperText text="This will be used as the primary name for your organization across the platform. Choose a recognizable name your team will expect." />
                      }
                    />
                  </FormFieldLabel>

                  {rootData.me.emails.some((item) => !!rootData.emailsToShowLatestCapabilities.find((email) => email.toLocaleLowerCase() === item.toLocaleLowerCase())) && (
                    <FormFieldLabel label="Unique Name" required={requiredFields.customDomain}>
                      <TextField name="customDomain" required={requiredFields.customDomain} />
                    </FormFieldLabel>
                  )}

                  <ListingMetadata
                    fields={['about']}
                    helperTexts={{
                      about: <HelperText text="Briefly describe what your organization does. This helps coworkers and team members understand your company’s focus and purpose." />,
                    }}
                    requiredFields={requiredFields}
                  />
                </StackColumn>
              </SettingsSectionCard>

              <SettingsSectionCard title="Public Links" description="Provide the website and customer-facing policy links people will use when interacting with the organization.">
                <StackColumn>
                  <FormFieldLabel label="Website" required={requiredFields.website}>
                    <TextField
                      name="website"
                      required={requiredFields.website}
                      helperText={<HelperText text="Provide your company’s official website so members can learn more or verify your organization." />}
                    />
                  </FormFieldLabel>

                  <FormFieldLabel label="Terms and Conditions URL" required={requiredFields.customerFacingTermsAndConditionsUrl}>
                    <TextField
                      name="customerFacingTermsAndConditionsUrl"
                      required={requiredFields.customerFacingTermsAndConditionsUrl}
                      helperText={<HelperText text="Provide the URL to your customer-facing terms and conditions." />}
                    />
                  </FormFieldLabel>
                </StackColumn>
              </SettingsSectionCard>

              <SettingsSectionCard title="Acceptance" description="Confirm the platform terms before the organization is created.">
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

export default memo(AddPrivateOrganization);
