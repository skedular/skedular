import { Dayjs } from 'dayjs';
import { toHourAndMinute } from '../../libs/utils';
import { TAG_TYPE_LOCATION_ZONE } from '../zone';

export type LocationDetails = {
  name: string;
};

export type TeamDetails = {
  name: string;
};

export type LocationTagDetails = {
  uniqueId: string;
  name: string;
  tagType?: string;
};

export type DeskDetails = {
  name: string;
  locationTags: LocationTagDetails[];
};

export type BookingDetails = {
  from: Dayjs | string;
  to: Dayjs | string;
  location?: LocationDetails;
  team?: TeamDetails;
  desks: DeskDetails[];
};

export const getBookingSummaryMessage = (booking: BookingDetails, includeTime: boolean) => {
  let message = includeTime ? `${toHourAndMinute(booking.from)} - ${toHourAndMinute(booking.to)}, Working` : 'Working';
  if (booking.team) {
    message += ` in team "${booking.team!.name}"`;
  }

  if (booking.location) {
    message += ` from the "${booking.location!.name}"`;
  }

  if (booking.desks.length > 0) {
    message += ` at desk "${booking.desks.map(({ name }) => name).join(', ')}"`;

    const zones = booking.desks.flatMap(({ locationTags }) => locationTags).filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE);
    if (zones.length > 0) {
      const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

      message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
    }
  }

  return message;
};
