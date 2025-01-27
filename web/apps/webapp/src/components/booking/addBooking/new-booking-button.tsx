import type { newBookingButton_rootQuery } from '@/queries/__generated__/newBookingButton_rootQuery.graphql';
import Button from '@mui/material/Button';
import type { SxProps, Theme } from '@mui/system';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@repo/shared/components/commons';
import { NewIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { coal } from '@repo/shared/libs/theme';
import { startOfDay } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { memo, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import NewBookingDialog from './new-booking-dialog';

type Props = {
  queryReference: PreloadedQuery<newBookingButton_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  connectionIds?: string[];
  organizationId: string;
  defaultLocationId?: string;
  defaultDate?: Dayjs;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  sx?: SxProps<Theme>;
  invertDefaultColor?: boolean;
};

const RootQuery = graphql`
  query newBookingButton_rootQuery(
    $organizationId: String!
    $peopleNameSearchText: String
    $locationId: String!
    $locationExists: Boolean!
    $dateToGetAvailableDesks: DateTime!
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $customerId: String!
    $customerExists: Boolean!
    $teamsSortingValues: [TeamOrderInput!]
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    ...newBookingDialog_query
    ...newBookingDialog_organizationMembers_query
    ...newBookingDialog_customerTeams_query
    ...newBookingDialog_availableLocationDesks_query
  }
`;

const NewBookingButton = ({
  queryReference,
  onReloadRequired,
  connectionIds,
  organizationId,
  defaultLocationId,
  defaultDate,
  fullWidth,
  label,
  hideIcon,
  variant,
  size,
  sx,
  invertDefaultColor,
}: Props) => {
  const rootData = usePreloadedQuery<newBookingButton_rootQuery>(RootQuery, queryReference);
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

  const borderSx = variant === 'contained' ? { backgroundColor: 'white', borderColor: coal, borderWidth: 1, borderStyle: 'solid' } : {};

  return (
    <>
      <Button variant={variant ?? 'text'} onClick={handleButtonClicked} fullWidth={fullWidth} sx={{ ...sx, ...borderSx }}>
        {size === 'small' && (
          <SmallIconTypography label={label ?? 'Add Booking'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} invertDefaultColor={invertDefaultColor} />
        )}
        {size === 'medium' && (
          <BodyIconTypography label={label ?? 'Add Booking'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} invertDefaultColor={invertDefaultColor} />
        )}
        {(size === 'large' || !size) && (
          <LeadIconTypography label={label ?? 'Add Booking'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} invertDefaultColor={invertDefaultColor} />
        )}
      </Button>
      <NewBookingDialog
        rootDataRelay={rootData}
        rootDataTeamsRelay={rootData}
        rootDataOrganizationMembersRelay={rootData}
        rootDataAvailableLocationDesksRelay={rootData}
        connectionIds={connectionIds ?? []}
        isDialogOpen={isDialogOpen}
        onAddClicked={handleAddClicked}
        onCancel={handleCancelClicked}
        organizationId={organizationId}
        defaultLocationId={defaultLocationId}
        defaultDate={defaultDate}
      />
    </>
  );
};

const MemoNewBookingButton = memo(NewBookingButton);

type RelayProps = {
  onReloadRequired?: () => void;
  connectionIds?: string[];
  organizationId: string;
  defaultLocationId?: string;
  defaultDate?: Dayjs;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  sx?: SxProps<Theme>;
  invertDefaultColor?: boolean;
};

const NewBookingButtonWithRelay = ({
  onReloadRequired,
  connectionIds,
  organizationId,
  defaultLocationId,
  defaultDate,
  fullWidth,
  label,
  hideIcon,
  variant,
  size,
  sx,
  invertDefaultColor,
}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<newBookingButton_rootQuery>(RootQuery);

  useEffect(() => {
    const date = startOfDay().toISOString();

    loadQuery(
      {
        organizationId,
        locationId: defaultLocationId ?? '',
        locationExists: false,
        dateToGetAvailableDesks: date,
        customerId: '',
        customerExists: false,
        teamsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        locationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        organizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationId, defaultLocationId]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoNewBookingButton
        queryReference={queryReference}
        connectionIds={connectionIds}
        organizationId={organizationId}
        defaultLocationId={defaultLocationId}
        onReloadRequired={onReloadRequired}
        defaultDate={defaultDate}
        fullWidth={fullWidth}
        label={label}
        hideIcon={hideIcon}
        variant={variant}
        size={size}
        sx={sx}
        invertDefaultColor={invertDefaultColor}
      />
    </ErrorBoundary>
  );
};

export default memo(NewBookingButtonWithRelay);
