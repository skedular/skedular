import { PdfIcon } from '@/components/icons';
import Link from '@mui/material/Link';
import { BodyIconTypography, SmallIconTypography, StackColumn } from '@skedular/ui';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { memo } from 'react';

export type InvoiceLinkItem = {
  invoiceNumber?: string | null | undefined;
  invoiceUrl?: string | null | undefined;
  billingPeriodStartInclusive?: string | null | undefined;
  billingPeriodEndExclusive?: string | null | undefined;
};

type Props = {
  emptyLabel?: string;
  invoices: readonly InvoiceLinkItem[];
  legacyInvoiceUrl?: string | null;
  linkLabel?: string;
  size?: 'body' | 'small';
};

const InvoiceDownloadLinks = ({ emptyLabel, invoices, legacyInvoiceUrl, linkLabel = 'View invoice', size = 'small' }: Props) => {
  const validInvoices = invoices.filter((item) => !!item.invoiceUrl);

  if (validInvoices.length === 0) {
    if (!legacyInvoiceUrl) {
      return emptyLabel ? <SmallIconTypography label={emptyLabel} sx={{ opacity: 0.72 }} /> : null;
    }

    return (
      <Link component={NextLink} href={legacyInvoiceUrl} target="_blank" rel="noopener noreferrer" underline="none">
        {size === 'body' ? <BodyIconTypography label={linkLabel} startElement={<PdfIcon />} /> : <SmallIconTypography label={linkLabel} startElement={<PdfIcon />} />}
      </Link>
    );
  }

  return (
    <StackColumn spacing={0.75}>
      {validInvoices.map((invoice) => {
        const label = invoice.invoiceNumber || buildPeriodLabel(invoice.billingPeriodStartInclusive, invoice.billingPeriodEndExclusive) || linkLabel;

        return (
          <Link
            key={`${invoice.invoiceUrl}-${invoice.invoiceNumber ?? ''}-${invoice.billingPeriodStartInclusive ?? ''}`}
            component={NextLink}
            href={invoice.invoiceUrl!}
            target="_blank"
            rel="noopener noreferrer"
            underline="none"
          >
            {size === 'body' ? <BodyIconTypography label={label} startElement={<PdfIcon />} /> : <SmallIconTypography label={label} startElement={<PdfIcon />} />}
          </Link>
        );
      })}
    </StackColumn>
  );
};

const buildPeriodLabel = (start?: string | null, endExclusive?: string | null) => {
  if (!start || !endExclusive) {
    return null;
  }

  return `Invoice ${dayjs.utc(start).format('DD MMM YYYY')} - ${dayjs.utc(endExclusive).subtract(1, 'day').format('DD MMM YYYY')}`;
};

export default memo(InvoiceDownloadLinks);
