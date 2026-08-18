import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import MenuItem from '@mui/material/MenuItem';
import TextField from '@mui/material/TextField';
import { useState } from 'react';
import { graphql, useMutation } from 'react-relay';
import { v7 as uuid } from 'uuid';
import OperatorEntitlementSelector, { type OperatorEntitlementOption } from '@/components/marketplaceEntitlement/operator-entitlement-selector';
import type { operatorMarketplaceBookingDialog_addMarketplaceBookingMutation } from '@/queries/__generated__/operatorMarketplaceBookingDialog_addMarketplaceBookingMutation.graphql';

type Product = {
  id: string;
  latestProductVersionId: string | null | undefined;
  title: string | null | undefined;
  pricingOptions: readonly { id: string; title: string | null | undefined; fulfillmentType: string }[];
};
type Props = {
  open: boolean;
  organizationCustomDomain: string;
  customerId: string;
  products: readonly Product[];
  entitlements: readonly OperatorEntitlementOption[];
  onClose: () => void;
  onCompleted: () => void;
};

export default function OperatorMarketplaceBookingDialog({ open, organizationCustomDomain, customerId, products, entitlements, onClose, onCompleted }: Props) {
  const [productId, setProductId] = useState('');
  const [pricingId, setPricingId] = useState('');
  const [entitlementId, setEntitlementId] = useState<string | null>(null);
  const [from, setFrom] = useState('');
  const [until, setUntil] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [commit, inFlight] = useMutation<operatorMarketplaceBookingDialog_addMarketplaceBookingMutation>(graphql`
    mutation operatorMarketplaceBookingDialog_addMarketplaceBookingMutation($input: AddMarketplaceBookingInput!) {
      addMarketplaceBooking(input: $input) {
        booking {
          id
        }
        accessError {
          message
        }
      }
    }
  `);
  const product = products.find((item) => item.id === productId);
  const pricing = product?.pricingOptions.find((item) => item.id === pricingId);
  const submit = () => {
    if (!product?.latestProductVersionId || !pricing || !from || !until) return;
    setError(null);
    commit({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: uuid(),
          customerIds: [customerId],
          organizationCustomDomains: [organizationCustomDomain],
          teamIds: [],
          resourceIds: [],
          from: new Date(from).toISOString(),
          until: new Date(until).toISOString(),
          category: 'WORKING_FROM_COWORKING_SPACE',
          paymentMethod: 'CARD',
          invoiceEmailList: [],
          quantity: 1,
          productVersionId: product.latestProductVersionId,
          pricingId: pricing.id,
          entitlementId,
        },
      },
      onCompleted: (response) => {
        if (response.addMarketplaceBooking.booking) onCompleted();
        else setError(response.addMarketplaceBooking.accessError?.message ?? 'The booking could not be created.');
      },
      onError: (err) => setError(err.message),
    });
  };
  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Make marketplace booking for customer</DialogTitle>
      <DialogContent sx={{ display: 'grid', gap: 2, pt: 2 }}>
        {error && <Alert severity="error">{error}</Alert>}
        <TextField
          select
          label="Product"
          value={productId}
          onChange={(event) => {
            setProductId(event.target.value);
            setPricingId('');
          }}
        >
          {products.map((item) => (
            <MenuItem key={item.id} value={item.id}>
              {item.title ?? item.id}
            </MenuItem>
          ))}
        </TextField>
        <TextField select label="Pricing" value={pricingId} onChange={(event) => setPricingId(event.target.value)} disabled={!product}>
          {(product?.pricingOptions ?? []).map((item) => (
            <MenuItem key={item.id} value={item.id}>
              {item.title ?? item.id} ({item.fulfillmentType})
            </MenuItem>
          ))}
        </TextField>
        <OperatorEntitlementSelector options={entitlements} value={entitlementId} onChange={setEntitlementId} />
        <TextField label="From" type="datetime-local" value={from} onChange={(event) => setFrom(event.target.value)} slotProps={{ inputLabel: { shrink: true } }} />
        <TextField label="Until" type="datetime-local" value={until} onChange={(event) => setUntil(event.target.value)} slotProps={{ inputLabel: { shrink: true } }} />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" onClick={submit} disabled={inFlight || !product || !pricing || !from || !until}>
          Create booking
        </Button>
      </DialogActions>
    </Dialog>
  );
}
