import type { GuestStoreFrontData } from './types';

export const defaultGuestStoreFrontData: GuestStoreFrontData = {
  organizationName: 'Downtown Hub',
  heroTitle: 'Your workspace in the heart of downtown',
  heroSubtitle: 'Flexible memberships, premium amenities, and a community that inspires.',
  heroImageUrl: 'https://images.unsplash.com/photo-1678282931256-370578a0d036?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1600',
  productsHeading: 'Explore our spaces',
  productsSubtitle: 'From hot desks to private offices, find the workspace that matches your needs.',
  products: [
    {
      id: 'hot-desk',
      name: 'Hot Desk',
      type: 'Flexible Workspace',
      description: 'Access any available desk in our open workspace.',
      imageUrl: 'https://images.unsplash.com/photo-1642665358815-310df20dc8dd?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1200',
      amenities: ['WiFi', 'Coffee', 'Printing', '24/7 Access'],
      availableCount: 12,
      pricingOptions: [
        { id: 'hot-day', name: 'Day Pass', periodLabel: 'Daily', description: 'Drop in anytime', price: 25 },
        { id: 'hot-week', name: 'Weekly Pass', periodLabel: 'Weekly', description: '5 days access', price: 100 },
      ],
    },
    {
      id: 'dedicated-desk',
      name: 'Dedicated Desk',
      type: 'Reserved Workspace',
      description: 'Your own desk in a shared office setup.',
      imageUrl: 'https://images.unsplash.com/photo-1514905565314-fea02285fa69?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1200',
      amenities: ['WiFi', 'Storage', 'Monitor', '24/7 Access'],
      availableCount: 8,
      pricingOptions: [
        { id: 'ded-month', name: 'Monthly Desk', periodLabel: 'Monthly', description: 'Personal desk monthly', price: 450 },
        { id: 'ded-quarter', name: 'Quarterly Plan', periodLabel: '3 Months', description: 'Save with commitment', price: 1200 },
      ],
    },
    {
      id: 'meeting-room',
      name: 'Meeting Room',
      type: 'Hourly Booking',
      description: 'Professional room with video conferencing and whiteboard.',
      imageUrl: 'https://images.unsplash.com/photo-1763412050485-d7e1688f8858?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1200',
      amenities: ['TV Screen', 'Video Call', 'Whiteboard', 'Coffee'],
      availableCount: 3,
      pricingOptions: [
        { id: 'meet-hour', name: 'Hourly Rate', periodLabel: 'Hourly', description: 'Pay as you go', price: 30 },
        { id: 'meet-day', name: 'Full Day', periodLabel: 'Daily', description: 'All-day access', price: 200 },
      ],
    },
    {
      id: 'private-office',
      name: 'Private Office',
      type: 'Team Workspace',
      description: 'Lockable office space for teams needing privacy.',
      imageUrl: 'https://images.unsplash.com/photo-1759803545394-041ea7b71989?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1200',
      amenities: ['Private Entry', 'Storage', 'Monitors', '24/7 Access'],
      availableCount: 2,
      pricingOptions: [
        { id: 'office-month', name: 'Monthly Office', periodLabel: 'Monthly', description: 'Private office monthly', price: 1200 },
        { id: 'office-six', name: 'Long-Term Lease', periodLabel: '6 Months', description: 'Best value plan', price: 6500 },
      ],
    },
  ],
  footerAddressLines: ['123 Main Street, Suite 100', 'New York, NY 10001'],
  footerEmail: 'info@downtownhub.com',
};
