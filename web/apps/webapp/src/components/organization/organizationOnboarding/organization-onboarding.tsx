import { AddOrganization } from '@/components/organization/addOrganization';
import type { organizationOnboarding_rootQuery } from '@/queries/__generated__/organizationOnboarding_rootQuery.graphql';
import Step from '@mui/material/Step';
import StepLabel from '@mui/material/StepLabel';
import Stepper from '@mui/material/Stepper';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organizationOnboarding_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query organizationOnboarding_rootQuery {
    me {
      id
      isOrganizationOnboardingDone
    }
  }
`;

const OrganizationOnboarding = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<organizationOnboarding_rootQuery>(RootQuery, queryReference);
  const [activeStep] = useState(0);
  const [isOnboardingOpen, setIsOnboardingOpen] = useState(!rootData.me?.isOrganizationOnboardingDone);

  const handleAdded = () => {
    setIsOnboardingOpen(false);
  };

  return (
    <>
      {isOnboardingOpen && (
        <>
          <Stepper activeStep={activeStep}>
            <Step>
              <StepLabel>Create Organization</StepLabel>
            </Step>
          </Stepper>
          {activeStep === 0 && <AddOrganization showCancel={false} onAdded={handleAdded} onReloadRequired={onReloadRequired} />}
        </>
      )}
    </>
  );
};

const MemoOrganizationOnboarding = memo(OrganizationOnboarding);

type RelayProps = {
  onReloadRequired: () => void;
};

const OrganizationOnboardingWithRelay = ({ onReloadRequired }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationOnboarding_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
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
      setTriggerReloadId(nanoid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationOnboarding queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationOnboardingWithRelay);
