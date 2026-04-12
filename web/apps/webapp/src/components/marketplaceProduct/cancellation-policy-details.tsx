import { BodyIconTypography, CaptionIconTypography, StackColumn } from '@/components/commons';
import Box from '@mui/material/Box';
import { memo, useMemo } from 'react';

type CancellationRefundRule = {
  minutesBefore: number;
  refundPercentage: number;
};

type Props = {
  cancellationPolicyType: string | null | undefined;
  cancellationRefundRules: ReadonlyArray<CancellationRefundRule> | null | undefined;
  compact?: boolean;
  eventLabel?: string;
  title?: string;
};

const formatMinutesBeforeLabel = (minutesBefore: number) => {
  if (minutesBefore % 1440 === 0) {
    const days = minutesBefore / 1440;
    return `${days} day${days === 1 ? '' : 's'}`;
  }

  if (minutesBefore % 60 === 0) {
    const hours = minutesBefore / 60;
    return `${hours} hour${hours === 1 ? '' : 's'}`;
  }

  return `${minutesBefore} minute${minutesBefore === 1 ? '' : 's'}`;
};

const getCancellationPolicyLines = (
  cancellationPolicyType: string | null | undefined,
  cancellationRefundRules: ReadonlyArray<CancellationRefundRule> | null | undefined,
  eventLabel: string,
) => {
  const rules = [...(cancellationRefundRules ?? [])].sort((left, right) => right.minutesBefore - left.minutesBefore);

  switch (cancellationPolicyType) {
    case 'NO_CANCELLATION':
      return ['This purchase cannot be cancelled after checkout.'];
    case 'FULL_REFUND_BEFORE_CUTOFF': {
      const rule = rules[0];
      return rule
        ? [
            `100% refund until ${formatMinutesBeforeLabel(rule.minutesBefore)} before ${eventLabel}.`,
            `After that, there is no refund for this ${eventLabel.includes('renewal') ? 'plan' : 'booking'}.`,
          ]
        : ['You will see the cancellation details before checkout.'];
    }
    case 'TIERED_REFUND':
      return rules.length > 0
        ? [
            ...rules.map((rule) => `${rule.refundPercentage}% refund until ${formatMinutesBeforeLabel(rule.minutesBefore)} before ${eventLabel}.`),
            'After the final cutoff, refunds are no longer available.',
          ]
        : ['You will see the cancellation details before checkout.'];
    default:
      return ['You will see the cancellation details before checkout.'];
  }
};

const MarketplaceCancellationPolicyDetails = ({
  cancellationPolicyType,
  cancellationRefundRules,
  compact = false,
  eventLabel = 'the booking starts',
  title = 'Cancellation policy',
}: Props) => {
  const lines = useMemo(
    () => getCancellationPolicyLines(cancellationPolicyType, cancellationRefundRules, eventLabel),
    [cancellationPolicyType, cancellationRefundRules, eventLabel],
  );

  return (
    <Box
      sx={{
        p: compact ? 1.35 : 2,
        borderRadius: compact ? 2 : 3,
        bgcolor: (theme) => theme.palette.action.hover,
        border: 1,
        borderColor: (theme) => theme.palette.divider,
      }}
    >
      <StackColumn spacing={compact ? 0.6 : 0.9}>
        <CaptionIconTypography label={title} sx={{ letterSpacing: '0.04em', textTransform: 'uppercase', opacity: 0.72 }} />
        {lines.map((line) => (
          <BodyIconTypography key={line} label={line} sx={{ opacity: 0.88, fontSize: compact ? '0.875rem' : undefined }} />
        ))}
      </StackColumn>
    </Box>
  );
};

export default memo(MarketplaceCancellationPolicyDetails);
