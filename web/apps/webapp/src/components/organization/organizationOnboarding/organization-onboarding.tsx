import { StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { AddLocation } from '@/components/location/addLocation';
import { AddOrganization } from '@/components/organization/addOrganization';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { AddTeam } from '@/components/team/addTeam';
import type { organizationOnboarding_rootQuery } from '@/queries/__generated__/organizationOnboarding_rootQuery.graphql';
import Step from '@mui/material/Step';
import StepLabel from '@mui/material/StepLabel';
import Stepper from '@mui/material/Stepper';
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
    }
  }
`;

const OrganizationOnboarding = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<organizationOnboarding_rootQuery>(RootQuery, queryReference);
  const [activeStep, setActiveStep] = useState(0);
  const [organizationId, setOrganizationId] = useState<string | null>(null);

  const handleOrganizationAdded = (id: string) => {
    setOrganizationId(id);
    setActiveStep(1);
  };

  const handleLocationAdded = () => {
    setActiveStep(2);
  };

  const handleLocationDismissed = () => {
    setActiveStep(2);
  };

  const handleTeamAdded = () => {
    onReloadRequired();
  };

  const handleTeamDismissed = () => {
    onReloadRequired();
  };

  return (
    <StackColumn>
      <Stepper activeStep={activeStep}>
        <Step>
          <StepLabel>Create Organization</StepLabel>
        </Step>
        <Step>
          <StepLabel>Create Location</StepLabel>
        </Step>
        <Step>
          <StepLabel>Create Team</StepLabel>
        </Step>
      </Stepper>
      {activeStep === 0 && <AddOrganization onReloadRequired={() => {}} showCancel={false} onAdded={handleOrganizationAdded} addLabel="Create Organization" />}
      {activeStep === 1 && organizationId && (
        <AddLocation organizationId={organizationId} onReloadRequired={() => {}} onAdded={handleLocationAdded} onCancel={handleLocationDismissed} addLabel="Create Location" />
      )}
      {activeStep === 2 && organizationId && (
        <AddTeam organizationId={organizationId} onReloadRequired={() => {}} onAdded={handleTeamAdded} onCancel={handleTeamDismissed} addLabel="Create Team" />
      )}
    </StackColumn>
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
