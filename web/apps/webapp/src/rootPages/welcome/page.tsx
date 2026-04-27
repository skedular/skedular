import { getOrganizationAddIndividualLink, getOrganizationAddMarketplaceLink, getOrganizationAddPrivateLink, getRootLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { NoOrganizationRootShell } from '@/components/rootShell';
import type { UserType } from '@/components/setupFlow';
import { SetupFlow } from '@/components/setupFlow';
import { PaletteModeContext, useIntegratedPlatrform } from '@skedular/shared';
import { getRelayErrorMessage } from '@skedular/shared';
import type { pageWelcome_completeOnboardingMutation } from '@/queries/__generated__/pageWelcome_completeOnboardingMutation.graphql';
import type { pageWelcome_rootQuery } from '@/queries/__generated__/pageWelcome_rootQuery.graphql';
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageWelcome_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageWelcome_rootQuery {
    me {
      id
      isOnboardingDone
    }
  }
`;

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageWelcome_rootQuery>(RootQuery, queryReference);

  const [commitCompleteOnboarding] = useMutation<pageWelcome_completeOnboardingMutation>(graphql`
    mutation pageWelcome_completeOnboardingMutation($input: CompleteOrganizationOnboardingInput!) @raw_response_type {
      completeOnboarding(input: $input) {
        customer {
          id
          isOnboardingDone
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;

  const handleUserTypeClick = (userType: UserType) => {
    switch (userType) {
      case 'private':
        router.push(getOrganizationAddPrivateLink(integratedPlatrform));
        break;

      case 'marketplace':
        router.push(getOrganizationAddMarketplaceLink(integratedPlatrform));
        break;

      case 'individual-organization':
        router.push(getOrganizationAddIndividualLink(integratedPlatrform));
        break;

      case 'individual-user':
        {
          if (rootData.me.isOnboardingDone) {
            router.push(getRootLink(integratedPlatrform));
            onReloadRequired();

            return;
          }

          commitCompleteOnboarding({
            variables: {
              input: {
                clientMutationId: uuid(),
              },
            },
            onCompleted: (_, errors) => {
              if (errors && errors.length > 0) {
                themedToast(<NotificationContent content={`We couldn't finish setting up your account. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
              }

              router.push(getRootLink(integratedPlatrform));
              onReloadRequired();
            },
            onError: (error) => {
              themedToast(<NotificationContent content={`We couldn't finish setting up your account. ${error.message}`} />, errorNotificationOptions);

              router.push(getRootLink(integratedPlatrform));
              onReloadRequired();
            },
            optimisticResponse: {
              completeOnboarding: {
                customer: {
                  id: rootData.me.id,
                  isOnboardingDone: true,
                },
              },
            },
          });
        }
        break;
    }
  };

  return (
    <NoOrganizationRootShell hideOrganizationSelector collapsed>
      <SetupFlow onUserTypeClick={handleUserTypeClick} userTypesToShow={['private', 'marketplace', 'individual-organization', 'individual-user']} />
    </NoOrganizationRootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageWelcome_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
