import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { DeleteIcon } from '@/components/icons';
import { ListingMetadata, listingMetadataSchemaShape } from '@/components/listingMetadata';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { OrganizationTermsOfUse } from '@/components/organization';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import type { addMarketplaceOrganization_addOrganizationMutation } from '@/queries/__generated__/addMarketplaceOrganization_addOrganizationMutation.graphql';
import type { addMarketplaceOrganization_query$key } from '@/queries/__generated__/addMarketplaceOrganization_query.graphql';
import AdminPanelSettingsIcon from '@mui/icons-material/AdminPanelSettings';
import BarChartIcon from '@mui/icons-material/BarChart';
import ForumIcon from '@mui/icons-material/Forum';
import PublicIcon from '@mui/icons-material/Public';
import TodayIcon from '@mui/icons-material/Today';
import ViewQuiltIcon from '@mui/icons-material/ViewQuilt';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import IconButton from '@mui/material/IconButton';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import {
  BodyIconTypography,
  defaultButtonStyle,
  EditorActionBar,
  FormFieldLabel,
  FormStackColumn,
  HelperText,
  SettingsSectionCard,
  SetupFeatureCard,
  SetupSplitLayout,
  StackColumn,
  StackRow,
} from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { boolean, object, string } from 'yup';

type Props = {
  rootDataRelay: addMarketplaceOrganization_query$key;
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

const AddMarketplaceOrganization = ({ rootDataRelay, onReloadRequired, onAdded, onCancel, cancelLabel, createLabel }: Props) => {
  const rootData = useFragment<addMarketplaceOrganization_query$key>(
    graphql`
      fragment addMarketplaceOrganization_query on Query {
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

  const [commitAddOrganization] = useMutation<addMarketplaceOrganization_addOrganizationMutation>(graphql`
    mutation addMarketplaceOrganization_addOrganizationMutation($input: AddOrganizationInput!) @raw_response_type {
      addOrganization(input: $input) {
        organization {
          id
          customDomain
          name
          logoUrl
          marketplaceListingMetadata {
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
  const [logoUrl, setLogoUrl] = useState<string | null>(null);

  const handleOrganizationAddClick = ({ customDomain, name, about, website, customerFacingTermsAndConditionsUrl }: OrganizationDetails) => {
    const id = uuid();
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
          logoUrl,
          marketplaceListingMetadata: {
            about: about ?? '',
            title: '',
            subTitle: '',
            includedFeatures: [],
          },
          website,
          customerFacingTermsAndConditionsUrl,
          type: 'MARKETPLACE',
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
          themedToast(<NotificationContent content={`Failed to add new organization '${name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);

          return;
        }

        onAdded(response.addOrganization.organization.id, response.addOrganization.organization.customDomain!);
        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add new organization '${name}'. Error: ${error.message}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addOrganization: {
          organization: {
            id,
            customDomain,
            name,
            logoUrl,
            marketplaceListingMetadata: {
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

  const handleLogoUploadCompleted = (response: FileUploadResponse) => {
    setLogoUrl(response.original?.url ?? response.thumbnail?.url ?? null);
  };

  return (
    <SetupSplitLayout
      asideTitle="Set up your co-working space"
      asideDescription="List your space, manage availability, and connect with people looking for flexible work environments."
      asideChildren={
        <>
          <SetupFeatureCard
            icon={<ViewQuiltIcon sx={{ color: '#4CAF50', fontSize: 40 }} />}
            title="Space Customization"
            description="Easily set up rooms, desks, and shared areas the way your space is structured."
          />
          <SetupFeatureCard
            icon={<TodayIcon sx={{ color: '#2196F3', fontSize: 40 }} />}
            title="Real-Time Availability"
            description="Keep availability up to date and let users book in real time."
          />
          <SetupFeatureCard
            icon={<PublicIcon sx={{ color: '#FF9800', fontSize: 40 }} />}
            title="Marketplace Visibility"
            description="Showcase your space to people looking for flexible workspaces."
          />
          <SetupFeatureCard
            icon={<ForumIcon sx={{ color: '#9C27B0', fontSize: 40 }} />}
            title="User Communication"
            description="Message and manage members directly through the platform."
          />
          <SetupFeatureCard
            icon={<BarChartIcon sx={{ color: '#3F51B5', fontSize: 40 }} />}
            title="Booking Insights"
            description="Track utilization, revenue, and occupancy trends with smart analytics."
          />
          <SetupFeatureCard
            icon={<AdminPanelSettingsIcon sx={{ color: '#F44336', fontSize: 40 }} />}
            title="Admin Controls"
            description="Manage access, edit listings, and approve bookings with ease."
          />
        </>
      }
      mainTitle="Let's Get Your Space Listed"
      mainDescription="Start with the organization details that represent your co-working brand before you move into listings, availability, and operations."
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
              <SettingsSectionCard title="Brand and Identity" description="Set the core organization profile details customers and operators will rely on first.">
                <StackColumn>
                  <FormFieldLabel label="Logo">
                    <StackColumn>
                      {logoUrl ? (
                        <Box
                          sx={{
                            width: 128,
                            height: 128,
                            borderRadius: 2,
                            border: 1,
                            borderColor: 'divider',
                            backgroundColor: paletteMode === 'dark' ? 'grey.900' : 'grey.50',
                            display: 'grid',
                            placeItems: 'center',
                            overflow: 'hidden',
                            p: 1,
                          }}
                        >
                          {/* eslint-disable-next-line @next/next/no-img-element */}
                          <img src={logoUrl} alt="Organization logo" style={{ maxWidth: '100%', maxHeight: '100%', objectFit: 'contain' }} />
                        </Box>
                      ) : null}

                      <StackRow>
                        <ImageFileUploaderWithCropper helperText="Upload a square logo or icon for organization branding." onUploadCompleted={handleLogoUploadCompleted} />
                        {logoUrl ? (
                          <Button variant="outlined" size="small" onClick={() => setLogoUrl(null)} sx={{ textTransform: 'none' }}>
                            Remove logo
                          </Button>
                        ) : null}
                      </StackRow>
                    </StackColumn>
                  </FormFieldLabel>

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
                        helperText="Upload a high-quality image that represents the organization. This image will be used in dashboards and admin surfaces to visually identify the workspace."
                      />
                    </StackColumn>
                  </FormFieldLabel>

                  <FormFieldLabel label="Name" required={requiredFields.name}>
                    <TextField
                      name="name"
                      required={requiredFields.name}
                      helperText={<HelperText text="Enter the official name of your co-working space as you want it to appear to members and visitors." />}
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
                      about: <HelperText text="Briefly describe your co-working space, its mission, community vibe, and what makes it unique." />,
                    }}
                    requiredFields={requiredFields}
                  />
                </StackColumn>
              </SettingsSectionCard>

              <SettingsSectionCard title="Public Links" description="Add the website and customer-facing policy links people should see before they book or join.">
                <StackColumn>
                  <FormFieldLabel label="Website" required={requiredFields.website}>
                    <TextField name="website" required={requiredFields.website} helperText={<HelperText text="Provide your co-working space’s website to share with members." />} />
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

              <SettingsSectionCard title="Acceptance" description="Confirm the platform terms before the marketplace organization is created.">
                <FormFieldLabel label="" required={requiredFields.agreedToTermsOfUse}>
                  <OrganizationTermsOfUse rootDataRelay={rootData} name="agreedToTermsOfUse" />
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

export default memo(AddMarketplaceOrganization);
