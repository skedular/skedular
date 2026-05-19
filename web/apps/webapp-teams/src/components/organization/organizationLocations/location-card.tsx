import { NewBookingButton } from '@/components/booking/addBooking';
import {
  BodyIconTypography,
  DefaultDialogTitle,
  LeadIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
  SubtitleIconTypography,
  TwoButtonsDialogActions,
} from '@skedular/ui';
import { EllipseMenuIcon, FloorPlanIcon, LocationIcon, ResourceIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationLocationFloorPlansLink, getOrganizationLocationSetupBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { Zones } from '@/components/zone';
import { useIntegratedPlatrform } from '@skedular/shared';
import { getRelayErrorMessage } from '@skedular/shared';
import type { locationCard_addCustomerPreferredLocationMutation } from '@/queries/__generated__/locationCard_addCustomerPreferredLocationMutation.graphql';
import type { locationCard_deleteLocationMutation } from '@/queries/__generated__/locationCard_deleteLocationMutation.graphql';
import type { locationCard_LocationDetails$key } from '@/queries/__generated__/locationCard_LocationDetails.graphql';
import type { locationCard_query$key } from '@/queries/__generated__/locationCard_query.graphql';
import type { locationCard_removeCustomerPreferredLocationMutation } from '@/queries/__generated__/locationCard_removeCustomerPreferredLocationMutation.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import LinearProgress from '@mui/material/LinearProgress';
import Link from '@mui/material/Link';
import Tooltip from '@mui/material/Tooltip';
import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';
import { Dayjs } from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  rootDataRelay: locationCard_query$key;
  locationDetailsRelay: locationCard_LocationDetails$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  connectionIds: string[];
  availableResourcesCount: number;
  availablePercentage: number;
  defaultDate: Dayjs;
};

const LocationCard = ({
  rootDataRelay,
  locationDetailsRelay,
  connectionIds,
  onReloadRequired,
  organizationCustomDomain,
  availableResourcesCount,
  availablePercentage,
  defaultDate,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationCard_query on Query {
        me {
          preferredLocations {
            id
          }
        }
      }
    `,
    rootDataRelay,
  );

  const locationDetails = useFragment(
    graphql`
      fragment locationCard_LocationDetails on LocationDetails {
        id
        name
        zones {
          id
          name
          color
        }
        resources {
          totalCount
        }
        physicalAddress {
          multilinesFormattedAddress
        }
        featureImages {
          original {
            url
          }
          thumbnail {
            url
          }
        }
        floorPlanCount
        canDelete
        organization {
          customDomain
        }
        uniqueClaimCode
      }
    `,
    locationDetailsRelay,
  );

  const [commitDeleteLocation] = useMutation<locationCard_deleteLocationMutation>(graphql`
    mutation locationCard_deleteLocationMutation($connectionIds: [ID!]!, $input: DeleteLocationInput!) {
      deleteLocation(input: $input) {
        location {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerPreferredLocation] = useMutation<locationCard_addCustomerPreferredLocationMutation>(graphql`
    mutation locationCard_addCustomerPreferredLocationMutation($input: AddCustomerPreferredLocationInput!) {
      addCustomerPreferredLocation(input: $input) {
        customer {
          id
          preferredLocations {
            id
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerPreferredLocation] = useMutation<locationCard_removeCustomerPreferredLocationMutation>(graphql`
    mutation locationCard_removeCustomerPreferredLocationMutation($input: RemoveCustomerPreferredLocationInput!) {
      removeCustomerPreferredLocation(input: $input) {
        customer {
          id
          preferredLocations {
            id
          }
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const themedToast = toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);
  const isPreferred = useMemo(() => rootData.me?.preferredLocations.some((item) => item.id === locationDetails.id), [locationDetails.id, rootData.me?.preferredLocations]);

  let moreActionsOption: MoreActionsMenuItemType[] = [moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditLocation]];

  if (locationDetails.canDelete) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteLocation]);
  }

  moreActionsOption = moreActionsOption.concat(
    isPreferred ? moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveAsPreferredLocation] : moreActionsMenuAllOptions[MoreActionsMenuOptionType.SetAsPreferredLocation],
  );

  moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.ViewLocationBookings]);

  const editLink = getOrganizationLocationSetupBaseLink(integratedPlatrform, locationDetails.organization!.customDomain!, locationDetails.id);

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditLocation:
        router.push(editLink);
        break;

      case MoreActionsMenuOptionType.DeleteLocation:
        handleRemoveLocationClicked();
        break;

      case MoreActionsMenuOptionType.ViewLocationBookings:
        router.push(getOrganizationBookingsBaseLink(integratedPlatrform, locationDetails.organization!.customDomain!, { locationId: locationDetails.id }));
        break;

      case MoreActionsMenuOptionType.SetAsPreferredLocation:
        handleSetAsPreferredLocationClicked();
        break;

      case MoreActionsMenuOptionType.RemoveAsPreferredLocation:
        handleRemoveAsPreferredLocationClicked();
        break;
    }
  };

  const handleRemoveLocationClicked = () => {
    setLocationRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingLocationClick = () => {
    setLocationRemoveConfirmationDialogOpen(false);
  };

  const handleViewFloorPlanClick = () => {
    router.push(getOrganizationLocationFloorPlansLink(integratedPlatrform, organizationCustomDomain, locationDetails.id));
  };

  const handleConfirmRemovingLocationClick = () => {
    const toastId = themedToast(<NotificationContent content={`Removing location '${locationDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteLocation({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: uuid(),
          id: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove location '${locationDetails.name}'. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' has been successfully removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove location '${locationDetails.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSetAsPreferredLocationClicked = () => {
    const toastId = themedToast(<NotificationContent content={`Setting location '${locationDetails.name}' as your preferred location...`} />, infoNotificationOptions);

    commitAddCustomerPreferredLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
          locationId: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set location '${locationDetails.name}' as your preferred location. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' has been set as the preferred location.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set location '${locationDetails.name}' as your preferred location. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredLocationClicked = () => {
    const toastId = themedToast(<NotificationContent content={`Removing location '${locationDetails.name}' as your preferred location...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
          locationId: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the location '${locationDetails.name}' as your preferred location. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' has been removed as your preferred location.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the location '${locationDetails.name}' as your preferred location. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const resourcesCount = locationDetails.resources.totalCount;
  const hasFloorPlans = locationDetails.floorPlanCount > 0;
  const zones = locationDetails.zones.map(({ id, name, color }) => ({ id, name, color }));
  const primaryFeatureImage = locationDetails.featureImages[0]?.thumbnail?.url ?? locationDetails.featureImages[0]?.original?.url;
  const safeAvailablePercentage = Number.isFinite(availablePercentage) ? Math.max(0, Math.min(100, availablePercentage)) : 0;
  const availableTodayLabel = `${availableResourcesCount} resource${availableResourcesCount === 1 ? '' : 's'} available today`;
  const fullAddressLabel = locationDetails.physicalAddress?.multilinesFormattedAddress?.trim() ?? 'No address configured';
  const compactAddressLabel =
    fullAddressLabel === 'No address configured'
      ? fullAddressLabel
      : fullAddressLabel
          .split('\n')
          .map((line) => line.trim())
          .filter(Boolean)
          .join(', ');

  const sectionSx: SxProps<Theme> = {
    border: 1,
    borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
    borderRadius: 3,
    p: 1.25,
    backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.02)' : 'transparent'),
  };

  return (
    <>
      <Card
        sx={{
          width: '100%',
          height: '100%',
          borderRadius: 4,
          border: 1,
          borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
          boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 10px 28px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
          backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.92)' : theme.palette.background.paper),
        }}
      >
        <CardContent sx={{ p: 2, height: '100%' }}>
          <StackColumn spacing={2} sx={{ height: '100%' }}>
            <StackRow sx={{ alignItems: 'center', flexWrap: 'nowrap', gap: 2, minHeight: 56 }}>
              <Box
                sx={{
                  width: 56,
                  height: 56,
                  borderRadius: 3,
                  border: 1,
                  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  overflow: 'hidden',
                  flexShrink: 0,
                  bgcolor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.04)' : theme.palette.action.hover),
                }}
              >
                {primaryFeatureImage ? (
                  <>
                    {/* eslint-disable-next-line @next/next/no-img-element */}
                    <img src={primaryFeatureImage} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                  </>
                ) : (
                  <LocationIcon excludeTooltip />
                )}
              </Box>

              <StackColumn spacing={0.75} sx={{ minWidth: 0, flexGrow: 1, justifyContent: 'center' }}>
                <Tooltip title={locationDetails.name}>
                  <Link component={NextLink} href={editLink} underline="none" color="inherit" sx={{ display: 'block', minWidth: 0 }}>
                    <LeadIconTypography label={locationDetails.name} noWrap sx={{ minWidth: 0 }} />
                  </Link>
                </Tooltip>
              </StackColumn>

              <StackRow sx={{ gap: 0.5, flexWrap: 'nowrap' }}>
                {hasFloorPlans && (
                  <Tooltip title="View floor plan">
                    <IconButton onClick={handleViewFloorPlanClick} aria-label="View floor plan">
                      <FloorPlanIcon />
                    </IconButton>
                  </Tooltip>
                )}

                {moreActionsOption.length > 0 && (
                  <IconButton onClick={handleMoreActionsMenuClick} aria-label="Open location actions">
                    <EllipseMenuIcon />
                  </IconButton>
                )}
              </StackRow>
            </StackRow>

            <Divider />
            <StackColumn spacing={1.25} sx={{ flexGrow: 1 }}>
              <Box sx={sectionSx}>
                <StackColumn spacing={0.75}>
                  <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', gap: 1 }}>
                    <SubtitleIconTypography label="Availability" />
                    <SmallIconTypography label={`${availableResourcesCount}/${resourcesCount || 0} open`} />
                  </StackRow>
                  <LinearProgress value={safeAvailablePercentage} variant="determinate" sx={{ width: '100%', height: 8, borderRadius: 999, bgcolor: 'action.hover' }} />
                  <BodyIconTypography label={availableTodayLabel} startElement={<ResourceIcon />} />
                  {locationDetails.uniqueClaimCode ? <SmallIconTypography label={`Claim code ${locationDetails.uniqueClaimCode}`} /> : null}
                </StackColumn>
              </Box>

              <Box sx={sectionSx}>
                <StackColumn spacing={0.75}>
                  <SubtitleIconTypography label="Address" />
                  <Tooltip title={<Box sx={{ whiteSpace: 'pre-line' }}>{fullAddressLabel}</Box>}>
                    <Box sx={{ minWidth: 0 }}>
                      <SmallIconTypography
                        label={compactAddressLabel}
                        sx={{
                          minWidth: 0,
                          overflow: 'hidden',
                          textOverflow: 'ellipsis',
                          whiteSpace: 'nowrap',
                        }}
                      />
                    </Box>
                  </Tooltip>
                </StackColumn>
              </Box>

              {zones.length > 0 && (
                <Box sx={sectionSx}>
                  <StackColumn spacing={0.75}>
                    <SubtitleIconTypography label="Zones" />
                    <Zones zones={zones} hideIcon hideNAText={false} sx={{ flexWrap: 'wrap' }} />
                  </StackColumn>
                </Box>
              )}
            </StackColumn>

            <StackRow
              sx={{
                gap: 1,
                flexWrap: 'wrap',
                mt: 'auto',
                pt: 1.5,
                borderTop: 1,
                borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
                justifyContent: 'flex-end',
              }}
            >
              {locationDetails.organization?.customDomain !== 'skedularpubliclocations' && (
                <NewBookingButton
                  onReloadRequired={onReloadRequired}
                  defaultDate={defaultDate}
                  organizationCustomDomain={organizationCustomDomain}
                  defaultLocationId={locationDetails.id}
                  label="Book Now"
                  hideIcon
                  variant="contained"
                  size="small"
                  invertDefaultColor
                  sx={{
                    textTransform: 'none',
                    minWidth: 132,
                    backgroundColor: 'primary.main',
                    borderColor: 'primary.main',
                    color: 'primary.contrastText',
                    '&:hover': {
                      backgroundColor: 'primary.dark',
                      borderColor: 'primary.dark',
                    },
                  }}
                />
              )}
            </StackRow>
          </StackColumn>
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />

      <Dialog slots={{ transition: DialogTransition }} open={locationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingLocationClick}>
        <DefaultDialogTitle title="Remove Location" />
        <DialogContent sx={{ marginTop: 2 }}>
          <DialogContentText>{`Are you sure you want to remove the location "${locationDetails.name}"?`}</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemovingLocationClick}
            onSecondaryClicked={handleCancelRemovingLocationClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(LocationCard);
