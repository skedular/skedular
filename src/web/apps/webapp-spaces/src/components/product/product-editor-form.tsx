import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
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
import { DurationInput, FeatureImageGallery, FieldHelp } from '@skedular/ui';
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
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import AddRoundedIcon from '@mui/icons-material/AddRounded';
import CheckCircleRoundedIcon from '@mui/icons-material/CheckCircleRounded';
import MoreVertRoundedIcon from '@mui/icons-material/MoreVertRounded';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import {
  BodyIconTypography,
  defaultButtonStyle,
  defaultPadding,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  PageHeaderPanel,
  SectionIconTypography,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@skedular/ui';
import { Switches, TextField } from 'mui-rff';
import { memo, useEffect, useMemo, useState } from 'react';
import type { PropsWithChildren } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
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
  { id: 'basics', title: 'Basics', subtitle: 'Identity, media, booking groups, currency' },
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

const editorStepIds = new Set<ProductEditorStep['id']>(['basics', 'offers', 'review']);
const editorSectionIds = new Set(['presentation', 'classification', 'offer-basics', 'fulfillment', 'booking-rules', 'payments', 'cancellation', 'advanced']);
const collapsedSectionQueryValue = 'none';

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
}: Props) => {
  const pathname = usePathname();
  const router = useRouter();
  const searchParams = useSearchParams();
  const theme = useTheme();
  const isMobileStepNav = useMediaQuery(theme.breakpoints.down('sm'), { noSsr: true });
  const pricingOptions = useMemo(() => values.pricingOptions ?? [], [values.pricingOptions]);
  const [activeStep, setActiveStep] = useState<ProductEditorStep['id']>('basics');
  const [expandedBasicsSection, setExpandedBasicsSection] = useState('presentation');
  const [expandedOfferId, setExpandedOfferId] = useState<string | false>(values.pricingOptions[0]?.id ?? false);
  const [expandedOfferSection, setExpandedOfferSection] = useState('offer-basics');
  const [offerActionsAnchor, setOfferActionsAnchor] = useState<HTMLElement | null>(null);
  const [offerActionsIndex, setOfferActionsIndex] = useState<number | null>(null);
  const [stepMenuAnchor, setStepMenuAnchor] = useState<HTMLElement | null>(null);
  const isEventProduct = isEventType(values?.type);
  const validationItems = useMemo(() => Array.from(new Set(summarizeErrors(errors))).slice(0, 8), [errors]);
  const updateEditorUrl = (updates: { tab?: ProductEditorStep['id']; offer?: string | false; section?: string }) => {
    const params = new URLSearchParams(searchParams.toString());
    if (updates.tab) params.set('tab', updates.tab);
    if (updates.offer !== undefined) {
      if (updates.offer) params.set('offer', updates.offer);
      else params.delete('offer');
    }
    if (updates.section !== undefined) {
      if (updates.section) params.set('section', updates.section);
      else params.set('section', collapsedSectionQueryValue);
    }
    const query = params.toString();
    router.replace(query ? `${pathname}?${query}` : pathname, { scroll: false });
  };
  const setEditorStep = (step: ProductEditorStep['id']) => {
    setActiveStep(step);
    updateEditorUrl({ tab: step, section: step === 'basics' ? 'presentation' : step === 'offers' ? 'offer-basics' : '' });
  };
  const setOfferSection = (section: string) => {
    setExpandedOfferSection(section);
    updateEditorUrl({ section });
  };
  const setBasicsSection = (section: string) => {
    setExpandedBasicsSection(section);
    updateEditorUrl({ section });
  };
  const selectOffer = (offer: string | false) => {
    setExpandedOfferId(offer);
    setOfferSection('offer-basics');
    updateEditorUrl({ offer, section: 'offer-basics' });
  };

  /* eslint-disable react-hooks/set-state-in-effect -- restore the view from browser URL navigation. */
  useEffect(() => {
    const requestedStep = searchParams.get('tab');
    const requestedSection = searchParams.get('section');
    const requestedOffer = searchParams.get('offer');
    if (requestedStep && editorStepIds.has(requestedStep as ProductEditorStep['id'])) setActiveStep(requestedStep as ProductEditorStep['id']);
    if (requestedSection === collapsedSectionQueryValue) {
      setExpandedOfferSection('');
      setExpandedBasicsSection('');
    } else if (requestedSection && editorSectionIds.has(requestedSection)) {
      if (requestedSection === 'presentation' || requestedSection === 'classification') setExpandedBasicsSection(requestedSection);
      else setExpandedOfferSection(requestedSection);
    }
    if (requestedOffer && pricingOptions.some((option) => option.id === requestedOffer)) setExpandedOfferId(requestedOffer);
  }, [pricingOptions, searchParams]);
  /* eslint-enable react-hooks/set-state-in-effect */
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
  const renderStepTabs = () =>
    isMobileStepNav ? (
      <Box sx={{ borderTop: 1, borderColor: 'divider', pt: 1.5 }}>
        <Button
          fullWidth
          variant="outlined"
          color="inherit"
          onClick={(event) => setStepMenuAnchor(event.currentTarget)}
          aria-haspopup="menu"
          aria-expanded={stepMenuAnchor ? 'true' : undefined}
          aria-controls={stepMenuAnchor ? 'product-editor-sections-menu' : undefined}
          endIcon={<ExpandMoreRoundedIcon />}
          sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
        >
          {`Section: ${steps.find((step) => step.id === activeStep)?.title ?? 'Basics'}`}
        </Button>
        <Menu anchorEl={stepMenuAnchor} open={Boolean(stepMenuAnchor)} onClose={() => setStepMenuAnchor(null)} id="product-editor-sections-menu">
          {steps.map((step) => (
            <MenuItem
              key={step.id}
              selected={activeStep === step.id}
              onClick={() => {
                setEditorStep(step.id);
                setStepMenuAnchor(null);
              }}
            >
              {step.title}
            </MenuItem>
          ))}
        </Menu>
      </Box>
    ) : (
      <Tabs
        value={activeStep}
        variant="scrollable"
        scrollButtons="auto"
        aria-label="Product setup sections"
        sx={{ mb: -2, borderTop: 1, borderColor: 'divider', '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0' } }}
      >
        {steps.map((step) => (
          <Tab
            key={step.id}
            value={step.id}
            label={step.title}
            onClick={() => setEditorStep(step.id)}
            disableRipple
            sx={{
              minWidth: 112,
              minHeight: 52,
              px: 2.5,
              textTransform: 'none',
              color: 'text.secondary',
              fontWeight: 500,
              '&.Mui-selected': { color: 'primary.main', fontWeight: 600 },
              '&:hover': { color: 'text.primary', backgroundColor: 'action.hover' },
            }}
          />
        ))}
      </Tabs>
    );

  const changeNestedField = (path: string, value: unknown) => {
    form.change(path, value);
  };

  useEffect(() => {
    values.pricingOptions.forEach((pricingOption, index) => {
      if (pricingOption.cadence === 'DAILY' && pricingOption.requiredDaysPerWeek) {
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
    selectOffer(nextOffer.id);
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
    selectOffer(duplicate.id);
    closeOfferActions();
  };

  const removeOffer = () => {
    if (offerActionsIndex === null || pricingOptions.length <= 1) return;
    const nextPricingOptions = pricingOptions.filter((_, index) => index !== offerActionsIndex);
    form.change('pricingOptions', nextPricingOptions);
    selectOffer(nextPricingOptions[0]?.id ?? false);
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
    <StackColumn spacing={1}>
      <EditorSection
        title="Offer basics"
        description="Set the customer-facing name and price."
        summary={`${pricingOption.title?.trim() || 'Untitled offer'} · ${pricingOption.price || 'No price'}`}
        expanded={expandedOfferSection === 'offer-basics'}
        onChange={() => setOfferSection(expandedOfferSection === 'offer-basics' ? '' : 'offer-basics')}
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
            : 'Reservations use the selected purchase term and reserve resources or time.'
        }
        summary={`${prettifyEnum(pricingOption.fulfillmentType)} · ${prettifyEnum(pricingOption.cadence)}`}
        expanded={expandedOfferSection === 'fulfillment'}
        onChange={() => setOfferSection(expandedOfferSection === 'fulfillment' ? '' : 'fulfillment')}
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
                  if (event.target.value === 'ENTITLEMENT') {
                    form.change(`pricingOptions[${index}].cadence`, 'NOT_SET');
                    form.change(`pricingOptions[${index}].supportsSubscriptionAutoRenewal`, false);
                  }
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
                help="The purchase term defines the offer period. Auto-renewal, when enabled, repeats that term; it does not constrain the booking duration."
              >
                <SingleChoiceProductPricingCadence rootDataRelay={rootDataRelay as never} name={`pricingOptions[${index}].cadence`} required />
              </FormFieldLabel>
              <SmallIconTypography label="Choose a term of one day or longer. Auto-renewal is configured separately in Payments." />
            </StackColumn>
          )}
        </StackColumn>
      </EditorSection>

      <EditorSection
        title="Booking rules"
        description={
          pricingOption.fulfillmentType === 'ENTITLEMENT'
            ? 'Define the booking limits that apply when customers use credits from this offer.'
            : 'Define availability, quantity, and booking duration.'
        }
        summary={`${pricingOption.availableDays.length || 7} days · ${pricingOption.numberOfResourcesToBook || 1} resource${pricingOption.numberOfResourcesToBook === '1' ? '' : 's'} · ${formatDurationSummary(pricingOption.minDurationMinutes)}–${formatDurationSummary(pricingOption.maxDurationMinutes)}`}
        expanded={expandedOfferSection === 'booking-rules'}
        onChange={() => setOfferSection(expandedOfferSection === 'booking-rules' ? '' : 'booking-rules')}
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
            unit={pricingOption.minDurationDisplayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
            onUnitChange={(unit) => changeNestedField(`pricingOptions[${index}].minDurationDisplayUnit`, unit.toUpperCase())}
            required
          />
          <DurationInput
            label="Maximum booking duration"
            help="The longest time a customer can book for this offer. It works with the minimum duration to define the allowed booking range and is saved in minutes."
            value={pricingOption.maxDurationMinutes}
            onChange={(value) => changeNestedField(`pricingOptions[${index}].maxDurationMinutes`, value)}
            unit={pricingOption.maxDurationDisplayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
            onUnitChange={(unit) => changeNestedField(`pricingOptions[${index}].maxDurationDisplayUnit`, unit.toUpperCase())}
            required
          />
        </Box>
        {pricingOption.cadence !== 'DAILY' ? (
          <FormFieldLabel
            label={pricingOption.fulfillmentType === 'ENTITLEMENT' ? 'Maximum redemptions per week' : 'Required selected days per week'}
            help={
              pricingOption.fulfillmentType === 'ENTITLEMENT'
                ? 'This limits successful credit redemptions in each complete Monday-through-Sunday UTC week. Customers do not select weekdays for this limit.'
                : 'This requires customers to book a specific number of the selected weekdays each week. The calendar days above define the choices.'
            }
          >
            <TextField
              name={`pricingOptions[${index}].requiredDaysPerWeek`}
              type="text"
              slotProps={{ htmlInput: { inputMode: 'numeric', pattern: '[0-9]*', maxLength: 1 } }}
              fieldProps={{ parse: sanitizeWeeklyRequiredDays }}
              helperText={
                pricingOption.fulfillmentType === 'ENTITLEMENT'
                  ? 'Leave empty for unlimited weekly redemptions; choose 1 to 7.'
                  : `Leave empty for unrestricted weekly booking; choose 1 to ${pricingOption.availableDays.length || 7}.`
              }
            />
          </FormFieldLabel>
        ) : null}
        {pricingOption.cadence !== 'DAILY' ? (
          <SmallIconTypography
            label={
              pricingOption.fulfillmentType === 'ENTITLEMENT'
                ? 'Leave this field empty for unlimited weekly redemptions.'
                : 'Leave this field empty to keep unrestricted weekly booking behavior.'
            }
          />
        ) : null}
      </EditorSection>

      <EditorSection
        title="Payments"
        description="Choose billing behavior and accepted payment methods."
        summary={`${pricingOption.acceptedPaymentMethods.length || 0} payment method${pricingOption.acceptedPaymentMethods.length === 1 ? '' : 's'} · ${prettifyEnum(pricingOption.billingMode)}`}
        expanded={expandedOfferSection === 'payments'}
        onChange={() => setOfferSection(expandedOfferSection === 'payments' ? '' : 'payments')}
      >
        <StackRow sx={{ gap: 3, flexWrap: 'wrap' }}>
          <FormFieldLabel
            helpLabel="Price includes tax"
            help="This controls whether the displayed price includes tax. It changes how the amount is presented and calculated for customers."
          >
            <Switches name={`pricingOptions[${index}].isTaxInclusive`} data={{ label: 'Price includes tax', value: 'isTaxInclusive' }} />
          </FormFieldLabel>
          {!isEventProduct && pricingOption.fulfillmentType === 'RESERVATION' ? (
            <FormFieldLabel helpLabel="Auto-renew subscription" help="When enabled, eligible purchases renew automatically according to the purchase cadence.">
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
        onChange={() => setOfferSection(expandedOfferSection === 'cancellation' ? '' : 'cancellation')}
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
                  unit={pricingOption.cancellationRefundRules[0]?.displayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
                  onUnitChange={(unit) => changeNestedField(`pricingOptions[${index}].cancellationRefundRules[0].displayUnit`, unit.toUpperCase())}
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
                      <Box>
                        <StackRow sx={{ minHeight: { md: 48 }, alignItems: 'flex-start', flexWrap: 'nowrap', gap: 0.25, mb: 1 }}>
                          <BodyIconTypography label="Refund timing before booking" />
                          <FieldHelp label="Refund timing before booking">
                            How early the customer must cancel to receive this refund percentage. This timing also applies before a renewal.
                          </FieldHelp>
                        </StackRow>
                        <DurationInput
                          label="Refund timing before booking"
                          value={rule.minutesBefore}
                          onChange={(value) => changeNestedField(`pricingOptions[${index}].cancellationRefundRules[${ruleIndex}].minutesBefore`, value)}
                          unit={rule.displayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
                          onUnitChange={(unit) => changeNestedField(`pricingOptions[${index}].cancellationRefundRules[${ruleIndex}].displayUnit`, unit.toUpperCase())}
                          required
                          hideLabel
                        />
                      </Box>
                      <Box>
                        <StackRow sx={{ minHeight: { md: 48 }, alignItems: 'flex-start', flexWrap: 'nowrap', gap: 0.25, mb: 1 }}>
                          <BodyIconTypography label="Refund percentage" />
                          <FieldHelp label="Refund percentage">
                            The percentage of the purchase price refunded when the customer cancels within this timing window. Rules are evaluated from the cancellation policy.
                          </FieldHelp>
                        </StackRow>
                        <TextField name={`pricingOptions[${index}].cancellationRefundRules[${ruleIndex}].refundPercentage`} helperText="0–100%" />
                      </Box>
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
        summary={`Card ${formatDurationSummary(pricingOption.maxAllowedResourcesLockTimePaidViaCard)} · Bank transfer ${formatDurationSummary(pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer)}`}
        expanded={expandedOfferSection === 'advanced'}
        onChange={() => setOfferSection(expandedOfferSection === 'advanced' ? '' : 'advanced')}
      >
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 2 }}>
          <Box>
            <StackRow sx={{ minHeight: { md: 48 }, alignItems: 'flex-start', flexWrap: 'nowrap', gap: 0.25, mb: 1 }}>
              <BodyIconTypography label="Card payment lock window *" />
              <FieldHelp label="Card payment lock window">
                How long resources remain held while a card payment is pending. It protects the booking without holding availability indefinitely.
              </FieldHelp>
            </StackRow>
            <DurationInput
              label="Card payment lock window"
              value={pricingOption.maxAllowedResourcesLockTimePaidViaCard}
              onChange={(value) => changeNestedField(`pricingOptions[${index}].maxAllowedResourcesLockTimePaidViaCard`, value)}
              unit={pricingOption.maxAllowedResourcesLockTimePaidViaCardDisplayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
              onUnitChange={(unit) => changeNestedField(`pricingOptions[${index}].maxAllowedResourcesLockTimePaidViaCardDisplayUnit`, unit.toUpperCase())}
              required
              hideLabel
            />
          </Box>
          <Box>
            <StackRow sx={{ minHeight: { md: 48 }, alignItems: 'flex-start', flexWrap: 'nowrap', gap: 0.25, mb: 1 }}>
              <BodyIconTypography label="Bank transfer lock window" />
              <FieldHelp label="Bank transfer lock window">
                How long resources remain held while a bank-transfer payment is pending. It protects the booking without holding availability indefinitely.
              </FieldHelp>
            </StackRow>
            <DurationInput
              label="Bank transfer lock window"
              value={pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer}
              onChange={(value) => changeNestedField(`pricingOptions[${index}].maxAllowedResourcesLockTimePaidViaBankTransfer`, value)}
              unit={pricingOption.maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
              onUnitChange={(unit) => changeNestedField(`pricingOptions[${index}].maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit`, unit.toUpperCase())}
              required
              hideLabel
            />
          </Box>
        </Box>
      </EditorSection>
    </StackColumn>
  );

  const renderBasics = () => {
    return (
      <StackColumn spacing={2}>
        <EditorSection
          title="Presentation"
          description="Pair a strong cover image with clear customer-facing copy."
          summary={`${values.title?.trim() || 'Untitled product'} · ${featureImages.length} image${featureImages.length === 1 ? '' : 's'}`}
          expanded={expandedBasicsSection === 'presentation'}
          onChange={() => setBasicsSection(expandedBasicsSection === 'presentation' ? '' : 'presentation')}
        >
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'minmax(280px, 0.9fr) minmax(0, 1.1fr)' }, gap: { xs: 2.5, md: 3 }, alignItems: 'start' }}>
            <StackColumn spacing={1.5}>
              <StackColumn spacing={0.25}>
                <LeadIconTypography label="Cover and gallery" />
                <SmallIconTypography label="Use a bright, landscape image that helps customers recognize the space quickly." />
              </StackColumn>

              <FeatureImageGallery
                images={featureImages}
                coverImage={primaryFeatureImage}
                onRemove={onRemoveFeatureImage}
                onMakeCover={onSetPrimaryFeatureImage}
                uploadControl={<ImageFileUploaderWithCropper onUploadCompleted={onUploadCompleted} />}
              />
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
          summary={`${prettifyEnum(values.type)} · ${values.currency || 'No currency'} · ${values.productTagIds.length} booking group${values.productTagIds.length === 1 ? '' : 's'}`}
          expanded={expandedBasicsSection === 'classification'}
          onChange={() => setBasicsSection(expandedBasicsSection === 'classification' ? '' : 'classification')}
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
              <BodyIconTypography label="Event products support explicit-time bookings only. Recurring plan cadences are unavailable, and the full matching booking-group resource set is reserved." />
            </Box>
          ) : null}

          <FormFieldLabel
            label="Booking groups"
            help="Booking groups determine which resources can match this product. A booking can only use resources that share one of the product's booking groups."
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
            help="Amenities describe what the product provides to customers. They explain the product but do not determine which resources it can match."
          >
            <MultipleChoicesAmenities rootDataRelay={rootDataRelay as never} name="amenityIds" required={requiredFields.amenityIds} />
          </FormFieldLabel>
        </EditorSection>
      </StackColumn>
    );
  };

  const renderOffers = () => (
    <StackColumn spacing={2}>
      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', lg: '248px minmax(0, 1fr)' }, alignItems: 'start', gap: { xs: 2, lg: 2.5 } }}>
        <Card variant="outlined" sx={{ borderRadius: 3, position: { lg: 'sticky' }, top: { lg: 88 }, boxShadow: 'none', backgroundColor: 'background.paper' }}>
          <CardContent sx={{ p: 1.5 }}>
            <StackColumn spacing={1.25}>
              <StackRow sx={{ alignItems: 'center', justifyContent: 'space-between' }}>
                <LeadIconTypography label="Offers" />
                <Chip size="small" label={`${pricingOptions.length}`} />
              </StackRow>
              <SmallIconTypography label="Manage the purchase options available for this product." />
              <Button fullWidth variant="outlined" startIcon={<AddRoundedIcon />} onClick={() => addOffer('DAILY')} sx={{ textTransform: 'none' }}>
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
                        selectOffer(pricingOption.id);
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

        <StackColumn spacing={1} sx={{ minWidth: 0 }}>
          {activeOffer ? (
            <>
              <Box
                sx={{
                  px: { xs: 2, sm: 2.5 },
                  py: 1.75,
                  border: 1,
                  borderColor: 'divider',
                  borderRadius: 3,
                  backgroundColor: 'background.paper',
                }}
              >
                <StackColumn spacing={0.25}>
                  <StackRow sx={{ alignItems: 'center', justifyContent: 'space-between', gap: 1 }}>
                    <LeadIconTypography label={activeOffer.title?.trim() || `Offer ${activeOfferIndex + 1}`} />
                    <Chip size="small" color="success" label={`Offer ${activeOfferIndex + 1} of ${pricingOptions.length}`} />
                  </StackRow>
                  <SmallIconTypography label="Edit the commercial details below." />
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
    <FormStackColumn onSubmit={onSubmit} sx={{ pt: { xs: 2, md: 3 } }}>
      <Box
        sx={{
          width: '100%',
          maxWidth: '100vw',
          minWidth: 0,
          display: 'flex',
          justifyContent: 'center',
          overflowX: 'hidden',
          boxSizing: 'border-box',
          px: { xs: 0, sm: 1, md: 2 },
          pb: defaultPadding,
        }}
      >
        <StackColumn sx={{ width: '100%', maxWidth: 1200, minWidth: 0, mx: 'auto', overflowX: 'hidden', backgroundColor: 'transparent', gap: 2 }}>
          <PageHeaderPanel eyebrow="Product setup" title={pageTitle} description={pageDescription} sx={{ width: '100%', minWidth: 0, maxWidth: '100%' }}>
            {renderStepTabs()}
          </PageHeaderPanel>

          <Box
            sx={{ display: 'grid', gridTemplateColumns: 'minmax(0, 1fr)', gap: { xs: 2, lg: 3 }, '@media (min-width: 1200px)': { gridTemplateColumns: 'minmax(0, 1fr) 288px' } }}
          >
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

            <StackColumn
              spacing={2}
              sx={{ display: 'none', '@media (min-width: 1200px)': { display: 'flex' }, position: 'sticky', top: 24, alignSelf: 'start', pl: 0, pr: 0, pt: 0 }}
            >
              <SettingsSectionCard title="Summary" description="The essential details customers use to recognize this product.">
                <StackColumn spacing={1.5}>
                  <LeadIconTypography label={values.title?.trim() || 'Untitled product'} />
                  <SmallIconTypography label={values.subTitle?.trim() || 'Add a subtitle so customers understand the product quickly.'} />
                  <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
                    <Chip size="small" label={prettifyEnum(values.type)} />
                    <Chip size="small" label={values.currency || 'No currency'} />
                    <Chip size="small" label={`${featureImages.length} image${featureImages.length === 1 ? '' : 's'}`} />
                  </StackRow>
                  <Divider />
                  <BodyIconTypography label={values.includedFeatures?.trim() || 'Add the included features customers should know about.'} />
                  <StackRow sx={{ gap: 0.75, flexWrap: 'wrap' }}>
                    <Chip size="small" label={`${values.productTagIds.length} tag${values.productTagIds.length === 1 ? '' : 's'}`} />
                    <Chip size="small" label={`${values.amenityIds.length} amenit${values.amenityIds.length === 1 ? 'y' : 'ies'}`} />
                  </StackRow>
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
            </StackColumn>
          </Box>
        </StackColumn>
      </Box>
    </FormStackColumn>
  );
};

export default memo(ProductEditorForm);
