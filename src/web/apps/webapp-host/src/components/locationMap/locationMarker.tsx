'use client';

import { memo } from 'react';
import HostMarker from './hostMarker';

type LocationMarkerProps = {
  id: string;
  name: string;
  organizationType?: string;
};

const LocationMarker = ({ id, name, organizationType }: LocationMarkerProps) => {
  if (organizationType === 'HOST') {
    return <HostMarker name={name} />;
  }

  return (
    <div data-testid={`location-marker-${id}`}>
      <span>{name}</span>
    </div>
  );
};

export default memo(LocationMarker);
