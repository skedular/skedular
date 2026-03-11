import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, HelperText, PushToRight, StackColumn, StackRow } from '@/components/commons';
import { DeleteIcon } from '@/components/icons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { OrganizationTermsOfUse } from '@/components/organization';
import { FeatureBox, LeftSidePanel, RightSidePanel, TwoSideVerticalWizard } from '@/components/wizard';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
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
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
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
  onAdded: (id: string, uniqueAlphanumericName: string) => void;
  onCancel?: () => void;
  cancelLabel?: string;
  createLabel?: string;
};

type OrganizationDetails = {
  uniqueAlphanumericName: string | null;
  name: string;
  about: string | null;
  website: string | null;
  agreedToTermsOfUse: boolean;
};

const organizationSchema = object({
  uniqueAlphanumericName: string().nullable(),
  name: string().min(3, 'Organization name must be at least three characters long.').required('Organization name is required'),
  about: string().nullable(),
  website: string().nullable(),
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
          uniqueAlphanumericName
          name
          listingMetadata {
            about
            title
            subTitle
          }
          website
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

  const handleOrganizationAddClick = ({ uniqueAlphanumericName, name, about, website }: OrganizationDetails) => {
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
          uniqueAlphanumericName,
          name,
          listingMetadata: {
            about: about ?? '',
            title: '',
            subTitle: '',
          },
          website,
          type: 'MARKETPLACE',
          agreedToTermsOfUse: true,
          termsOfUseId: rootData.activeOrganizationTermsOfUse.id,
          industrySubCategoryIds: [],
          featureImages: finalFeatureImages,
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

        onAdded(response.addOrganization.organization.id, response.addOrganization.organization.uniqueAlphanumericName!);
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
            name,
            listingMetadata: {
              about: about ?? '',
              title: '',
              subTitle: '',
            },
            website,
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
    <TwoSideVerticalWizard>
      <LeftSidePanel
        title="Set up your co-working space"
        description="List your space, manage availability, and connect with individuals or teams looking for flexible work environments. We'll guide you through the basics to get started."
      >
        <FeatureBox
          icon={<ViewQuiltIcon sx={{ color: '#4CAF50', fontSize: 40 }} />}
          title="Space Customization"
          subtitle="Easily set up rooms, desks, and shared areas the way your space is structured."
        />
        <FeatureBox
          icon={<TodayIcon sx={{ color: '#2196F3', fontSize: 40 }} />}
          title="Real-Time Availability"
          subtitle="Keep availability up to date and let users book in real time."
        />
        <FeatureBox
          icon={<PublicIcon sx={{ color: '#FF9800', fontSize: 40 }} />}
          title="Marketplace Visibility"
          subtitle="Showcase your space to individuals and teams looking for flexible workspaces."
        />
        <FeatureBox icon={<ForumIcon sx={{ color: '#9C27B0', fontSize: 40 }} />} title="User Communication" subtitle="Message and manage members directly through the platform." />
        <FeatureBox
          icon={<BarChartIcon sx={{ color: '#3F51B5', fontSize: 40 }} />}
          title="Booking Insights"
          subtitle="Track utilization, revenue, and occupancy trends with smart analytics."
        />
        <FeatureBox
          icon={<AdminPanelSettingsIcon sx={{ color: '#F44336', fontSize: 40 }} />}
          title="Admin Controls"
          subtitle="Manage access, edit listings, and approve bookings with ease."
        />
      </LeftSidePanel>

      <RightSidePanel
        title="Let's Get Your Space Listed"
        description="We'll start with a few details about your organization. This helps represent your co-working space and makes onboarding seamless later on."
      >
        <Form
          onSubmit={handleOrganizationAddClick}
          initialValues={{
            uniqueAlphanumericName: null,
            name: '',
            about: null,
            website: null,
          }}
          validate={validateOrganizationDetails}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <Divider />

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
                    defaultAspectRatio={1}
                    onUploadCompleted={handleFeatureImageUploadCompleted}
                    helperText="Upload a high-quality image that represents this location. This image will be used in dashboards and reports to visually identify the workspace."
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
                <FormFieldLabel label="Unique Name" required={requiredFields.uniqueAlphanumericName}>
                  <TextField name="uniqueAlphanumericName" required={requiredFields.uniqueAlphanumericName} />
                </FormFieldLabel>
              )}

              <FormFieldLabel label="About" required={requiredFields.about}>
                <TextField
                  name="about"
                  required={requiredFields.about}
                  multiline
                  rows={3}
                  helperText={<HelperText text="Briefly describe your co-working space, its mission, community vibe, and what makes it unique." />}
                />
              </FormFieldLabel>

              <FormFieldLabel label="Website" required={requiredFields.website}>
                <TextField name="website" required={requiredFields.website} helperText={<HelperText text="Provide your co-working space's website to share with members." />} />
              </FormFieldLabel>

              <FormFieldLabel label="" required={requiredFields.agreedToTermsOfUse}>
                <OrganizationTermsOfUse rootDataRelay={rootData} name="agreedToTermsOfUse" />
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

export default memo(AddMarketplaceOrganization);
