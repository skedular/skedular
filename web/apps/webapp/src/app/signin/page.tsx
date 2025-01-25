'use client';

import { getSignInUrlAction } from '@/components/authActions';
import { useRouter } from 'next/navigation';
import { memo, useEffect } from 'react';

const SignInPage = () => {
  const router = useRouter();

  useEffect(() => {
    async function load() {
      router.push(await getSignInUrlAction());
    }

    load();
  }, [router]);

  return <></>;
};

export default memo(SignInPage);
