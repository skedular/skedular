import { BodyIconTypography, GridContainer, LeadIconTypography } from '@/components/commons';
import type { guestStoreFrontFooter_query$key } from '@/queries/__generated__/guestStoreFrontFooter_query.graphql';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import Box from '@mui/system/Box';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: guestStoreFrontFooter_query$key;
};

const GuestStoreFrontFooter = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<guestStoreFrontFooter_query$key>(
    graphql`
      fragment guestStoreFrontFooter_query on Query {
        organizationPublic(customDomain: $organizationCustomDomain) {
          name
          contactPhone
          contactEmail
          physicalAddress {
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
          }
        }
      }
    `,
    rootDataRelay,
  );

  if (!rootData.organizationPublic) {
    return null;
  }

  return (
    <Box
      component="footer"
      sx={{
        mt: 10,
        borderTop: 1,
        borderColor: (theme) => theme.palette.divider,
        backgroundColor: (theme) => theme.palette.background.paper,
        py: 5,
      }}
    >
      <Container maxWidth="xl">
        <GridContainer spacing={4}>
          <Grid sx={{ xs: 12, md: 6 }}>
            <LeadIconTypography label={rootData.organizationPublic.name} sx={{ mb: 1 }} />
            {rootData.organizationPublic.physicalAddress?.addressLine1 && (
              <BodyIconTypography label={rootData.organizationPublic.physicalAddress?.addressLine1} sx={{ opacity: 0.8 }} />
            )}
            {rootData.organizationPublic.physicalAddress?.addressLine2 && (
              <BodyIconTypography label={rootData.organizationPublic.physicalAddress?.addressLine2} sx={{ opacity: 0.8 }} />
            )}
            {rootData.organizationPublic.physicalAddress?.suburb && <BodyIconTypography label={rootData.organizationPublic.physicalAddress?.suburb} sx={{ opacity: 0.8 }} />}
            {rootData.organizationPublic.physicalAddress?.city && <BodyIconTypography label={rootData.organizationPublic.physicalAddress?.city} sx={{ opacity: 0.8 }} />}
            {rootData.organizationPublic.physicalAddress?.province && <BodyIconTypography label={rootData.organizationPublic.physicalAddress?.province} sx={{ opacity: 0.8 }} />}
            {rootData.organizationPublic.physicalAddress?.zipcode && <BodyIconTypography label={rootData.organizationPublic.physicalAddress?.zipcode} sx={{ opacity: 0.8 }} />}
            {rootData.organizationPublic.physicalAddress?.country && <BodyIconTypography label={rootData.organizationPublic.physicalAddress?.country} sx={{ opacity: 0.8 }} />}

            <BodyIconTypography label={rootData.organizationPublic.contactEmail} sx={{ opacity: 0.8 }} />
            <BodyIconTypography label={rootData.organizationPublic.contactPhone} sx={{ opacity: 0.8 }} />
          </Grid>
        </GridContainer>

        <Box sx={{ mt: 4, pt: 3, borderTop: 1, borderColor: (theme) => theme.palette.divider }}>
          <Box component="a" href="https://getskedular.com" target="_blank" rel="noopener noreferrer" sx={{ display: 'block', textDecoration: 'none' }}>
            <BodyIconTypography label="Powered by Skedular" sx={{ textAlign: 'center', opacity: 0.7 }} />
          </Box>
        </Box>
      </Container>
    </Box>
  );
};

export default memo(GuestStoreFrontFooter);
