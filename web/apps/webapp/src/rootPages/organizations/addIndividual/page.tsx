import { getOrganizationBaseLink, getOrganizationLocationAddIndividualLink, getOrganizationLocationsBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { AddIndividualOrganization } from '@/components/organization/addOrganization';
import { RelayError, toRootError } from '@/components/relayError';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { pageAddIndividualOrganization_claimLocationOwnershipMutation } from '@/queries/__generated__/pageAddIndividualOrganization_claimLocationOwnershipMutation.graphql';
import type { pageAddIndividualOrganization_completeOnboardingMutation } from '@/queries/__generated__/pageAddIndividualOrganization_completeOnboardingMutation.graphql';
import type { pageAddIndividualOrganization_rootQuery } from '@/queries/__generated__/pageAddIndividualOrganization_rootQuery.graphql';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageAddIndividualOrganization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageAddIndividualOrganization_rootQuery {
    me {
      id
      isOnboardingDone
    }
    ...addIndividualOrganization_query
  }
`;

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageAddIndividualOrganization_rootQuery>(RootQuery, queryReference);

  const [commitCompleteOnboarding] = useMutation<pageAddIndividualOrganization_completeOnboardingMutation>(graphql`
    mutation pageAddIndividualOrganization_completeOnboardingMutation($input: CompleteOrganizationOnboardingInput!) @raw_response_type {
      completeOnboarding(input: $input) {
        customer {
          id
          isOnboardingDone
        }
      }
    }
  `);

  const [commitClaimLocationOwnership] = useMutation<pageAddIndividualOrganization_claimLocationOwnershipMutation>(graphql`
    mutation pageAddIndividualOrganization_claimLocationOwnershipMutation($input: ClaimLocationOwnershipInput!) {
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
              <NotificationContent content={`Failed to claim location with unique code ${locationUniqueClaimCode}. Error: ${joinErrors(errors)}.`} />,
              errorNotificationOptions,
            );
          }

          router.push(getOrganizationLocationsBaseLink(integratedPlatrform, customDomain));
          onReloadRequired();
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to claim location with unique code ${locationUniqueClaimCode}. Error: ${error.message}.`} />, errorNotificationOptions);

          if (rootData.me.isOnboardingDone) {
            router.push(
              getOrganizationLocationAddIndividualLink(integratedPlatrform, customDomain, {
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
                themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
              }

              router.push(
                getOrganizationLocationAddIndividualLink(integratedPlatrform, customDomain, {
                  redirectUrl: getOrganizationBaseLink(integratedPlatrform, customDomain),
                }),
              );
              onReloadRequired();
            },
            onError: (error) => {
              themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${error.message}.`} />, errorNotificationOptions);

              router.push(
                getOrganizationLocationAddIndividualLink(integratedPlatrform, customDomain, {
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
          getOrganizationLocationAddIndividualLink(integratedPlatrform, customDomain, {
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
            themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
          }

          router.push(
            getOrganizationLocationAddIndividualLink(integratedPlatrform, customDomain, {
              redirectUrl: getOrganizationBaseLink(integratedPlatrform, customDomain),
            }),
          );
          onReloadRequired();
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to complete onboarding. Error: ${error.message}.`} />, errorNotificationOptions);

          router.push(
            getOrganizationLocationAddIndividualLink(integratedPlatrform, customDomain, {
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
      <AddIndividualOrganization
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
  const [queryReference, loadQuery] = useQueryLoader<pageAddIndividualOrganization_rootQuery>(RootQuery);
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
