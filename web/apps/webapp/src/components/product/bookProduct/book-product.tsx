import { AppBarWithStackColumn } from '@/components/commons';
import type { bookProduct_query$key } from '@/queries/__generated__/bookProduct_query.graphql';
import Box from '@mui/material/Box';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import { array, boolean, date, object, string } from 'yup';

type Props = {
  rootDataRelay: bookProduct_query$key;
  onReloadRequired?: () => void;
  organizationId: string;
};

type BookingDetails = {
  date: Date;
  allDay: boolean;
  member: string;
  notes: string;
  team: string | undefined;
  location: string | undefined;
  resources: string[];
};

const bookingSchema = object({
  date: date().required('Date/Time is required'),
  allDay: boolean(),
  member: string().required('User is required'),
  notes: string().notRequired(),
  team: string().notRequired(),
  location: string().notRequired(),
  resources: array().nullable(),
});

const BookProduct = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<bookProduct_query$key>(
    graphql`
      fragment bookProduct_query on Query {
        product(id: $productId) {
          id
          name
        }
      }
    `,
    rootDataRelay,
  );

  if (!rootData.product) {
    return <></>;
  }

  const product = rootData.product;

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={() => {}} label="Book Product"></AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(BookProduct);
