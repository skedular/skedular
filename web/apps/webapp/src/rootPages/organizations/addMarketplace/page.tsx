import { getOrganizationBaseLink, getOrganizationLocationAddMarketplaceLink, getOrganizationLocationsBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { AddMarketplaceOrganization } from '@/components/organization/addOrganization';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { pageAddMarketplaceOrganization_claimLocationOwnershipMutation } from '@/queries/__generated__/pageAddMarketplaceOrganization_claimLocationOwnershipMutation.graphql';
import type { pageAddMarketplaceOrganization_completeOnboardingMutation } from '@/queries/__generated__/pageAddMarketplaceOrganization_completeOnboardingMutation.graphql';
import type { pageAddMarketplaceOrganization_rootQuery } from '@/queries/__generated__/pageAddMarketplaceOrganization_rootQuery.graphql';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageAddMarketplaceOrganization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageAddMarketplaceOrganization_rootQuery {
    me {
      id
      isOnboardingDone
    }
    ...addMarketplaceOrganization_query
  }
`;

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageAddMarketplaceOrganization_rootQuery>(RootQuery, queryReference);

  const [commitCompleteOnboarding] = useMutation<pageAddMarketplaceOrganization_completeOnboardingMutation>(graphql`
    mutation pageAddMarketplaceOrganization_completeOnboardingMutation($input: CompleteOrganizationOnboardingInput!) @raw_response_type {
      completeOnboarding(input: $input) {
        customer {
          id
          isOnboardingDone
        }
      }
    }
  `);

  const [commitClaimLocationOwnership] = useMutation<pageAddMarketplaceOrganization_claimLocationOwnershipMutation>(graphql`
    mutation pageAddMarketplaceOrganization_claimLocationOwnershipMutation($input: ClaimLocationOwnershipInput!) {
      claimLocationOwnership(input: $input) {
        clientMutationId
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const locationUniqueClaimCode = searchParams.get('locationUniqueClaimCode');

  const handleAdded = (id: string) => {
    if (rootData.me.isOnboardingDone) {
      router.push(getOrganizationLocationAddMarketplaceLink(integratedPlatrform, id, { redirectUrl: getOrganizationBaseLink(integratedPlatrform, id) }));
      onReloadRequired();

      return;
    }

    if (locationUniqueClaimCode) {
      commitClaimLocationOwnership({
        variables: {
          input: {
            clientMutationId: uuid(),
            organizationUniqueAlphanumericName: id,
            uniqueClaimCode: locationUniqueClaimCode.toLocaleUpperCase(),
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            themedToast(
              <NotificationContent content={`Failed to claim location with unique code ${locationUniqueClaimCode}. Error: ${joinErrors(errors)}.`} />,
              errorNotificationOptions,
            );
          }

          router.push(getOrganizationLocationsBaseLink(integratedPlatrform, id));
          onReloadRequired();
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to claim location with unique code ${locationUniqueClaimCode}. Error: ${error.message}.`} />, errorNotificationOptions);

          commitCompleteOnboarding({
            variables: {
              input: {
                clientMutationId: uuid(),
              },
            },
            onCompleted: (_, errors) => {
              if (errors && errors.length > 0) {
                themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
              }

              router.push(getOrganizationLocationAddMarketplaceLink(integratedPlatrform, id, { redirectUrl: getOrganizationBaseLink(integratedPlatrform, id) }));
              onReloadRequired();
            },
            onError: (error) => {
              themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${error.message}.`} />, errorNotificationOptions);

              router.push(getOrganizationLocationAddMarketplaceLink(integratedPlatrform, id, { redirectUrl: getOrganizationBaseLink(integratedPlatrform, id) }));
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
          onReloadRequired();
        },
      });
    } else {
      commitCompleteOnboarding({
        variables: {
          input: {
            clientMutationId: uuid(),
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
          }

          router.push(getOrganizationLocationAddMarketplaceLink(integratedPlatrform, id, { redirectUrl: getOrganizationBaseLink(integratedPlatrform, id) }));
          onReloadRequired();
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${error.message}.`} />, errorNotificationOptions);

          router.push(getOrganizationLocationAddMarketplaceLink(integratedPlatrform, id, { redirectUrl: getOrganizationBaseLink(integratedPlatrform, id) }));
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
  };

  const handleCancelled = () => {
    router.back();
  };

  return (
    <NoOrganizationRootShell hideOrganizationSelector>
      <AddMarketplaceOrganization
        rootDataRelay={rootData}
        onAdded={handleAdded}
        onCancel={handleCancelled}
        onReloadRequired={onReloadRequired}
        cancelLabel="Back"
        createLabel="Create"
      />
    </NoOrganizationRootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageAddMarketplaceOrganization_rootQuery>(RootQuery);
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
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
