import { memo } from 'react';
import { WorkingFromHomeIcon, WorkingFromOfficeIcon } from '../icons';
import type { BookingDetails } from './utils';
import { getBookingSummaryMessage } from './utils';

type Props = {
  booking?: BookingDetails;
};

const BookingIcon = ({ booking }: Props) => (booking ? <WorkingFromOfficeIcon tip={getBookingSummaryMessage(booking, false)} /> : <WorkingFromHomeIcon />);

export default memo(BookingIcon);
