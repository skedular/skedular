import { DeleteIcon } from '@/components/icons';
import { getRootLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { useIntegratedPlatrform, PaletteModeContext } from '@skedular/shared';
import { getRelayErrorMessage } from '@skedular/shared';
import type { organizationAdminManageOrganizationSectionQuery } from '@/queries/__generated__/organizationAdminManageOrganizationSectionQuery.graphql';
import type { organizationAdminManageOrganizationSection_deleteOrganizationMutation } from '@/queries/__generated__/organizationAdminManageOrganizationSection_deleteOrganizationMutation.graphql';
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
  queryReference: PreloadedQuery<organizationAdminManageOrganizationSectionQuery>;
};

const RootQuery = graphql`
  query organizationAdminManageOrganizationSectionQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
    }
  }
`;

const OrganizationAdminManageOrganizationSectionContent = ({ queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminManageOrganizationSectionQuery>(RootQuery, queryReference);
  const [commitDeleteOrganization] = useMutation<organizationAdminManageOrganizationSection_deleteOrganizationMutation>(graphql`
    mutation organizationAdminManageOrganizationSection_deleteOrganizationMutation($input: DeleteOrganizationInput!) {
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
  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();

  if (!organization) {
    return null;
  }

  const handleRemoveOrganizationClicked = () => {
    const name = organization.name;
    const toastId = themedToast(<NotificationContent content={`Removing organization '${name}'...`} />, infoNotificationOptions);

    commitDeleteOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: organization.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the organization '${name}'. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization '${name}' removed.`} />,
        });

        router.push(getRootLink(integratedPlatrform));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the organization '${name}'. Error: ${error.message}.`} />,
        });
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

const OrganizationAdminManageOrganizationSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationAdminManageOrganizationSectionQuery>(RootQuery);

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

  return <OrganizationAdminManageOrganizationSectionContent queryReference={queryReference} />;
};

export default memo(OrganizationAdminManageOrganizationSection);
