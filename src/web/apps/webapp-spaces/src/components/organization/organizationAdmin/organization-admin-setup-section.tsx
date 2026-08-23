import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { DeleteIcon } from '@/components/icons';
import { getOrganizationAdminSetupBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { OrganizationMultipleChoicesIndustries } from '@/components/organization';
import { OrganizationDetails, organizationSchema, splitNotificationEmails } from '@/components/organization/organizationAdmin/organization-admin-shared';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import type { organizationAdminSetupSectionQuery } from '@/queries/__generated__/organizationAdminSetupSectionQuery.graphql';
import type {
  organizationAdminSetupSection_updateOrganizationMutation,
  OrganizationBillingCycle,
  OrganizationPatchField,
} from '@/queries/__generated__/organizationAdminSetupSection_updateOrganizationMutation.graphql';
import Box from '@mui/material/Box';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import IconButton from '@mui/material/IconButton';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import AddPhotoAlternateRoundedIcon from '@mui/icons-material/AddPhotoAlternateRounded';
import Grid from '@mui/material/Grid';
import { getRelayErrorMessage, PaletteModeContext, useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, HelperText, LeadIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import Image from 'next/image';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';
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

const inlinePatchDebounceTimeout = 1000;
type OrganizationSetupPatchValues = {
  customDomain: string | null | undefined;
  name: string;
  website: string | null | undefined;
  logoUrl: string | null | undefined;
  customerFacingTermsAndConditionsUrl: string | null | undefined;
  industrySubCategoryIds: string[];
  contactEmail: string | null | undefined;
  contactPhone: string | null | undefined;
  refundNotificationEmails: string[];
  featureImages: {
    original: { url: string; height: number | null | undefined; width: number | null | undefined } | null;
    thumbnail: { url: string; height: number | null | undefined; width: number | null | undefined } | null;
  }[];
  billingCycle: OrganizationBillingCycle | null | undefined;
  invoiceDueInDays: number | null | undefined;
  marketplaceListingMetadata:
    | {
        about: string | null | undefined;
        title: string | null | undefined;
        subTitle: string | null | undefined;
        includedFeatures: readonly string[] | null | undefined;
      }
    | null
    | undefined;
};

const mapFeatureImagesToPatchInput = (featureImages: FileUploadResponse[]): OrganizationSetupPatchValues['featureImages'] =>
  featureImages.map((image) => ({
    original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
    thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
  }));

const areStringListsEqual = (left: readonly string[], right: readonly string[]) => left.length === right.length && left.every((value, index) => value === right[index]);

const patchValidationFields: Partial<Record<OrganizationPatchField, (keyof OrganizationDetails)[]>> = {
  CUSTOM_DOMAIN: ['customDomain'],
  NAME: ['name'],
  WEBSITE: ['website'],
  CUSTOMER_FACING_TERMS_AND_CONDITIONS_URL: ['customerFacingTermsAndConditionsUrl'],
  INDUSTRY_SUB_CATEGORIES: ['industrySubCategoryIds'],
  CONTACT_EMAIL: ['contactEmail'],
  CONTACT_PHONE: ['contactPhone'],
  REFUND_NOTIFICATION_EMAILS: ['refundNotificationEmailsText'],
};

const getValidationValues = (values: OrganizationSetupPatchValues): OrganizationDetails => ({
  customDomain: values.customDomain ?? null,
  name: values.name,
  website: values.website ?? null,
  customerFacingTermsAndConditionsUrl: values.customerFacingTermsAndConditionsUrl ?? null,
  industrySubCategoryIds: values.industrySubCategoryIds,
  contactEmail: values.contactEmail ?? '',
  contactPhone: values.contactPhone ?? null,
  refundNotificationEmailsText: values.refundNotificationEmails.join('\n'),
});

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
      logoUrl
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
  const [commitUpdateOrganizationPatch] = useMutation<organizationAdminSetupSection_updateOrganizationMutation>(graphql`
    mutation organizationAdminSetupSection_updateOrganizationMutation($input: UpdateOrganizationInput!) {
      updateOrganization(input: $input) {
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
  const router = useRouter();
  const { integratedPlatform } = useIntegratedPlatform();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateOrganizationDetails = makeValidate(organizationSchema);
  const requiredOrganizationDetailsFields = makeRequired(organizationSchema);
  const formColumnSx = {
    width: '100%',
    maxWidth: 760,
  };

  const [organizationLogoUrl, setOrganizationLogoUrl] = useState<string | null>(organization?.logoUrl ?? null);
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
  const searchParams = useSearchParams();
  const [expandedSection, setExpandedSectionState] = useState(() => searchParams.get('section') ?? 'presentation');
  const setExpandedSection = (section: string) => {
    setExpandedSectionState(section);
    const params = new URLSearchParams(searchParams.toString());
    params.set('tab', 'profile');
    if (section) params.set('section', section);
    else params.delete('section');
    router.replace(`?${params.toString()}`, { scroll: false });
  };
  const initialPatchValues: OrganizationSetupPatchValues = {
    customDomain: organization?.customDomain,
    name: organization?.name ?? '',
    website: organization?.website,
    logoUrl: organization?.logoUrl,
    customerFacingTermsAndConditionsUrl: organization?.customerFacingTermsAndConditionsUrl,
    industrySubCategoryIds: organization?.industrySubCategories.map(({ id }) => id) ?? [],
    contactEmail: organization?.contactEmail,
    contactPhone: organization?.contactPhone,
    refundNotificationEmails: [...(organization?.refundNotificationEmails ?? [])],
    featureImages:
      organization?.featureImages.map((image) => ({
        original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
        thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
      })) ?? [],
    billingCycle: organization?.billingCycle.type,
    invoiceDueInDays: organization?.invoiceDueInDays,
    marketplaceListingMetadata: organization?.marketplaceListingMetadata,
  };
  const draftPatchValues = useRef<OrganizationSetupPatchValues>(initialPatchValues);
  const submittedPatchValues = useRef<OrganizationSetupPatchValues>(initialPatchValues);
  const initialFormValues = useMemo(
    () => ({
      customDomain: organization?.customDomain,
      name: organization?.name ?? '',
      website: organization?.website,
      customerFacingTermsAndConditionsUrl: organization?.customerFacingTermsAndConditionsUrl,
      industrySubCategoryIds: organization?.industrySubCategories.map(({ id }) => id) ?? [],
      contactEmail: organization?.contactEmail ?? '',
      contactPhone: organization?.contactPhone,
      refundNotificationEmailsText: organization?.refundNotificationEmails?.join('\n') ?? '',
    }),
    [organization],
  );

  useEffect(() => {
    const values: OrganizationSetupPatchValues = {
      customDomain: organization?.customDomain,
      name: organization?.name ?? '',
      website: organization?.website,
      logoUrl: organization?.logoUrl,
      customerFacingTermsAndConditionsUrl: organization?.customerFacingTermsAndConditionsUrl,
      industrySubCategoryIds: organization?.industrySubCategories.map(({ id }) => id) ?? [],
      contactEmail: organization?.contactEmail,
      contactPhone: organization?.contactPhone,
      refundNotificationEmails: [...(organization?.refundNotificationEmails ?? [])],
      featureImages:
        organization?.featureImages.map((image) => ({
          original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
          thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
        })) ?? [],
      billingCycle: organization?.billingCycle.type,
      invoiceDueInDays: organization?.invoiceDueInDays,
      marketplaceListingMetadata: organization?.marketplaceListingMetadata,
    };
    draftPatchValues.current = values;
    submittedPatchValues.current = values;
  }, [organization]);

  const commitOrganizationPatch = useCallback(
    async (fieldsToUpdate: OrganizationPatchField[], values: Partial<OrganizationSetupPatchValues>) => {
      if (!organization || fieldsToUpdate.length === 0) {
        return;
      }

      const nextPatchValues: OrganizationSetupPatchValues = {
        ...submittedPatchValues.current,
        ...values,
        name: values.name ?? submittedPatchValues.current.name,
        industrySubCategoryIds: values.industrySubCategoryIds ?? submittedPatchValues.current.industrySubCategoryIds,
        refundNotificationEmails: values.refundNotificationEmails ?? submittedPatchValues.current.refundNotificationEmails,
        featureImages: values.featureImages ?? submittedPatchValues.current.featureImages,
        billingCycle: values.billingCycle ?? submittedPatchValues.current.billingCycle,
        invoiceDueInDays: values.invoiceDueInDays ?? submittedPatchValues.current.invoiceDueInDays,
        marketplaceListingMetadata: values.marketplaceListingMetadata ?? submittedPatchValues.current.marketplaceListingMetadata,
      };

      if (fieldsToUpdate.includes('NAME') && nextPatchValues.name.trim().length < 3) {
        return;
      }

      const validationValues = getValidationValues(nextPatchValues);
      const validationFields = fieldsToUpdate.flatMap((field) => patchValidationFields[field] ?? []);
      try {
        await Promise.all(validationFields.map((field) => organizationSchema.validateAt(field, validationValues)));
      } catch {
        return;
      }

      const hasChanges = fieldsToUpdate.some((field) => {
        const submittedValues = submittedPatchValues.current;
        switch (field) {
          case 'CUSTOM_DOMAIN':
            return nextPatchValues.customDomain !== submittedValues.customDomain;
          case 'NAME':
            return nextPatchValues.name !== submittedValues.name;
          case 'WEBSITE':
            return nextPatchValues.website !== submittedValues.website;
          case 'LOGO_URL':
            return nextPatchValues.logoUrl !== submittedValues.logoUrl;
          case 'CUSTOMER_FACING_TERMS_AND_CONDITIONS_URL':
            return nextPatchValues.customerFacingTermsAndConditionsUrl !== submittedValues.customerFacingTermsAndConditionsUrl;
          case 'INDUSTRY_SUB_CATEGORIES':
            return !areStringListsEqual(nextPatchValues.industrySubCategoryIds, submittedValues.industrySubCategoryIds);
          case 'CONTACT_EMAIL':
            return nextPatchValues.contactEmail !== submittedValues.contactEmail;
          case 'CONTACT_PHONE':
            return nextPatchValues.contactPhone !== submittedValues.contactPhone;
          case 'REFUND_NOTIFICATION_EMAILS':
            return !areStringListsEqual(nextPatchValues.refundNotificationEmails, submittedValues.refundNotificationEmails);
          case 'FEATURE_IMAGES':
            return JSON.stringify(nextPatchValues.featureImages) !== JSON.stringify(submittedValues.featureImages);
          case 'BILLING_CYCLE':
            return nextPatchValues.billingCycle !== submittedValues.billingCycle;
          case 'INVOICE_DUE_IN_DAYS':
            return nextPatchValues.invoiceDueInDays !== submittedValues.invoiceDueInDays;
          case 'MARKETPLACE_LISTING_METADATA':
            return JSON.stringify(nextPatchValues.marketplaceListingMetadata) !== JSON.stringify(submittedValues.marketplaceListingMetadata);
          default:
            return true;
        }
      });
      if (!hasChanges) {
        return;
      }

      const previousPatchValues = submittedPatchValues.current;
      submittedPatchValues.current = nextPatchValues;

      const selectedIndustrySubCategories = rootData.organizationIndustryMainCategoriesReferences
        .flatMap((mainCategory) => mainCategory.subCategories)
        .filter(({ id }) => nextPatchValues.industrySubCategoryIds.includes(id))
        .map(({ id, name }) => ({ id, name }));

      commitUpdateOrganizationPatch({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: organization.id,
            fieldsToUpdate,
            ...(fieldsToUpdate.includes('CUSTOM_DOMAIN') ? { customDomain: nextPatchValues.customDomain } : {}),
            ...(fieldsToUpdate.includes('NAME') ? { name: nextPatchValues.name } : {}),
            ...(fieldsToUpdate.includes('WEBSITE') ? { website: nextPatchValues.website } : {}),
            ...(fieldsToUpdate.includes('LOGO_URL') ? { logoUrl: nextPatchValues.logoUrl } : {}),
            ...(fieldsToUpdate.includes('CUSTOMER_FACING_TERMS_AND_CONDITIONS_URL')
              ? { customerFacingTermsAndConditionsUrl: nextPatchValues.customerFacingTermsAndConditionsUrl }
              : {}),
            ...(fieldsToUpdate.includes('INDUSTRY_SUB_CATEGORIES') ? { industrySubCategoryIds: nextPatchValues.industrySubCategoryIds } : {}),
            ...(fieldsToUpdate.includes('CONTACT_EMAIL') ? { contactEmail: nextPatchValues.contactEmail } : {}),
            ...(fieldsToUpdate.includes('CONTACT_PHONE') ? { contactPhone: nextPatchValues.contactPhone } : {}),
            ...(fieldsToUpdate.includes('REFUND_NOTIFICATION_EMAILS') ? { refundNotificationEmails: nextPatchValues.refundNotificationEmails } : {}),
            ...(fieldsToUpdate.includes('FEATURE_IMAGES') ? { featureImages: nextPatchValues.featureImages } : {}),
            ...(fieldsToUpdate.includes('BILLING_CYCLE') ? { billingCycle: nextPatchValues.billingCycle } : {}),
            ...(fieldsToUpdate.includes('INVOICE_DUE_IN_DAYS') ? { invoiceDueInDays: nextPatchValues.invoiceDueInDays } : {}),
            ...(fieldsToUpdate.includes('MARKETPLACE_LISTING_METADATA') ? { marketplaceListingMetadata: nextPatchValues.marketplaceListingMetadata } : {}),
          },
        },
        onCompleted: (response, errors) => {
          if (errors && errors.length > 0) {
            submittedPatchValues.current = previousPatchValues;
            themedToast(<NotificationContent content={`We couldn't update organisation '${organization.name}'. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
            return;
          }

          const updatedCustomDomain = response.updateOrganization.organization.customDomain;
          if (fieldsToUpdate.includes('CUSTOM_DOMAIN') && updatedCustomDomain && updatedCustomDomain !== organization.customDomain) {
            router.replace(getOrganizationAdminSetupBaseLink(integratedPlatform, updatedCustomDomain));
          }
        },
        onError: (error) => {
          submittedPatchValues.current = previousPatchValues;
          themedToast(<NotificationContent content={`We couldn't update organisation '${organization.name}'. ${error.message}`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateOrganization: {
            organization: {
              id: organization.id,
              customDomain: nextPatchValues.customDomain,
              name: nextPatchValues.name,
              logoUrl: nextPatchValues.logoUrl,
              marketplaceListingMetadata: nextPatchValues.marketplaceListingMetadata,
              website: nextPatchValues.website,
              customerFacingTermsAndConditionsUrl: nextPatchValues.customerFacingTermsAndConditionsUrl,
              industrySubCategories: selectedIndustrySubCategories,
              contactEmail: nextPatchValues.contactEmail,
              contactPhone: nextPatchValues.contactPhone,
              refundNotificationEmails: nextPatchValues.refundNotificationEmails,
              featureImages: nextPatchValues.featureImages,
              billingCycle: {
                ...organization.billingCycle,
                type: nextPatchValues.billingCycle ?? organization.billingCycle.type,
              },
              invoiceDueInDays: nextPatchValues.invoiceDueInDays ?? organization.invoiceDueInDays,
            },
          },
        },
      });
    },
    [commitUpdateOrganizationPatch, integratedPlatform, organization, rootData.organizationIndustryMainCategoriesReferences, router, themedToast],
  );
  const debouncedCommitOrganizationPatch = useDebounceCallback(commitOrganizationPatch, inlinePatchDebounceTimeout);

  if (!organization) {
    return null;
  }

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setFeatureImages((prev) => {
      const next = [response, ...prev];
      commitOrganizationPatch(['FEATURE_IMAGES'], { featureImages: mapFeatureImagesToPatchInput(next) });
      return next;
    });
    setPrimaryFeatureImage((prevPrimary) => prevPrimary ?? response);
  };

  const handleRemoveFeatureImage = (image: FileUploadResponse) => {
    setFeatureImages((prev) => {
      const next = prev.filter((item) => item.original?.url !== image.original?.url);

      if (primaryFeatureImage?.original?.url === image.original?.url) {
        setPrimaryFeatureImage(next[0] ?? null);
      }

      commitOrganizationPatch(['FEATURE_IMAGES'], { featureImages: mapFeatureImagesToPatchInput(next) });
      return next;
    });
  };

  const handleSetPrimaryFeatureImage = (image: FileUploadResponse) => {
    setPrimaryFeatureImage(image);
    setFeatureImages((prev) => {
      const next = [image, ...prev.filter((item) => item.original?.url !== image.original?.url)];
      commitOrganizationPatch(['FEATURE_IMAGES'], { featureImages: mapFeatureImagesToPatchInput(next) });
      return next;
    });
  };

  const handleLogoUploadCompleted = (response: FileUploadResponse) => {
    const nextLogoUrl = response.original?.url ?? response.thumbnail?.url ?? null;
    setOrganizationLogoUrl(nextLogoUrl);
    commitOrganizationPatch(['LOGO_URL'], { logoUrl: nextLogoUrl });
  };

  const handleRemoveLogo = () => {
    setOrganizationLogoUrl(null);
    commitOrganizationPatch(['LOGO_URL'], { logoUrl: null });
  };

  return (
    <Form
      onSubmit={() => undefined}
      initialValues={initialFormValues}
      keepDirtyOnReinitialize
      validate={validateOrganizationDetails}
      render={({ handleSubmit, values }) => {
        const formValues = values!;

        const nextName = formValues.name ?? '';
        const nextCustomDomain = formValues.customDomain;
        const nextWebsite = formValues.website;
        const nextCustomerFacingTermsAndConditionsUrl = formValues.customerFacingTermsAndConditionsUrl;
        const nextIndustrySubCategoryIds = formValues.industrySubCategoryIds ?? [];
        const nextContactEmail = formValues.contactEmail ?? null;
        const nextContactPhone = formValues.contactPhone;
        const nextRefundNotificationEmailsText = formValues.refundNotificationEmailsText ?? '';
        const nextRefundNotificationEmails = splitNotificationEmails(nextRefundNotificationEmailsText);

        if (draftPatchValues.current.customDomain !== nextCustomDomain) {
          draftPatchValues.current = { ...draftPatchValues.current, customDomain: nextCustomDomain };
          debouncedCommitOrganizationPatch(['CUSTOM_DOMAIN'], { customDomain: nextCustomDomain });
        }
        if (draftPatchValues.current.name !== nextName) {
          draftPatchValues.current = { ...draftPatchValues.current, name: nextName };
          debouncedCommitOrganizationPatch(['NAME'], { name: nextName });
        }
        if (draftPatchValues.current.website !== nextWebsite) {
          draftPatchValues.current = { ...draftPatchValues.current, website: nextWebsite };
          debouncedCommitOrganizationPatch(['WEBSITE'], { website: nextWebsite });
        }
        if (draftPatchValues.current.customerFacingTermsAndConditionsUrl !== nextCustomerFacingTermsAndConditionsUrl) {
          draftPatchValues.current = { ...draftPatchValues.current, customerFacingTermsAndConditionsUrl: nextCustomerFacingTermsAndConditionsUrl };
          debouncedCommitOrganizationPatch(['CUSTOMER_FACING_TERMS_AND_CONDITIONS_URL'], {
            customerFacingTermsAndConditionsUrl: nextCustomerFacingTermsAndConditionsUrl,
          });
        }
        if (!areStringListsEqual(draftPatchValues.current.industrySubCategoryIds, nextIndustrySubCategoryIds)) {
          draftPatchValues.current = { ...draftPatchValues.current, industrySubCategoryIds: nextIndustrySubCategoryIds };
          debouncedCommitOrganizationPatch(['INDUSTRY_SUB_CATEGORIES'], { industrySubCategoryIds: nextIndustrySubCategoryIds });
        }
        if (draftPatchValues.current.contactEmail !== nextContactEmail) {
          draftPatchValues.current = { ...draftPatchValues.current, contactEmail: nextContactEmail };
          debouncedCommitOrganizationPatch(['CONTACT_EMAIL'], { contactEmail: nextContactEmail });
        }
        if (draftPatchValues.current.contactPhone !== nextContactPhone) {
          draftPatchValues.current = { ...draftPatchValues.current, contactPhone: nextContactPhone };
          debouncedCommitOrganizationPatch(['CONTACT_PHONE'], { contactPhone: nextContactPhone });
        }
        if (!areStringListsEqual(draftPatchValues.current.refundNotificationEmails, nextRefundNotificationEmails)) {
          draftPatchValues.current = { ...draftPatchValues.current, refundNotificationEmails: nextRefundNotificationEmails };
          debouncedCommitOrganizationPatch(['REFUND_NOTIFICATION_EMAILS'], { refundNotificationEmails: nextRefundNotificationEmails });
        }

        return (
          <FormStackColumn onSubmit={handleSubmit}>
            <Box>
              <StackColumn spacing={2}>
                <Accordion
                  disableGutters
                  elevation={0}
                  expanded={expandedSection === 'presentation'}
                  onChange={() => setExpandedSection(expandedSection === 'presentation' ? '' : 'presentation')}
                  sx={{ border: 1, borderColor: 'divider', borderRadius: '16px !important', overflow: 'hidden', '&::before': { display: 'none' } }}
                >
                  <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />}>
                    <StackColumn spacing={0.35}>
                      <LeadIconTypography label="Presentation" />
                      <BodyIconTypography label="Edit the organization identity, domain, industry, and customer-facing details." />
                    </StackColumn>
                  </AccordionSummary>
                  <AccordionDetails sx={{ borderTop: 1, borderColor: 'divider', p: { xs: 2, sm: 2.5 } }}>
                    <Grid container spacing={{ xs: 2, md: 3 }}>
                      <Grid size={{ xs: 12, md: 5 }}>
                        <StackColumn sx={formColumnSx}>
                          <FormFieldLabel label="Logo">
                            <StackColumn>
                              {organizationLogoUrl ? (
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
                                    position: 'relative',
                                    p: 1,
                                  }}
                                >
                                  <Image
                                    width={128}
                                    height={128}
                                    unoptimized
                                    alt={`${organization.name} logo`}
                                    src={organizationLogoUrl}
                                    style={{ width: '100%', height: '100%', objectFit: 'contain' }}
                                  />
                                  <IconButton
                                    size="small"
                                    aria-label="Remove logo"
                                    onClick={handleRemoveLogo}
                                    sx={{
                                      position: 'absolute',
                                      top: 6,
                                      right: 6,
                                      color: 'common.white',
                                      backgroundColor: 'rgba(0, 0, 0, 0.72)',
                                      '&:hover': { backgroundColor: 'rgba(0, 0, 0, 0.9)' },
                                    }}
                                  >
                                    <DeleteIcon fontSize="small" />
                                  </IconButton>
                                </Box>
                              ) : null}

                              <Box
                                sx={{
                                  position: 'relative',
                                  overflow: 'hidden',
                                  border: 1,
                                  borderStyle: 'dashed',
                                  borderColor: 'success.main',
                                  borderRadius: 2.5,
                                  p: 1.5,
                                  backgroundColor: 'action.hover',
                                  '& .MuiFormControl-root': { position: 'absolute', inset: 0, width: '100%', height: '100%', opacity: 0, zIndex: 1 },
                                  '& .MuiInput-root, & input': { width: '100%', height: '100%', cursor: 'pointer' },
                                }}
                              >
                                <StackRow sx={{ alignItems: 'center', justifyContent: 'center', gap: 1 }}>
                                  <AddPhotoAlternateRoundedIcon color="success" />
                                  <BodyIconTypography label="Add or replace logo" />
                                </StackRow>
                                <ImageFileUploaderWithCropper helperText="Upload a square logo or icon for organization branding." onUploadCompleted={handleLogoUploadCompleted} />
                              </Box>
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
                                      aspectRatio: '16 / 9',
                                    }}
                                  >
                                    <Image
                                      width={800}
                                      height={600}
                                      unoptimized
                                      alt=""
                                      src={image.original?.url ?? image.thumbnail?.url ?? ''}
                                      style={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block' }}
                                    />
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

                              <Box
                                sx={{
                                  position: 'relative',
                                  overflow: 'hidden',
                                  border: 1,
                                  borderStyle: 'dashed',
                                  borderColor: 'success.main',
                                  borderRadius: 2.5,
                                  p: 1.5,
                                  backgroundColor: 'action.hover',
                                  '& .MuiFormControl-root': { position: 'absolute', inset: 0, width: '100%', height: '100%', opacity: 0, zIndex: 1 },
                                  '& .MuiInput-root, & input': { width: '100%', height: '100%', cursor: 'pointer' },
                                }}
                              >
                                <StackRow sx={{ alignItems: 'center', justifyContent: 'center', gap: 1 }}>
                                  <AddPhotoAlternateRoundedIcon color="success" />
                                  <BodyIconTypography label="Add another image" />
                                </StackRow>
                                <ImageFileUploaderWithCropper onUploadCompleted={handleFeatureImageUploadCompleted} />
                              </Box>
                            </StackColumn>
                          </FormFieldLabel>
                        </StackColumn>
                      </Grid>
                      <Grid size={{ xs: 12, md: 7 }}>
                        <StackColumn sx={formColumnSx}>
                          <FormFieldLabel label="Name">
                            <TextField
                              name="name"
                              required={requiredOrganizationDetailsFields.name}
                              onBlur={() => commitOrganizationPatch(['NAME'], { name: draftPatchValues.current.name })}
                            />
                          </FormFieldLabel>

                          {rootData.me?.emails.some(
                            (item) => !!rootData.emailsToShowLatestCapabilities.find((email) => email.toLocaleLowerCase() === item.toLocaleLowerCase()),
                          ) && (
                            <FormFieldLabel label="Custom Domain" required={requiredOrganizationDetailsFields.customDomain}>
                              <TextField name="customDomain" required={requiredOrganizationDetailsFields.customDomain} />
                            </FormFieldLabel>
                          )}

                          <FormFieldLabel label="Website">
                            <TextField name="website" required={requiredOrganizationDetailsFields.website} helperText="https://" />
                          </FormFieldLabel>

                          <FormFieldLabel label="Terms and Conditions">
                            <TextField
                              name="customerFacingTermsAndConditionsUrl"
                              required={requiredOrganizationDetailsFields.customerFacingTermsAndConditionsUrl}
                              helperText={<HelperText text="Provide your company's official website so members can learn more or verify your organisation." />}
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
                      </Grid>
                    </Grid>
                  </AccordionDetails>
                </Accordion>

                <Accordion
                  disableGutters
                  elevation={0}
                  expanded={expandedSection === 'contact'}
                  onChange={() => setExpandedSection(expandedSection === 'contact' ? '' : 'contact')}
                  sx={{ border: 1, borderColor: 'divider', borderRadius: '16px !important', overflow: 'hidden', '&::before': { display: 'none' } }}
                >
                  <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />}>
                    <StackColumn spacing={0.35}>
                      <LeadIconTypography label="Contact details" />
                      <BodyIconTypography label="Operational contact points and refund notification recipients." />
                    </StackColumn>
                  </AccordionSummary>
                  <AccordionDetails sx={{ borderTop: 1, borderColor: 'divider', p: { xs: 2, sm: 2.5 } }}>
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
                    </StackColumn>
                  </AccordionDetails>
                </Accordion>
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
