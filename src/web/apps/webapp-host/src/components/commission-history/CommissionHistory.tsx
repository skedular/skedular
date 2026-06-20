'use client';

import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import { BodyIconTypography } from '@skedular/ui';

export type CommissionEntry = {
  bookingId: string;
  bookingValue: number;
  commission: number;
  rate: number;
  hostPayout: number;
  date: string;
};

const currency = (value: number) => value.toLocaleString('en-US', { style: 'currency', currency: 'USD' });

const CommissionHistory = ({ entries }: { entries: CommissionEntry[] }) => {
  if (entries.length === 0) return <BodyIconTypography label="No commissions yet." />;

  return (
    <TableContainer component={Paper} variant="outlined">
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Date</TableCell>
            <TableCell>Booking</TableCell>
            <TableCell align="right">Booking value</TableCell>
            <TableCell align="right">Rate</TableCell>
            <TableCell align="right">Commission</TableCell>
            <TableCell align="right">Gross Host proceeds</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {entries.map((entry) => (
            <TableRow key={entry.bookingId}>
              <TableCell>{new Date(entry.date).toLocaleDateString()}</TableCell>
              <TableCell>{entry.bookingId}</TableCell>
              <TableCell align="right">{currency(entry.bookingValue)}</TableCell>
              <TableCell align="right">{entry.rate}%</TableCell>
              <TableCell align="right">{currency(entry.commission)}</TableCell>
              <TableCell align="right">{currency(entry.hostPayout)}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default CommissionHistory;
