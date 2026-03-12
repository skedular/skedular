import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackRow, SubtitleIconTypography } from '@/components/commons';
import { AddressIcon, PreferredIcon } from '@/components/icons';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import FormControlLabel from '@mui/material/FormControlLabel';
import Radio from '@mui/material/Radio';
import RadioGroup from '@mui/material/RadioGroup';
import Box from '@mui/system/Box';
import { memo, useMemo, useState } from 'react';
import type { MarketplaceProductDetail } from './types';

type Props = {
  product: MarketplaceProductDetail;
  onContinue?: (options: { locationId: string; resourceId: string; pricingPlanId: string }) => void;
};

const MarketplaceProductDetailBookingCard = ({ product, onContinue }: Props) => {
  const [selectedLocationId, setSelectedLocationId] = useState('');
  const [selectedResourceId, setSelectedResourceId] = useState('');
  const [selectedPricingPlanId, setSelectedPricingPlanId] = useState('');

  const selectedLocation = useMemo(() => product.locations.find((location) => location.id === selectedLocationId) ?? null, [product.locations, selectedLocationId]);
  const canContinue = selectedLocationId !== '' && selectedResourceId !== '' && selectedPricingPlanId !== '';

  return (
    <Box sx={{ position: { md: 'sticky' }, top: { md: 90 } }}>
      <Card sx={{ borderRadius: 3, border: 1, borderColor: (theme) => theme.palette.divider }}>
        <CardContent sx={{ p: { xs: 2.5, md: 3 }, '&:last-child': { pb: { xs: 2.5, md: 3 } } }}>
          <CaptionIconTypography label={product.typeLabel} sx={{ letterSpacing: '0.04em', textTransform: 'uppercase', opacity: 0.7, mb: 0.75 }} />
          <LeadIconTypography label={product.title} sx={{ mb: 0.5 }} />
          <BodyIconTypography label={product.shortDescription} sx={{ opacity: 0.85, mb: 2.5 }} />

          <LeadIconTypography label="1. Select location" sx={{ mb: 1.25 }} />
          <RadioGroup
            value={selectedLocationId}
            onChange={(event) => {
              setSelectedLocationId(event.target.value);
              setSelectedResourceId('');
            }}
          >
            {product.locations.map((location) => (
              <Box
                key={location.id}
                sx={{
                  border: 1,
                  borderColor: location.id === selectedLocationId ? (theme) => theme.palette.primary.main : (theme) => theme.palette.divider,
                  borderRadius: 2,
                  mb: 1,
                  bgcolor: location.id === selectedLocationId ? (theme) => theme.palette.action.selected : (theme) => theme.palette.background.paper,
                }}
              >
                <FormControlLabel
                  value={location.id}
                  control={<Radio size="small" />}
                  sx={{ m: 0, px: 1.25, py: 0.7, width: '100%', alignItems: 'flex-start' }}
                  label={
                    <Box sx={{ minWidth: 0, py: 0.45 }}>
                      <BodyIconTypography label={location.name} fontWeight={600} />
                      <CaptionIconTypography label={location.address} startElement={<AddressIcon sx={{ fontSize: 14, opacity: 0.75 }} />} sx={{ opacity: 0.75 }} />
                      <CaptionIconTypography label={location.availableLabel} sx={{ color: (theme) => theme.palette.success.main, mt: 0.35 }} />
                    </Box>
                  }
                />
              </Box>
            ))}
          </RadioGroup>

          {selectedLocation && (
            <>
              <LeadIconTypography label="2. Choose resource" sx={{ mt: 2.2, mb: 1.25 }} />
              <RadioGroup value={selectedResourceId} onChange={(event) => setSelectedResourceId(event.target.value)}>
                {selectedLocation.resources.map((resource) => (
                  <Box
                    key={resource.id}
                    sx={{
                      border: 1,
                      borderColor: resource.id === selectedResourceId ? (theme) => theme.palette.primary.main : (theme) => theme.palette.divider,
                      borderRadius: 2,
                      mb: 1,
                      bgcolor: resource.id === selectedResourceId ? (theme) => theme.palette.action.selected : (theme) => theme.palette.background.paper,
                    }}
                  >
                    <FormControlLabel
                      value={resource.id}
                      control={<Radio size="small" />}
                      sx={{ m: 0, px: 1.25, py: 0.7, width: '100%', alignItems: 'flex-start' }}
                      label={
                        <Box sx={{ py: 0.45 }}>
                          <BodyIconTypography label={resource.name} fontWeight={600} />
                          <StackRow spacing={0.75} sx={{ mt: 0.7 }}>
                            {resource.details.map((detail) => (
                              <CaptionIconTypography
                                key={detail}
                                label={detail}
                                sx={{
                                  px: 0.9,
                                  py: 0.35,
                                  borderRadius: 1,
                                  bgcolor: (theme) => theme.palette.action.hover,
                                }}
                              />
                            ))}
                          </StackRow>
                        </Box>
                      }
                    />
                  </Box>
                ))}
              </RadioGroup>
            </>
          )}

          {selectedResourceId && (
            <>
              <LeadIconTypography label="3. Select plan" sx={{ mt: 2.2, mb: 1.25 }} />
              <RadioGroup value={selectedPricingPlanId} onChange={(event) => setSelectedPricingPlanId(event.target.value)}>
                {product.pricingPlans.map((pricingPlan) => (
                  <Box
                    key={pricingPlan.id}
                    sx={{
                      border: 1,
                      borderColor: pricingPlan.id === selectedPricingPlanId ? (theme) => theme.palette.primary.main : (theme) => theme.palette.divider,
                      borderRadius: 2,
                      mb: 1,
                      px: 1.25,
                      py: 0.95,
                      bgcolor: pricingPlan.id === selectedPricingPlanId ? (theme) => theme.palette.action.selected : (theme) => theme.palette.background.paper,
                    }}
                  >
                    {pricingPlan.highlighted && (
                      <CaptionIconTypography
                        label="Best Value"
                        startElement={<PreferredIcon sx={{ fontSize: 13 }} />}
                        sx={{
                          color: (theme) => theme.palette.success.main,
                          textTransform: 'uppercase',
                          letterSpacing: '0.03em',
                          mb: 0.35,
                        }}
                      />
                    )}
                    <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', flexWrap: 'nowrap' }}>
                      <FormControlLabel
                        value={pricingPlan.id}
                        control={<Radio size="small" />}
                        sx={{ m: 0, mr: 1, minWidth: 0 }}
                        label={
                          <Box sx={{ minWidth: 0 }}>
                            <CaptionIconTypography label={pricingPlan.cadenceLabel} fontWeight={600} />
                            <CaptionIconTypography label={pricingPlan.name} sx={{ opacity: 0.75 }} />
                          </Box>
                        }
                      />
                      <Box sx={{ textAlign: 'right', flexShrink: 0 }}>
                        <SubtitleIconTypography label={pricingPlan.amountLabel} fontWeight={600} sx={{ lineHeight: 1.2 }} />
                        <CaptionIconTypography label={pricingPlan.note} sx={{ opacity: 0.7 }} />
                      </Box>
                    </StackRow>
                  </Box>
                ))}
              </RadioGroup>
            </>
          )}

          <Button
            fullWidth
            variant="contained"
            disabled={!canContinue}
            onClick={() =>
              canContinue &&
              onContinue?.({
                locationId: selectedLocationId,
                resourceId: selectedResourceId,
                pricingPlanId: selectedPricingPlanId,
              })
            }
            sx={{ mt: 2, textTransform: 'none' }}
          >
            {canContinue ? 'Continue to checkout' : 'Select options to continue'}
          </Button>
        </CardContent>
      </Card>
    </Box>
  );
};

export default memo(MarketplaceProductDetailBookingCard);
