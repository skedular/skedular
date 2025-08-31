import { LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { LocationIcon } from '@/components/icons';
import type { marketplaceLocationCard_LocationDetails$key } from '@/queries/__generated__/marketplaceLocationCard_LocationDetails.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import CardMedia from '@mui/material/CardMedia';
import { memo } from 'react';
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

  return (
    <Card sx={{ width: { xs: '100%', sm: 300 } }}>
      {locationDetails.primaryFeatureImage && locationDetails.primaryFeatureImage.thumbnail && (
        <CardMedia component="img" image={locationDetails.primaryFeatureImage.thumbnail.url} />
      )}
      <CardHeader
        title={
          <StackRow>
            <LeadIconTypography label={locationDetails.name} startElement={<LocationIcon />} sx={{ flexWrap: undefined }} invertDefaultColor />
            <PushToRight />
          </StackRow>
        }
      />
      <CardContent>
        <SmallIconTypography
          label={locationDetails.physicalAddress?.multilinesFormattedAddress ? locationDetails.physicalAddress?.multilinesFormattedAddress : 'N/A'}
          sx={{ whiteSpace: 'pre-line' }}
        />
      </CardContent>
    </Card>
  );
};

export default memo(MarketplaceLocationCard);
