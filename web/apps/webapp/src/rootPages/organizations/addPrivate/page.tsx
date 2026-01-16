import { getOrganizationBaseLink, getOrganizationLocationAddPrivateLink, getOrganizationLocationsBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { AddPrivateOrganization } from '@/components/organization/addOrganization';
import { RelayError, toRootError } from '@/components/relayError';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { pageAddPrivateOrganization_claimLocationOwnershipMutation } from '@/queries/__generated__/pageAddPrivateOrganization_claimLocationOwnershipMutation.graphql';
import type { pageAddPrivateOrganization_completeOnboardingMutation } from '@/queries/__generated__/pageAddPrivateOrganization_completeOnboardingMutation.graphql';
import type { pageAddPrivateOrganization_rootQuery } from '@/queries/__generated__/pageAddPrivateOrganization_rootQuery.graphql';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageAddPrivateOrganization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageAddPrivateOrganization_rootQuery {
    me {
      id
      isOnboardingDone
    }
    ...addPrivateOrganization_query
  }
`;

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageAddPrivateOrganization_rootQuery>(RootQuery, queryReference);

  const [commitCompleteOnboarding] = useMutation<pageAddPrivateOrganization_completeOnboardingMutation>(graphql`
    mutation pageAddPrivateOrganization_completeOnboardingMutation($input: CompleteOrganizationOnboardingInput!) @raw_response_type {
      completeOnboarding(input: $input) {
        customer {
          id
          isOnboardingDone
        }
      }
    }
  `);

  const [commitClaimLocationOwnership] = useMutation<pageAddPrivateOrganization_claimLocationOwnershipMutation>(graphql`
    mutation pageAddPrivateOrganization_claimLocationOwnershipMutation($input: ClaimLocationOwnershipInput!) {
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

  const handleAdded = (id: string, uniqueAlphanumericName: string) => {
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
              <NotificationContent content={`Failed to claim location with unique code ${locationUniqueClaimCode}. Error: ${joinErrors(errors)}.`} />,
              errorNotificationOptions,
            );
          }

          router.push(getOrganizationLocationsBaseLink(integratedPlatrform, uniqueAlphanumericName));
          onReloadRequired();
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to claim location with unique code ${locationUniqueClaimCode}. Error: ${error.message}.`} />, errorNotificationOptions);

          if (rootData.me.isOnboardingDone) {
            router.push(
              getOrganizationLocationAddPrivateLink(integratedPlatrform, uniqueAlphanumericName, {
                redirectUrl: getOrganizationBaseLink(integratedPlatrform, uniqueAlphanumericName),
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
                themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
              }

              router.push(
                getOrganizationLocationAddPrivateLink(integratedPlatrform, uniqueAlphanumericName, {
                  redirectUrl: getOrganizationBaseLink(integratedPlatrform, uniqueAlphanumericName),
                }),
              );
              onReloadRequired();
            },
            onError: (error) => {
              themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${error.message}.`} />, errorNotificationOptions);

              router.push(
                getOrganizationLocationAddPrivateLink(integratedPlatrform, uniqueAlphanumericName, {
                  redirectUrl: getOrganizationBaseLink(integratedPlatrform, uniqueAlphanumericName),
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
          getOrganizationLocationAddPrivateLink(integratedPlatrform, uniqueAlphanumericName, {
            redirectUrl: getOrganizationBaseLink(integratedPlatrform, uniqueAlphanumericName),
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
            themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
          }

          router.push(
            getOrganizationLocationAddPrivateLink(integratedPlatrform, uniqueAlphanumericName, {
              redirectUrl: getOrganizationBaseLink(integratedPlatrform, uniqueAlphanumericName),
            }),
          );
          onReloadRequired();
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${error.message}.`} />, errorNotificationOptions);

          router.push(
            getOrganizationLocationAddPrivateLink(integratedPlatrform, uniqueAlphanumericName, {
              redirectUrl: getOrganizationBaseLink(integratedPlatrform, uniqueAlphanumericName),
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
      <AddPrivateOrganization
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
  const [queryReference, loadQuery] = useQueryLoader<pageAddPrivateOrganization_rootQuery>(RootQuery);
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
