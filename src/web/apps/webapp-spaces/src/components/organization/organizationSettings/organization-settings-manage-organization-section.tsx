import { PaletteModeContext, getRelayErrorMessage, useIntegratedPlatform } from '@skedular/shared';
import { DeleteIcon } from '@/components/icons';
import { getRootLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';

import type { organizationSettingsManageOrganizationSectionQuery } from '@/queries/__generated__/organizationSettingsManageOrganizationSectionQuery.graphql';
import type { organizationSettingsManageOrganizationSection_deleteOrganizationMutation } from '@/queries/__generated__/organizationSettingsManageOrganizationSection_deleteOrganizationMutation.graphql';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import { EditorActionBar, SettingsSectionCard } from '@skedular/ui';
import { memo, useContext, useEffect } from 'react';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useRouter } from 'next/navigation';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
};

type InnerProps = {
  queryReference: PreloadedQuery<organizationSettingsManageOrganizationSectionQuery>;
};

const RootQuery = graphql`
  query organizationSettingsManageOrganizationSectionQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
    }
  }
`;

const OrganizationSettingsManageOrganizationSectionContent = ({ queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationSettingsManageOrganizationSectionQuery>(RootQuery, queryReference);
  const [commitDeleteOrganization] = useMutation<organizationSettingsManageOrganizationSection_deleteOrganizationMutation>(graphql`
    mutation organizationSettingsManageOrganizationSection_deleteOrganizationMutation($input: DeleteOrganizationInput!) {
      deleteOrganization(input: $input) {
        organization {
          id
        }
      }
    }
  `);

  const organization = rootData.organization;
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();

  if (!organization) {
    return null;
  }

  const handleRemoveOrganizationClicked = () => {
    const name = organization.name;

    commitDeleteOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: organization.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to remove the organization '${name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);

          return;
        }

        router.push(getRootLink(integratedPlatform));
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to remove the organization '${name}'. Error: ${error.message}.`} />, errorNotificationOptions);
      },
    });
  };

  return (
    <Box sx={{ pb: 2 }}>
      <SettingsSectionCard title="Manage organization" description="Use destructive actions carefully. Removing an organization is not reversible.">
        <EditorActionBar
          primaryAction={
            <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveOrganizationClicked} sx={{ textTransform: 'none' }}>
              Remove Organization
            </Button>
          }
        />
      </SettingsSectionCard>
    </Box>
  );
};

const OrganizationSettingsManageOrganizationSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationSettingsManageOrganizationSectionQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      { organizationCustomDomain },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return <Loading />;
  }

  return <OrganizationSettingsManageOrganizationSectionContent queryReference={queryReference} />;
};

export default memo(OrganizationSettingsManageOrganizationSection);
