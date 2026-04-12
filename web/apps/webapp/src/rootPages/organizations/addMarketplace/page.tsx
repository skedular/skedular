import { getOrganizationBaseLink, getOrganizationLocationAddMarketplaceLink, getOrganizationLocationsBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { AddMarketplaceOrganization } from '@/components/organization/addOrganization';
import { RelayError, toRootError } from '@/components/relayError';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { getRelayErrorMessage } from '@/libs/utils';
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

  const handleAdded = (id: string, customDomain: string) => {
    if (locationUniqueClaimCode) {
      commitClaimLocationOwnership({
        variables: {
          input: {
            clientMutationId: uuid(),
            organizationId: id,
            uniqueClaimCode: locationUniqueClaimCode.toLocaleUpperCase(),
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            themedToast(
              <NotificationContent content={`We couldn't claim the location with code ${locationUniqueClaimCode}. ${getRelayErrorMessage(errors)}`} />,
              errorNotificationOptions,
            );
          }

          router.push(getOrganizationLocationsBaseLink(integratedPlatrform, customDomain));
          onReloadRequired();
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`We couldn't claim the location with code ${locationUniqueClaimCode}. ${error.message}`} />, errorNotificationOptions);

          if (rootData.me.isOnboardingDone) {
            router.push(
              getOrganizationLocationAddMarketplaceLink(integratedPlatrform, customDomain, {
                redirectUrl: getOrganizationBaseLink(integratedPlatrform, customDomain),
              }),
            );
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

              router.push(
                getOrganizationLocationAddMarketplaceLink(integratedPlatrform, customDomain, {
                  redirectUrl: getOrganizationBaseLink(integratedPlatrform, customDomain),
                }),
              );
              onReloadRequired();
            },
            onError: (error) => {
              themedToast(<NotificationContent content={`We couldn't finish setting up your account. ${error.message}`} />, errorNotificationOptions);

              router.push(
                getOrganizationLocationAddMarketplaceLink(integratedPlatrform, customDomain, {
                  redirectUrl: getOrganizationBaseLink(integratedPlatrform, customDomain),
                }),
              );
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
      if (rootData.me.isOnboardingDone) {
        router.push(
          getOrganizationLocationAddMarketplaceLink(integratedPlatrform, customDomain, {
            redirectUrl: getOrganizationBaseLink(integratedPlatrform, customDomain),
          }),
        );
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

          router.push(
            getOrganizationLocationAddMarketplaceLink(integratedPlatrform, customDomain, {
              redirectUrl: getOrganizationBaseLink(integratedPlatrform, customDomain),
            }),
          );
          onReloadRequired();
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`We couldn't finish setting up your account. ${error.message}`} />, errorNotificationOptions);

          router.push(
            getOrganizationLocationAddMarketplaceLink(integratedPlatrform, customDomain, {
              redirectUrl: getOrganizationBaseLink(integratedPlatrform, customDomain),
            }),
          );
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
        cancelLabel="Go back"
        createLabel="Create organisation"
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
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
