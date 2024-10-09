'use client';

import { OrganizationOnboarding } from '@/components/organization/organizationOnboarding';
import { RootShell } from '@/components/rootShell';
import { SmallMonthlyViewCalendar } from '@/components/smallMonthlyViewCalendar';
import { memo } from 'react';

const Home = () => (
  <RootShell>
    <OrganizationOnboarding />
    <SmallMonthlyViewCalendar />
  </RootShell>
);

export default memo(Home);
