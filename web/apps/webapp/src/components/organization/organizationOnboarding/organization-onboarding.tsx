import { StackColumn } from '@/components/commons';
import { getOrganizationBaseLink } from '@/components/links';
import { AddLocation } from '@/components/location/addLocation';
import { AddOrganization } from '@/components/organization/addOrganization';
import Step from '@mui/material/Step';
import StepLabel from '@mui/material/StepLabel';
import Stepper from '@mui/material/Stepper';
import { useRouter } from 'next/navigation';
import { memo, useState } from 'react';

const OrganizationOnboarding = () => {
  const router = useRouter();
  const [activeStep, setActiveStep] = useState(0);
  const [organizationId, setOrganizationId] = useState<string | null>(null);

  const handleOrganizationAdded = (id: string) => {
    setOrganizationId(id);
    setActiveStep(1);
  };

  const handleLocationAdded = () => {
    if (organizationId) {
      router.push(getOrganizationBaseLink(organizationId));
    }
  };

  const handleLocationDismissed = () => {
    if (organizationId) {
      router.push(getOrganizationBaseLink(organizationId));
    }
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
      </Stepper>
      {activeStep === 0 && <AddOrganization onReloadRequired={() => {}} showCancel={false} onAdded={handleOrganizationAdded} addLabel="Create Organization" />}
      {activeStep === 1 && organizationId && (
        <AddLocation
          organizationId={organizationId}
          onReloadRequired={() => {}}
          showDismiss
          onAdded={handleLocationAdded}
          onCancel={handleLocationDismissed}
          addLabel="Create Location"
        />
      )}
    </StackColumn>
  );
};

export default memo(OrganizationOnboarding);
