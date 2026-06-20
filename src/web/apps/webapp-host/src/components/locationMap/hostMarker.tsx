'use client';

import { memo } from 'react';

type HostMarkerProps = {
  name: string;
};

const HostMarker = ({ name }: HostMarkerProps) => (
  <div data-testid="host-marker" style={{ position: 'relative' }}>
    <span style={{ background: '#ff9800', color: 'white', padding: '2px 6px', borderRadius: 4 }}>HOST</span>
    <span style={{ marginLeft: 4 }}>{name}</span>
  </div>
);

export default memo(HostMarker);
