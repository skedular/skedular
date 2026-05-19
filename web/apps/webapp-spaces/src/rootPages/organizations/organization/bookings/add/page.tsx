import { AddPrivateBooking } from '@/components/booking/addBooking';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const RootPage = () => (
  <RootShell>
    <AddPrivateBooking />
  </RootShell>
);

export default memo(RootPage);
