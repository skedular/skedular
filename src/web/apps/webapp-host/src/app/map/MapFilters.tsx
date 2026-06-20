'use client';

import { memo, useState } from 'react';

type OrganizationType = 'PRIVATE' | 'MARKETPLACE' | 'HOST';

type MapFiltersProps = {
  onFilterChange?: (types: OrganizationType[]) => void;
};

const ALL_TYPES: OrganizationType[] = ['PRIVATE', 'MARKETPLACE', 'HOST'];

const MapFilters = ({ onFilterChange }: MapFiltersProps) => {
  const [selected, setSelected] = useState<OrganizationType[]>(ALL_TYPES);

  const toggle = (type: OrganizationType) => {
    const next = selected.includes(type) ? selected.filter((t) => t !== type) : [...selected, type];
    setSelected(next);
    onFilterChange?.(next);
  };

  return (
    <div data-testid="map-filters">
      {ALL_TYPES.map((type) => (
        <label key={type}>
          <input type="checkbox" checked={selected.includes(type)} onChange={() => toggle(type)} />
          {type}
        </label>
      ))}
    </div>
  );
};

export default memo(MapFilters);
