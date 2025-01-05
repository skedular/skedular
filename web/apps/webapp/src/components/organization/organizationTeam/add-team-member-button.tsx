import type { addTeamMemberButton_rootQuery } from '@/queries/__generated__/addTeamMemberButton_rootQuery.graphql';
import Button from '@mui/material/Button';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@repo/shared/components/commons';
import { NewIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import { memo, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<addTeamMemberButton_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  connectionIds?: string[];
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const RootQuery = graphql`
  query addTeamMemberButton_rootQuery {
    me {
      id
    }
  }
`;

const AddTeamMemberButton = ({ queryReference, onReloadRequired, fullWidth, label, hideIcon, variant, size }: Props) => {
  const rootData = usePreloadedQuery<addTeamMemberButton_rootQuery>(RootQuery, queryReference);
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
        {size === 'small' && (
          <SmallIconTypography label={label ?? 'Add Memebr'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />
        )}
        {size === 'medium' && (
          <BodyIconTypography label={label ?? 'Add Memebr'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />
        )}
        {(size === 'large' || !size) && (
          <LeadIconTypography label={label ?? 'Add Memebr'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />
        )}
      </Button>
    </>
  );
};

const MemoAddTeamMemberButton = memo(AddTeamMemberButton);

type RelayProps = {
  onReloadRequired?: () => void;
  connectionIds?: string[];
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const AddTeamMemberButtonWithRelay = ({ onReloadRequired, connectionIds, fullWidth, label, hideIcon, variant, size }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addTeamMemberButton_rootQuery>(RootQuery);

  useEffect(() => {
    const date = startOfDay().toISOString();

    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoAddTeamMemberButton
        queryReference={queryReference}
        connectionIds={connectionIds}
        fullWidth={fullWidth}
        label={label}
        hideIcon={hideIcon}
        variant={variant}
        size={size}
      />
    </ErrorBoundary>
  );
};

export default memo(AddTeamMemberButtonWithRelay);
