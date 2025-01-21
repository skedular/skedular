import { NewBookingButton } from '@/components/booking/addBooking';
import { getModernOrganizationLocationSetupBaseLink } from '@/components/organization';
import type { myLocationCard__query$key } from '@/queries/__generated__/myLocationCard__query.graphql';
import type { myLocationCard_addCustomerDefaultLocationMutation } from '@/queries/__generated__/myLocationCard_addCustomerDefaultLocationMutation.graphql';
import type { myLocationCard_deleteLocationMutation } from '@/queries/__generated__/myLocationCard_deleteLocationMutation.graphql';
import type { myLocationCard_LocationDetails$key } from '@/queries/__generated__/myLocationCard_LocationDetails.graphql';
import type { myLocationCard_removeCustomerDefaultLocationMutation } from '@/queries/__generated__/myLocationCard_removeCustomerDefaultLocationMutation.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import LinearProgress from '@mui/material/LinearProgress';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import {
  DefaultDialogTitle,
  LeadIconTypography,
  PushToRight,
  SmallIconTypography,
  StackColumn,
  StackRow,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
import { DeskIcon, EllipseMenuIcon, LocationIcon, NotPreferredIcon, PreferredIcon } from '@repo/shared/components/icons';
import {
  MoreActionsMenu,
  moreActionsMenuAllOptions,
  MoreActionsMenuItemType,
  MoreActionsMenuOptionType,
} from '@repo/shared/components/moreActionsMenu';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { Zones } from '@repo/shared/components/zone';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { coal, sandstone } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  rootDataRelay: myLocationCard__query$key;
  locationDetailsRelay: myLocationCard_LocationDetails$key;
  onReloadRequired: () => void;
  organizationId: string;
  connectionIds: string[];
  sharedWithTeammates: CustomerDetails[];
  availableDesksCount: number;
  availablePercentage: number;
  defaultDate: Dayjs;
};

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

const MyLocationCard = ({
  rootDataRelay,
  locationDetailsRelay,
  connectionIds,
  onReloadRequired,
  organizationId,
  sharedWithTeammates,
  availableDesksCount,
  availablePercentage,
  defaultDate,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment myLocationCard__query on Query {
        me {
          id
          defaultLocations {
            uniqueId
          }
        }
      }
    `,
    rootDataRelay,
  );

  const locationDetails = useFragment(
    graphql`
      fragment myLocationCard_LocationDetails on LocationDetails {
        id
        name
        customTags {
          uniqueId
          name
          color
        }
        zones {
          uniqueId
          name
          color
        }
        desks {
          id
        }
        physicalAddress {
          formattedAddress
        }
        hasFutureBooking
        canModify
        canDelete
        organization {
          uniqueId
        }
      }
    `,
    locationDetailsRelay,
  );

  const [commitDeleteLocation] = useMutation<myLocationCard_deleteLocationMutation>(graphql`
    mutation myLocationCard_deleteLocationMutation($connectionIds: [ID!]!, $input: DeleteLocationInput!) {
      deleteLocation(input: $input) {
        location {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultLocation] = useMutation<myLocationCard_addCustomerDefaultLocationMutation>(graphql`
    mutation myLocationCard_addCustomerDefaultLocationMutation($input: AddCustomerDefaultLocationInput!) {
      addCustomerDefaultLocation(input: $input) {
        customer {
          id
          defaultLocations {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultLocation] = useMutation<myLocationCard_removeCustomerDefaultLocationMutation>(graphql`
    mutation myLocationCard_removeCustomerDefaultLocationMutation($input: RemoveCustomerDefaultLocationInput!) {
      removeCustomerDefaultLocation(input: $input) {
        customer {
          id
          defaultLocations {
            uniqueId
          }
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);
  const isPreferred = useMemo(
    () => rootData.me?.defaultLocations.some((item) => item.uniqueId == locationDetails.id),
    [locationDetails.id, rootData.me?.defaultLocations],
  );

  let moreActionsOption: MoreActionsMenuItemType[] = [moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditLocation]];

  if (locationDetails.canDelete) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteLocation]);
  }

  const editLink = getModernOrganizationLocationSetupBaseLink(locationDetails.organization?.uniqueId!, locationDetails.id);

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
        handleRemoveTeamClicked();
        break;
    }
  };

  const handleRemoveTeamClicked = () => {
    setLocationRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingTeamClick = () => {
    setLocationRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingTeamClick = () => {
    const toastId = themedToast(<NotificationContent content={`Removing location '${locationDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteLocation({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove location '${locationDetails.name}'. Error: ${joinErrors(errors)}.`} />,
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
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting location '${locationDetails.name}' as your preferred location...`} />,
      infoNotificationOptions,
    );

    commitAddCustomerDefaultLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationId: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to set location '${locationDetails.name}' as your preferred location. Error: ${joinErrors(errors)}.`}
              />
            ),
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
          render: (
            <NotificationContent content={`Failed to set location '${locationDetails.name}' as your preferred location. Error: ${error.message}.`} />
          ),
        });
      },

      optimisticResponse: {
        addCustomerDefaultLocation: {
          customer: {
            id: rootData.me.id,
            defaultLocations: rootData.me.defaultLocations.concat([
              {
                uniqueId: locationDetails.id,
              },
            ]),
          },
        },
      },
    });
  };

  const handleRemoveAsPreferredLocationClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Removing location '${locationDetails.name}' as your preferred location...`} />,
      infoNotificationOptions,
    );

    commitRemoveCustomerDefaultLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationId: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to remove the location '${locationDetails.name}' as your preferred location. Error: ${joinErrors(errors)}.`}
              />
            ),
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
          render: (
            <NotificationContent
              content={`Failed to remove the location '${locationDetails.name}' as your preferred location. Error: ${error.message}.`}
            />
          ),
        });
      },
      optimisticResponse: {
        addCustomerDefaultLocation: {
          customer: {
            id: rootData.me.id,
            defaultLocations: rootData.me.defaultLocations.filter(({ uniqueId }) => uniqueId === locationDetails.id),
          },
        },
      },
    });
  };

  const desksCount = locationDetails.desks.length;
  const zones = locationDetails.zones.map(({ uniqueId, name, color }) => ({ id: uniqueId, name, color }));

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 600 } }}>
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={editLink}>
                <LeadIconTypography label={locationDetails.name} startElement={<LocationIcon />} sx={{ flexWrap: undefined }} invertDefaultColor />
              </Link>
              <PushToRight />
              <NewBookingButton
                onReloadRequired={onReloadRequired}
                defaultDate={defaultDate}
                organizationId={organizationId}
                defaultLocationId={locationDetails.id}
                label="Book Now"
                hideIcon
                variant="contained"
                size="small"
              />

              <Box color={paletteMode === 'dark' ? coal : sandstone}>
                {isPreferred && (
                  <IconButton onClick={handleRemoveAsPreferredLocationClicked} color="inherit">
                    <PreferredIcon />
                  </IconButton>
                )}
                {!isPreferred && (
                  <IconButton onClick={handleSetAsPreferredLocationClicked} color="inherit">
                    <NotPreferredIcon />
                  </IconButton>
                )}
              </Box>
            </StackRow>
          }
          action={
            <>
              {moreActionsOption.length > 0 && (
                <Box color={paletteMode === 'dark' ? coal : sandstone}>
                  <IconButton onClick={handleMoreActionsMenuClick} color="inherit">
                    <EllipseMenuIcon />
                  </IconButton>
                </Box>
              )}
            </>
          }
        />
        <CardContent>
          <StackRow sx={{ paddingTop: 1, paddingBottom: 1, width: '100%' }}>
            <SmallIconTypography label={`${desksCount} Desks`} sx={{ flexGrow: 0, flexShrink: 0 }} startElement={<DeskIcon />} />
            <StackColumn sx={{ paddingLeft: 40, alignItems: 'flex-end', width: '100%' }}>
              <SmallIconTypography label={`${availableDesksCount} Available Today`} />
              <LinearProgress value={availablePercentage} variant="determinate" sx={{ width: '100%' }} />
            </StackColumn>
          </StackRow>

          <Divider />

          <Zones zones={zones} sx={{ paddingTop: 1, paddingBottom: 1 }} />

          <Divider />

          <StackRow>
            <StackColumn>
              <SmallIconTypography label="Shared with teammates" />
              <StackRow>
                <AvatarGroup max={5}>
                  {sharedWithTeammates.map((item) => (
                    <CustomerAvatar key={item?.uniqueId} name={item} photo={{ url: item?.photoUrl }} size="medium" showFullName />
                  ))}
                </AvatarGroup>
              </StackRow>
            </StackColumn>

            <Divider orientation="vertical" flexItem />

            <SmallIconTypography
              label={locationDetails.physicalAddress?.formattedAddress ? locationDetails.physicalAddress?.formattedAddress : 'N/A'}
              sx={{ whiteSpace: 'pre-line' }}
            />
          </StackRow>
        </CardContent>
      </Card>

      <MoreActionsMenu
        anchorEl={moreActionsAnchorEl}
        open={moreActionsMenuOpen}
        onMenuItemClick={handleMoreActionsMenuItemClick}
        options={moreActionsOption}
      />

      <Dialog TransitionComponent={DialogTransition} open={locationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingTeamClick}>
        <DefaultDialogTitle title="Remove Location" />
        <DialogContent>
          <DialogContentText>
            {locationDetails.hasFutureBooking
              ? `Bookings are scheduled for the location "${locationDetails.name}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the location "${locationDetails.name}"?`}
          </DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemovingTeamClick}
            onSecondaryClicked={handleCancelRemovingTeamClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(MyLocationCard);
