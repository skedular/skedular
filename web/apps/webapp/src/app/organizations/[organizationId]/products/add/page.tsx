'use client';

import { AddProduct } from '@/components/product/addProduct';
import { RootShell } from '@/components/rootShell';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const AddProductPage = () => {
  const router = useRouter();

  const handleAdded = () => {
    router.back();
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell>
      <AddProduct onReloadRequired={handleReloadRequired} onAdded={handleAdded} onCancel={handleCancelled} />
    </RootShell>
  );
};

export default memo(AddProductPage);
