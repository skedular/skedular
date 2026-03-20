import { NewBookingButton } from '@/components/booking/addBooking';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { DeskIcon, OtherResourceIcon, ParkingIcon, RoomIcon } from '@/components/icons';
import { getOrganizationLocationResourceBaseLink } from '@/components/links';
import { Zones } from '@/components/zone';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import type { resourceCard_ResourceDetails$key } from '@/queries/__generated__/resourceCard_ResourceDetails.graphql';
import type { resourceCard_query$key } from '@/queries/__generated__/resourceCard_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import Link from '@mui/material/Link';
import { Dayjs } from 'dayjs';
import NextLink from 'next/link';
import { memo, useContext } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: resourceCard_query$key;
  resourceDetailsRelay: resourceCard_ResourceDetails$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  locationId: string;
  date: Dayjs;
};

const ResourceCard = ({ rootDataRelay, resourceDetailsRelay, onReloadRequired, organizationCustomDomain, locationId, date }: Props) => {
  const rootData = useFragment<resourceCard_query$key>(
    graphql`
      fragment resourceCard_query on Query {
        deskResourceType
        roomResourceType
        parkingResourceType
      }
    `,
    rootDataRelay,
  );

  const resourceDetails = useFragment(
    graphql`
      fragment resourceCard_ResourceDetails on ResourceDetails {
        id
        name
        inactive
        color
        capacity
        customTags {
          id
          name
          color
        }
        zones {
          id
          name
          color
        }
        productTags {
          id
          name
          color
        }
        resourceType {
          id
          name
          color
          type
        }
      }
    `,
    resourceDetailsRelay,
  );

  const { integratedPlatrform } = useIntegratedPlatrform();
  const paletteMode = useContext(PaletteModeContext);

  return (
    <Card sx={{ width: { xs: '100%', sm: 380 } }}>
      <CardHeader
        title={
          <StackRow>
            <Link component={NextLink} href={getOrganizationLocationResourceBaseLink(integratedPlatrform, organizationCustomDomain, locationId, resourceDetails.id)}>
              <LeadIconTypography
                startElement={
                  resourceDetails.resourceType.type === rootData.deskResourceType ? (
                    <DeskIcon />
                  ) : resourceDetails.resourceType.type === rootData.roomResourceType ? (
                    <RoomIcon />
                  ) : resourceDetails.resourceType.type === rootData.parkingResourceType ? (
                    <ParkingIcon />
                  ) : (
                    <OtherResourceIcon />
                  )
                }
                label={resourceDetails.name}
                sx={{ flexWrap: undefined }}
                invertDefaultColor
              />
            </Link>
            <PushToRight />

            <NewBookingButton
              onReloadRequired={onReloadRequired}
              defaultDate={date}
              organizationCustomDomain={organizationCustomDomain}
              defaultLocationId={locationId}
              defaultResourceIds={[resourceDetails.id]}
              label="Book Now"
              hideIcon
              variant="contained"
              size="small"
              sx={{ textTransform: 'none' }}
              invertDefaultColor={paletteMode === 'dark'}
            />
          </StackRow>
        }
      />
      <CardContent>
        <CustomTags customTags={resourceDetails.customTags.map((item) => ({ id: item.id, name: item.name, color: item.color }))} sx={{ paddingTop: 1, paddingBottom: 1 }} />
        <Divider />
        <Zones zones={resourceDetails.zones.map((item) => ({ id: item.id, name: item.name, color: item.color }))} sx={{ paddingTop: 1, paddingBottom: 1 }} />
        <Divider />

        <StackRow>
          <BodyIconTypography label="Capacity:" />
          <SmallIconTypography label={`${resourceDetails.capacity}`} />
        </StackRow>
      </CardContent>
    </Card>
  );
};

export default memo(ResourceCard);
