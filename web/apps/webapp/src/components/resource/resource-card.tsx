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
  organizationId: string;
  locationId: string;
  date: Dayjs;
};

const ResourceCard = ({ rootDataRelay, resourceDetailsRelay, onReloadRequired, organizationId, locationId, date }: Props) => {
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
          uniqueId
          name
          color
        }
        zones {
          uniqueId
          name
          color
        }
        productTags {
          uniqueId
          name
          color
        }
        resourceType {
          uniqueId
          name
          color
          tagType
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
            <Link component={NextLink} href={getOrganizationLocationResourceBaseLink(integratedPlatrform, organizationId, locationId, resourceDetails.id)}>
              <LeadIconTypography
                startElement={
                  resourceDetails.resourceType.tagType === rootData.deskResourceType ? (
                    <DeskIcon />
                  ) : resourceDetails.resourceType.tagType === rootData.roomResourceType ? (
                    <RoomIcon />
                  ) : resourceDetails.resourceType.tagType === rootData.parkingResourceType ? (
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
              organizationId={organizationId}
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
        <CustomTags customTags={resourceDetails.customTags.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color }))} sx={{ paddingTop: 1, paddingBottom: 1 }} />
        <Divider />
        <Zones zones={resourceDetails.zones.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color }))} sx={{ paddingTop: 1, paddingBottom: 1 }} />
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
