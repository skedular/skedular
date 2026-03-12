import type { MarketplaceProductLocation } from './types';

export const marketplaceProductDetailLocationMocks: MarketplaceProductLocation[] = [
  {
    id: 'location-1',
    name: 'Auckland CBD',
    address: '3 Pheasant Close, Stanmore Bay, Whangaparaoa',
    availableLabel: '5 resources available',
    resources: [
      { id: 'desk-a12', name: 'Desk A12', details: ['Window view', 'Quiet zone'] },
      { id: 'desk-b08', name: 'Desk B08', details: ['Corner spot', 'Extra outlets'] },
    ],
  },
  {
    id: 'location-2',
    name: 'Wynyard Quarter',
    address: '10 Madden Street, Auckland',
    availableLabel: '3 resources available',
    resources: [
      { id: 'desk-d04', name: 'Desk D04', details: ['Natural light', 'Near lounge'] },
      { id: 'desk-e11', name: 'Desk E11', details: ['Private corner', 'Standing desk'] },
    ],
  },
];
