import { WorkingFromHomeIcon, WorkingFromOfficeIcon } from '@/components/icons';
import { memo } from 'react';
import type { BookingDetails } from './utils';
import { getBookingSummaryMessage } from './utils';

type Props = {
  booking?: BookingDetails;
};

const BookingIcon = ({ booking }: Props) => (booking ? <WorkingFromOfficeIcon tip={getBookingSummaryMessage(booking, false)} /> : <WorkingFromHomeIcon />);

export default memo(BookingIcon);
