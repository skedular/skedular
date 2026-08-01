'use client';

import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Stack from '@mui/material/Stack';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography } from '@skedular/ui';

export type ModificationRefundPreviewProps = {
  /** Amount before the modification (original booking price). */
  originalAmount: number;
  /** Amount after the modification (new booking price). */
  newAmount: number;
  /** ISO 4217 currency code, e.g. "AUD". */
  currency: string;
  /** Human-readable label for the refund provider, e.g. "Stripe", "Xero", or "Bank transfer". */
  provider: string;
};

/**
 * Shows a compact refund preview when a booking modification reduces the total price.
 *
 * Intended use:
 *   Render this inside the modification confirmation dialog before the user confirms.
 *   The delta amount and provider label are calculated and resolved by the parent, which
 *   should call `marketplaceBookingModificationRefundPreview` (or derive them from the
 *   booking payload) before rendering this component.
 *
 * Integration note (T076):
 *   This component is ready to be wired into `edit-marketplace-booking.tsx` once the
 *   `updateMarketplaceBooking` mutation is extended with price-affecting fields
 *   (from, until, quantity, productPricingId, resourceIds). When those fields are added,
 *   render `<ModificationRefundPreview>` in the submission confirmation step whenever
 *   `originalAmount > newAmount`.
 */
export function ModificationRefundPreview({ originalAmount, newAmount, currency, provider }: ModificationRefundPreviewProps) {
  const delta = originalAmount - newAmount;

  if (delta <= 0) {
    return null;
  }

  const formatted = (amount: number) => `${amount.toFixed(2)} ${currency}`;

  return (
    <Card variant="outlined">
      <CardContent>
        <Stack spacing={1.5}>
          <LeadIconTypography label="Refund preview" />
          <BodyIconTypography label={`You will receive a refund of ${formatted(delta)}`} />
          <CaptionIconTypography label={`Original amount: ${formatted(originalAmount)}`} />
          <CaptionIconTypography label={`New amount: ${formatted(newAmount)}`} />
          <CaptionIconTypography label={`Processed via: ${provider}`} />
        </Stack>
      </CardContent>
    </Card>
  );
}
