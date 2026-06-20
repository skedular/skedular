import { ErrorIcon } from '@/components/icons';
import Alert from '@mui/material/Alert';
import AlertTitle from '@mui/material/AlertTitle';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import { BodyIconTypography } from '@skedular/ui';
import { memo } from 'react';

type UpgradePlan = {
  planCode: number;
  name: string;
  availability: string;
  priceDescription: string | null | undefined;
};

type Props = {
  currentUsage: number;
  quotaLimit: number;
  upgradePlans: readonly UpgradePlan[];
  onUpgradeClick?: (planCode: number) => void;
};

const SpacesQuotaUpgradePrompt = memo(({ currentUsage, quotaLimit, upgradePlans, onUpgradeClick }: Props) => {
  if (upgradePlans.length === 0) {
    return (
      <Alert severity="warning" icon={<ErrorIcon />}>
        <AlertTitle>Booking quota exceeded</AlertTitle>
        <BodyIconTypography label={`Your organization has used ${currentUsage} of ${quotaLimit} booking instances this period. Please contact sales for options.`} />
      </Alert>
    );
  }

  return (
    <Alert severity="warning" icon={<ErrorIcon />}>
      <AlertTitle>Booking quota exceeded</AlertTitle>
      <BodyIconTypography label={`Your organization has used ${currentUsage} of ${quotaLimit} booking instances this period. Upgrade to continue booking:`} sx={{ mb: 1 }} />
      <Stack direction="row" spacing={1}>
        {upgradePlans.map((plan) => (
          <Box key={plan.planCode}>
            {plan.availability === 'SelfService' ? (
              <Button variant="contained" size="small" onClick={() => onUpgradeClick?.(plan.planCode)}>
                {plan.name}
                {plan.priceDescription ? ` - ${plan.priceDescription}` : ''}
              </Button>
            ) : (
              <Button variant="outlined" size="small" disabled>
                {plan.name} - Contact Sales
              </Button>
            )}
          </Box>
        ))}
      </Stack>
    </Alert>
  );
});

SpacesQuotaUpgradePrompt.displayName = 'SpacesQuotaUpgradePrompt';

export { SpacesQuotaUpgradePrompt };
