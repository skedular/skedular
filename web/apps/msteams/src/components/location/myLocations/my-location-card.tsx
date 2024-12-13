import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import LinearProgress from '@mui/material/LinearProgress';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackColumn, StackRow } from '@repo/shared/components/commons';
import { DeskIcon, LocationIcon, ZoneIcon } from '@repo/shared/components/icons';
import { Zones } from '@repo/shared/components/zone';
import graphql from 'babel-plugin-relay/macro';
import { NewBookingButton } from 'components/booking/addBooking';
import { Dayjs } from 'dayjs';
import { memo } from 'react';
import { useFragment } from 'react-relay';
import type { myLocationCard_LocationDetails$key } from './__generated__/myLocationCard_LocationDetails.graphql';

type Props = {
  locationDetailsRelay: myLocationCard_LocationDetails$key;
  onReloadRequired: () => void;
  organizationId: string;
  connectionIds: string[];
  sharedWithTeammates: CustomerDetails[];
  availableDesksCount: number;
  availablePercentage: number;
  defaultDate: Dayjs;
};

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

const MyLocationCard = ({
  locationDetailsRelay,
  onReloadRequired,
  organizationId,
  sharedWithTeammates,
  availableDesksCount,
  availablePercentage,
  defaultDate,
}: Props) => {
  const locationDetails = useFragment(
    graphql`
      fragment myLocationCard_LocationDetails on LocationDetails {
        id
        name
        deskTypes {
          uniqueId
          name
        }
        zones {
          uniqueId
          name
        }
        desks {
          id
        }
        physicalAddress {
          formattedAddress
        }
      }
    `,
    locationDetailsRelay,
  );

  const desksCount = locationDetails.desks.length;
  const zones = locationDetails.zones.map(({ uniqueId, name }) => ({ id: uniqueId, name }));

  return (
    <Card sx={{ width: 600 }}>
      <CardHeader
        title={
          <StackRow>
            <LeadIconTypography label={locationDetails.name} icon={<LocationIcon />} />
            <PushToRight />
            <NewBookingButton
              hideLocationControl={false}
              hideOrganizationControl={true}
              onReloadRequired={onReloadRequired}
              defaultDate={defaultDate}
              organizationId={organizationId}
              locationId={locationDetails.id}
              label="Book Now"
              hideIcon
              variant="contained"
              size="small"
            />
          </StackRow>
        }
      />
      <CardContent>
        <StackRow sx={{ paddingTop: 1, paddingBottom: 1, width: '100%' }}>
          <BodyIconTypography label={`${desksCount} Desks`} sx={{ flexGrow: 0, flexShrink: 0 }} icon={<DeskIcon />} />
          <StackColumn sx={{ paddingLeft: 40, alignItems: 'flex-end', width: '100%' }}>
            <SmallIconTypography label={`${availableDesksCount} Available Today`} />
            <LinearProgress value={availablePercentage} variant="determinate" sx={{ width: '100%' }} />
          </StackColumn>
        </StackRow>

        <Divider />

        <StackRow sx={{ paddingTop: 1, paddingBottom: 1 }}>
          <ZoneIcon />
          <Zones zones={zones} />
        </StackRow>

        <Divider />

        <StackRow>
          <StackColumn>
            <BodyIconTypography label="Shared with teammates" />
            <StackRow>
              <AvatarGroup max={5}>
                {sharedWithTeammates.map((item) => (
                  <CustomerAvatar key={item?.uniqueId} name={item} photo={{ url: item?.photoUrl }} size="medium" showFullName />
                ))}
              </AvatarGroup>
            </StackRow>
          </StackColumn>

          <Divider orientation="vertical" flexItem />

          <BodyIconTypography
            label={locationDetails.physicalAddress?.formattedAddress ? locationDetails.physicalAddress?.formattedAddress : 'N/A'}
            sx={{ whiteSpace: 'pre-line' }}
          />
        </StackRow>
      </CardContent>
    </Card>
  );
};

export default memo(MyLocationCard);
