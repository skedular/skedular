const getRefundCurrencyPrefix = (currencyType?: string | null | undefined, currencyToDisplay?: string | null | undefined) => {
  switch (currencyType) {
    case 'NZD':
      return 'NZ$';
    case 'USD':
      return 'US$';
    default:
      return currencyToDisplay && currencyToDisplay !== 'N/A' && !currencyToDisplay.includes(' - ') ? currencyToDisplay : null;
  }
};

export const hasDisplayCurrency = (currencyType?: string | null | undefined, currencyToDisplay?: string | null | undefined) =>
  !!getRefundCurrencyPrefix(currencyType, currencyToDisplay);

export const formatRefundAmount = (refundAmount?: number | null | undefined, currencyType?: string | null | undefined, currencyToDisplay?: string | null | undefined) => {
  if (refundAmount == null) {
    return null;
  }

  const prefix = getRefundCurrencyPrefix(currencyType, currencyToDisplay);
  const formattedAmount = new Intl.NumberFormat('en-NZ', { maximumFractionDigits: 2, minimumFractionDigits: 0 }).format(refundAmount);

  return prefix ? `${prefix}${formattedAmount}` : formattedAmount;
};
