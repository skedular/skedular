import { LeadIconTypography, SmallIconTypography, StackRow } from '@/components/commons';
import { AreaIcon, CloseIcon, PersonIcon } from '@/components/icons';
import { getMarketplaceLocationLink } from '@/components/links';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import type { marketplaceLocationCard_LocationDetails$key } from '@/queries/__generated__/marketplaceLocationCard_LocationDetails.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import CardMedia from '@mui/material/CardMedia';
import IconButton from '@mui/material/IconButton';
import NextLink from 'next/link';
import { memo, useContext, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  locationDetailsRelay: marketplaceLocationCard_LocationDetails$key;
  onReloadRequired: () => void;
  onClose?: () => void;
};

const MarketplaceLocationCard = ({ locationDetailsRelay, onClose }: Props) => {
  const locationDetails = useFragment(
    graphql`
      fragment marketplaceLocationCard_LocationDetails on LocationDetails {
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
        featureImages {
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
      sx={{ width: { xs: '100%', sm: 300 }, textDecoration: 'none', display: 'block' }}
      component={NextLink}
      href={getMarketplaceLocationLink(integratedPlatrform, locationDetails.id)}
    >
      {locationDetails.featureImages && locationDetails.featureImages.length > 0 && locationDetails.featureImages[0].thumbnail && (
        <CardMedia component="img" image={locationDetails.featureImages[0].thumbnail.url} />
      )}
      <CardHeader
        sx={{ height: 60 }}
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
              sx={{ color: paletteMode === 'dark' ? coal : sandstone, borderRadius: '50%' }}
            >
              <CloseIcon fontSize="medium" />
            </IconButton>
          ) : null
        }
      />
      <CardContent sx={{ height: onClose ? null : 200 }}>
        <LeadIconTypography label={locationDetails.name} />
        {locationDetails.physicalAddress?.multilinesFormattedAddress && <SmallIconTypography label={locationDetails.physicalAddress?.multilinesFormattedAddress} />}
      </CardContent>
    </Card>
  );
};

export default memo(MarketplaceLocationCard);
