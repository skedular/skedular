import Button from '@mui/material/Button';
import { NewIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Dayjs } from 'dayjs';
import { memo, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { newBookingButton_rootQuery } from './__generated__/newBookingButton_rootQuery.graphql';
import NewBookingDialog from './new-booking-dialog';

type Props = {
  queryReference: PreloadedQuery<newBookingButton_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  connectionIds?: string[];
  organizationId: string;
  locationId?: string;
  defaultTeamId?: string;
  hideOrganizationControl?: boolean;
  hideLocationControl?: boolean;
  defaultDate?: Dayjs;
};

const RootQuery = graphql`
  query newBookingButton_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $locationId: String!
    $locationExists: Boolean!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $bookingPeopleNameSearchText: String
  ) {
    ...newBookingDialog_query
  }
`;

const NewBookingButton = ({
  queryReference,
  onReloadRequired,
  connectionIds,
  organizationId,
  locationId,
  defaultTeamId,
  hideOrganizationControl,
  hideLocationControl,
  defaultDate,
}: Props) => {
  const rootData = usePreloadedQuery<newBookingButton_rootQuery>(RootQuery, queryReference);
  const [isAddBookingDialogOpen, setIsAddBookingDialogOpen] = useState(false);

  const handleAddBookingClick = () => {
    setIsAddBookingDialogOpen(true);
  };

  const handleAddBookingDialogAddClick = () => {
    setIsAddBookingDialogOpen(false);

    if (onReloadRequired) {
      onReloadRequired();
    }
  };

  const handleAddBookingDialogCancelClick = () => {
    setIsAddBookingDialogOpen(false);
  };

  return (
    <>
      <Button variant="contained" startIcon={<NewIcon />} size="small" sx={{ alignSelf: 'flex-start' }} onClick={handleAddBookingClick}>
        Make a booking
      </Button>
      <NewBookingDialog
        rootDataRelay={rootData}
        connectionIds={connectionIds ?? []}
        isDialogOpen={isAddBookingDialogOpen}
        onAddClicked={handleAddBookingDialogAddClick}
        onCancelClicked={handleAddBookingDialogCancelClick}
        organizationId={organizationId}
        locationId={locationId}
        defaultTeamId={defaultTeamId}
        hideOrganizationControl={hideOrganizationControl}
        hideLocationControl={hideLocationControl}
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
  locationId?: string;
  defaultTeamId?: string;
  hideOrganizationControl?: boolean;
  hideLocationControl?: boolean;
  defaultDate?: Dayjs;
};

const NewBookingButtonWithRelay = ({
  onReloadRequired,
  connectionIds,
  organizationId,
  locationId,
  defaultTeamId,
  hideOrganizationControl,
  hideLocationControl,
  defaultDate,
}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<newBookingButton_rootQuery>(RootQuery);

  useEffect(() => {
    const date = startOfDay().toISOString();

    loadQuery(
      {
        organizationId: organizationId ?? '',
        organizationExists: !!organizationId,
        locationId: locationId ?? '',
        locationExists: !!locationId,
        deskIdsToIncludeToGetAvailableDesks: [],
        bookingDetailsSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        dateToGetAvailableDesks: date,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationId, locationId]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoNewBookingButton
        queryReference={queryReference}
        connectionIds={connectionIds}
        organizationId={organizationId}
        locationId={locationId}
        defaultTeamId={defaultTeamId}
        hideOrganizationControl={hideOrganizationControl}
        hideLocationControl={hideLocationControl}
        onReloadRequired={onReloadRequired}
        defaultDate={defaultDate}
      />
    </ErrorBoundary>
  );
};

export default memo(NewBookingButtonWithRelay);
