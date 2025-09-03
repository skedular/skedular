import { LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { LocationIcon } from '@/components/icons';
import type { marketplaceLocationCard_LocationDetails$key } from '@/queries/__generated__/marketplaceLocationCard_LocationDetails.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import CardMedia from '@mui/material/CardMedia';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  locationDetailsRelay: marketplaceLocationCard_LocationDetails$key;
  onReloadRequired: () => void;
};

const MarketplaceLocationCard = ({ locationDetailsRelay }: Props) => {
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
          suburb
          city
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

  const shortAddress = useMemo(() => {
    if (locationDetails.physicalAddress?.suburb && locationDetails.physicalAddress.city) {
      return `${locationDetails.physicalAddress.suburb}, ${locationDetails.physicalAddress.city}`;
    } else if (locationDetails.physicalAddress?.suburb) {
      return locationDetails.physicalAddress.suburb;
    } else if (locationDetails.physicalAddress?.city) {
      return locationDetails.physicalAddress.city;
    } else {
      return '';
    }
  }, [locationDetails.physicalAddress?.suburb, locationDetails.physicalAddress?.city]);

  return (
    <Card sx={{ width: { xs: '100%', sm: 300 } }}>
      {locationDetails.primaryFeatureImage && locationDetails.primaryFeatureImage.thumbnail && (
        <CardMedia component="img" image={locationDetails.primaryFeatureImage.thumbnail.url} />
      )}
      <CardHeader
        title={
          <StackRow>
            <LeadIconTypography label={locationDetails.name} startElement={<LocationIcon excludeTooltip />} sx={{ flexWrap: undefined }} invertDefaultColor />
            <PushToRight />
          </StackRow>
        }
      />
      <CardContent>
        {areaSize && <SmallIconTypography label={areaSize} />}
        {shortAddress && <SmallIconTypography label={shortAddress} />}
      </CardContent>
    </Card>
  );
};

export default memo(MarketplaceLocationCard);
