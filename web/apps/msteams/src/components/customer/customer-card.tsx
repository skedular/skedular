import Card from '@mui/material/Card';
import CardHeader from '@mui/material/CardHeader';
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
    <Card elevation={24} sx={{ minWidth: 200, height: '100%' }}>
      <CardHeader
        title={
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
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
            <Typography gutterBottom variant="body1">
              {getCustomerFullName(customerDetails)}
            </Typography>
          </Stack>
        }
      />
    </Card>
  );
};

export default memo(CustomerCard);
