'use client';

import Link from '@mui/material/Link';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Typography from '@/components/commons/Typography';

export type Product = {
  id: string;
  name: string;
  price?: number | null;
  type?: string | null;
  productType?: string | null;
  capacity?: number | null;
  inactive?: boolean;
};

export type ProductTableProps = {
  products?: Product[];
  locationId?: string;
  onEdit?: (id: string) => void;
};

const formatPrice = (price?: number | null): string => {
  if (typeof price !== 'number' || Number.isNaN(price)) {
    return '—';
  }
  return new Intl.NumberFormat(undefined, { style: 'currency', currency: 'USD', maximumFractionDigits: 2 }).format(price);
};

const resolveType = (product: Product): string => product.type ?? product.productType ?? 'Event';

const ProductTable = ({ products = [], locationId, onEdit }: ProductTableProps) => {
  if (products.length === 0) {
    return (
      <Paper variant="outlined" sx={{ p: 4, textAlign: 'center' }}>
        <Typography variant="body1" color="text.secondary">
          No products yet. Create your first product to start accepting bookings.
        </Typography>
      </Paper>
    );
  }

  return (
    <TableContainer component={Paper} variant="outlined">
      <Table aria-label="Host products" size="medium">
        <TableHead>
          <TableRow>
            <TableCell sx={{ fontWeight: 700 }}>Name</TableCell>
            <TableCell sx={{ fontWeight: 700 }} align="right">
              Price
            </TableCell>
            <TableCell sx={{ fontWeight: 700 }}>Type</TableCell>
            <TableCell sx={{ fontWeight: 700 }} align="right">
              Capacity
            </TableCell>
            <TableCell sx={{ fontWeight: 700 }} align="right">
              Actions
            </TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {products.map((product) => {
            const editHref = locationId
              ? `/locations/${encodeURIComponent(locationId)}/products/${encodeURIComponent(product.id)}/edit`
              : `/products/${encodeURIComponent(product.id)}/edit`;
            return (
              <TableRow key={product.id} hover>
                <TableCell>
                  <Typography variant="body2" sx={{ fontWeight: 600 }}>
                    {product.name}
                  </Typography>
                  {product.inactive && (
                    <Typography component="span" variant="caption" color="text.disabled">
                      {' '}
                      (inactive)
                    </Typography>
                  )}
                </TableCell>
                <TableCell align="right">
                  <Typography variant="body2">{formatPrice(product.price)}</Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2">{resolveType(product)}</Typography>
                </TableCell>
                <TableCell align="right">
                  <Typography variant="body2">{typeof product.capacity === 'number' ? product.capacity : '—'}</Typography>
                </TableCell>
                <TableCell align="right">
                  <Link
                    href={editHref}
                    underline="hover"
                    variant="body2"
                    onClick={(event) => {
                      if (onEdit) {
                        event.preventDefault();
                        onEdit(product.id);
                      }
                    }}
                    sx={{ fontWeight: 600, cursor: 'pointer' }}
                  >
                    Edit
                  </Link>
                </TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default ProductTable;
