import type { MarketplaceProductDetail } from './types';

export const marketplaceProductDetailMock: MarketplaceProductDetail = {
  id: 'placeholder-product',
  title: 'Dedicated Desk',
  typeLabel: 'Reserved Workspace',
  shortDescription: 'Your own desk in a shared office with stable setup and premium amenities.',
  longDescription:
    'Each dedicated desk includes ergonomic seating, storage, and access to shared areas. This structure mirrors the latest Figma detail page and is intentionally data-driven so GraphQL fields can be wired in without changing layout.',
  imageUrls: [
    'https://images.unsplash.com/photo-1497366216548-37526070297c?auto=format&fit=crop&w=1600&q=80',
    'https://images.unsplash.com/photo-1519389950473-47ba0277781c?auto=format&fit=crop&w=1200&q=80',
    'https://images.unsplash.com/photo-1520607162513-77705c0f0d4a?auto=format&fit=crop&w=1200&q=80',
  ],
  features: ['Lockable storage', 'Ergonomic chair', 'High-speed internet', 'Access to meeting rooms', 'Community lounge access', 'Printing and utilities'],
  amenities: ['Wi-Fi', 'Coffee', 'Dual Monitor', 'Phone Booth', 'Kitchenette', 'Recording Room'],
  pricingPlans: [
    {
      id: 'monthly-plan',
      name: 'Monthly Desk',
      cadenceLabel: 'Per Month',
      amountLabel: 'NZD - $450',
      note: 'incl. tax',
    },
    {
      id: 'quarterly-plan',
      name: 'Quarterly Plan',
      cadenceLabel: 'Per Quarter',
      amountLabel: 'NZD - $1,200',
      note: 'incl. tax',
      highlighted: true,
    },
    {
      id: 'semi-annual-plan',
      name: 'Half-Year Plan',
      cadenceLabel: 'Per 6 Months',
      amountLabel: 'NZD - $2,300',
      note: 'incl. tax',
    },
  ],
  locations: [
    {
      id: 'location-1',
      name: 'Auckland CBD',
      address: '3 Pheasant Close, Stanmore Bay, Whangaparaoa',
      availableLabel: '5 desks available',
      resources: [
        { id: 'desk-a12', name: 'Desk A12', details: ['Window view', 'Quiet zone'] },
        { id: 'desk-b08', name: 'Desk B08', details: ['Corner spot', 'Extra outlets'] },
      ],
    },
    {
      id: 'location-2',
      name: 'Wynyard Quarter',
      address: '10 Madden Street, Auckland',
      availableLabel: '3 desks available',
      resources: [
        { id: 'desk-d04', name: 'Desk D04', details: ['Natural light', 'Near lounge'] },
        { id: 'desk-e11', name: 'Desk E11', details: ['Private corner', 'Standing desk'] },
      ],
    },
  ],
};
