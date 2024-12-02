import AvatarGroup from '@mui/material/AvatarGroup';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import LinearProgress from '@mui/material/LinearProgress';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
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
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <LocationIcon fontSize="medium" />
            <Typography variant="h6">{locationDetails.name}</Typography>
            <Box sx={{ flexGrow: 1 }} /> {/* This will push NewBookingButton to the right */}
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
          </Stack>
        }
      />
      <CardContent>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', paddingTop: 1, paddingBottom: 1, width: '100%' }}>
          <DeskIcon fontSize="medium" />
          <Typography variant="body1" sx={{ flexGrow: 0, flexShrink: 0 }}>{`${desksCount} Desks`}</Typography>

          <Stack direction="column" sx={{ paddingLeft: 20, alignItems: 'flex-end', width: '100%' }}>
            <Typography variant="body2">{`${availableDesksCount} Available Today`}</Typography>
            <LinearProgress value={availablePercentage} variant="determinate" sx={{ width: '100%' }} />
          </Stack>
        </Stack>

        <Divider />

        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', paddingTop: 1, paddingBottom: 1 }}>
          <ZoneIcon fontSize="medium" />
          <Zones zones={zones} />
        </Stack>

        <Divider />

        <Stack direction="row" spacing={1}>
          <Stack direction="column" spacing={1}>
            <Typography variant="body1">Shared with teammates</Typography>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <AvatarGroup max={5}>
                {sharedWithTeammates.map((item) => (
                  <CustomerAvatar key={item?.uniqueId} name={item} photo={{ url: item?.photoUrl }} size="medium" showFullName />
                ))}
              </AvatarGroup>
            </Stack>
          </Stack>

          <Divider orientation="vertical" flexItem />

          <Stack direction="column" spacing={1}>
            <Typography variant="body1" sx={{ whiteSpace: 'pre-line' }}>
              {locationDetails.physicalAddress?.formattedAddress ? locationDetails.physicalAddress?.formattedAddress : 'N/A'}
            </Typography>
          </Stack>
        </Stack>
      </CardContent>
    </Card>
  );
};

export default memo(MyLocationCard);
