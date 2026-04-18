import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { FormFieldLabel, FormStackColumn, HelperText, StackColumn, StackRow } from '@/components/commons';
import { DeleteIcon } from '@/components/icons';
import { ListingMetadata } from '@/components/listingMetadata';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { OrganizationMultipleChoicesIndustries } from '@/components/organization';
import { OrganizationDetails, organizationSchema, splitNotificationEmails } from '@/components/organization/organizationAdmin/organization-admin-shared';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { getRelayErrorMessage, keyboardTextFieldDebounceTimeout } from '@/libs/utils';
import type { organizationAdminSetupSectionQuery } from '@/queries/__generated__/organizationAdminSetupSectionQuery.graphql';
import type { organizationAdminSetupSection_updateOrganizationMutation } from '@/queries/__generated__/organizationAdminSetupSection_updateOrganizationMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import IconButton from '@mui/material/IconButton';
import { EditorActionBar, SettingsSectionCard } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
};

type InnerProps = {
  queryReference: PreloadedQuery<organizationAdminSetupSectionQuery>;
};

const RootQuery = graphql`
  query organizationAdminSetupSectionQuery($organizationCustomDomain: String!) {
    emailsToShowLatestCapabilities
    me {
      id
      emails
    }
    organizationIndustryMainCategoriesReferences {
      subCategories {
        id
        name
      }
    }
    organization(customDomain: $organizationCustomDomain) {
      id
      customDomain
      name
      billingCycle {
        type
        name
      }
      invoiceDueInDays
      listingMetadata {
        about
        title
        subTitle
      }
      marketplaceListingMetadata {
        about
        title
        subTitle
        includedFeatures
      }
      website
      customerFacingTermsAndConditionsUrl
      industrySubCategories {
        id
        name
      }
      contactEmail
      contactPhone
      refundNotificationEmails
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
    ...organizationMultipleChoicesIndustries_query
  }
`;

const OrganizationAdminSetupSectionContent = ({ queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminSetupSectionQuery>(RootQuery, queryReference);
  const [commitUpdateOrganization] = useMutation<organizationAdminSetupSection_updateOrganizationMutation>(graphql`
    mutation organizationAdminSetupSection_updateOrganizationMutation($input: UpdateOrganizationInput!) @raw_response_type {
      updateOrganization(input: $input) {
        organization {
          id
          customDomain
          name
          listingMetadata {
            about
            title
            subTitle
          }
          marketplaceListingMetadata {
            about
            title
            subTitle
            includedFeatures
          }
          website
          customerFacingTermsAndConditionsUrl
          industrySubCategories {
            id
            name
          }
          contactEmail
          contactPhone
          refundNotificationEmails
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
          billingCycle {
            type
            name
          }
          invoiceDueInDays
        }
      }
    }
  `);

  const organization = rootData.organization;
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateOrganizationDetails = makeValidate(organizationSchema);
  const requiredOrganizationDetailsFields = makeRequired(organizationSchema);
  const formColumnSx = {
    width: '100%',
    maxWidth: 760,
  };

  const [organizationEditableCustomDomain, setOrganizationEditableCustomDomain] = useState(organization?.customDomain);
  const debounceSetOrganizationEditableCustomDomain = useDebounceCallback(setOrganizationEditableCustomDomain, keyboardTextFieldDebounceTimeout);
  const [organizationName, setOrganizationName] = useState<string>(organization?.name ?? '');
  const debounceSetOrganizationName = useDebounceCallback(setOrganizationName, keyboardTextFieldDebounceTimeout);
  const [organizationAbout, setOrganizationAbout] = useState(organization?.listingMetadata.about ?? null);
  const debounceSetOrganizationAbout = useDebounceCallback(setOrganizationAbout, keyboardTextFieldDebounceTimeout);
  const [organizationTitle, setOrganizationTitle] = useState(organization?.listingMetadata.title ?? null);
  const debounceSetOrganizationTitle = useDebounceCallback(setOrganizationTitle, keyboardTextFieldDebounceTimeout);
  const [organizationSubTitle, setOrganizationSubTitle] = useState(organization?.listingMetadata.subTitle ?? null);
  const debounceSetOrganizationSubTitle = useDebounceCallback(setOrganizationSubTitle, keyboardTextFieldDebounceTimeout);
  const [organizationWebsite, setOrganizationWebsite] = useState(organization?.website);
  const debounceSetOrganizationWebsite = useDebounceCallback(setOrganizationWebsite, keyboardTextFieldDebounceTimeout);
  const [organizationCustomerFacingTermsAndConditionsUrl, setOrganizationCustomerFacingTermsAndConditionsUrl] = useState(organization?.customerFacingTermsAndConditionsUrl);
  const debounceSetOrganizationCustomerFacingTermsAndConditionsUrl = useDebounceCallback(setOrganizationCustomerFacingTermsAndConditionsUrl, keyboardTextFieldDebounceTimeout);
  const [organizationIndustrySubCategoryIds, setOrganizationIndustrySubCategoryIds] = useState<string[]>(organization?.industrySubCategories.map(({ id }) => id) ?? []);
  const debounceSetOrganizationIndustrySubCategoryIds = useDebounceCallback(setOrganizationIndustrySubCategoryIds, keyboardTextFieldDebounceTimeout);
  const [organizationContactEmail, setOrganizationContactEmail] = useState<string>(organization?.contactEmail ?? '');
  const debounceSetOrganizationContactEmail = useDebounceCallback(setOrganizationContactEmail, keyboardTextFieldDebounceTimeout);
  const [organizationContactPhone, setOrganizationContactPhone] = useState(organization?.contactPhone);
  const debounceSetOrganizationContactPhone = useDebounceCallback(setOrganizationContactPhone, keyboardTextFieldDebounceTimeout);
  const [organizationRefundNotificationEmailsText, setOrganizationRefundNotificationEmailsText] = useState<string>(organization?.refundNotificationEmails?.join('\n') ?? '');
  const debounceSetOrganizationRefundNotificationEmailsText = useDebounceCallback(setOrganizationRefundNotificationEmailsText, keyboardTextFieldDebounceTimeout);
  const [featureImages, setFeatureImages] = useState<FileUploadResponse[]>(
    organization
      ? organization.featureImages
          .filter((item) => !!item.original)
          .map((item) => ({
            id: '',
            original: { url: item.original!.url, height: item.original!.height, width: item.original!.width },
            thumbnail: item.thumbnail ? { url: item.thumbnail.url, height: item.thumbnail.height, width: item.thumbnail.width } : null,
          }))
      : [],
  );
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(featureImages[0] ?? null);

  if (!organization) {
    return null;
  }

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

  const handleOrganizationDetailUpdateClick = ({
    customDomain,
    name,
    about,
    title,
    subTitle,
    website,
    customerFacingTermsAndConditionsUrl,
    industrySubCategoryIds,
    contactEmail,
    contactPhone,
    refundNotificationEmailsText,
  }: OrganizationDetails) => {
    const selectedIndustrySubCategoryIds = industrySubCategoryIds ?? [];
    const refundNotificationEmails = splitNotificationEmails(refundNotificationEmailsText);
    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}'...`} />, infoNotificationOptions);
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    commitUpdateOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: organization.id,
          customDomain,
          name,
          listingMetadata: {
            about: about ?? '',
            title: title ?? '',
            subTitle: subTitle ?? '',
          },
          marketplaceListingMetadata: organization.marketplaceListingMetadata,
          website,
          customerFacingTermsAndConditionsUrl,
          industrySubCategoryIds: selectedIndustrySubCategoryIds,
          contactEmail,
          contactPhone,
          refundNotificationEmails,
          featureImages: finalFeatureImages,
          billingCycle: organization.billingCycle.type,
          invoiceDueInDays: organization.invoiceDueInDays,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't update organisation '${organization.name}'. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`The details for organisation '${name}' have been updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't update organisation '${organization.name}'. ${error.message}`} />,
        });
      },
      optimisticResponse: {
        updateOrganization: {
          organization: {
            id: organization.id,
            customDomain: organization.customDomain,
            name,
            listingMetadata: {
              about: about ?? '',
              title: title ?? '',
              subTitle: subTitle ?? '',
            },
            marketplaceListingMetadata: organization.marketplaceListingMetadata,
            website,
            customerFacingTermsAndConditionsUrl,
            industrySubCategories: rootData.organizationIndustryMainCategoriesReferences
              .flatMap((mainCategory) => mainCategory.subCategories)
              .filter(({ id }) => selectedIndustrySubCategoryIds.includes(id))
              .map(({ id, name }) => ({ id, name })),
            contactEmail,
            contactPhone,
            refundNotificationEmails,
            featureImages: finalFeatureImages,
            billingCycle: organization.billingCycle,
            invoiceDueInDays: organization.invoiceDueInDays,
          },
        },
      },
    });
  };

  return (
    <Form
      onSubmit={handleOrganizationDetailUpdateClick}
      initialValues={{
        customDomain: organizationEditableCustomDomain,
        name: organizationName,
        about: organizationAbout,
        title: organizationTitle,
        subTitle: organizationSubTitle,
        website: organizationWebsite,
        customerFacingTermsAndConditionsUrl: organizationCustomerFacingTermsAndConditionsUrl,
        industrySubCategoryIds: organizationIndustrySubCategoryIds,
        contactEmail: organizationContactEmail,
        contactPhone: organizationContactPhone,
        refundNotificationEmailsText: organizationRefundNotificationEmailsText,
      }}
      validate={validateOrganizationDetails}
      render={({ handleSubmit, values }) => {
        const formValues = values!;

        debounceSetOrganizationEditableCustomDomain(formValues.customDomain);
        debounceSetOrganizationName(formValues.name);
        debounceSetOrganizationWebsite(formValues.website);
        debounceSetOrganizationCustomerFacingTermsAndConditionsUrl(formValues.customerFacingTermsAndConditionsUrl);
        debounceSetOrganizationIndustrySubCategoryIds(formValues.industrySubCategoryIds);
        debounceSetOrganizationContactEmail(formValues.contactEmail);
        debounceSetOrganizationContactPhone(formValues.contactPhone);
        debounceSetOrganizationRefundNotificationEmailsText(formValues.refundNotificationEmailsText ?? '');

        return (
          <FormStackColumn onSubmit={handleSubmit}>
            <Box sx={{ pb: 2 }}>
              <StackColumn spacing={2}>
                <SettingsSectionCard title="Organization setup" description="Edit identity, presentation, domain, industry, and customer-facing details.">
                  <StackColumn sx={formColumnSx}>
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

                        <ImageFileUploaderWithCropper onUploadCompleted={handleFeatureImageUploadCompleted} />
                      </StackColumn>
                    </FormFieldLabel>

                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredOrganizationDetailsFields.name} />
                    </FormFieldLabel>

                    {rootData.me?.emails.some((item) => !!rootData.emailsToShowLatestCapabilities.find((email) => email.toLocaleLowerCase() === item.toLocaleLowerCase())) && (
                      <FormFieldLabel label="Custom Domain" required={requiredOrganizationDetailsFields.customDomain}>
                        <TextField name="customDomain" required={requiredOrganizationDetailsFields.customDomain} />
                      </FormFieldLabel>
                    )}

                    <ListingMetadata
                      fields={['about', 'title', 'subTitle']}
                      requiredFields={requiredOrganizationDetailsFields}
                      onChange={({ about, title, subTitle }) => {
                        debounceSetOrganizationAbout(about);
                        debounceSetOrganizationTitle(title);
                        debounceSetOrganizationSubTitle(subTitle);
                      }}
                    />

                    <FormFieldLabel label="Website">
                      <TextField name="website" required={requiredOrganizationDetailsFields.about} helperText="https://" />
                    </FormFieldLabel>

                    <FormFieldLabel label="Terms and Conditions">
                      <TextField
                        name="customerFacingTermsAndConditionsUrl"
                        required={requiredOrganizationDetailsFields.customerFacingTermsAndConditionsUrl}
                        helperText={<HelperText text="Provide your company's official website so members can learn more or verify your organization." />}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Industry">
                      <OrganizationMultipleChoicesIndustries
                        rootDataRelay={rootData}
                        name="industrySubCategoryIds"
                        required={requiredOrganizationDetailsFields.industrySubCategoryIds}
                      />
                    </FormFieldLabel>
                  </StackColumn>
                </SettingsSectionCard>

                <SettingsSectionCard title="Contact details" description="Set the operational contact points used for member communication and refund notifications.">
                  <StackColumn>
                    <FormFieldLabel label="Email">
                      <TextField name="contactEmail" required={requiredOrganizationDetailsFields.contactEmail} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Phone Number">
                      <TextField name="contactPhone" required={requiredOrganizationDetailsFields.contactPhone} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Refund Notification Emails">
                      <StackColumn>
                        <TextField name="refundNotificationEmailsText" required={requiredOrganizationDetailsFields.refundNotificationEmailsText} multiline minRows={3} />
                        <HelperText text="Optional. One email per line, or separate multiple emails with commas. These addresses receive internal refund status updates." />
                      </StackColumn>
                    </FormFieldLabel>

                    <EditorActionBar primaryAction="Update" />
                  </StackColumn>
                </SettingsSectionCard>
              </StackColumn>
            </Box>
          </FormStackColumn>
        );
      }}
    />
  );
};

const OrganizationAdminSetupSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationAdminSetupSectionQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      { organizationCustomDomain },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return <Loading />;
  }

  return <OrganizationAdminSetupSectionContent queryReference={queryReference} />;
};

export default memo(OrganizationAdminSetupSection);
