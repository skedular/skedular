import { memo } from 'react';
import { WorkingFromHomeIcon, WorkingFromOfficeIcon } from '../icons';
import { TAG_TYPE_LOCATION_ZONE } from '../zone';

type BookingDetails = {
  from: string;
  to: string;
  location:
    | {
        name: string;
      }
    | null
    | undefined;
  desks: Array<{
    locationTags: Array<{
      name: string;
      tagType: string | null | undefined;
      uniqueId: string;
    }>;
    name: string;
  }>;
};

type Props = {
  booking: BookingDetails;
};

const BookingIcon = ({ booking }: Props) => {
  let tip = '';

  if (booking) {
    tip = `Working`;
    if (booking.location) {
      tip += ` from the "${booking.location!.name}"`;
    }

    if (booking.desks.length > 0) {
      tip += ` at desk "${booking.desks.map(({ name }) => name).join(', ')}"`;

      const zones = booking.desks.flatMap(({ locationTags }) => locationTags).filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE);
      if (zones.length > 0) {
        const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

        tip += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
      }
    }
  }

  return booking ? <WorkingFromOfficeIcon tip={tip} /> : <WorkingFromHomeIcon />;
};

export default memo(BookingIcon);
