'use client';

import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Typography from '@/components/commons/Typography';

export type LocationCardProps = {
  id: string;
  name: string;
  address?: string;
  verified?: boolean;
  productCount?: number;
  pricingMode?: 'per-booking' | 'subscription'; // NEW: Pricing mode for quick display
  isPublished?: boolean; // NEW: Whether the listing is published
};

const LocationCard = ({ id, name, address, verified = false, productCount, pricingMode, isPublished = true }: LocationCardProps) => (
  <Card
    component="article"
    sx={{
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      transition: 'box-shadow 200ms ease, transform 200ms ease',
      '&:hover': { boxShadow: (theme) => theme.shadows[4], transform: 'translateY(-2px)' },
    }}
    variant="outlined"
  >
    <CardContent sx={{ flexGrow: 1 }}>
      <Stack direction="row" spacing={1} sx={{ mb: 1, alignItems: 'flex-start', justifyContent: 'space-between' }}>
        <Typography variant="h6" component="h2" sx={{ fontWeight: 600, pr: 1 }}>
          {name}
        </Typography>
        {!isPublished ? (
          <Chip label="Draft" color="warning" size="small" variant="outlined" sx={{ fontWeight: 600 }} />
        ) : verified ? (
          <Chip label="Verified" color="success" size="small" variant="outlined" sx={{ fontWeight: 600 }} />
        ) : (
          <Chip label="Pending" color="warning" size="small" variant="outlined" sx={{ fontWeight: 600 }} />
        )}
      </Stack>
      {address && (
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
          {address}
        </Typography>
      )}
      {typeof productCount === 'number' && (
        <Typography variant="caption" color="text.secondary">
          {productCount} {productCount === 1 ? 'product' : 'products'}
        </Typography>
      )}
      {/* NEW: Show pricing mode badge */}
      {pricingMode && <Chip label={pricingMode === 'subscription' ? 'Subscription' : 'Per-booking'} size="small" sx={{ mt: 1, fontSize: '0.75rem', height: 20 }} />}
    </CardContent>
    <CardActions sx={{ px: 2, pb: 2, pt: 0 }}>
      {/* UPDATED: Link to unified listing page instead of products */}
      <Link href={`/locations/${encodeURIComponent(id)}`} underline="hover" variant="body2" sx={{ fontWeight: 600 }}>
        Manage Listing
      </Link>
    </CardActions>
  </Card>
);

export default LocationCard;
