'use client';

import BodyIconTypography from '../typography/body-icon-typography';
import LeadIconTypography from '../typography/lead-icon-typography';
import SmallIconTypography from '../typography/small-icon-typography';
import PushToRight from './push-to-right';
import StackColumn from '../stack-column';
import StackRow from '../stack-row';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { memo } from 'react';
import type { PaymentType } from 'react-svg-credit-card-payment-icons';
import { PaymentIcon } from 'react-svg-credit-card-payment-icons';

type Props = {
  lastFourDigits?: string | null | undefined;
  cardHolderName?: string | null | undefined;
  expiryDate?: string | null | undefined;
  cardBrand?: string | null | undefined;
};

const CreditCard = ({ lastFourDigits, cardHolderName, expiryDate, cardBrand }: Props) => (
  <Card
    sx={{
      width: 310,
      height: 180,
      borderRadius: 2,
      background: 'linear-gradient(135deg, #4e54c8, #8f94fb)',
      display: 'flex',
      padding: 1,
    }}
  >
    <CardContent>
      <StackColumn>
        <StackRow>
          <PaymentIcon type={cardBrand as PaymentType} format="flatRounded" width={50} />
          <PushToRight />
          <BodyIconTypography label="CREDIT CARD" />
        </StackRow>

        <LeadIconTypography label={`•••• •••• •••• ${lastFourDigits}`} sx={{ letterSpacing: 5 }} />

        <StackRow>
          <StackColumn>
            <BodyIconTypography label="Card Holder" />
            <SmallIconTypography label={cardHolderName || 'NAME'} />
          </StackColumn>

          <PushToRight />

          <StackColumn>
            <BodyIconTypography label="Expires" />
            <SmallIconTypography label={expiryDate || 'MM/YY'} />
          </StackColumn>
        </StackRow>
      </StackColumn>
    </CardContent>
  </Card>
);

export default memo(CreditCard);
