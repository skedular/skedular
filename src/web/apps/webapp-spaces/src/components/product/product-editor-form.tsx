import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { DeleteIcon } from '@/components/icons';
import { ListingMetadata } from '@/components/listingMetadata';
import {
  MultipleChoicesPaymentMethodTypes,
  MultipleChoicesProductTags,
  SingleChoiceCurrency,
  SingleChoiceProductPricingBillingMode,
  SingleChoiceProductPricingCadence,
  SingleChoiceProductType,
} from '@/components/organization';
import MultipleChoicesAmenities from '@/components/organization/multiple-choices-amenities';
import CalendarDayPicker from '@/components/product/calendar-day-picker';
import { DurationInput, FieldHelp } from '@skedular/ui';
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
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import AddRoundedIcon from '@mui/icons-material/AddRounded';
import AddPhotoAlternateRoundedIcon from '@mui/icons-material/AddPhotoAlternateRounded';
import CheckCircleRoundedIcon from '@mui/icons-material/CheckCircleRounded';
import MoreVertRoundedIcon from '@mui/icons-material/MoreVertRounded';
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
import type { PropsWithChildren } from 'react';
import Image from 'next/image';
import { v7 as uuid } from 'uuid';

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

type EditorSectionProps = {
  title: string;
  description: string;
  summary: string;
  expanded: boolean;
  onChange: () => void;
};

const EditorSection = ({ title, description, summary, expanded, onChange, children }: PropsWithChildren<EditorSectionProps>) => (
  <Accordion
    disableGutters
    elevation={0}
    expanded={expanded}
    onChange={onChange}
    sx={{
      border: 1,
      borderColor: 'divider',
      borderRadius: '16px !important',
      overflow: 'hidden',
      backgroundColor: 'background.paper',
      '&::before': { display: 'none' },
    }}
  >
    <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />} sx={{ px: 2.5, py: 0.75, minHeight: 72, '& .MuiAccordionSummary-content': { my: 1 } }}>
      <StackColumn spacing={0.35} sx={{ minWidth: 0 }}>
        <LeadIconTypography label={title} />
        <SmallIconTypography label={expanded ? description : summary} />
      </StackColumn>
    </AccordionSummary>
    <AccordionDetails sx={{ borderTop: 1, borderColor: 'divider', p: { xs: 2, sm: 2.5 } }}>
      <StackColumn spacing={2}>{children}</StackColumn>
    </AccordionDetails>
  </Accordion>
);

const formatDurationSummary = (minutesValue: string) => {
  const minutes = Number(minutesValue);
  if (!minutesValue || !Number.isFinite(minutes)) return 'Not set';
  if (minutes % 60 === 0) return `${minutes / 60}h`;
  return `${minutes}m`;
};

const formatOfferPrice = (price: string, currency: string) => {
  const numericPrice = Number(price);
  if (!price || !Number.isFinite(numericPrice)) return 'Price not set';

  try {
    return new Intl.NumberFormat('en', { style: 'currency', currency, maximumFractionDigits: 2 }).format(numericPrice);
  } catch {
    return `${currency || ''} ${numericPrice.toLocaleString('en')}`.trim();
  }
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
  const summaryBits = [
    pricingOption.price ? `${pricingOption.price}` : 'No price',
    prettifyEnum(pricingOption.cadence),
    `${pricingOption.numberOfResourcesToBook || '1'} resource${pricingOption.numberOfResourcesToBook === '1' ? '' : 's'}`,
    prettifyEnum(pricingOption.billingMode),
  ];

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
  organizationCustomDomain,
  featureImages,
  primaryFeatureImage,
  onUploadCompleted,
  onRemoveFeatureImage,
  onSetPrimaryFeatureImage,
  paletteMode,
}: Props) => {
  const [activeStep, setActiveStep] = useState<ProductEditorStep['id']>('basics');
  const [expandedBasicsSection, setExpandedBasicsSection] = useState('presentation');
  const [expandedOfferId, setExpandedOfferId] = useState<string | false>(values.pricingOptions[0]?.id ?? false);
  const [expandedOfferSection, setExpandedOfferSection] = useState('offer-basics');
  const [offerActionsAnchor, setOfferActionsAnchor] = useState<HTMLElement | null>(null);
  const [offerActionsIndex, setOfferActionsIndex] = useState<number | null>(null);
  const isEventProduct = isEventType(values?.type);
  const previewImage = primaryFeatureImage ?? featureImages[0] ?? null;
  const previewImageUrl = previewImage?.thumbnail?.url ?? previewImage?.original?.url ?? '';
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
      ...createPricingOption(rootDataRelay.defaultMaxAllowedResourcesLockTimePaidViaCard, rootDataRelay.defaultMaxAllowedResourcesLockTimePaidViaBankTransfer),
      cadence,
    };
    const nextPricingOptions = [...(values?.pricingOptions ?? []), nextOffer];
    form.change('pricingOptions', nextPricingOptions);
    setExpandedOfferId(nextOffer.id);
    setExpandedOfferSection('offer-basics');
  };

  const closeOfferActions = () => {
    setOfferActionsAnchor(null);
    setOfferActionsIndex(null);
  };

  const duplicateOffer = () => {
    if (offerActionsIndex === null) return;
    const source = pricingOptions[offerActionsIndex];
    if (!source) return;

    const duplicate = {
      ...source,
      id: uuid(),
      title: source.title ? `${source.title} copy` : null,
      acceptedPaymentMethods: [...source.acceptedPaymentMethods],
      availableDays: [...source.availableDays],
      cancellationRefundRules: source.cancellationRefundRules.map((rule) => ({ ...rule })),
    };
    const nextPricingOptions = [...pricingOptions, duplicate];
    form.change('pricingOptions', nextPricingOptions);
    setExpandedOfferId(duplicate.id);
    setExpandedOfferSection('offer-basics');
    closeOfferActions();
  };

  const removeOffer = () => {
    if (offerActionsIndex === null || pricingOptions.length <= 1) return;
    const nextPricingOptions = pricingOptions.filter((_, index) => index !== offerActionsIndex);
    form.change('pricingOptions', nextPricingOptions);
    setExpandedOfferId(nextPricingOptions[0]?.id ?? false);
    closeOfferActions();
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
      <EditorSection
        title="Offer basics"
        description="Set the customer-facing name and price."
        summary={`${pricingOption.title?.trim() || 'Untitled offer'} · ${pricingOption.price || 'No price'}`}
        expanded={expandedOfferSection === 'offer-basics'}
        onChange={() => setExpandedOfferSection(expandedOfferSection === 'offer-basics' ? '' : 'offer-basics')}
      >
        <ListingMetadata
          fields={['title', 'subTitle']}
          namePrefix={`pricingOptions[${index}]`}
          requiredFields={requiredFields}
          helpTexts={{
            title: 'The customer-facing name for this purchase option. Use it to make the difference between offers obvious.',
            subTitle: 'A short explanation of who this offer is for. Offer-specific details belong here rather than in the product title.',
          }}
        />

        <FormFieldLabel
          label="Price"
          help="The amount charged for one purchase of this offer. Currency comes from Basics, while billing mode controls when the amount is collected."
        >
          <TextField name={`pricingOptions[${index}].price`} required />
        </FormFieldLabel>
      </EditorSection>

      <EditorSection
        title="Fulfillment"
        description={
          pricingOption.fulfillmentType === 'ENTITLEMENT'
            ? 'Credit entitlements are purchased once and provide credits customers can use later.'
            : 'Reservations are purchased against a booking cadence and reserve resources or time.'
        }
        summary={`${prettifyEnum(pricingOption.fulfillmentType)} · ${prettifyEnum(pricingOption.cadence)}`}
        expanded={expandedOfferSection === 'fulfillment'}
        onChange={() => setExpandedOfferSection(expandedOfferSection === 'fulfillment' ? '' : 'fulfillment')}
      >
        <StackColumn spacing={2}>
          <FormFieldLabel
            label="Fulfillment type"
            help="Reservation offers book time or resources. Entitlement offers grant credits customers can use later. This choice controls which fields appear below."
          >
            <TextField
              select
              fullWidth
              name={`pricingOptions[${index}].fulfillmentType`}
              fieldProps={{
                onChange: (event: { target: { value: string } }) => {
                  if (event.target.value === 'ENTITLEMENT') form.change(`pricingOptions[${index}].cadence`, 'ONE_TIME');
                },
              }}
            >
              <MenuItem sx={{ minHeight: 48, fontSize: 'inherit' }} value="RESERVATION">
                Reservation
              </MenuItem>
              <MenuItem sx={{ minHeight: 48, fontSize: 'inherit' }} value="ENTITLEMENT">
                Credit entitlement
              </MenuItem>
            </TextField>
          </FormFieldLabel>
          {pricingOption.fulfillmentType === 'ENTITLEMENT' ? (
            <StackRow sx={{ gap: 2, flexWrap: 'wrap' }}>
              <FormFieldLabel
                label="Credit quantity"
                help="How many credits this purchase grants. Customers spend these credits against eligible bookings until they are used or expire."
              >
                <TextField name={`pricingOptions[${index}].entitlementCreditQuantity`} />
              </FormFieldLabel>
              <FormFieldLabel label="Validity (days)" help="How long entitlement credits remain usable after purchase. Leave it empty when credits should not expire.">
                <TextField name={`pricingOptions[${index}].entitlementValidityDays`} />
              </FormFieldLabel>
            </StackRow>
          ) : (
            <StackColumn spacing={0.75}>
              <FormFieldLabel
                label="Cadence"
                help="One Time charges once. A repeating cadence describes when a recurring purchase renews. Booking cadence controls the resource schedule separately; auto-renewal is configured in Payments."
              >
                <SingleChoiceProductPricingCadence rootDataRelay={rootDataRelay as never} name={`pricingOptions[${index}].cadence`} required />
              </FormFieldLabel>
              <SmallIconTypography label="Choose One time for a single purchase, or a repeating cadence such as Monthly. Auto-renewal is configured separately in Payments." />
            </StackColumn>
          )}
        </StackColumn>
      </EditorSection>

      {pricingOption.fulfillmentType === 'RESERVATION' ? (
        <EditorSection
          title="Booking rules"
          description="Define availability, quantity, and booking duration."
          summary={`${pricingOption.availableDays.length || 7} days · ${pricingOption.numberOfResourcesToBook || 1} resource${pricingOption.numberOfResourcesToBook === '1' ? '' : 's'} · ${formatDurationSummary(pricingOption.minDurationMinutes)}–${formatDurationSummary(pricingOption.maxDurationMinutes)}`}
          expanded={expandedOfferSection === 'booking-rules'}
          onChange={() => setExpandedOfferSection(expandedOfferSection === 'booking-rules' ? '' : 'booking-rules')}
        >
          <CalendarDayPicker availableDays={pricingOption.availableDays} onChange={(availableDays) => changeNestedField(`pricingOptions[${index}].availableDays`, availableDays)} />
          <FormFieldLabel
            label="Number of Resources to Book"
            help="How many matching resources are reserved for each booking. Increasing this can reduce availability because every booking consumes this many resources."
          >
            <TextField
              name={`pricingOptions[${index}].numberOfResourcesToBook`}
              required
              disabled={isEventProduct}
              helperText={isEventProduct ? 'Ignored for event products. The full matching resource set will be booked.' : undefined}
            />
          </FormFieldLabel>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 2 }}>
            <DurationInput
              label="Minimum booking duration"
              help="The shortest time a customer can book for this offer. It must not exceed the maximum duration and is saved in minutes even when entered as hours."
              value={pricingOption.minDurationMinutes}
              onChange={(value) => changeNestedField(`pricingOptions[${index}].minDurationMinutes`, value)}
              required
            />
            <DurationInput
              label="Maximum booking duration"
              help="The longest time a customer can book for this offer. It works with the minimum duration to define the allowed booking range and is saved in minutes."
              value={pricingOption.maxDurationMinutes}
              onChange={(value) => changeNestedField(`pricingOptions[${index}].maxDurationMinutes`, value)}
              required
            />
          </Box>
          {pricingOption.cadence === 'WEEKLY' ? (
            <FormFieldLabel
              label="Required selected days per week"
              help="For weekly offers, this requires customers to book a specific number of the selected weekdays. The calendar days above define the choices."
            >
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
        </EditorSection>
      ) : null}

      <EditorSection
        title="Payments"
        description="Choose billing behavior and accepted payment methods."
        summary={`${pricingOption.acceptedPaymentMethods.length || 0} payment method${pricingOption.acceptedPaymentMethods.length === 1 ? '' : 's'} · ${prettifyEnum(pricingOption.billingMode)}`}
        expanded={expandedOfferSection === 'payments'}
        onChange={() => setExpandedOfferSection(expandedOfferSection === 'payments' ? '' : 'payments')}
      >
        <StackRow sx={{ gap: 3, flexWrap: 'wrap' }}>
          <FormFieldLabel
            helpLabel="Price includes tax"
            help="This controls whether the displayed price includes tax. It changes how the amount is presented and calculated for customers."
          >
            <Switches name={`pricingOptions[${index}].isTaxInclusive`} data={{ label: 'Price includes tax', value: 'isTaxInclusive' }} />
          </FormFieldLabel>
          {!isEventProduct ? (
            <FormFieldLabel
              helpLabel="Auto-renew subscription"
              help="When enabled, eligible recurring purchases renew automatically according to the purchase cadence. It is independent from the booking cadence."
            >
              <Switches name={`pricingOptions[${index}].supportsSubscriptionAutoRenewal`} data={{ label: 'Auto-renew subscription', value: 'supportsSubscriptionAutoRenewal' }} />
            </FormFieldLabel>
          ) : null}
        </StackRow>
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 2 }}>
          <FormFieldLabel label="Accepted payment methods" help="Choose how customers may pay this offer. The selected methods also affect the resource lock windows in Advanced.">
            <MultipleChoicesPaymentMethodTypes rootDataRelay={rootDataRelay as never} name={`pricingOptions[${index}].acceptedPaymentMethods`} required />
          </FormFieldLabel>
          <FormFieldLabel
            label="Billing mode"
            help="Upfront collects payment before service. In Arrears allows booking first and settles payment afterward. This is separate from purchase cadence."
          >
            <SingleChoiceProductPricingBillingMode rootDataRelay={rootDataRelay as never} name={`pricingOptions[${index}].billingMode`} required />
          </FormFieldLabel>
        </Box>
      </EditorSection>

      <EditorSection
        title="Cancellation"
        description="Choose the refund policy customers will see."
        summary={getCancellationPolicyPreview(pricingOption)}
        expanded={expandedOfferSection === 'cancellation'}
        onChange={() => setExpandedOfferSection(expandedOfferSection === 'cancellation' ? '' : 'cancellation')}
      >
        <StackRow sx={{ alignItems: 'center', gap: 0.25 }}>
          <BodyIconTypography label="Cancellation policy" />
          <FieldHelp label="Cancellation policy">
            Choose how much customers receive back when they cancel. The selected policy determines whether you configure one cutoff or multiple refund timing rules. Refund timing
            is measured before the booking or renewal, while the refund percentage controls the amount returned.
          </FieldHelp>
        </StackRow>
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' }, gap: 1.25 }}>
          {[
            { type: 'NO_CANCELLATION', label: 'No refunds', description: 'All purchases are final.' },
            { type: 'FULL_REFUND_BEFORE_CUTOFF', label: 'Full refund', description: 'Refund before one cutoff.' },
            { type: 'TIERED_REFUND', label: 'Tiered refunds', description: 'Refund varies by notice.' },
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
              sx={{ textTransform: 'none', justifyContent: 'flex-start', textAlign: 'left', borderRadius: 2.5, px: 1.5, py: 1.25 }}
            >
              <StackColumn spacing={0.25}>
                <BodyIconTypography label={policy.label} />
                <SmallIconTypography label={policy.description} />
              </StackColumn>
            </Button>
          ))}
        </Box>
        <Box sx={{ borderRadius: 2, backgroundColor: 'action.hover', px: 1.5, py: 1.25 }}>
          <SmallIconTypography label={getCancellationPolicyDescription(pricingOption.cancellationPolicyType)} />
        </Box>
        {pricingOption.cancellationPolicyType === 'FULL_REFUND_BEFORE_CUTOFF' ? (
          <Card variant="outlined" sx={{ borderRadius: 2 }}>
            <CardContent>
              <StackColumn spacing={1.5}>
                <LeadIconTypography label="Full Refund Window" />
                <SmallIconTypography label="If the customer cancels before this cutoff, they receive the full refund. After that point, no refund is offered." />
                <DurationInput
                  label="Full refund cutoff before booking or renewal"
                  value={pricingOption.cancellationRefundRules[0]?.minutesBefore ?? ''}
                  onChange={(value) => changeNestedField(`pricingOptions[${index}].cancellationRefundRules[0].minutesBefore`, value)}
                  required
                />
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
                    <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '2fr 1fr' }, gap: 2 }}>
                      <DurationInput
                        label="Refund timing before booking or renewal"
                        help="How early the customer must cancel to receive this refund percentage. Multiple rules create a timeline from the latest cutoff to the earliest."
                        value={rule.minutesBefore}
                        onChange={(value) => changeNestedField(`pricingOptions[${index}].cancellationRefundRules[${ruleIndex}].minutesBefore`, value)}
                        required
                      />
                      <FormFieldLabel
                        label="Refund percentage"
                        help="The percentage of the purchase price refunded when the customer cancels within this timing window. Rules are evaluated from the cancellation policy."
                      >
                        <TextField name={`pricingOptions[${index}].cancellationRefundRules[${ruleIndex}].refundPercentage`} helperText="0–100%" />
                      </FormFieldLabel>
                    </Box>
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
      </EditorSection>

      <EditorSection
        title="Advanced"
        description="Set how long resources remain reserved while payment completes."
        summary={`Card ${formatDurationSummary(pricingOption.maxAllowedResourcesLockTimePaidViaCard)} · Bank transfer ${pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer || 'not set'} days`}
        expanded={expandedOfferSection === 'advanced'}
        onChange={() => setExpandedOfferSection(expandedOfferSection === 'advanced' ? '' : 'advanced')}
      >
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 2 }}>
          <DurationInput
            label="Card payment lock window"
            help="How long resources remain held while a card payment is pending. It protects the booking without holding availability indefinitely."
            value={pricingOption.maxAllowedResourcesLockTimePaidViaCard}
            onChange={(value) => changeNestedField(`pricingOptions[${index}].maxAllowedResourcesLockTimePaidViaCard`, value)}
            required
          />
          <FormFieldLabel
            label="Bank transfer lock window (days)"
            help="How long resources remain held while a bank-transfer payment is pending. It protects the booking without holding availability indefinitely."
          >
            <TextField name={`pricingOptions[${index}].maxAllowedResourcesLockTimePaidViaBankTransfer`} required />
          </FormFieldLabel>
        </Box>
      </EditorSection>
    </StackColumn>
  );

  const renderBasics = () => {
    const coverImage = primaryFeatureImage ?? featureImages[0] ?? null;
    const coverImageUrl = coverImage?.original?.url ?? coverImage?.thumbnail?.url ?? '';

    return (
      <StackColumn spacing={2}>
        <StackColumn spacing={0.5} sx={{ px: { xs: 0, sm: 1 } }}>
          <SectionIconTypography label="Product basics" />
          <SmallIconTypography label="Shape how this product looks to customers, then add the details that control where it appears." />
        </StackColumn>

        <EditorSection
          title="Product presentation"
          description="Pair a strong cover image with clear customer-facing copy."
          summary={`${values.title?.trim() || 'Untitled product'} · ${featureImages.length} image${featureImages.length === 1 ? '' : 's'}`}
          expanded={expandedBasicsSection === 'presentation'}
          onChange={() => setExpandedBasicsSection(expandedBasicsSection === 'presentation' ? '' : 'presentation')}
        >
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'minmax(280px, 0.9fr) minmax(0, 1.1fr)' }, gap: { xs: 2.5, md: 3 }, alignItems: 'start' }}>
            <StackColumn spacing={1.5}>
              <StackColumn spacing={0.25}>
                <LeadIconTypography label="Cover and gallery" />
                <SmallIconTypography label="Use a bright, landscape image that helps customers recognize the space quickly." />
              </StackColumn>

              <Box
                sx={{
                  position: 'relative',
                  aspectRatio: '16 / 9',
                  overflow: 'hidden',
                  border: 1,
                  borderColor: coverImageUrl ? 'divider' : 'transparent',
                  borderRadius: 3,
                  backgroundColor: paletteMode === 'dark' ? 'grey.900' : 'grey.100',
                  backgroundImage: coverImageUrl ? undefined : 'linear-gradient(135deg, rgba(104, 211, 126, 0.18), rgba(104, 211, 126, 0.04))',
                }}
              >
                {coverImageUrl ? (
                  <>
                    <Image fill unoptimized alt="Product cover" src={coverImageUrl} sizes="(max-width: 900px) 100vw, 40vw" style={{ objectFit: 'cover' }} />
                    <Box sx={{ position: 'absolute', inset: 0, background: 'linear-gradient(180deg, transparent 55%, rgba(0,0,0,0.58))' }} />
                    <Chip size="small" color="success" label="Cover image" sx={{ position: 'absolute', left: 12, bottom: 12 }} />
                    <IconButton
                      size="small"
                      aria-label="Remove cover image"
                      onClick={() => coverImage && onRemoveFeatureImage(coverImage)}
                      sx={{
                        position: 'absolute',
                        top: 10,
                        right: 10,
                        color: 'common.white',
                        backgroundColor: 'rgba(0,0,0,0.48)',
                        '&:hover': { backgroundColor: 'rgba(0,0,0,0.68)' },
                      }}
                    >
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </>
                ) : (
                  <StackColumn spacing={0.75} sx={{ position: 'absolute', inset: 0, alignItems: 'center', justifyContent: 'center', textAlign: 'center', px: 3 }}>
                    <AddPhotoAlternateRoundedIcon color="success" sx={{ fontSize: 42 }} />
                    <LeadIconTypography label="Add a cover image" />
                    <SmallIconTypography label="Landscape images work best." />
                  </StackColumn>
                )}
              </Box>

              {featureImages.length > 1 ? (
                <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(92px, 1fr))', gap: 1 }}>
                  {featureImages.map((image, index) => {
                    const imageUrl = image.thumbnail?.url ?? image.original?.url ?? '';
                    const isCover = coverImage?.original?.url === image.original?.url;
                    if (!imageUrl) return null;

                    return (
                      <Box
                        key={image.original?.url ?? image.thumbnail?.url ?? index}
                        component="button"
                        type="button"
                        aria-label={isCover ? 'Current cover image' : 'Make image the cover'}
                        onClick={() => onSetPrimaryFeatureImage(image)}
                        sx={{
                          position: 'relative',
                          aspectRatio: '4 / 3',
                          overflow: 'hidden',
                          borderRadius: 2,
                          border: 2,
                          borderColor: isCover ? 'success.main' : 'divider',
                          p: 0,
                          cursor: 'pointer',
                          backgroundColor: 'background.paper',
                        }}
                      >
                        <Image fill unoptimized alt="" src={imageUrl} sizes="120px" style={{ objectFit: 'cover' }} />
                        {isCover ? (
                          <CheckCircleRoundedIcon color="success" sx={{ position: 'absolute', top: 5, right: 5, backgroundColor: 'background.paper', borderRadius: '50%' }} />
                        ) : null}
                      </Box>
                    );
                  })}
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
                  p: 2,
                  backgroundColor: 'action.hover',
                  '& .MuiFormControl-root': { position: 'absolute', inset: 0, width: '100%', height: '100%', opacity: 0, zIndex: 1 },
                  '& .MuiInput-root, & input': { width: '100%', height: '100%', cursor: 'pointer' },
                }}
              >
                <StackRow sx={{ alignItems: 'center', justifyContent: 'center', gap: 1 }}>
                  <AddPhotoAlternateRoundedIcon color="success" />
                  <BodyIconTypography label={featureImages.length === 0 ? 'Choose a cover image' : 'Add another image'} />
                </StackRow>
                <ImageFileUploaderWithCropper onUploadCompleted={onUploadCompleted} />
              </Box>
            </StackColumn>

            <StackColumn spacing={2}>
              <StackColumn spacing={0.25}>
                <LeadIconTypography label="Customer-facing details" />
                <SmallIconTypography label="Use concise language customers can scan before choosing an offer." />
              </StackColumn>
              <FormFieldLabel
                label="Title"
                help="The product name customers see before choosing an offer. Keep it broad enough to describe the product, not a specific price or cadence."
                required={requiredFields.title}
              >
                <TextField name="title" required={requiredFields.title} placeholder="For example, Premium Meeting Room" />
              </FormFieldLabel>
              <FormFieldLabel
                label="Subtitle"
                help="A short supporting statement that explains the product at a glance. Offer-specific differences belong in the Offers tab."
                required={requiredFields.subTitle}
              >
                <TextField name="subTitle" required={requiredFields.subTitle} multiline rows={2} placeholder="A short reason to choose this product" />
              </FormFieldLabel>
              <FormFieldLabel
                label="Included features"
                help="List what customers receive with the product. Booking quantity, duration, and availability are configured on each offer."
                required={requiredFields.includedFeatures}
              >
                <TextField
                  name="includedFeatures"
                  required={requiredFields.includedFeatures}
                  multiline
                  rows={4}
                  placeholder="Describe the equipment, services, or access included"
                  helperText="Keep this easy to scan. Separate longer lists with commas or line breaks."
                />
              </FormFieldLabel>
            </StackColumn>
          </Box>
        </EditorSection>

        <EditorSection
          title="Classification"
          description="Control how this product behaves and where customers discover it."
          summary={`${prettifyEnum(values.type)} · ${values.currency || 'No currency'} · ${values.productTagIds.length} tag${values.productTagIds.length === 1 ? '' : 's'}`}
          expanded={expandedBasicsSection === 'classification'}
          onChange={() => setExpandedBasicsSection(expandedBasicsSection === 'classification' ? '' : 'classification')}
        >
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' }, gap: 2 }}>
            <FormFieldLabel
              label="Product type"
              help="The product type determines how matching resources and bookings behave. Choose it before configuring offers because event products have different booking rules."
            >
              <SingleChoiceProductType rootDataRelay={rootDataRelay as never} name="type" required={requiredFields.type} />
            </FormFieldLabel>
            <FormFieldLabel label="Currency" help="The currency used by every offer on this product. Prices are entered per offer, but they all use this product-level currency.">
              <SingleChoiceCurrency rootDataRelay={rootDataRelay as never} name="currency" required={requiredFields.currency} />
            </FormFieldLabel>
          </Box>

          {isEventProduct ? (
            <Box sx={{ borderRadius: 2, backgroundColor: 'action.hover', p: 1.5 }}>
              <BodyIconTypography label="Event products support explicit-time bookings only. Recurring plan cadences are unavailable, and the full matching tagged resource set is reserved." />
            </Box>
          ) : null}

          <FormFieldLabel
            label="Product tags"
            help="Tags determine which resources can match this product. A booking can only use resources that satisfy the product's matching tags."
          >
            <MultipleChoicesProductTags
              rootDataRelay={rootDataRelay as never}
              name="productTagIds"
              required={requiredFields.productTagIds}
              organizationCustomDomain={organizationCustomDomain}
            />
          </FormFieldLabel>

          <FormFieldLabel
            label="Amenities"
            help="Amenities describe what the product provides to customers. They explain the product but do not determine resource matching like tags do."
          >
            <MultipleChoicesAmenities rootDataRelay={rootDataRelay as never} name="amenityIds" required={requiredFields.amenityIds} />
          </FormFieldLabel>
        </EditorSection>
      </StackColumn>
    );
  };

  const renderOffers = () => (
    <StackColumn spacing={2}>
      <StackColumn spacing={0.5} sx={{ px: { xs: 0, sm: 1 } }}>
        <SectionIconTypography label="Offers" />
        <SmallIconTypography label="Configure one commercial offer at a time. Add another only when the price, cadence, or booking rules are genuinely different." />
      </StackColumn>

      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', lg: '250px minmax(0, 1fr)' }, alignItems: 'start', gap: { xs: 2, lg: 3 } }}>
        <Card variant="outlined" sx={{ borderRadius: 3, position: { lg: 'sticky' }, top: { lg: 24 }, boxShadow: 'none', backgroundColor: 'background.paper' }}>
          <CardContent sx={{ p: 1.5 }}>
            <StackColumn spacing={1.25}>
              <StackRow sx={{ alignItems: 'center', justifyContent: 'space-between' }}>
                <LeadIconTypography label="Offers" />
                <Chip size="small" label={`${pricingOptions.length}`} />
              </StackRow>
              <SmallIconTypography label="Manage the purchase options available for this product." />
              <Button fullWidth variant="outlined" startIcon={<AddRoundedIcon />} onClick={() => addOffer('ONE_TIME')} sx={{ textTransform: 'none' }}>
                Add offer
              </Button>

              {pricingOptions.map((pricingOption, index) => {
                const selected = expandedOfferId === pricingOption.id;
                return (
                  <Box
                    key={pricingOption.id}
                    sx={{
                      position: 'relative',
                      border: 1,
                      borderLeft: 3,
                      borderColor: selected ? 'success.main' : 'divider',
                      borderRadius: 2.5,
                      overflow: 'hidden',
                      backgroundColor: selected ? 'action.selected' : 'transparent',
                    }}
                  >
                    <Button
                      variant="text"
                      fullWidth
                      onClick={() => {
                        setExpandedOfferId(pricingOption.id);
                        setExpandedOfferSection('offer-basics');
                      }}
                      sx={{ display: 'block', textAlign: 'left', textTransform: 'none', color: 'text.primary', borderRadius: 0, px: 1.25, py: 1.25, pr: 4.5 }}
                    >
                      <StackColumn spacing={0.75}>
                        <StackRow sx={{ alignItems: 'center', gap: 0.75 }}>
                          {selected ? <CheckCircleRoundedIcon color="success" fontSize="small" /> : null}
                          <LeadIconTypography label={pricingOption.title?.trim() || `Offer ${index + 1}`} />
                        </StackRow>
                        <BodyIconTypography label={formatOfferPrice(pricingOption.price, values.currency)} />
                        <StackRow sx={{ gap: 0.5, flexWrap: 'wrap' }}>
                          <Chip size="small" label={prettifyEnum(pricingOption.cadence)} />
                          <Chip size="small" label={`${pricingOption.numberOfResourcesToBook || 1} resource${pricingOption.numberOfResourcesToBook === '1' ? '' : 's'}`} />
                        </StackRow>
                        <SmallIconTypography label={prettifyEnum(pricingOption.billingMode)} />
                      </StackColumn>
                    </Button>
                    <IconButton
                      size="small"
                      aria-label={`Actions for ${pricingOption.title?.trim() || `offer ${index + 1}`}`}
                      onClick={(event) => {
                        setOfferActionsAnchor(event.currentTarget);
                        setOfferActionsIndex(index);
                      }}
                      sx={{ position: 'absolute', top: 6, right: 4 }}
                    >
                      <MoreVertRoundedIcon fontSize="small" />
                    </IconButton>
                  </Box>
                );
              })}
              <Menu anchorEl={offerActionsAnchor} open={Boolean(offerActionsAnchor)} onClose={closeOfferActions}>
                <MenuItem onClick={duplicateOffer}>Duplicate offer</MenuItem>
                <MenuItem disabled={pricingOptions.length <= 1} onClick={removeOffer} sx={{ color: 'error.main' }}>
                  Remove offer
                </MenuItem>
              </Menu>
            </StackColumn>
          </CardContent>
        </Card>

        <StackColumn spacing={2} sx={{ minWidth: 0 }}>
          {activeOffer ? (
            <>
              <Box sx={{ px: { xs: 0, sm: 1 }, py: 0.5 }}>
                <StackColumn spacing={0.25}>
                  <LeadIconTypography label={activeOffer.title?.trim() || `Offer ${activeOfferIndex + 1}`} />
                  <SmallIconTypography label="Edit the commercial details below. Changes stay focused on this offer." />
                </StackColumn>
              </Box>
              {renderOfferEditor(activeOffer, activeOfferIndex)}
            </>
          ) : (
            <Card variant="outlined" sx={{ borderRadius: 3, boxShadow: 'none' }}>
              <CardContent>
                <StackColumn spacing={1}>
                  <LeadIconTypography label="No offer selected" />
                  <SmallIconTypography label="Create or select an offer before editing its details." />
                </StackColumn>
              </CardContent>
            </Card>
          )}
        </StackColumn>
      </Box>

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
        <StackColumn sx={{ width: '100%', maxWidth: 1480, mx: 'auto', backgroundColor: 'transparent', gap: 2 }}>
          <PageHeaderPanel eyebrow="Product setup" title={pageTitle} description={pageDescription} />

          <Box sx={{ position: 'sticky', top: 12, zIndex: 10 }}>
            <GuidedEditorProgress steps={steps} activeStepId={activeStep} onStepChange={(stepId) => setActiveStep(stepId as ProductEditorStep['id'])} variant="compact" />
          </Box>

          <Box sx={{ display: 'grid', gridTemplateColumns: 'minmax(0, 1fr)', gap: 3, '@media (min-width: 1536px)': { gridTemplateColumns: 'minmax(0, 1fr) 288px' } }}>
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
              title={activeStep === 'basics' ? 'Product preview' : 'Review rail'}
              description={activeStep === 'basics' ? 'See the customer-facing identity take shape as you edit.' : 'A compact snapshot of the offer you are shaping.'}
              top={24}
              sx={{ display: 'none', '@media (min-width: 1536px)': { display: 'block' }, pl: 0, pr: 0, pt: 0 }}
            >
              <SettingsSectionCard
                title={activeStep === 'basics' ? 'Customer preview' : 'Summary'}
                description={activeStep === 'basics' ? 'The essential details customers use to recognize this product.' : 'A compact view of the product you are shaping.'}
              >
                <StackColumn spacing={1.5}>
                  {activeStep === 'basics' && previewImageUrl ? (
                    <Box sx={{ position: 'relative', overflow: 'hidden', aspectRatio: '16 / 9', borderRadius: 2.5, backgroundColor: 'action.hover' }}>
                      <Image fill unoptimized alt="Product preview" src={previewImageUrl} sizes="288px" style={{ objectFit: 'cover' }} />
                    </Box>
                  ) : null}
                  <LeadIconTypography label={values.title?.trim() || 'Untitled product'} />
                  <SmallIconTypography label={values.subTitle?.trim() || 'Add a subtitle so customers understand the product quickly.'} />
                  <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
                    <Chip size="small" label={prettifyEnum(values.type)} />
                    <Chip size="small" label={values.currency || 'No currency'} />
                    <Chip size="small" label={`${featureImages.length} image${featureImages.length === 1 ? '' : 's'}`} />
                  </StackRow>
                  {activeStep === 'basics' ? (
                    <>
                      <Divider />
                      <BodyIconTypography label={values.includedFeatures?.trim() || 'Add the included features customers should know about.'} />
                      <StackRow sx={{ gap: 0.75, flexWrap: 'wrap' }}>
                        <Chip size="small" label={`${values.productTagIds.length} tag${values.productTagIds.length === 1 ? '' : 's'}`} />
                        <Chip size="small" label={`${values.amenityIds.length} amenit${values.amenityIds.length === 1 ? 'y' : 'ies'}`} />
                      </StackRow>
                    </>
                  ) : (
                    <>
                      <Divider />
                      <BodyIconTypography label={`Offers: ${values.pricingOptions.length}`} />
                      {values.pricingOptions.map((pricingOption, index) => (
                        <Box key={pricingOption.id} sx={{ border: 1, borderColor: 'divider', borderRadius: 2, p: 1.25 }}>
                          <OfferSummary pricingOption={pricingOption} index={index} />
                        </Box>
                      ))}
                    </>
                  )}
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
