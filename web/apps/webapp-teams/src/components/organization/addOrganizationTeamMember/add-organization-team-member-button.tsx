import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@skedular/ui';
import { NewIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import type { addOrganizationTeamMemberButton_rootQuery } from '@/queries/__generated__/addOrganizationTeamMemberButton_rootQuery.graphql';
import Button from '@mui/material/Button';
import { memo, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import AddOrganizationTeamMemberDialog from './add-organization-team-member-dialog';

type Props = {
  queryReference: PreloadedQuery<addOrganizationTeamMemberButton_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  connectionIds: string[];
  teamId: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const RootQuery = graphql`
  query addOrganizationTeamMemberButton_rootQuery(
    $organizationCustomDomain: String!
    $peopleNameSearchText: String
    $addTeamMemberDialogOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    ...addOrganizationTeamMemberDialog_organizationMembers_query
  }
`;

const AddOrganizationTeamMemberButton = ({ queryReference, onReloadRequired, connectionIds, teamId, fullWidth, label, hideIcon, variant, size }: Props) => {
  const rootData = usePreloadedQuery<addOrganizationTeamMemberButton_rootQuery>(RootQuery, queryReference);
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const handleButtonClicked = () => {
    setIsDialogOpen(true);
  };

  const handleAddClicked = () => {
    setIsDialogOpen(false);

    if (onReloadRequired) {
      onReloadRequired();
    }
  };

  const handleCancelClicked = () => {
    setIsDialogOpen(false);
  };

  return (
    <>
      <Button variant={variant ?? 'text'} onClick={handleButtonClicked} fullWidth={fullWidth} sx={{ textTransform: 'none' }}>
        {size === 'small' && <SmallIconTypography label={label ?? 'Add Member'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
        {size === 'medium' && <BodyIconTypography label={label ?? 'Add Member'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
        {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Add Member'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />}
      </Button>
      <AddOrganizationTeamMemberDialog
        rootDataRelay={rootData}
        connectionIds={connectionIds}
        teamId={teamId}
        isDialogOpen={isDialogOpen}
        onAddClicked={handleAddClicked}
        onCancel={handleCancelClicked}
      />
    </>
  );
};

const MemoAddOrganizationTeamMemberButton = memo(AddOrganizationTeamMemberButton);

type RelayProps = {
  organizationCustomDomain: string;
  onReloadRequired?: () => void;
  connectionIds: string[];
  teamId: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const AddOrganizationTeamMemberButtonWithRelay = ({ organizationCustomDomain, onReloadRequired, connectionIds, teamId, fullWidth, label, hideIcon, variant, size }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addOrganizationTeamMemberButton_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        addTeamMemberDialogOrganizationMembersSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoAddOrganizationTeamMemberButton
        queryReference={queryReference}
        onReloadRequired={onReloadRequired}
        connectionIds={connectionIds}
        teamId={teamId}
        fullWidth={fullWidth}
        label={label}
        hideIcon={hideIcon}
        variant={variant}
        size={size}
      />
    </ErrorBoundary>
  );
};

export default memo(AddOrganizationTeamMemberButtonWithRelay);
