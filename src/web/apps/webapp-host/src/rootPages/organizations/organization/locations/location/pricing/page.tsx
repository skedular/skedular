'use client';

import { getOrganizationLocationBaseLink } from '@/components/links';
import { NotificationContent } from '@/components/notification';
import { RootShell } from '@/components/rootShell';
import { useHostListingCoordinator } from '@/components/unified-listing-form';
import type { CancellationRefundRuleForm, PricingOptionForm } from '@/components/unified-listing-form/HostListingProductSettings';
import HostListingProductSettings from '@/components/unified-listing-form/HostListingProductSettings';
import type { locationPricingEditQuery } from '@/queries/__generated__/locationPricingEditQuery.graphql';
import { getWeeklyRequiredDaysError, hasValidWeeklyRequiredDays, sanitizeWeeklyRequiredDays } from './weekly-required-days';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Stack from '@mui/material/Stack';
import { useIntegratedPlatform, useKnownParams } from '@skedular/shared';
import { BodyIconTypography, MediumHeadingIconTypography, SmallIconTypography } from '@skedular/ui';
import NextLink from 'next/link';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { graphql, useLazyLoadQuery, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';

const defaultPricingOption = (): PricingOptionForm => ({
  id: crypto.randomUUID(),
  title: '',
  price: '',
  cadence: 'DAILY',
  billingMode: 'UPFRONT',
  acceptedPaymentMethods: ['CARD'],
  availableDays: [],
  requiredDaysPerWeek: '',
  cancellationPolicyType: 'NO_CANCELLATION',
  cancellationRefundRules: [],
  minDurationMinutes: '',
  maxDurationMinutes: '',
  isTaxInclusive: false,
  supportsSubscriptionAutoRenewal: false,
  maxAllowedResourcesLockTimePaidViaCard: '15',
  maxAllowedResourcesLockTimePaidViaBankTransfer: '0',
  fulfillmentType: 'RESERVATION',
  entitlementCreditQuantity: '',
  entitlementValidityDays: '',
  minDurationDisplayUnit: null,
  maxDurationDisplayUnit: null,
  maxAllowedResourcesLockTimePaidViaCardDisplayUnit: null,
  maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit: null,
});

type FormValues = {
  listingTitle: string;
  listingAbout: string;
  currency: 'USD' | 'NZD';
  imageUrlsCsv: string;
  pricingOptions: PricingOptionForm[];
};

type QueryData = locationPricingEditQuery['response'];

const buildInitialValues = (data: QueryData): FormValues => {
  const product = data.location?.products?.[0];

  const pricingOptions: PricingOptionForm[] = (product?.pricingOptions ?? []).map((opt) => ({
    id: opt.id ?? crypto.randomUUID(),
    title: opt.listingMetadata?.title ?? '',
    price: opt.price != null ? String(opt.price) : '',
    cadence: opt.purchaseCadence ?? 'DAILY',
    billingMode: opt.billingMode ?? 'UPFRONT',
    acceptedPaymentMethods: opt.acceptedPaymentMethods ? [...opt.acceptedPaymentMethods] : ['CARD'],
    availableDays: opt.availableDays ? [...opt.availableDays] : [],
    requiredDaysPerWeek: opt.requiredDaysPerWeek != null ? String(opt.requiredDaysPerWeek) : '',
    cancellationPolicyType: opt.cancellationPolicyType ?? 'NO_CANCELLATION',
    cancellationRefundRules: (opt.cancellationRefundRules ?? []).map((r) => ({
      minutesBefore: String(r.minutesBefore),
      displayUnit: r.displayUnit ?? null,
      refundPercentage: String(r.refundPercentage),
    })),
    minDurationMinutes: opt.minDurationMinutes != null ? String(opt.minDurationMinutes) : '',
    minDurationDisplayUnit: opt.minDurationDisplayUnit ?? null,
    maxDurationMinutes: opt.maxDurationMinutes != null ? String(opt.maxDurationMinutes) : '',
    maxDurationDisplayUnit: opt.maxDurationDisplayUnit ?? null,
    isTaxInclusive: Boolean(opt.isTaxInclusive),
    supportsSubscriptionAutoRenewal: Boolean(opt.supportsSubscriptionAutoRenewal),
    maxAllowedResourcesLockTimePaidViaCard: opt.maxAllowedResourcesLockTimePaidViaCard != null ? String(opt.maxAllowedResourcesLockTimePaidViaCard) : '15',
    maxAllowedResourcesLockTimePaidViaCardDisplayUnit: opt.maxAllowedResourcesLockTimePaidViaCardDisplayUnit ?? null,
    maxAllowedResourcesLockTimePaidViaBankTransfer: opt.maxAllowedResourcesLockTimePaidViaBankTransfer != null ? String(opt.maxAllowedResourcesLockTimePaidViaBankTransfer) : '0',
    maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit: opt.maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit ?? null,
    fulfillmentType: opt.fulfillmentType ?? 'RESERVATION',
    entitlementCreditQuantity: opt.entitlementCreditQuantity != null ? String(opt.entitlementCreditQuantity) : '',
    entitlementValidityDays: opt.entitlementValidityDays != null ? String(opt.entitlementValidityDays) : '',
  }));

  return {
    listingTitle: product?.listingMetadata?.title ?? '',
    listingAbout: product?.listingMetadata?.about ?? '',
    currency: product?.currency?.type === 'NZD' ? 'NZD' : 'USD',
    imageUrlsCsv: (product?.featureImages ?? [])
      .map((item) => item?.original?.url)
      .filter((url): url is string => Boolean(url))
      .join('\n'),
    pricingOptions: pricingOptions.length > 0 ? pricingOptions : [defaultPricingOption()],
  };
};

export const PricingPage = ({ embedded = false }: { embedded?: boolean }) => {
  const { organizationCustomDomain, locationId } = useKnownParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const coordinator = useHostListingCoordinator();

  if (!organizationCustomDomain) throw new Error('organizationCustomDomain is required');
  if (!locationId) throw new Error('locationId is required');

  const backLink = getOrganizationLocationBaseLink(integratedPlatform, organizationCustomDomain, locationId);

  const data = useLazyLoadQuery<locationPricingEditQuery>(
    graphql`
      query locationPricingEditQuery($locationId: String!) {
        location(id: $locationId) {
          id
          canModify
          products {
            id
            currency {
              type
            }
            listingMetadata {
              title
              about
            }
            featureImages {
              original {
                url
              }
            }
            pricingOptions {
              id
              price
              purchaseCadence
              billingMode
              acceptedPaymentMethods
              availableDays
              requiredDaysPerWeek
              minDurationMinutes
              minDurationDisplayUnit
              maxDurationMinutes
              maxDurationDisplayUnit
              cancellationPolicyType
              cancellationRefundRules {
                minutesBefore
                displayUnit
                refundPercentage
              }
              isTaxInclusive
              supportsSubscriptionAutoRenewal
              maxAllowedResourcesLockTimePaidViaCard
              maxAllowedResourcesLockTimePaidViaCardDisplayUnit
              maxAllowedResourcesLockTimePaidViaBankTransfer
              maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit
              fulfillmentType
              entitlementCreditQuantity
              entitlementValidityDays
              listingMetadata {
                title
                subTitle
              }
            }
          }
        }
      }
    `,
    { locationId },
    { fetchPolicy: 'store-and-network' },
  );

  const product = data.location?.products?.[0];
  const productReady = Boolean(product?.id);
  const canModify = Boolean(data.location?.canModify);

  const initialValues = useMemo(() => buildInitialValues(data), [data]);
  const [values, setValues] = useState<FormValues>(initialValues);
  const [saveStatus, setSaveStatus] = useState<'idle' | 'saving' | 'saved' | 'error'>('idle');
  const weeklyRequiredDaysErrors = useMemo(
    () =>
      Object.fromEntries(
        values.pricingOptions.map((pricingOption, index) => [
          index,
          getWeeklyRequiredDaysError(pricingOption.cadence, pricingOption.requiredDaysPerWeek, pricingOption.availableDays, pricingOption.fulfillmentType),
        ]),
      ),
    [values.pricingOptions],
  );
  const hasInvalidWeeklyRequiredDays = Object.values(weeklyRequiredDaysErrors).some(Boolean);
  const displayedSaveStatus = hasInvalidWeeklyRequiredDays ? 'error' : saveStatus;
  const lastSavedRef = useRef<FormValues>(initialValues);
  const currentValuesRef = useRef<FormValues>(initialValues);
  const isInitialMountRef = useRef(true);

  useEffect(() => {
    // The mutation response and the query subscription can arrive out of order.
    // Do not overwrite a locally saved edit with a stale query snapshot.
    if (JSON.stringify(initialValues) !== JSON.stringify(lastSavedRef.current)) {
      return;
    }
    setValues(initialValues);
    lastSavedRef.current = initialValues;
  }, [initialValues]);

  useEffect(() => {
    currentValuesRef.current = values;
  }, [values]);

  const appliedPendingDraftRef = useRef(false);
  useEffect(() => {
    if (!productReady || appliedPendingDraftRef.current) return;
    const draft = coordinator.state.pendingProductDraft;
    if (!draft) return;

    // eslint-disable-next-line react-hooks/set-state-in-effect
    setValues((current) => {
      const firstOpt = current.pricingOptions[0] ?? defaultPricingOption();
      const updatedFirstOpt: PricingOptionForm = {
        ...firstOpt,
        price: draft.price != null ? String(draft.price) : firstOpt.price,
        cadence: draft.cadence ?? firstOpt.cadence,
        cancellationPolicyType: draft.cancellationPolicyType ?? firstOpt.cancellationPolicyType,
        minDurationMinutes: draft.minDurationMinutes != null ? String(draft.minDurationMinutes) : firstOpt.minDurationMinutes,
        maxDurationMinutes: draft.maxDurationMinutes != null ? String(draft.maxDurationMinutes) : firstOpt.maxDurationMinutes,
        isTaxInclusive: draft.isTaxInclusive ?? firstOpt.isTaxInclusive,
        supportsSubscriptionAutoRenewal: draft.supportsSubscriptionAutoRenewal ?? firstOpt.supportsSubscriptionAutoRenewal,
      };
      return {
        ...current,
        listingTitle: draft.title ?? current.listingTitle,
        listingAbout: draft.about ?? current.listingAbout,
        imageUrlsCsv: draft.imageUrls?.join('\n') ?? current.imageUrlsCsv,
        pricingOptions: [updatedFirstOpt, ...current.pricingOptions.slice(1)],
      };
    });

    coordinator.clearPendingProductDraft();
    appliedPendingDraftRef.current = true;
  }, [coordinator, productReady]);

  const [commitProduct] = useMutation(graphql`
    mutation locationPricingEditUpdateProductMutation($input: UpdateProductInput!) {
      updateProduct(input: $input) {
        product {
          id
        }
      }
    }
  `);

  const onChange = (field: keyof Omit<FormValues, 'pricingOptions'>) => (event: React.ChangeEvent<HTMLInputElement>) => {
    setValues((current) => ({ ...current, [field]: event.target.value }));
  };

  const onChangePricingOption = (index: number, field: keyof PricingOptionForm) => (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = field === 'requiredDaysPerWeek' ? sanitizeWeeklyRequiredDays(event.target.value) : event.target.value;
    setValues((current) => ({
      ...current,
      pricingOptions: current.pricingOptions.map((opt, i) =>
        i === index
          ? {
              ...opt,
              [field]: value,
              ...(field === 'cadence' && value === 'DAILY' ? { requiredDaysPerWeek: '' } : {}),
            }
          : opt,
      ),
    }));
  };

  const onChangeFulfillmentType = (index: number, value: string) => {
    setValues((current) => ({
      ...current,
      pricingOptions: current.pricingOptions.map((opt, i) => (i === index ? { ...opt, fulfillmentType: value } : opt)),
    }));
  };

  const onTogglePricingOption = (index: number, field: 'isTaxInclusive' | 'supportsSubscriptionAutoRenewal', value: boolean) => {
    setValues((current) => ({
      ...current,
      pricingOptions: current.pricingOptions.map((opt, i) => (i === index ? { ...opt, [field]: value } : opt)),
    }));
  };

  const onAddPricingOption = () => {
    setValues((current) => ({ ...current, pricingOptions: [...current.pricingOptions, defaultPricingOption()] }));
  };

  const onRemovePricingOption = (index: number) => {
    setValues((current) => ({ ...current, pricingOptions: current.pricingOptions.filter((_, i) => i !== index) }));
  };

  const onChangePaymentMethods = (optionIndex: number, methods: string[]) => {
    setValues((current) => ({
      ...current,
      pricingOptions: current.pricingOptions.map((opt, i) => (i === optionIndex ? { ...opt, acceptedPaymentMethods: methods } : opt)),
    }));
  };

  const onChangeAvailableDays = (optionIndex: number, availableDays: string[]) => {
    setValues((current) => ({
      ...current,
      pricingOptions: current.pricingOptions.map((opt, i) => (i === optionIndex ? { ...opt, availableDays } : opt)),
    }));
  };

  const onSetCancellationPolicy = (optionIndex: number, policy: string) => {
    setValues((current) => ({
      ...current,
      pricingOptions: current.pricingOptions.map((opt, i) => {
        if (i !== optionIndex) return opt;
        let rules: CancellationRefundRuleForm[] = [];
        if (policy === 'FULL_REFUND_BEFORE_CUTOFF') {
          rules = [{ minutesBefore: opt.cancellationRefundRules[0]?.minutesBefore ?? '', refundPercentage: '100' }];
        } else if (policy === 'TIERED_REFUND') {
          rules = opt.cancellationRefundRules.length > 0 ? opt.cancellationRefundRules : [{ minutesBefore: '', refundPercentage: '100' }];
        }
        return { ...opt, cancellationPolicyType: policy, cancellationRefundRules: rules };
      }),
    }));
  };

  const onChangeCancellationRule = (optionIndex: number, ruleIndex: number, field: keyof CancellationRefundRuleForm, value: string) => {
    setValues((current) => ({
      ...current,
      pricingOptions: current.pricingOptions.map((opt, i) =>
        i === optionIndex ? { ...opt, cancellationRefundRules: opt.cancellationRefundRules.map((rule, ri) => (ri === ruleIndex ? { ...rule, [field]: value } : rule)) } : opt,
      ),
    }));
  };

  const onAddCancellationRule = (optionIndex: number) => {
    setValues((current) => ({
      ...current,
      pricingOptions: current.pricingOptions.map((opt, i) =>
        i === optionIndex ? { ...opt, cancellationRefundRules: [...opt.cancellationRefundRules, { minutesBefore: '', refundPercentage: '0' }] } : opt,
      ),
    }));
  };

  const onRemoveCancellationRule = (optionIndex: number, ruleIndex: number) => {
    setValues((current) => ({
      ...current,
      pricingOptions: current.pricingOptions.map((opt, i) =>
        i === optionIndex ? { ...opt, cancellationRefundRules: opt.cancellationRefundRules.filter((_, ri) => ri !== ruleIndex) } : opt,
      ),
    }));
  };

  const executeSaveProduct = useCallback(
    (vals: FormValues) => {
      if (!productReady || !product) return Promise.resolve();
      if (
        vals.pricingOptions.some(
          (pricingOption) => !hasValidWeeklyRequiredDays(pricingOption.cadence, pricingOption.requiredDaysPerWeek, pricingOption.availableDays, pricingOption.fulfillmentType),
        )
      ) {
        return Promise.reject(new Error('Required selected days per week must be a whole number between 1 and the available-day count.'));
      }

      const imageUrls = vals.imageUrlsCsv
        .split('\n')
        .map((u) => u.trim())
        .filter(Boolean)
        .map((url) => ({ original: { url, width: null, height: null }, thumbnail: null }));
      return new Promise<void>((resolve, reject) => {
        commitProduct({
          variables: {
            input: {
              id: product.id,
              type: 'EVENT',
              currency: vals.currency,
              fieldsToUpdate: ['LISTING_METADATA', 'PRICING_OPTIONS', 'CURRENCY', 'FEATURE_IMAGES'],
              featureImages: imageUrls,
              listingMetadata: { title: vals.listingTitle.trim(), subTitle: '', about: vals.listingAbout.trim(), includedFeatures: [] },
              tagIds: [],
              pricingOptions: vals.pricingOptions.map((opt, idx) => ({
                id: opt.id,
                index: idx,
                listingMetadata: { title: opt.title || vals.listingTitle.trim(), subTitle: '', about: vals.listingAbout.trim(), includedFeatures: [] },
                purchaseCadence: opt.cadence,
                price: Number(opt.price),
                isTaxInclusive: opt.isTaxInclusive,
                acceptedPaymentMethods: opt.acceptedPaymentMethods.length > 0 ? opt.acceptedPaymentMethods : ['CARD'],
                availableDays: opt.availableDays,
                requiredDaysPerWeek: opt.cadence !== 'DAILY' && opt.requiredDaysPerWeek.trim() ? Number(opt.requiredDaysPerWeek) : null,
                minDurationMinutes: opt.minDurationMinutes.trim() ? Number(opt.minDurationMinutes) : null,
                minDurationDisplayUnit: opt.minDurationDisplayUnit,
                maxDurationMinutes: opt.maxDurationMinutes.trim() ? Number(opt.maxDurationMinutes) : null,
                maxDurationDisplayUnit: opt.maxDurationDisplayUnit,
                maxAllowedResourcesLockTimePaidViaCard: opt.maxAllowedResourcesLockTimePaidViaCard.trim() ? Number(opt.maxAllowedResourcesLockTimePaidViaCard) : 15,
                maxAllowedResourcesLockTimePaidViaCardDisplayUnit: opt.maxAllowedResourcesLockTimePaidViaCardDisplayUnit,
                maxAllowedResourcesLockTimePaidViaBankTransfer: opt.maxAllowedResourcesLockTimePaidViaBankTransfer.trim()
                  ? Number(opt.maxAllowedResourcesLockTimePaidViaBankTransfer)
                  : 0,
                maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit: opt.maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit,
                numberOfResourcesToBook: 1,
                billingMode: opt.billingMode || 'UPFRONT',
                supportsSubscriptionAutoRenewal: opt.supportsSubscriptionAutoRenewal,
                cancellationPolicyType: opt.cancellationPolicyType,
                cancellationRefundRules: opt.cancellationRefundRules
                  .filter((r) => r.minutesBefore.trim() && r.refundPercentage.trim())
                  .map((r) => ({ minutesBefore: Number(r.minutesBefore), displayUnit: r.displayUnit, refundPercentage: Number(r.refundPercentage) })),
                fulfillmentType: opt.fulfillmentType,
                entitlementCreditQuantity: opt.fulfillmentType === 'ENTITLEMENT' && opt.entitlementCreditQuantity.trim() ? Number(opt.entitlementCreditQuantity) : null,
                entitlementValidityDays: opt.fulfillmentType === 'ENTITLEMENT' && opt.entitlementValidityDays.trim() ? Number(opt.entitlementValidityDays) : null,
              })),
            },
          },
          onCompleted: (_, gqlErrors) => (gqlErrors?.length ? reject(new Error(gqlErrors[0].message)) : resolve()),
          onError: reject,
        });
      });
    },
    [commitProduct, product, productReady],
  );

  const runAutoSave = useCallback(() => {
    const current = currentValuesRef.current;
    const last = lastSavedRef.current;
    if (!canModify) return;

    const productChangedFlag =
      productReady &&
      (current.listingTitle !== last.listingTitle ||
        current.listingAbout !== last.listingAbout ||
        current.currency !== last.currency ||
        current.imageUrlsCsv !== last.imageUrlsCsv ||
        JSON.stringify(current.pricingOptions) !== JSON.stringify(last.pricingOptions));

    if (!productChangedFlag) {
      setSaveStatus('idle');
      return;
    }

    Promise.all([executeSaveProduct(current)])
      .then(() => {
        lastSavedRef.current = current;
        setSaveStatus('saved');
      })
      .catch((err) => {
        setSaveStatus('error');
        toast.error(<NotificationContent content={err instanceof Error ? err.message : 'Auto-save failed.'} />);
      });
  }, [canModify, productReady, executeSaveProduct]);

  const debouncedSave = useDebounceCallback(runAutoSave, 800);

  useEffect(() => {
    if (isInitialMountRef.current) {
      isInitialMountRef.current = false;
      return;
    }
    if (hasInvalidWeeklyRequiredDays) {
      debouncedSave.cancel();
      return;
    }

    // The visible save state is updated only when a valid user edit schedules persistence.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setSaveStatus('saving');
    debouncedSave();
  }, [debouncedSave, hasInvalidWeeklyRequiredDays, values]);

  return (
    <Container maxWidth={embedded ? false : 'lg'} disableGutters={embedded}>
      <Stack spacing={3} sx={{ py: embedded ? 0 : 4 }}>
        {!embedded ? (
          <Button component={NextLink} href={backLink} sx={{ alignSelf: 'flex-start' }}>
            ← Back to location
          </Button>
        ) : null}

        {!embedded ? (
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1} sx={{ justifyContent: 'space-between', alignItems: { sm: 'center' } }}>
            <Box>
              <MediumHeadingIconTypography label="Pricing and booking" />
              <BodyIconTypography label="Manage rates, cadences, cancellation policies, and booking rules." />
            </Box>
            <Box>
              {displayedSaveStatus === 'saving' && <SmallIconTypography label="Saving…" />}
              {displayedSaveStatus === 'saved' && <SmallIconTypography label="All changes saved" />}
              {displayedSaveStatus === 'error' && <SmallIconTypography label="Fix the highlighted weekly day selection before saving." />}
            </Box>
          </Stack>
        ) : null}

        {!canModify ? <Alert severity="warning">You do not have permission to edit this listing.</Alert> : null}

        {productReady ? (
          <HostListingProductSettings
            values={values}
            onChange={onChange}
            onChangePricingOption={onChangePricingOption}
            onChangeFulfillmentType={onChangeFulfillmentType}
            onTogglePricingOption={onTogglePricingOption}
            onChangePaymentMethods={onChangePaymentMethods}
            onChangeAvailableDays={onChangeAvailableDays}
            onSetCancellationPolicy={onSetCancellationPolicy}
            onChangeCancellationRule={onChangeCancellationRule}
            onAddCancellationRule={onAddCancellationRule}
            onRemoveCancellationRule={onRemoveCancellationRule}
            onAddPricingOption={onAddPricingOption}
            onRemovePricingOption={onRemovePricingOption}
            weeklyRequiredDaysErrors={weeklyRequiredDaysErrors}
          />
        ) : (
          <BodyIconTypography label="Pricing and booking settings are still being set up. Return once setup is complete." />
        )}
      </Stack>
    </Container>
  );
};

const PricingPageWithShell = () => (
  <RootShell>
    <PricingPage />
  </RootShell>
);

export default memo(PricingPageWithShell);
