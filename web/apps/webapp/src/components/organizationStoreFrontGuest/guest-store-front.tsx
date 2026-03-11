import { BodyIconTypography, LargeHeadingIconTypography, MediumHeadingIconTypography, SubtitleIconTypography } from '@/components/commons';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { useKnownParams } from '@/libs/providers';
import type { guestStoreFront_rootQuery } from '@/queries/__generated__/guestStoreFront_rootQuery.graphql';
import Container from '@mui/material/Container';
import { alpha } from '@mui/material/styles';
import Box from '@mui/system/Box';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import GuestStoreFrontFooter from './guest-store-front-footer';
import GuestStoreFrontProductCard from './guest-store-front-product-card';
import { defaultGuestStoreFrontData } from './mock-data';

type Props = {
  queryReference: PreloadedQuery<guestStoreFront_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query guestStoreFront_rootQuery($organizationUniqueAlphanumericName: String!) {
    organizationPublic(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
      listingMetadata {
        title
        subTitle
      }
    }
    ...guestStoreFrontFooter_query
  }
`;

const GuestStoreFront = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<guestStoreFront_rootQuery>(RootQuery, queryReference);
  const data = defaultGuestStoreFrontData;

  if (!rootData.organizationPublic) {
    return null;
  }

  return (
    <Box sx={{ bgcolor: (theme) => theme.palette.background.default, minHeight: '100vh' }}>
      <Container maxWidth="xl" sx={{ mt: { xs: 3, md: 5 }, mb: 7 }}>
        <Box
          sx={{
            position: 'relative',
            height: { xs: 340, md: 520 },
            borderRadius: 3,
            overflow: 'hidden',
            border: 1,
            borderColor: (theme) => theme.palette.divider,
          }}
        >
          <Box component="img" src={data.heroImageUrl} alt={data.organizationName} sx={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block' }} />
          <Box
            sx={{
              position: 'absolute',
              inset: 0,
              background: (theme) => `linear-gradient(180deg, ${alpha(theme.palette.common.black, 0.15)} 0%, ${alpha(theme.palette.common.black, 0.7)} 100%)`,
              display: 'flex',
              alignItems: 'flex-end',
            }}
          >
            <Box sx={{ p: { xs: 3, md: 5 }, maxWidth: 850 }}>
              {rootData.organizationPublic.listingMetadata.title && (
                <LargeHeadingIconTypography label={rootData.organizationPublic.listingMetadata.title} sx={{ color: (theme) => theme.palette.common.white, mb: 1 }} />
              )}
              {rootData.organizationPublic.listingMetadata.subTitle && (
                <SubtitleIconTypography
                  label={`${rootData.organizationPublic.listingMetadata.subTitle}`}
                  sx={{ color: (theme) => alpha(theme.palette.common.white, 0.92), mb: 3 }}
                />
              )}
            </Box>
          </Box>
        </Box>
      </Container>

      <Container maxWidth="xl" sx={{ mb: 6 }}>
        <Box sx={{ mb: 4 }}>
          <MediumHeadingIconTypography label={data.productsHeading} sx={{ mb: 1 }} />
          <BodyIconTypography label={data.productsSubtitle} sx={{ opacity: 0.85 }} />
        </Box>

        <Box
          sx={{
            display: 'grid',
            gap: 3,
            gridTemplateColumns: {
              xs: '1fr',
              sm: '1fr 1fr',
              lg: 'repeat(4, minmax(0, 1fr))',
            },
          }}
        >
          {data.products.map((product) => (
            <GuestStoreFrontProductCard key={product.id} product={product} />
          ))}
        </Box>
      </Container>

      <GuestStoreFrontFooter rootDataRelay={rootData} />
    </Box>
  );
};

const MemoGuestStoreFront = memo(GuestStoreFront);

const GuestStoreFrontWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<guestStoreFront_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationUniqueAlphanumericName } = useKnownParams();

  if (!organizationUniqueAlphanumericName) {
    throw new Error('organizationUniqueAlphanumericName is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationUniqueAlphanumericName,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoGuestStoreFront queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(GuestStoreFrontWithRelay);
