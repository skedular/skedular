import Box from '@mui/system/Box';
import { CaptionIconTypography, SmallIconTypography } from '@skedular/ui';
import { memo } from 'react';

type BookingWindow = {
  readonly bookingId: string;
  readonly from: string;
  readonly until: string;
  readonly isRecurring: boolean;
  readonly isCheckedIn: boolean;
  readonly bookedByName?: string | null;
  readonly notes?: string | null;
};

type Props = {
  bookingWindows: ReadonlyArray<BookingWindow>;
};

const BookingWindowList = ({ bookingWindows }: Props) => {
  if (bookingWindows.length === 0) {
    return <CaptionIconTypography label="No bookings" />;
  }

  return (
    <Box component="ul" sx={{ listStyle: 'none', p: 0, m: 0 }}>
      {bookingWindows.map((window) => (
        <Box component="li" key={window.bookingId} sx={{ py: 0.5 }}>
          <SmallIconTypography label={`${window.from} – ${window.until}`} />
          {window.bookedByName && <CaptionIconTypography label={window.bookedByName} />}
          {window.notes && <CaptionIconTypography label={window.notes} />}
          {window.isRecurring && <CaptionIconTypography label="Recurring" />}
          {window.isCheckedIn && <CaptionIconTypography label="Checked in" />}
        </Box>
      ))}
    </Box>
  );
};

export default memo(BookingWindowList);
