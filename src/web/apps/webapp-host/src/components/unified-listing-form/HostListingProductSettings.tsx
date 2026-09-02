import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import FormControl from '@mui/material/FormControl';
import Grid from '@mui/material/Grid';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import OutlinedInput from '@mui/material/OutlinedInput';
import type { SelectChangeEvent } from '@mui/material/Select';
import Select from '@mui/material/Select';
import TextField from '@mui/material/TextField';
import { DurationInput } from '@/components/product/duration-input';
import { BodyIconTypography, FormFieldLabel, LeadIconTypography, SettingsSectionCard, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';

export type CancellationRefundRuleForm = {
  minutesBefore: string;
  refundPercentage: string;
  displayUnit?: string | null;
};

export type PricingOptionForm = {
  id: string;
  title: string;
  price: string;
  cadence: string;
  billingMode: string;
  acceptedPaymentMethods: string[];
  availableDays: string[];
  requiredDaysPerWeek: string;
  cancellationPolicyType: string;
  cancellationRefundRules: CancellationRefundRuleForm[];
  minDurationMinutes: string;
  maxDurationMinutes: string;
  isTaxInclusive: boolean;
  supportsSubscriptionAutoRenewal: boolean;
  maxAllowedResourcesLockTimePaidViaCard: string;
  maxAllowedResourcesLockTimePaidViaBankTransfer: string;
  fulfillmentType: string;
  entitlementCreditQuantity: string;
  entitlementValidityDays: string;
  minDurationDisplayUnit?: string | null;
  maxDurationDisplayUnit?: string | null;
  maxAllowedResourcesLockTimePaidViaCardDisplayUnit?: string | null;
  maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit?: string | null;
};

export type HostListingProductSettingsValues = {
  listingTitle: string;
  listingAbout: string;
  currency: 'USD' | 'NZD';
  imageUrlsCsv: string;
  pricingOptions: PricingOptionForm[];
};

type Props = {
  values: HostListingProductSettingsValues;
  onChange: (field: keyof Omit<HostListingProductSettingsValues, 'pricingOptions'>) => (event: React.ChangeEvent<HTMLInputElement>) => void;
  onChangePricingOption: (index: number, field: keyof PricingOptionForm) => (event: React.ChangeEvent<HTMLInputElement>) => void;
  onTogglePricingOption: (index: number, field: 'isTaxInclusive' | 'supportsSubscriptionAutoRenewal', value: boolean) => void;
  onChangeFulfillmentType: (index: number, value: string) => void;
  onChangePaymentMethods: (optionIndex: number, methods: string[]) => void;
  onChangeAvailableDays: (optionIndex: number, availableDays: string[]) => void;
  onSetCancellationPolicy: (optionIndex: number, policy: string) => void;
  onChangeCancellationRule: (optionIndex: number, ruleIndex: number, field: keyof CancellationRefundRuleForm, value: string) => void;
  onAddCancellationRule: (optionIndex: number) => void;
  onRemoveCancellationRule: (optionIndex: number, ruleIndex: number) => void;
  onAddPricingOption: () => void;
  onRemovePricingOption: (index: number) => void;
  weeklyRequiredDaysErrors: Record<number, string | null>;
};

const cadenceOptions = [
  ['DAILY', 'Daily'],
  ['WEEKLY', 'Weekly'],
  ['FORTNIGHTLY', 'Fortnightly'],
  ['MONTHLY', 'Monthly'],
  ['TWO_MONTHS', 'Two months'],
  ['QUARTERLY', 'Quarterly'],
  ['FOUR_MONTHS', 'Four months'],
  ['FIVE_MONTHS', 'Five months'],
  ['SIX_MONTHS', 'Six months'],
  ['YEARLY', 'Yearly'],
] as const;

const billingModeOptions = [
  ['UPFRONT', 'Upfront'],
  ['IN_ARREARS', 'In arrears'],
] as const;

const paymentMethodLabel = (method: string) => (method === 'CARD' ? 'Card' : method === 'BANK_TRANSFER' ? 'Bank transfer' : method);

const cadenceDisplayLabel = (cadence: string) => cadenceOptions.find(([value]) => value === cadence)?.[1] ?? cadence;

const getCancellationDescription = (policy: string) => {
  switch (policy) {
    case 'NO_CANCELLATION':
      return 'Customers cannot receive a refund after purchase.';
    case 'FULL_REFUND_BEFORE_CUTOFF':
      return 'Customers receive a full refund if they cancel before the cutoff window.';
    case 'TIERED_REFUND':
      return 'Customers receive different refund percentages depending on how early they cancel.';
    default:
      return 'No cancellation policy selected.';
  }
};

const HostListingProductSettings = ({
  values,
  onChange,
  onChangePricingOption,
  onChangeFulfillmentType,
  onTogglePricingOption,
  onChangePaymentMethods,
  onChangeAvailableDays,
  onSetCancellationPolicy,
  onChangeCancellationRule,
  onAddCancellationRule,
  onRemoveCancellationRule,
  onAddPricingOption,
  onRemovePricingOption,
  weeklyRequiredDaysErrors,
}: Props) => {
  return (
    <StackColumn spacing={3}>
      {/* Product-level listing details */}
      <SettingsSectionCard title="Listing Details" description="The identity customers see in search results and the booking flow.">
        <StackColumn spacing={2}>
          <FormFieldLabel label="Listing title">
            <TextField fullWidth required value={values.listingTitle} onChange={onChange('listingTitle')} />
          </FormFieldLabel>
          <FormFieldLabel label="Description">
            <TextField fullWidth multiline minRows={3} value={values.listingAbout} onChange={onChange('listingAbout')} />
          </FormFieldLabel>
          <Grid container spacing={2}>
            <Grid size={{ xs: 12, md: 4 }}>
              <FormFieldLabel label="Currency">
                <TextField select fullWidth value={values.currency} onChange={onChange('currency')}>
                  <MenuItem value="USD">USD</MenuItem>
                  <MenuItem value="NZD">NZD</MenuItem>
                </TextField>
              </FormFieldLabel>
            </Grid>
          </Grid>
        </StackColumn>
      </SettingsSectionCard>

      {/* Per-option editors */}
      {values.pricingOptions.map((opt, index) => (
        <Box key={opt.id}>
          {index > 0 && <Divider sx={{ mb: 3 }} />}

          <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
            <LeadIconTypography label={opt.title ? `Option ${index + 1} — ${opt.title}` : `Pricing option ${index + 1} — ${cadenceDisplayLabel(opt.cadence)}`} />
            {values.pricingOptions.length > 1 && (
              <Button size="small" color="error" variant="outlined" onClick={() => onRemovePricingOption(index)}>
                Remove option
              </Button>
            )}
          </StackRow>

          <StackColumn spacing={2}>
            {/* Rate */}
            <SettingsSectionCard title="Rate" description="Set the customer-facing label, purchase term, and price for this option.">
              <StackColumn spacing={2}>
                <FormFieldLabel label="Customer-facing label (optional)">
                  <TextField fullWidth value={opt.title} onChange={onChangePricingOption(index, 'title')} placeholder="e.g. Daily rate, Weekend rate" />
                </FormFieldLabel>
                <Grid container spacing={2}>
                  <Grid size={{ xs: 12, md: 6 }}>
                    <FormFieldLabel label="Cadence">
                      <TextField select fullWidth required value={opt.cadence} onChange={onChangePricingOption(index, 'cadence')}>
                        {cadenceOptions.map(([value, label]) => (
                          <MenuItem key={value} value={value}>
                            {label}
                          </MenuItem>
                        ))}
                      </TextField>
                    </FormFieldLabel>
                  </Grid>
                  <Grid size={{ xs: 12, md: 6 }}>
                    <FormFieldLabel label="Price">
                      <TextField fullWidth required value={opt.price} onChange={onChangePricingOption(index, 'price')} />
                    </FormFieldLabel>
                  </Grid>
                </Grid>
              </StackColumn>
            </SettingsSectionCard>

            <SettingsSectionCard title="Credit entitlement" description="Offer prepaid credits that customers can use for eligible bookings.">
              <StackColumn spacing={2}>
                <FormFieldLabel label="Fulfillment type">
                  <TextField select fullWidth value={opt.fulfillmentType} onChange={(event) => onChangeFulfillmentType(index, event.target.value)}>
                    <MenuItem sx={{ minHeight: 48, fontSize: 'inherit' }} value="RESERVATION">
                      Reservation
                    </MenuItem>
                    <MenuItem sx={{ minHeight: 48, fontSize: 'inherit' }} value="ENTITLEMENT">
                      Credit entitlement
                    </MenuItem>
                  </TextField>
                </FormFieldLabel>
                {opt.fulfillmentType === 'ENTITLEMENT' ? (
                  <Grid container spacing={2}>
                    <Grid size={{ xs: 12, md: 4 }}>
                      <FormFieldLabel label="Credit quantity">
                        <TextField fullWidth value={opt.entitlementCreditQuantity} onChange={onChangePricingOption(index, 'entitlementCreditQuantity')} />
                      </FormFieldLabel>
                    </Grid>
                    <Grid size={{ xs: 12, md: 4 }}></Grid>
                    <Grid size={{ xs: 12, md: 4 }}>
                      <FormFieldLabel label="Validity (days)">
                        <TextField fullWidth value={opt.entitlementValidityDays} onChange={onChangePricingOption(index, 'entitlementValidityDays')} />
                      </FormFieldLabel>
                    </Grid>
                  </Grid>
                ) : null}
              </StackColumn>
            </SettingsSectionCard>

            {/* Booking Rules */}
            <SettingsSectionCard title="Booking Rules" description="Set the minimum and maximum booking duration for this rate.">
              <StackColumn spacing={1.5} sx={{ mb: 2 }}>
                <BodyIconTypography label="Available calendar days" />
                <SmallIconTypography label="Leave all days unselected to make this price available every day." />
                <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
                  {[
                    ['MONDAY', 'Mon'],
                    ['TUESDAY', 'Tue'],
                    ['WEDNESDAY', 'Wed'],
                    ['THURSDAY', 'Thu'],
                    ['FRIDAY', 'Fri'],
                    ['SATURDAY', 'Sat'],
                    ['SUNDAY', 'Sun'],
                  ].map(([day, label]) => (
                    <Button
                      key={day}
                      size="small"
                      variant={opt.availableDays.includes(day) ? 'contained' : 'outlined'}
                      onClick={() => onChangeAvailableDays(index, opt.availableDays.includes(day) ? opt.availableDays.filter((item) => item !== day) : [...opt.availableDays, day])}
                    >
                      {label}
                    </Button>
                  ))}
                </StackRow>
              </StackColumn>
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, md: 6 }}>
                  <DurationInput
                    label="Minimum booking duration"
                    value={opt.minDurationMinutes}
                    onChange={(value) => onChangePricingOption(index, 'minDurationMinutes')({ target: { value } } as React.ChangeEvent<HTMLInputElement>)}
                    unit={opt.minDurationDisplayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
                    onUnitChange={(unit) =>
                      onChangePricingOption(index, 'minDurationDisplayUnit')({ target: { value: unit.toUpperCase() } } as React.ChangeEvent<HTMLInputElement>)
                    }
                    required
                  />
                </Grid>
                <Grid size={{ xs: 12, md: 6 }}>
                  <DurationInput
                    label="Maximum booking duration"
                    value={opt.maxDurationMinutes}
                    onChange={(value) => onChangePricingOption(index, 'maxDurationMinutes')({ target: { value } } as React.ChangeEvent<HTMLInputElement>)}
                    unit={opt.maxDurationDisplayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
                    onUnitChange={(unit) =>
                      onChangePricingOption(index, 'maxDurationDisplayUnit')({ target: { value: unit.toUpperCase() } } as React.ChangeEvent<HTMLInputElement>)
                    }
                    required
                  />
                </Grid>
              </Grid>
              {opt.cadence !== 'DAILY' ? (
                <Grid container spacing={2} sx={{ mt: 0.5 }}>
                  <Grid size={{ xs: 12, md: 6 }}>
                    <FormFieldLabel label={opt.fulfillmentType === 'ENTITLEMENT' ? 'Maximum redemptions per week' : 'Required selected days per week'}>
                      <TextField
                        fullWidth
                        type="text"
                        value={opt.requiredDaysPerWeek}
                        slotProps={{ htmlInput: { inputMode: 'numeric', pattern: '[0-9]*', maxLength: 1 } }}
                        error={Boolean(weeklyRequiredDaysErrors[index])}
                        helperText={weeklyRequiredDaysErrors[index] ?? (opt.fulfillmentType === 'ENTITLEMENT' ? 'Leave empty for no weekly redemption limit; choose 1 to 7.' : `Leave empty for unrestricted weekly booking; choose 1 to ${opt.availableDays.length || 7} when required.`)}
                        onChange={onChangePricingOption(index, 'requiredDaysPerWeek')}
                      />
                    </FormFieldLabel>
                  </Grid>
                </Grid>
              ) : null}
            </SettingsSectionCard>

            {/* Payments */}
            <SettingsSectionCard title="Payments" description="Configure how customers pay for this pricing option. Host bookings are processed through Stripe Connect.">
              <StackColumn spacing={2}>
                <FormFieldLabel label="Accepted payment methods">
                  <FormControl fullWidth>
                    <InputLabel>Accepted payment methods</InputLabel>
                    <Select<string[]>
                      multiple
                      value={opt.acceptedPaymentMethods}
                      label="Accepted payment methods"
                      input={<OutlinedInput label="Accepted payment methods" />}
                      onChange={(e: SelectChangeEvent<string[]>) => onChangePaymentMethods(index, typeof e.target.value === 'string' ? [e.target.value] : e.target.value)}
                      renderValue={(selected) => (
                        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                          {selected.map((val) => (
                            <Chip key={val} label={paymentMethodLabel(val)} size="small" />
                          ))}
                        </Box>
                      )}
                    >
                      <MenuItem value="CARD">Card</MenuItem>
                      <MenuItem value="BANK_TRANSFER">Bank transfer</MenuItem>
                    </Select>
                  </FormControl>
                </FormFieldLabel>
                <Grid container spacing={2}>
                  <Grid size={{ xs: 12, md: 4 }}>
                    <FormFieldLabel label="Billing mode">
                      <TextField select fullWidth value={opt.billingMode} onChange={onChangePricingOption(index, 'billingMode')}>
                        {billingModeOptions.map(([value, label]) => (
                          <MenuItem key={value} value={value}>
                            {label}
                          </MenuItem>
                        ))}
                      </TextField>
                    </FormFieldLabel>
                  </Grid>
                  <Grid size={{ xs: 12, md: 4 }}>
                    <FormFieldLabel label="Tax">
                      <TextField
                        select
                        fullWidth
                        value={opt.isTaxInclusive ? 'inclusive' : 'exclusive'}
                        onChange={(e) => onTogglePricingOption(index, 'isTaxInclusive', e.target.value === 'inclusive')}
                      >
                        <MenuItem value="inclusive">Tax inclusive</MenuItem>
                        <MenuItem value="exclusive">Tax exclusive</MenuItem>
                      </TextField>
                    </FormFieldLabel>
                  </Grid>
                  <Grid size={{ xs: 12, md: 4 }}>
                    <FormFieldLabel label="Auto-renewal">
                      <TextField
                        select
                        fullWidth
                        value={opt.supportsSubscriptionAutoRenewal ? 'enabled' : 'disabled'}
                        onChange={(e) => onTogglePricingOption(index, 'supportsSubscriptionAutoRenewal', e.target.value === 'enabled')}
                      >
                        <MenuItem value="enabled">Enabled</MenuItem>
                        <MenuItem value="disabled">Disabled</MenuItem>
                      </TextField>
                    </FormFieldLabel>
                  </Grid>
                </Grid>
              </StackColumn>
            </SettingsSectionCard>

            {/* Cancellation */}
            <SettingsSectionCard title="Cancellation" description="Set the customer-facing refund policy. Choose a type, then configure the timing.">
              <StackColumn spacing={2}>
                <Card variant="outlined" sx={{ bgcolor: 'action.hover', borderRadius: 2 }}>
                  <CardContent>
                    <StackColumn spacing={0.5}>
                      <LeadIconTypography label="Policy Summary" />
                      <BodyIconTypography label={getCancellationDescription(opt.cancellationPolicyType)} />
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
                      variant={opt.cancellationPolicyType === policy.type ? 'contained' : 'outlined'}
                      size="small"
                      onClick={() => onSetCancellationPolicy(index, policy.type)}
                      sx={{ textTransform: 'none' }}
                    >
                      {policy.label}
                    </Button>
                  ))}
                </StackRow>

                {opt.cancellationPolicyType === 'FULL_REFUND_BEFORE_CUTOFF' && (
                  <Card variant="outlined" sx={{ borderRadius: 2 }}>
                    <CardContent>
                      <StackColumn spacing={1.5}>
                        <LeadIconTypography label="Full Refund Window" />
                        <SmallIconTypography label="Customers receive a full refund if they cancel at least this many minutes before the booking." />
                        <DurationInput
                          label="Full refund cutoff before booking"
                          value={opt.cancellationRefundRules[0]?.minutesBefore ?? ''}
                          onChange={(value) => onChangeCancellationRule(index, 0, 'minutesBefore', value)}
                          unit={opt.cancellationRefundRules[0]?.displayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
                          onUnitChange={(unit) => onChangeCancellationRule(index, 0, 'displayUnit', unit.toUpperCase())}
                          required
                        />
                      </StackColumn>
                    </CardContent>
                  </Card>
                )}

                {opt.cancellationPolicyType === 'TIERED_REFUND' && (
                  <StackColumn spacing={1.5}>
                    <SmallIconTypography label="Each rule: if the customer cancels at least this many minutes before, refund this percentage." />
                    {opt.cancellationRefundRules.map((rule, ruleIndex) => (
                      <Card key={ruleIndex} variant="outlined" sx={{ borderRadius: 2 }}>
                        <CardContent>
                          <StackColumn spacing={1.5}>
                            <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                              <BodyIconTypography label={`Refund Rule ${ruleIndex + 1}`} />
                              {opt.cancellationRefundRules.length > 1 && (
                                <Button color="error" size="small" onClick={() => onRemoveCancellationRule(index, ruleIndex)}>
                                  Remove
                                </Button>
                              )}
                            </StackRow>
                            <Grid container spacing={2}>
                              <Grid size={{ xs: 12, md: 6 }}>
                                <DurationInput
                                  label="Refund timing before booking"
                                  value={rule.minutesBefore}
                                  onChange={(value) => onChangeCancellationRule(index, ruleIndex, 'minutesBefore', value)}
                                  unit={rule.displayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
                                  onUnitChange={(unit) => onChangeCancellationRule(index, ruleIndex, 'displayUnit', unit.toUpperCase())}
                                  required
                                />
                              </Grid>
                              <Grid size={{ xs: 12, md: 6 }}>
                                <FormFieldLabel label="Refund %">
                                  <TextField
                                    fullWidth
                                    value={rule.refundPercentage}
                                    onChange={(e) => onChangeCancellationRule(index, ruleIndex, 'refundPercentage', e.target.value)}
                                    helperText="Refund percentage (0–100)."
                                  />
                                </FormFieldLabel>
                              </Grid>
                            </Grid>
                          </StackColumn>
                        </CardContent>
                      </Card>
                    ))}
                    <Button variant="outlined" size="small" onClick={() => onAddCancellationRule(index)} sx={{ alignSelf: 'flex-start' }}>
                      Add refund rule
                    </Button>
                  </StackColumn>
                )}
              </StackColumn>
            </SettingsSectionCard>

            {/* Advanced */}
            <SettingsSectionCard title="Advanced" description="Payment lock window settings control how long a booking slot is reserved while payment is being processed.">
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, md: 6 }}>
                  <DurationInput
                    label="Card payment lock window"
                    value={opt.maxAllowedResourcesLockTimePaidViaCard}
                    onChange={(value) => onChangePricingOption(index, 'maxAllowedResourcesLockTimePaidViaCard')({ target: { value } } as React.ChangeEvent<HTMLInputElement>)}
                    unit={opt.maxAllowedResourcesLockTimePaidViaCardDisplayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
                    onUnitChange={(unit) =>
                      onChangePricingOption(
                        index,
                        'maxAllowedResourcesLockTimePaidViaCardDisplayUnit',
                      )({ target: { value: unit.toUpperCase() } } as React.ChangeEvent<HTMLInputElement>)
                    }
                    required
                  />
                </Grid>
                <Grid size={{ xs: 12, md: 6 }}>
                  <DurationInput
                    label="Bank transfer lock window"
                    value={opt.maxAllowedResourcesLockTimePaidViaBankTransfer}
                    onChange={(value) =>
                      onChangePricingOption(index, 'maxAllowedResourcesLockTimePaidViaBankTransfer')({ target: { value } } as React.ChangeEvent<HTMLInputElement>)
                    }
                    unit={opt.maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit?.toLowerCase() as 'minutes' | 'hours' | undefined}
                    onUnitChange={(unit) =>
                      onChangePricingOption(
                        index,
                        'maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit',
                      )({ target: { value: unit.toUpperCase() } } as React.ChangeEvent<HTMLInputElement>)
                    }
                    required
                  />
                </Grid>
              </Grid>
            </SettingsSectionCard>
          </StackColumn>
        </Box>
      ))}

      <Button variant="outlined" onClick={onAddPricingOption} sx={{ alignSelf: 'flex-start' }}>
        + Add pricing option
      </Button>

      <Divider />

      <FormFieldLabel label="Image URLs (one per line)">
        <TextField fullWidth value={values.imageUrlsCsv} onChange={onChange('imageUrlsCsv')} multiline minRows={3} />
      </FormFieldLabel>
    </StackColumn>
  );
};

export default HostListingProductSettings;
