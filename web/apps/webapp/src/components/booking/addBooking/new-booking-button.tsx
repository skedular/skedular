import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@/components/commons';
import { NewIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { coal } from '@/libs/theme';
import { startOfDay } from '@/libs/utils';
import type { newBookingButton_rootQuery } from '@/queries/__generated__/newBookingButton_rootQuery.graphql';
import Button from '@mui/material/Button';
import type { SxProps, Theme } from '@mui/system';
import { Dayjs } from 'dayjs';
import { memo, Suspense, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import NewBookingDialog from './new-booking-dialog';

type Props = {
  queryReference?: PreloadedQuery<newBookingButton_rootQuery, Record<string, unknown>> | null;
  onReloadRequired?: () => void;
  connectionIds?: string[];
  organizationCustomDomain: string;
  defaultLocationId?: string;
  defaultDate?: Dayjs;
  defaultResourceIds?: string[];
  isInitiallyOpen?: boolean;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  sx?: SxProps<Theme>;
  invertDefaultColor?: boolean;
  onOpenRequested?: () => void;
};

const RootQuery = graphql`
  query newBookingButton_rootQuery(
    $organizationCustomDomain: String!
    $peopleNameSearchText: String
    $locationId: String!
    $dateFromToGetAvailableResources: DateTime!
    $dateUntilToGetAvailableResources: DateTime!
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $customerId: String!
    $customerExists: Boolean!
    $teamsSortingValues: [TeamOrderInput!]
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    ...newBookingDialog_query
    ...newBookingDialog_organizationMembers_query
    ...newBookingDialog_customerTeams_query
    ...newBookingDialog_availableResources_query
  }
`;

type NewBookingDialogWithQueryProps = {
  queryReference: PreloadedQuery<newBookingButton_rootQuery, Record<string, unknown>>;
  connectionIds?: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
  organizationCustomDomain: string;
  defaultLocationId?: string;
  defaultDate?: Dayjs;
  defaultResourceIds?: string[];
};

const NewBookingDialogWithQuery = ({
  queryReference,
  connectionIds,
  isDialogOpen,
  onAddClicked,
  onCancel,
  organizationCustomDomain,
  defaultLocationId,
  defaultDate,
  defaultResourceIds,
}: NewBookingDialogWithQueryProps) => {
  const rootData = usePreloadedQuery<newBookingButton_rootQuery>(RootQuery, queryReference);

  return (
    <NewBookingDialog
      rootDataRelay={rootData}
      rootDataTeamsRelay={rootData}
      rootDataOrganizationMembersRelay={rootData}
      rootDataAvailableResourcesRelay={rootData}
      connectionIds={connectionIds ?? []}
      isDialogOpen={isDialogOpen}
      onAddClicked={onAddClicked}
      onCancel={onCancel}
      organizationCustomDomain={organizationCustomDomain}
      defaultLocationId={defaultLocationId}
      defaultDate={defaultDate}
      defaultResourceIds={defaultResourceIds}
    />
  );
};

const NewBookingButton = ({
  queryReference,
  onReloadRequired,
  connectionIds,
  organizationCustomDomain,
  defaultLocationId,
  defaultDate,
  defaultResourceIds,
  isInitiallyOpen = false,
  fullWidth,
  label,
  hideIcon,
  variant,
  size,
  sx,
  invertDefaultColor,
  onOpenRequested,
}: Props) => {
  const [isDialogOpen, setIsDialogOpen] = useState(isInitiallyOpen);

  useEffect(() => {
    if (!isInitiallyOpen) {
      return;
    }

    onOpenRequested?.();

    queueMicrotask(() => {
      setIsDialogOpen(true);
    });
  }, [isInitiallyOpen, onOpenRequested]);

  const handleButtonClicked = () => {
    onOpenRequested?.();
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
      {isDialogOpen && !queryReference && <Loading />}
      {isDialogOpen && queryReference && (
        <Suspense fallback={<Loading />}>
          <NewBookingDialogWithQuery
            queryReference={queryReference}
            connectionIds={connectionIds}
            isDialogOpen={isDialogOpen}
            onAddClicked={handleAddClicked}
            onCancel={handleCancelClicked}
            organizationCustomDomain={organizationCustomDomain}
            defaultLocationId={defaultLocationId}
            defaultDate={defaultDate}
            defaultResourceIds={defaultResourceIds}
          />
        </Suspense>
      )}
    </>
  );
};

const MemoNewBookingButton = memo(NewBookingButton);

type RelayProps = {
  onReloadRequired?: () => void;
  connectionIds?: string[];
  organizationCustomDomain: string;
  defaultLocationId?: string;
  defaultDate?: Dayjs;
  defaultResourceIds?: string[];
  isInitiallyOpen?: boolean;
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
  organizationCustomDomain,
  defaultLocationId,
  defaultDate,
  defaultResourceIds,
  isInitiallyOpen,
  fullWidth,
  label,
  hideIcon,
  variant,
  size,
  sx,
  invertDefaultColor,
}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<newBookingButton_rootQuery>(RootQuery);

  const loadNewBookingQuery = useCallback(() => {
    const date = startOfDay();
    const startDate = date.toISOString();
    const endDate = date.add(1, 'day').toISOString();

    loadQuery(
      {
        organizationCustomDomain,
        locationId: defaultLocationId ?? '',
        dateFromToGetAvailableResources: startDate,
        dateUntilToGetAvailableResources: endDate,
        customerId: '',
        customerExists: false,
        teamsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        locationsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        organizationMembersSortingValues: [
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
  }, [loadQuery, organizationCustomDomain, defaultLocationId]);

  useEffect(() => {
    if (!isInitiallyOpen) {
      return;
    }

    loadNewBookingQuery();
  }, [isInitiallyOpen, loadNewBookingQuery]);

  const handleOpenRequested = () => {
    if (queryReference) {
      return;
    }

    loadNewBookingQuery();
  };

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoNewBookingButton
        queryReference={queryReference}
        connectionIds={connectionIds}
        organizationCustomDomain={organizationCustomDomain}
        defaultLocationId={defaultLocationId}
        onReloadRequired={onReloadRequired}
        defaultDate={defaultDate}
        defaultResourceIds={defaultResourceIds}
        isInitiallyOpen={isInitiallyOpen}
        fullWidth={fullWidth}
        label={label}
        hideIcon={hideIcon}
        variant={variant}
        size={size}
        sx={sx}
        invertDefaultColor={invertDefaultColor}
        onOpenRequested={handleOpenRequested}
      />
    </ErrorBoundary>
  );
};

export default memo(NewBookingButtonWithRelay);
