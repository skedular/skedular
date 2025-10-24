import { LeadIconTypography, SmallIconTypography, StackRow } from '@/components/commons';
import { AreaIcon, CloseIcon, PersonIcon } from '@/components/icons';
import { getMarketplaceLocationLink } from '@/components/links';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import type { marketplaceLocationPopupCard_LocationDetails$key } from '@/queries/__generated__/marketplaceLocationPopupCard_LocationDetails.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import CardMedia from '@mui/material/CardMedia';
import IconButton from '@mui/material/IconButton';
import NextLink from 'next/link';
import { memo, useContext, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  locationDetailsRelay: marketplaceLocationPopupCard_LocationDetails$key;
  onReloadRequired: () => void;
  onClose?: () => void;
};

const MarketplaceLocationPopupCard = ({ locationDetailsRelay, onClose }: Props) => {
  const locationDetails = useFragment(
    graphql`
      fragment marketplaceLocationPopupCard_LocationDetails on LocationDetails {
        id
        name
        extraMetadata {
          areaRange {
            fromInSqm
            toInSqm
          }
          peopleCapacity {
            from
            to
          }
        }
        physicalAddress {
          multilinesFormattedAddress
        }
        primaryFeatureImage {
          thumbnail {
            url
            height
            width
          }
        }
      }
    `,
    locationDetailsRelay,
  );

  const { integratedPlatrform } = useIntegratedPlatrform();
  const paletteMode = useContext(PaletteModeContext);
  const capacity = useMemo(() => {
    if (!locationDetails.extraMetadata?.peopleCapacity) {
      return '';
    }

    if (locationDetails.extraMetadata?.peopleCapacity.from === locationDetails.extraMetadata?.peopleCapacity.to) {
      return `${locationDetails.extraMetadata?.peopleCapacity.from} People`;
    } else {
      return `${locationDetails.extraMetadata?.peopleCapacity.from} - ${locationDetails.extraMetadata?.peopleCapacity.to} People`;
    }
  }, [locationDetails.extraMetadata?.peopleCapacity]);

  const areaSize = useMemo(() => {
    if (!locationDetails.extraMetadata?.areaRange) {
      return '';
    }

    if (locationDetails.extraMetadata?.areaRange.fromInSqm === locationDetails.extraMetadata?.areaRange.toInSqm) {
      return `${locationDetails.extraMetadata?.areaRange.fromInSqm} m2`;
    } else {
      return `${locationDetails.extraMetadata?.areaRange.fromInSqm} - ${locationDetails.extraMetadata?.areaRange.toInSqm} m2`;
    }
  }, [locationDetails.extraMetadata?.areaRange]);

  return (
    <Card
      sx={{ width: { xs: '80%', sm: 300 }, textDecoration: 'none', display: 'block' }}
      component={NextLink}
      href={getMarketplaceLocationLink(integratedPlatrform, locationDetails.id)}
    >
      {locationDetails.primaryFeatureImage && locationDetails.primaryFeatureImage.thumbnail && (
        <CardMedia component="img" image={locationDetails.primaryFeatureImage.thumbnail.url} />
      )}
      <CardHeader
        title={
          <StackRow>
            {capacity && <SmallIconTypography label={capacity} startElement={<PersonIcon fontSize="small" />} invertDefaultColor />}
            {areaSize && <SmallIconTypography label={areaSize} startElement={<AreaIcon fontSize="small" />} invertDefaultColor />}
          </StackRow>
        }
        action={
          onClose ? (
            <IconButton
              onClick={(event) => {
                event.preventDefault();
                event.stopPropagation();
                onClose?.();
              }}
              sx={(theme) => ({
                color: paletteMode === 'dark' ? coal : sandstone,
                borderRadius: '50%',
                border: `1px solid ${theme.palette.divider}`,
                padding: theme.spacing(0.1),
              })}
            >
              <CloseIcon fontSize="medium" />
            </IconButton>
          ) : null
        }
      />
      <CardContent>
        <LeadIconTypography label={locationDetails.name} />
        {locationDetails.physicalAddress?.multilinesFormattedAddress && <SmallIconTypography label={locationDetails.physicalAddress?.multilinesFormattedAddress} />}
      </CardContent>
    </Card>
  );
};

export default memo(MarketplaceLocationPopupCard);
