'use client';

import { getSignUpUrlAction } from '@/components/authActions';
import { useRouter } from 'next/navigation';
import { memo, useEffect } from 'react';

const SignUpPage = () => {
  const router = useRouter();

  useEffect(() => {
    async function load() {
      router.push(await getSignUpUrlAction());
    }

    load();
  }, [router]);

  return <></>;
};

export default memo(SignUpPage);
