'use client';

import { Loading } from '@/components/loading';
import { redirect, useSearchParams } from 'next/navigation';
import { memo, Suspense } from 'react';

const RedirectToWorkOSSignIn = () => {
  const searchParams = useSearchParams();
  const query = searchParams.toString();

  redirect(`/signin${query ? `?${query}` : ''}`);
};

const RootPage = () => (
  <Suspense fallback={<Loading />}>
    <RedirectToWorkOSSignIn />
  </Suspense>
);

export default memo(RootPage);
