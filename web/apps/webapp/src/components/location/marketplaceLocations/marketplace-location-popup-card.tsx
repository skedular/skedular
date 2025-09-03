import { LeadIconTypography, SmallIconTypography, StackRow } from '@/components/commons';
import { AreaIcon, PersonIcon } from '@/components/icons';
import type { marketplaceLocationPopupCard_LocationDetails$key } from '@/queries/__generated__/marketplaceLocationPopupCard_LocationDetails.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import CardMedia from '@mui/material/CardMedia';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  locationDetailsRelay: marketplaceLocationPopupCard_LocationDetails$key;
  onReloadRequired: () => void;
};

const MarketplaceLocationPopupCard = ({ locationDetailsRelay }: Props) => {
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
    <Card sx={{ width: { xs: '100%', sm: 300 } }}>
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
      />
      <CardContent>
        <LeadIconTypography label={locationDetails.name} />
        {locationDetails.physicalAddress?.multilinesFormattedAddress && <SmallIconTypography label={locationDetails.physicalAddress?.multilinesFormattedAddress} />}
      </CardContent>
    </Card>
  );
};

export default memo(MarketplaceLocationPopupCard);
