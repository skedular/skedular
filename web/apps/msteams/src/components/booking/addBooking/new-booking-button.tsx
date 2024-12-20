import Button from '@mui/material/Button';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@repo/shared/components/commons';
import { NewIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { coal, emerald, sandstone } from '@repo/shared/libs/theme';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Dayjs } from 'dayjs';
import { memo, useContext, useEffect, useState } from 'react';
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
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  invertDefaultColor?: boolean;
};

const RootQuery = graphql`
  query newBookingButton_rootQuery(
    $organizationId: String!
    $nullableOrganizationId: String
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
  fullWidth,
  label,
  hideIcon,
  variant,
  size,
  invertDefaultColor,
}: Props) => {
  const rootData = usePreloadedQuery<newBookingButton_rootQuery>(RootQuery, queryReference);
  const paletteMode = useContext(PaletteModeContext);
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
      <Button
        variant={variant ?? 'text'}
        onClick={handleButtonClicked}
        fullWidth={fullWidth}
        sx={{ borderRadius: 4, backgroundColor: invertDefaultColor ? (paletteMode === 'dark' ? coal : sandstone) : 'inherit' }}
      >
        {size === 'small' && (
          <SmallIconTypography
            label={label ?? 'Add Booking'}
            endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} sx={{ color: emerald }} />}
          />
        )}
        {size === 'medium' && (
          <BodyIconTypography
            label={label ?? 'Add Booking'}
            endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} sx={{ color: emerald }} />}
          />
        )}
        {(size === 'large' || !size) && (
          <LeadIconTypography
            label={label ?? 'Add Booking'}
            endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} sx={{ color: emerald }} />}
          />
        )}
      </Button>
      <NewBookingDialog
        rootDataRelay={rootData}
        connectionIds={connectionIds ?? []}
        isDialogOpen={isDialogOpen}
        onAddClicked={handleAddClicked}
        onCancelClicked={handleCancelClicked}
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
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  invertDefaultColor?: boolean;
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
  fullWidth,
  label,
  hideIcon,
  variant,
  size,
  invertDefaultColor,
}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<newBookingButton_rootQuery>(RootQuery);

  useEffect(() => {
    const date = startOfDay().toISOString();

    loadQuery(
      {
        organizationId,
        nullableOrganizationId: organizationId,
        locationId: locationId ?? '',
        locationExists: !!locationId,
        deskIdsToIncludeToGetAvailableDesks: [],
        bookingDetailsSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
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
        fullWidth={fullWidth}
        label={label}
        hideIcon={hideIcon}
        variant={variant}
        size={size}
        invertDefaultColor={invertDefaultColor}
      />
    </ErrorBoundary>
  );
};

export default memo(NewBookingButtonWithRelay);
