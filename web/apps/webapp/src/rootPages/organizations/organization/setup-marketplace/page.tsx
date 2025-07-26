import {BodyIconTypography, StackColumn} from '@/components/commons';
import {Loading} from '@/components/loading';
import {OrganizationMarketplaceSetup} from '@/components/organization/organizationMarketplaceSetup';
import type {RootError} from '@/components/relayError';
import {RelayError} from '@/components/relayError';
import {RootShell} from '@/components/rootShell';
import type {
    pageOrganizationMarketplaceSetup_rootQuery
} from '@/queries/__generated__/pageOrganizationMarketplaceSetup_rootQuery.graphql';
import {Breadcrumbs} from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import {useParams, useRouter} from 'next/navigation';
import {memo, useEffect, useState, useTransition} from 'react';
import {ErrorBoundary} from 'react-error-boundary';
import {graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader} from 'react-relay';
import {v7 as uuid} from 'uuid';

type Props = {
    queryReference: PreloadedQuery<pageOrganizationMarketplaceSetup_rootQuery, Record<string, unknown>>;
    onReloadRequired: () => void;
    organizationId: string;
};

const RootQuery = graphql`
    query pageOrganizationMarketplaceSetup_rootQuery(
        $organizationId: String!
        $productNameSearchText: String
        $productTagNameSearchText: String
        $locationTagNameSearchText: String
        $organizationStripeConnectAccountNameSearchText: String
        $organizationBankAccountNameSearchText: String
    ) {
        organization(id: $organizationId) {
            name
        }
        ...organizationMarketplaceSetup_query
        ...organizationMarketplaceSetup_products_query
        ...organizationMarketplaceSetup_productTags_query
        ...organizationMarketplaceSetup_locationTags_query
        ...organizationMarketplaceSetup_organizationStripeConnectAccounts_query
        ...organizationMarketplaceSetup_organizationBankAccounts_query
    }
`;

const RootPage = ({queryReference, onReloadRequired, organizationId}: Props) => {
    const rootData = usePreloadedQuery<pageOrganizationMarketplaceSetup_rootQuery>(RootQuery, queryReference);
    const router = useRouter();

    const handleBackClick = () => {
        router.back();
    };

    const breadcrumbs = (
        <StackColumn sx={{alignItems: 'flex-start'}} spacing={0}>
            <Button variant="text" onClick={handleBackClick} sx={{whiteSpace: 'nowrap', textTransform: 'none'}}>
                {'< back'}
            </Button>
            <Box sx={{display: {xs: 'none', sm: 'block'}}}>
                <Breadcrumbs>
                    <BodyIconTypography label="Marketplace Setup"/>
                    <BodyIconTypography label={rootData.organization?.name}/>
                </Breadcrumbs>
            </Box>
        </StackColumn>
    );

    return (
        <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
            <OrganizationMarketplaceSetup
                rootDataRelay={rootData}
                rootDataProductsRelay={rootData}
                rootDataProductTagsRelay={rootData}
                rootDataLocationTagsRelay={rootData}
                rootDataOrganizationStripeConnectAccountsRelay={rootData}
                rootDataOrganizationBankAccountsRelay={rootData}
                onReloadRequired={onReloadRequired}
                organizationId={organizationId}
            />
        </RootShell>
    );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
    const [queryReference, loadQuery] = useQueryLoader<pageOrganizationMarketplaceSetup_rootQuery>(RootQuery);
    const [triggerReloadId, setTriggerReloadId] = useState(uuid());
    const [, startTransition] = useTransition();
    const {organizationId, teamId} = useParams();
    let finalOrganizationId = '';

    if (typeof organizationId === 'string') {
        finalOrganizationId = organizationId;
    } else if (Array.isArray(organizationId)) {
        if (typeof organizationId[0] === 'undefined') {
            throw new Error('organizationId is required');
        }

        finalOrganizationId = organizationId[0];
    } else {
        throw new Error('organizationId is required');
    }

    useEffect(() => {
        loadQuery(
            {
                organizationId: finalOrganizationId,
            },
            {
                fetchPolicy: 'store-and-network',
            },
        );
    }, [loadQuery, triggerReloadId, finalOrganizationId]);

    const handleReloadRequired = () => {
        startTransition(() => {
            setTriggerReloadId(uuid());
        });
    };

    if (!queryReference) {
        return <Loading/>;
    }

    return (
        <ErrorBoundary fallbackRender={({error}: { error: RootError }) => <RelayError error={error}/>}>
            <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired}
                          organizationId={finalOrganizationId}/>
        </ErrorBoundary>
    );
};

export default memo(RootPageWithRelay);
