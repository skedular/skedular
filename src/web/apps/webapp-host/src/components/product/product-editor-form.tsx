import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { DeleteIcon } from '@/components/icons';
import { ListingMetadata } from '@/components/listingMetadata';
import { SingleChoiceCurrency, SingleChoiceProductPricingCadence, SingleChoiceProductPricingCancellationType } from '@/components/organization';
import MultipleChoicesAmenities from '@/components/organization/multiple-choices-amenities';
import CalendarDayPicker from '@/components/product/calendar-day-picker';
import {
  createCancellationRefundRule,
  createPricingOption,
  isEventType,
  PricingOptionForm,
  ProductDetails,
  sanitizeWeeklyRequiredDays,
} from '@/components/product/product-editor-shared';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import {
  BodyIconTypography,
  defaultButtonStyle,
  defaultPadding,
  FormFieldLabel,
  FormStackColumn,
  GuidedEditorProgress,
  LeadIconTypography,
  PageHeaderPanel,
  SectionIconTypography,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  StackRow,
  StickyReviewRail,
} from '@skedular/ui';
import { Switches, TextField } from 'mui-rff';
import { memo, useEffect, useMemo, useState } from 'react';
import Image from 'next/image';

type Props = {
  mode: 'add' | 'edit';
  onSubmit: (event?: Partial<Pick<React.SyntheticEvent, 'preventDefault' | 'stopPropagation'>>) => Promise<Record<string, unknown> | undefined> | undefined;
  rootDataRelay: {
    defaultMaxAllowedResourcesLockTimePaidViaCard: number;
    defaultMaxAllowedResourcesLockTimePaidViaBankTransfer: number;
  };
  values: ProductDetails;
  errors: unknown;
  form: { change: (name: string, nextValue: unknown) => void };
  requiredFields: Record<string, boolean>;
  organizationCustomDomain: string;
  featureImages: FileUploadResponse[];
  primaryFeatureImage: FileUploadResponse | null;
  onUploadCompleted: (response: FileUploadResponse) => void;
  onRemoveFeatureImage: (image: FileUploadResponse) => void;
  onSetPrimaryFeatureImage: (image: FileUploadResponse) => void;
  paletteMode: 'dark' | 'light';
};

const baseSteps = [
  { id: 'basics', title: 'Basics', subtitle: 'Identity, media, tags, currency' },
  { id: 'offers', title: 'Offers', subtitle: 'Sellable options, pricing, payment, cancellation' },
  { id: 'review', title: 'Review', subtitle: 'What customers and admins will understand' },
] as const;

type ProductEditorStep = {
  id: (typeof baseSteps)[number]['id'];
  title: string;
  subtitle: string;
};

const prettifyEnum = (value?: string | null) =>
  (value ?? 'Not set')
    .toLowerCase()
    .split('_')
    .map((item) => item.charAt(0).toUpperCase() + item.slice(1))
    .join(' ');

const summarizeErrors = (errors: unknown): string[] => {
  if (!errors) {
    return [];
  }

  if (typeof errors === 'string') {
    return [errors];
  }

  if (Array.isArray(errors)) {
    return errors.flatMap((item) => summarizeErrors(item));
  }

  if (typeof errors === 'object') {
    return Object.values(errors).flatMap((value) => summarizeErrors(value));
  }

  return [];
};

const OfferSummary = ({ pricingOption, index }: { pricingOption: PricingOptionForm; index: number }) => {
  const title = pricingOption.title?.trim() || `Offer ${index + 1}`;
  const summaryBits = [pricingOption.price ? `${pricingOption.price}` : 'No price', prettifyEnum(pricingOption.cadence), 'Entire place', prettifyEnum(pricingOption.billingMode)];

  return (
    <StackColumn spacing={0.5}>
      <LeadIconTypography label={title} />
      <SmallIconTypography label={summaryBits.join(' • ')} />
    </StackColumn>
  );
};

const getCancellationPolicyDescription = (cancellationPolicyType: string) => {
  switch (cancellationPolicyType) {
    case 'NO_CANCELLATION':
      return 'Customers cannot receive a refund after purchase. Use this only when the offer is genuinely non-refundable.';
    case 'FULL_REFUND_BEFORE_CUTOFF':
      return 'Customers receive a full refund until one clear cutoff before the booking or renewal.';
    case 'TIERED_REFUND':
      return 'Customers receive different refund percentages depending on how early they cancel.';
    default:
      return 'Choose a cancellation policy before defining the refund timing.';
  }
};

const getCancellationPolicyPreview = (pricingOption: PricingOptionForm) => {
  if (pricingOption.cancellationPolicyType === 'NO_CANCELLATION') {
    return 'No refunds will be offered for this purchase option.';
  }

  if (pricingOption.cancellationPolicyType === 'FULL_REFUND_BEFORE_CUTOFF') {
    const cutoff = pricingOption.cancellationRefundRules?.[0]?.minutesBefore?.trim();
    return cutoff ? `Customers receive 100% refund if they cancel at least ${cutoff} minutes before the booking or renewal.` : 'Set the cutoff window for the full refund.';
  }

  if (pricingOption.cancellationPolicyType === 'TIERED_REFUND') {
    const ruleCount = pricingOption.cancellationRefundRules?.length ?? 0;
    return ruleCount > 0
      ? `${ruleCount} refund rule${ruleCount === 1 ? '' : 's'} configured. Customers will see a sliding refund policy based on cancellation timing.`
      : 'Add refund rules to define the sliding refund policy.';
  }

  return 'No cancellation policy selected yet.';
};

const ProductEditorForm = ({
  mode,
  onSubmit,
  rootDataRelay,
  values,
  errors,
  form,
  requiredFields,
  featureImages,
  primaryFeatureImage,
  onUploadCompleted,
  onRemoveFeatureImage,
  onSetPrimaryFeatureImage,
  paletteMode,
}: Props) => {
  const [activeStep, setActiveStep] = useState<ProductEditorStep['id']>('basics');
  const [expandedOfferId, setExpandedOfferId] = useState<string | false>(values.pricingOptions[0]?.id ?? false);
  const isEventProduct = isEventType(values?.type);
  const validationItems = useMemo(() => Array.from(new Set(summarizeErrors(errors))).slice(0, 8), [errors]);
  const pricingOptions = values?.pricingOptions ?? [];
  const activeOfferIndex = Math.max(
    0,
    pricingOptions.findIndex((pricingOption) => pricingOption.id === expandedOfferId),
  );
  const activeOffer = pricingOptions[activeOfferIndex] ?? null;
  const pageTitle = mode === 'add' ? 'Create Product' : 'Edit Product';
  const pageDescription =
    mode === 'add'
      ? 'Move through the setup in a clearer order: basics first, then offers, then a final review.'
      : 'Update the product in focused sections instead of editing one long block.';
  const submitLabel = mode === 'add' ? 'Create' : 'Update';
  const steps: ProductEditorStep[] = baseSteps
    .filter((step) => mode !== 'edit' || step.id !== 'review')
    .map((step) =>
      step.id === 'review'
        ? {
            ...step,
            title: 'Review & Create',
            subtitle: 'Final check before creating the product',
          }
        : step,
    );

  const changeNestedField = (path: string, value: unknown) => {
    form.change(path, value);
  };

  useEffect(() => {
    values.pricingOptions.forEach((pricingOption, index) => {
      if (pricingOption.cadence !== 'WEEKLY' && pricingOption.requiredDaysPerWeek) {
        form.change(`pricingOptions[${index}].requiredDaysPerWeek`, '');
      }
    });
  }, [form, values.pricingOptions]);

  const addOffer = (cadence: string) => {
    const nextOffer = {
      ...createPricingOption(rootDataRelay.defaultMaxAllowedResourcesLockTimePaidViaCard),
      cadence,
    };
    const nextPricingOptions = [...(values?.pricingOptions ?? []), nextOffer];
    form.change('pricingOptions', nextPricingOptions);
    setExpandedOfferId(nextOffer.id);
  };

  const handleCreateClick = () => {
    void onSubmit();
    if (errors && typeof errors === 'object' && !Array.isArray(errors)) {
      const errorsRecord = errors as Record<string, unknown>;
      if (Object.keys(errorsRecord).length > 0) {
        const basicsFields = ['title', 'subTitle', 'includedFeatures', 'type', 'currency', 'productTagIds', 'amenityIds'];
        const hasBasicsErrors = basicsFields.some((f) => errorsRecord[f] !== undefined);
        setActiveStep(hasBasicsErrors ? 'basics' : 'offers');
      }
    }
  };

  const renderOfferEditor = (pricingOption: PricingOptionForm, index: number) => (
    <StackColumn spacing={2}>
      <SettingsSectionCard title="Offer Basics" description="Set the label customers will understand first, then set cadence and price.">
        <ListingMetadata fields={['title', 'subTitle']} namePrefix={`pricingOptions[${index}]`} requiredFields={requiredFields} />

        <FormFieldLabel label="Cadence">
          <SingleChoiceProductPricingCadence rootDataRelay={rootDataRelay as never} name={`pricingOptions[${index}].cadence`} required />
        </FormFieldLabel>

        <FormFieldLabel label="Price">
          <TextField name={`pricingOptions[${index}].price`} required />
        </FormFieldLabel>
      </SettingsSectionCard>

      <SettingsSectionCard title="Booking Rules" description="Each booking reserves this entire place. Set the minimum and maximum booking length for this price.">
        <BodyIconTypography label="The booking resource is managed automatically for this location." sx={{ opacity: 0.78 }} />
        <CalendarDayPicker availableDays={pricingOption.availableDays} onChange={(availableDays) => changeNestedField(`pricingOptions[${index}].availableDays`, availableDays)} />
        <FormFieldLabel label="Minimum Duration (minutes)">
          <TextField name={`pricingOptions[${index}].minDurationMinutes`} required />
        </FormFieldLabel>
        <FormFieldLabel label="Maximum Duration (minutes)">
          <TextField name={`pricingOptions[${index}].maxDurationMinutes`} required />
        </FormFieldLabel>
        {pricingOption.cadence === 'WEEKLY' ? (
          <FormFieldLabel label="Required selected days per week">
            <TextField
              name={`pricingOptions[${index}].requiredDaysPerWeek`}
              type="text"
              slotProps={{ htmlInput: { inputMode: 'numeric', pattern: '[0-9]*', maxLength: 1 } }}
              fieldProps={{ parse: sanitizeWeeklyRequiredDays }}
              helperText={`Leave empty for unrestricted weekly booking; choose 1 to ${pricingOption.availableDays.length || 7}.`}
            />
          </FormFieldLabel>
        ) : null}
        {pricingOption.cadence === 'WEEKLY' ? <SmallIconTypography label="Leave this field empty to keep the existing unrestricted weekly booking behavior." /> : null}
      </SettingsSectionCard>

      <SettingsSectionCard
        title="Payments"
        description="Host bookings are paid securely by card. Skedular routes the Host proceeds through Stripe Connect after retaining its commission."
      >
        <FormFieldLabel>
          <Switches name={`pricingOptions[${index}].isTaxInclusive`} data={{ label: 'Is price tax inclusive?', value: 'isTaxInclusive' }} />
        </FormFieldLabel>
        {!isEventProduct ? (
          <FormFieldLabel>
            <Switches
              name={`pricingOptions[${index}].supportsSubscriptionAutoRenewal`}
              data={{ label: 'Supports subscription auto renewal?', value: 'supportsSubscriptionAutoRenewal' }}
            />
          </FormFieldLabel>
        ) : null}
        <BodyIconTypography label="Card payment only • Paid upfront" />
      </SettingsSectionCard>

      <SettingsSectionCard title="Cancellation" description="Set the customer-facing refund policy separately from the purchase price.">
        <Card variant="outlined" sx={{ borderRadius: 2, backgroundColor: 'action.hover' }}>
          <CardContent>
            <StackColumn spacing={1}>
              <LeadIconTypography label="Policy Summary" />
              <BodyIconTypography label={getCancellationPolicyDescription(pricingOption.cancellationPolicyType)} />
              <SmallIconTypography label={getCancellationPolicyPreview(pricingOption)} />
            </StackColumn>
          </CardContent>
        </Card>

        <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
          {[
            { type: 'NO_CANCELLATION', label: 'No Refunds' },
            { type: 'FULL_REFUND_BEFORE_CUTOFF', label: 'Full Refund Before Cutoff' },
            { type: 'TIERED_REFUND', label: 'Tiered Refunds' },
          ].map((policy) => (
            <Button
              key={policy.type}
              variant={pricingOption.cancellationPolicyType === policy.type ? 'contained' : 'outlined'}
              onClick={() => {
                changeNestedField(`pricingOptions[${index}].cancellationPolicyType`, policy.type);

                if (policy.type === 'NO_CANCELLATION') {
                  changeNestedField(`pricingOptions[${index}].cancellationRefundRules`, []);
                } else if (policy.type === 'FULL_REFUND_BEFORE_CUTOFF') {
                  changeNestedField(`pricingOptions[${index}].cancellationRefundRules`, [
                    {
                      minutesBefore: pricingOption.cancellationRefundRules?.[0]?.minutesBefore ?? '',
                      refundPercentage: '100',
                    },
                  ]);
                } else if (policy.type === 'TIERED_REFUND' && (pricingOption.cancellationRefundRules?.length ?? 0) === 0) {
                  changeNestedField(`pricingOptions[${index}].cancellationRefundRules`, [createCancellationRefundRule('100')]);
                }
              }}
              sx={{ textTransform: 'none' }}
            >
              {policy.label}
            </Button>
          ))}
        </StackRow>

        <FormFieldLabel label="Cancellation Policy">
          <SingleChoiceProductPricingCancellationType
            rootDataRelay={rootDataRelay as never}
            name={`pricingOptions[${index}].cancellationPolicyType`}
            required
            fieldProps={{
              onChange: (event: { target: { value: string } }) => {
                const nextPolicy = event.target.value;
                changeNestedField(`pricingOptions[${index}].cancellationPolicyType`, nextPolicy);

                if (nextPolicy === 'NO_CANCELLATION') {
                  changeNestedField(`pricingOptions[${index}].cancellationRefundRules`, []);
                } else if (nextPolicy === 'FULL_REFUND_BEFORE_CUTOFF' && (pricingOption.cancellationRefundRules?.length ?? 0) === 0) {
                  changeNestedField(`pricingOptions[${index}].cancellationRefundRules`, [createCancellationRefundRule('100')]);
                } else if (nextPolicy === 'TIERED_REFUND' && (pricingOption.cancellationRefundRules?.length ?? 0) === 0) {
                  changeNestedField(`pricingOptions[${index}].cancellationRefundRules`, [createCancellationRefundRule('100')]);
                }
              },
            }}
          />
        </FormFieldLabel>
        {pricingOption.cancellationPolicyType === 'FULL_REFUND_BEFORE_CUTOFF' ? (
          <Card variant="outlined" sx={{ borderRadius: 2 }}>
            <CardContent>
              <StackColumn spacing={1.5}>
                <LeadIconTypography label="Full Refund Window" />
                <SmallIconTypography label="If the customer cancels before this cutoff, they receive the full refund. After that point, no refund is offered." />
                <FormFieldLabel label="Full Refund Cutoff (minutes before booking or renewal)">
                  <TextField
                    name={`pricingOptions[${index}].cancellationRefundRules[0].minutesBefore`}
                    helperText="Example: 1440 means customers can cancel up to one day before."
                  />
                </FormFieldLabel>
              </StackColumn>
            </CardContent>
          </Card>
        ) : null}
        {pricingOption.cancellationPolicyType === 'TIERED_REFUND' ? (
          <StackColumn spacing={1.5}>
            <Card variant="outlined" sx={{ borderRadius: 2 }}>
              <CardContent>
                <StackColumn spacing={1}>
                  <LeadIconTypography label="Refund Timeline" />
                  <SmallIconTypography label="Each row means: if the customer cancels at least this far ahead, refund this percentage." />
                </StackColumn>
              </CardContent>
            </Card>
            {(pricingOption.cancellationRefundRules ?? []).map((rule, ruleIndex) => (
              <Card key={`${pricingOption.id}-${ruleIndex}`} variant="outlined" sx={{ borderRadius: 2 }}>
                <CardContent>
                  <StackColumn spacing={1.25}>
                    <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                      <BodyIconTypography label={`Refund Rule ${ruleIndex + 1}`} />
                      {(pricingOption.cancellationRefundRules?.length ?? 0) > 1 ? (
                        <Button
                          color="error"
                          onClick={() => {
                            changeNestedField(
                              `pricingOptions[${index}].cancellationRefundRules`,
                              pricingOption.cancellationRefundRules.filter((_, itemIndex) => itemIndex !== ruleIndex),
                            );
                          }}
                        >
                          Remove
                        </Button>
                      ) : null}
                    </StackRow>
                    <FormFieldLabel label="Minutes Before">
                      <TextField
                        name={`pricingOptions[${index}].cancellationRefundRules[${ruleIndex}].minutesBefore`}
                        helperText="How long before the booking or renewal this rule still applies."
                      />
                    </FormFieldLabel>
                    <FormFieldLabel label="Refund Percentage">
                      <TextField
                        name={`pricingOptions[${index}].cancellationRefundRules[${ruleIndex}].refundPercentage`}
                        helperText="The refund percentage customers receive within this timing window."
                      />
                    </FormFieldLabel>
                    <SmallIconTypography
                      label={
                        rule.minutesBefore?.trim() && rule.refundPercentage?.trim()
                          ? `If the customer cancels at least ${rule.minutesBefore} minutes before, refund ${rule.refundPercentage}%.`
                          : 'Complete both fields to preview this refund rule.'
                      }
                    />
                  </StackColumn>
                </CardContent>
              </Card>
            ))}
            <StackRow>
              <Button
                variant="outlined"
                onClick={() =>
                  changeNestedField(`pricingOptions[${index}].cancellationRefundRules`, [...(pricingOption.cancellationRefundRules ?? []), createCancellationRefundRule('0')])
                }
              >
                Add Refund Rule
              </Button>
            </StackRow>
          </StackColumn>
        ) : null}
      </SettingsSectionCard>

      <SettingsSectionCard title="Advanced" description="Keep the operational lock windows here so the commercial setup stays readable.">
        <FormFieldLabel label="Maximum Permitted Resource Lock Duration Paid via Card (minutes)">
          <TextField name={`pricingOptions[${index}].maxAllowedResourcesLockTimePaidViaCard`} required />
        </FormFieldLabel>
        <FormFieldLabel label="Maximum Permitted Resource Lock Duration Paid via Bank Transfer (days)">
          <TextField name={`pricingOptions[${index}].maxAllowedResourcesLockTimePaidViaBankTransfer`} required />
        </FormFieldLabel>
      </SettingsSectionCard>
    </StackColumn>
  );

  const renderBasics = () => (
    <StackColumn spacing={2}>
      <SettingsSectionCard title="Product Media" description="Set the visual identity first. The cover image anchors the whole product.">
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
                    minHeight: 140,
                  }}
                >
                  <Image width={800} height={600} unoptimized alt="" src={image.original?.url ?? image.thumbnail?.url ?? ''} style={{ width: '100%', height: 'auto' }} />
                  <StackRow sx={{ position: 'absolute', top: 8, right: 8 }}>
                    <IconButton size="small" aria-label="Remove feature image" onClick={() => onRemoveFeatureImage(image)}>
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </StackRow>
                  <StackRow sx={{ position: 'absolute', left: 8, bottom: 8 }}>
                    {primaryFeatureImage?.original?.url === image.original?.url ? (
                      <Chip size="small" color="success" label="Cover image" />
                    ) : (
                      <Button variant="contained" size="small" onClick={() => onSetPrimaryFeatureImage(image)} sx={{ textTransform: 'none' }}>
                        Make cover
                      </Button>
                    )}
                  </StackRow>
                </Box>
              ))}
            </Box>
            <ImageFileUploaderWithCropper onUploadCompleted={onUploadCompleted} />
          </StackColumn>
        </FormFieldLabel>
      </SettingsSectionCard>

      <SettingsSectionCard title="Customer-Facing Details" description="Write the product name, subtitle, and included features the way customers will read them.">
        <ListingMetadata fields={['title', 'subTitle', 'includedFeatures']} requiredFields={requiredFields} />
      </SettingsSectionCard>

      <SettingsSectionCard title="Listing Details" description="Choose the currency and optional amenities customers should see. The booking setup is managed automatically.">
        <BodyIconTypography label="Every offer reserves the entire location. Product tags and resources are managed by Skedular Host." sx={{ opacity: 0.78 }} />
        <FormFieldLabel label="Currency">
          <SingleChoiceCurrency rootDataRelay={rootDataRelay as never} name="currency" required={requiredFields.currency} />
        </FormFieldLabel>

        <FormFieldLabel label="Amenities">
          <MultipleChoicesAmenities rootDataRelay={rootDataRelay as never} name="amenityIds" required={requiredFields.amenityIds} />
        </FormFieldLabel>
      </SettingsSectionCard>
    </StackColumn>
  );

  const renderOffers = () => (
    <StackColumn spacing={2}>
      <SettingsSectionCard
        title="Offer Setup"
        description="Choose or create one offer at a time. This page is slower on purpose so the pricing, payments, and cancellation rules stay readable."
      >
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', lg: '320px minmax(0, 1fr)' }, gap: 2 }}>
          <Card variant="outlined" sx={{ borderRadius: 2 }}>
            <CardContent>
              <StackColumn spacing={1.5}>
                <StackColumn spacing={0.5}>
                  <LeadIconTypography label="Offer List" />
                  <SmallIconTypography label="Select one offer to edit. Add new ones only when the commercial model is genuinely different." />
                </StackColumn>
                <StackRow sx={{ flexWrap: 'wrap', gap: 1 }}>
                  <Button variant="outlined" onClick={() => addOffer('ONE_TIME')}>
                    Add One-Time Offer
                  </Button>
                  {!isEventProduct ? (
                    <Button variant="outlined" onClick={() => addOffer('MONTHLY')}>
                      Add Recurring Offer
                    </Button>
                  ) : null}
                </StackRow>

                {pricingOptions.map((pricingOption, index) => (
                  <Button
                    key={pricingOption.id}
                    variant={expandedOfferId === pricingOption.id ? 'contained' : 'outlined'}
                    onClick={() => setExpandedOfferId(pricingOption.id)}
                    sx={{ justifyContent: 'space-between', textTransform: 'none', px: 1.5, py: 1.25 }}
                  >
                    <Box sx={{ textAlign: 'left' }}>
                      <OfferSummary pricingOption={pricingOption} index={index} />
                    </Box>
                    <Chip size="small" label={prettifyEnum(pricingOption.cadence)} />
                  </Button>
                ))}
              </StackColumn>
            </CardContent>
          </Card>

          <StackColumn spacing={2}>
            {activeOffer ? (
              <>
                <Card variant="outlined" sx={{ borderRadius: 2, backgroundColor: 'action.hover' }}>
                  <CardContent>
                    <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', gap: 2 }}>
                      <StackColumn spacing={0.5}>
                        <LeadIconTypography label={activeOffer.title?.trim() || `Offer ${activeOfferIndex + 1}`} />
                        <SmallIconTypography label="Work through this offer section by section. The pricing logic is isolated here from the rest of the product." />
                      </StackColumn>
                      {pricingOptions.length > 1 ? (
                        <Button
                          color="error"
                          onClick={() => {
                            const nextPricingOptions = pricingOptions.filter((_, itemIndex) => itemIndex !== activeOfferIndex);
                            form.change('pricingOptions', nextPricingOptions);
                            setExpandedOfferId(nextPricingOptions[0]?.id ?? false);
                          }}
                        >
                          Remove Offer
                        </Button>
                      ) : null}
                    </StackRow>
                  </CardContent>
                </Card>
                {renderOfferEditor(activeOffer, activeOfferIndex)}
              </>
            ) : (
              <Card variant="outlined" sx={{ borderRadius: 2 }}>
                <CardContent>
                  <StackColumn spacing={1}>
                    <LeadIconTypography label="No Offer Selected" />
                    <SmallIconTypography label="Create or select an offer from the left before editing pricing details." />
                  </StackColumn>
                </CardContent>
              </Card>
            )}
          </StackColumn>
        </Box>
      </SettingsSectionCard>

      {typeof errors === 'object' && errors !== null && 'pricingOptions' in errors && typeof errors.pricingOptions === 'string' ? (
        <BodyIconTypography label={errors.pricingOptions} sx={{ color: 'error.main' }} />
      ) : null}
    </StackColumn>
  );

  const renderReview = () => (
    <StackColumn>
      <SectionIconTypography label="Review & Create" />
      <BodyIconTypography label="Check the high-level shape before creating the product. This is the compact product story people need to understand." />
      <Divider />

      <Card variant="outlined">
        <CardContent>
          <StackColumn spacing={1.25}>
            <LeadIconTypography label={values.title?.trim() || 'Untitled product'} />
            <SmallIconTypography label={values.subTitle?.trim() || 'No subtitle yet'} />
            <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
              <Chip size="small" label={prettifyEnum(values.type)} />
              <Chip size="small" label={values.currency || 'No currency'} />
              <Chip size="small" label={`${values.productTagIds.length} tag${values.productTagIds.length === 1 ? '' : 's'}`} />
              <Chip size="small" label={`${values.pricingOptions.length} offer${values.pricingOptions.length === 1 ? '' : 's'}`} />
            </StackRow>
            <Divider />
            {values.pricingOptions.map((pricingOption, index) => (
              <Box key={pricingOption.id} sx={{ border: 1, borderColor: 'divider', borderRadius: 2, p: 1.5 }}>
                <OfferSummary pricingOption={pricingOption} index={index} />
              </Box>
            ))}
          </StackColumn>
        </CardContent>
      </Card>
    </StackColumn>
  );

  return (
    <FormStackColumn onSubmit={onSubmit}>
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
        <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', backgroundColor: 'transparent', gap: 2 }}>
          <PageHeaderPanel eyebrow="Product setup" title={pageTitle} description={pageDescription}>
            <SmallIconTypography
              label={mode === 'add' ? 'Basics first, then offers, then a final check before saving.' : 'Update the product in smaller sections instead of one long form.'}
            />
          </PageHeaderPanel>

          <GuidedEditorProgress steps={steps} activeStepId={activeStep} onStepChange={(stepId) => setActiveStep(stepId as ProductEditorStep['id'])} variant="compact" />

          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: 'minmax(0, 1fr) 320px' }, gap: 2 }}>
            <StackColumn>
              <Box sx={{ display: activeStep !== 'basics' ? 'none' : undefined }}>{renderBasics()}</Box>
              <Box sx={{ display: activeStep !== 'offers' ? 'none' : undefined }}>{renderOffers()}</Box>
              {activeStep === 'review' ? renderReview() : null}

              {mode === 'add' && activeStep === 'review' ? (
                <StackColumn>
                  <StackRow sx={{ justifyContent: 'flex-end', flexWrap: 'wrap' }}>
                    <Button variant="contained" onClick={handleCreateClick} sx={defaultButtonStyle}>
                      {submitLabel}
                    </Button>
                  </StackRow>
                </StackColumn>
              ) : null}
            </StackColumn>

            <StickyReviewRail
              title="Review rail"
              description="Keep the product story and validation visible while editing longer sections."
              top={24}
              sx={{ pl: { xs: 0, xl: 0 }, pr: 0, pt: 0 }}
            >
              <SettingsSectionCard title="Summary" description="A compact view of the product you are shaping.">
                <StackColumn spacing={1.5}>
                  <LeadIconTypography label={values.title?.trim() || 'Untitled product'} />
                  <SmallIconTypography label={values.subTitle?.trim() || 'Add a subtitle so people understand the offer quickly.'} />
                  <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
                    <Chip size="small" label={prettifyEnum(values.type)} />
                    <Chip size="small" label={values.currency || 'No currency'} />
                    <Chip size="small" label={`${featureImages.length} image${featureImages.length === 1 ? '' : 's'}`} />
                  </StackRow>
                  <Divider />
                  <BodyIconTypography label={`Offers: ${values.pricingOptions.length}`} />
                  {values.pricingOptions.map((pricingOption, index) => (
                    <Box key={pricingOption.id} sx={{ border: 1, borderColor: 'divider', borderRadius: 2, p: 1.25 }}>
                      <OfferSummary pricingOption={pricingOption} index={index} />
                    </Box>
                  ))}
                </StackColumn>
              </SettingsSectionCard>

              <SettingsSectionCard title="Validation" description="Surface the most important issues without forcing the user back to the top of the form.">
                <StackColumn spacing={1.25}>
                  {validationItems.length === 0 ? (
                    <SmallIconTypography label="No blocking validation issues yet." />
                  ) : (
                    validationItems.map((item) => <SmallIconTypography key={item} label={item} />)
                  )}
                </StackColumn>
              </SettingsSectionCard>
            </StickyReviewRail>
          </Box>
        </StackColumn>
      </Box>
    </FormStackColumn>
  );
};

export default memo(ProductEditorForm);
