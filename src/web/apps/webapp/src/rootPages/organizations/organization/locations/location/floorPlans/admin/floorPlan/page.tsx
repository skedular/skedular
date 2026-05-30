'use client';

import { EditFloorPlan } from '@/components/floorPlan/editFloorPlan';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';
import useKnownParams from '@/hooks/use-known-params';

const EditFloorPlanPage = () => {
  const { locationId, floorPlanId } = useKnownParams();

  if (!locationId) {
    throw new Error('locationId is required');
  }

  if (!floorPlanId) {
    throw new Error('floorPlanId is required');
  }

  const handleReloadRequired = () => {};

  return (
    <RootShell>
      <EditFloorPlan locationId={locationId} floorPlanId={floorPlanId} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(EditFloorPlanPage);
