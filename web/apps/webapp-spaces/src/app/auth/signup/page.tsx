'use client';

import { Loading } from '@/components/loading';
import { redirect, useSearchParams } from 'next/navigation';
import { memo, Suspense } from 'react';

const RedirectToWorkOSSignUp = () => {
  const searchParams = useSearchParams();
  const query = searchParams.toString();

  redirect(`/signup${query ? `?${query}` : ''}`);
};

const RootPage = () => (
  <Suspense fallback={<Loading />}>
    <RedirectToWorkOSSignUp />
  </Suspense>
);

export default memo(RootPage);
