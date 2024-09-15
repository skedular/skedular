import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { getCustomerFullName } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { memo } from 'react';
import { useFragment } from 'react-relay';
import type { customerCard_CustomerDetails$key } from './__generated__/customerCard_CustomerDetails.graphql';

type Props = {
  customerDetailsRelay: customerCard_CustomerDetails$key;
};

const CustomerCard = ({ customerDetailsRelay }: Props) => {
  const customerDetails = useFragment(
    graphql`
      fragment customerCard_CustomerDetails on CustomerDetails {
        name
        givenName
        middleName
        familyName
        photoUrl
      }
    `,
    customerDetailsRelay,
  );

  return (
    <Paper
      elevation={24}
      sx={{
        minWidth: 300,
        maxWidth: 300,
      }}
    >
      <Card
        sx={{
          minWidth: 300,
          maxWidth: 300,
        }}
      >
        <CardContent>
          <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
            <CustomerAvatar
              name={{
                name: customerDetails.name,
                givenName: customerDetails.givenName,
                middleName: customerDetails.middleName,
                familyName: customerDetails.familyName,
              }}
              photo={{
                url: customerDetails.photoUrl,
              }}
            />
          </Stack>

          <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
            <Typography gutterBottom variant="body1">
              {getCustomerFullName(customerDetails)}
            </Typography>
          </Stack>
        </CardContent>
      </Card>
    </Paper>
  );
};

export default memo(CustomerCard);
