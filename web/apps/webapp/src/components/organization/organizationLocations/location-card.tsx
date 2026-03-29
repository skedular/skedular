import { CustomerAvatar } from '@/components/avatars';
import { NewBookingButton } from '@/components/booking/addBooking';
import { CardMediaCarousel } from '@/components/carousel';
import { BodyIconTypography, DefaultDialogTitle, LeadIconTypography, PushToRight, SmallIconTypography, StackColumn, StackRow, TwoButtonsDialogActions } from '@/components/commons';
import { ContactEmails } from '@/components/contactEmail';
import { ContactPeople } from '@/components/contactPeople';
import { ContactPhones } from '@/components/contactPhone';
import { EllipseMenuIcon, FloorPlanIcon, LocationIcon, NotPreferredIcon, PreferredIcon, ResourceIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationLocationFloorPlansLink, getOrganizationLocationSetupBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { Zones } from '@/components/zone';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import { getRelayErrorMessage } from '@/libs/utils';
import type { locationCard_addCustomerPreferredLocationMutation } from '@/queries/__generated__/locationCard_addCustomerPreferredLocationMutation.graphql';
import type { locationCard_deleteLocationMutation } from '@/queries/__generated__/locationCard_deleteLocationMutation.graphql';
import type { locationCard_LocationDetails$key } from '@/queries/__generated__/locationCard_LocationDetails.graphql';
import type { locationCard_query$key } from '@/queries/__generated__/locationCard_query.graphql';
import type { locationCard_removeCustomerPreferredLocationMutation } from '@/queries/__generated__/locationCard_removeCustomerPreferredLocationMutation.graphql';
import type { locationCard_toggleContactedViaCallMutation } from '@/queries/__generated__/locationCard_toggleContactedViaCallMutation.graphql';
import type { locationCard_toggleContactedViaEmailMutation } from '@/queries/__generated__/locationCard_toggleContactedViaEmailMutation.graphql';
import type { locationCard_toggleContactedViaSmsMutation } from '@/queries/__generated__/locationCard_toggleContactedViaSmsMutation.graphql';
import type { locationCard_toggleContactedViaWhatsappMutation } from '@/queries/__generated__/locationCard_toggleContactedViaWhatsappMutation.graphql';
import '@/styles/leaflet/leaflet.css';
import AvatarGroup from '@mui/material/AvatarGroup';
import Button from '@mui/material/Button';
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
import Switch from '@mui/material/Switch';
import Tooltip from '@mui/material/Tooltip';
import Box from '@mui/system/Box';
import { Dayjs } from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

let L: typeof import('leaflet');
let MapContainer: typeof import('react-leaflet').MapContainer;
let Marker: typeof import('react-leaflet').Marker;
let TileLayer: typeof import('react-leaflet').TileLayer;

type Props = {
  rootDataRelay: locationCard_query$key;
  locationDetailsRelay: locationCard_LocationDetails$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  connectionIds: string[];
  sharedWithTeammates: CustomerDetails[];
  availableResourcesCount: number;
  availablePercentage: number;
  defaultDate: Dayjs;
};

type CustomerDetails = {
  id: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

const LocationCard = ({
  rootDataRelay,
  locationDetailsRelay,
  connectionIds,
  onReloadRequired,
  organizationCustomDomain,
  sharedWithTeammates,
  availableResourcesCount,
  availablePercentage,
  defaultDate,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationCard_query on Query {
        me {
          id
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
        customTags {
          id
          name
          color
        }
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
          latitude
          longitude
        }
        featureImages {
          thumbnail {
            url
            height
            width
          }
        }
        canModify
        canDelete
        organization {
          customDomain
        }
        extraMetadata {
          contactDetails {
            contactPeople
            contactEmails
            contactPhones
          }
        }
        uniqueClaimCode
        contactedViaEmail
        contactedViaCall
        contactedViaSms
        contactedViaWhatsapp
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

  const [commitToggleContactedViaEmail] = useMutation<locationCard_toggleContactedViaEmailMutation>(graphql`
    mutation locationCard_toggleContactedViaEmailMutation($input: ToggleContactedViaEmailInput!) {
      toggleContactedViaEmail(input: $input) {
        location {
          id
          contactedViaEmail
        }
      }
    }
  `);

  const [commitToggleContactedViaCall] = useMutation<locationCard_toggleContactedViaCallMutation>(graphql`
    mutation locationCard_toggleContactedViaCallMutation($input: ToggleContactedViaCallInput!) {
      toggleContactedViaCall(input: $input) {
        location {
          id
          contactedViaCall
        }
      }
    }
  `);

  const [commitToggleContactedViaSms] = useMutation<locationCard_toggleContactedViaSmsMutation>(graphql`
    mutation locationCard_toggleContactedViaSmsMutation($input: ToggleContactedViaSmsInput!) {
      toggleContactedViaSms(input: $input) {
        location {
          id
          contactedViaSms
        }
      }
    }
  `);

  const [commitToggleContactedViaWhatsapp] = useMutation<locationCard_toggleContactedViaWhatsappMutation>(graphql`
    mutation locationCard_toggleContactedViaWhatsappMutation($input: ToggleContactedViaWhatsappInput!) {
      toggleContactedViaWhatsapp(input: $input) {
        location {
          id
          contactedViaWhatsapp
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);
  const isPreferred = useMemo(() => rootData.me?.preferredLocations.some((item) => item.id === locationDetails.id), [locationDetails.id, rootData.me?.preferredLocations]);
  const contactedViaEmail = useMemo(() => locationDetails.contactedViaEmail, [locationDetails.contactedViaEmail]);
  const contactedViaCall = useMemo(() => locationDetails.contactedViaCall, [locationDetails.contactedViaCall]);
  const contactedViaSms = useMemo(() => locationDetails.contactedViaSms, [locationDetails.contactedViaSms]);
  const contactedViaWhatsapp = useMemo(() => locationDetails.contactedViaWhatsapp, [locationDetails.contactedViaWhatsapp]);
  const [dynamicLoadReady, setDynamicLoadReady] = useState(false);

  useEffect(() => {
    (async () => {
      // core libraries
      const leaflet = await import('leaflet');
      const rl = await import('react-leaflet');

      L = leaflet;
      MapContainer = rl.MapContainer;
      Marker = rl.Marker;
      TileLayer = rl.TileLayer;

      L.Icon.Default.mergeOptions({
        iconRetinaUrl: '/leaflet/images/marker-icon-2x.png',
        iconUrl: '/leaflet/images/marker-icon.png',
        shadowUrl: '/leaflet/images/marker-shadow.png',
      });

      setDynamicLoadReady(true);
    })();
  }, []);

  let moreActionsOption: MoreActionsMenuItemType[] = [moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditLocation]];

  if (locationDetails.canDelete) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteLocation]);
  }

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

  const handleContactedViaEmailChange = () => {
    const toastId = themedToast(<NotificationContent content={`Toggling '${locationDetails.name}' contaced via Email...`} />, infoNotificationOptions);

    commitToggleContactedViaEmail({
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
            render: <NotificationContent content={`Failed to toggle the location '${locationDetails.name}' contaced via Email. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' contacted via Email toggled.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to toggle the location '${locationDetails.name}' contaced via Email. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleContactedViaCallChange = () => {
    const toastId = themedToast(<NotificationContent content={`Toggling '${locationDetails.name}' contaced via call...`} />, infoNotificationOptions);

    commitToggleContactedViaCall({
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
            render: <NotificationContent content={`Failed to toggle the location '${locationDetails.name}' contaced via call. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' contacted via call toggled.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to toggle the location '${locationDetails.name}' contaced via call. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleContactedViaSmsChange = () => {
    const toastId = themedToast(<NotificationContent content={`Toggling '${locationDetails.name}' contaced via SMS...`} />, infoNotificationOptions);

    commitToggleContactedViaSms({
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
            render: <NotificationContent content={`Failed to toggle the location '${locationDetails.name}' contaced via SMS. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' contacted via SMS toggled.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to toggle the location '${locationDetails.name}' contaced via SMS. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleContactedViaWhatsappChange = () => {
    const toastId = themedToast(<NotificationContent content={`Toggling '${locationDetails.name}' contaced via Whatsapp...`} />, infoNotificationOptions);

    commitToggleContactedViaWhatsapp({
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
            render: <NotificationContent content={`Failed to toggle the location '${locationDetails.name}' contaced via Whatsapp. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' contacted via Whatsapp toggled.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to toggle the location '${locationDetails.name}' contaced via Whatsapp. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const resourcesCount = locationDetails.resources.totalCount;
  const zones = locationDetails.zones.map(({ id, name, color }) => ({ id, name, color }));
  const contactPeople = locationDetails.extraMetadata?.contactDetails?.contactPeople ?? [];
  const contactEmails = locationDetails.extraMetadata?.contactDetails?.contactEmails ?? [];
  const contactPhones = locationDetails.extraMetadata?.contactDetails?.contactPhones ?? [];

  if (!dynamicLoadReady) {
    return null;
  }

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 600 }, height: '100%', display: 'flex', flexDirection: 'column' }}>
        <CardMediaCarousel images={locationDetails.featureImages} placeholderImageUrl="/images/location_placeholder.webp" />
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={editLink}>
                <LeadIconTypography label={locationDetails.name} startElement={<LocationIcon excludeTooltip />} sx={{ flexWrap: undefined }} invertDefaultColor />
              </Link>
              <PushToRight />
              {locationDetails.uniqueClaimCode && <LeadIconTypography label={locationDetails.uniqueClaimCode} invertDefaultColor />}
              <Tooltip title="View floor plan and book resources">
                <Button variant="outlined" size="small" startIcon={<FloorPlanIcon />} onClick={handleViewFloorPlanClick} sx={{ textTransform: 'none', mr: 1 }}>
                  Floor Plan
                </Button>
              </Tooltip>
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
                  sx={{ textTransform: 'none' }}
                  invertDefaultColor={paletteMode === 'dark'}
                />
              )}

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
                <Box color={paletteMode === 'dark' ? coal : sandstone} sx={{ paddingTop: 0.5 }}>
                  <IconButton onClick={handleMoreActionsMenuClick} color="inherit">
                    <EllipseMenuIcon />
                  </IconButton>
                </Box>
              )}
            </>
          }
        />
        <CardContent sx={{ flexGrow: 1 }}>
          <StackRow sx={{ paddingTop: 1, paddingBottom: 1, width: '100%', flexWrap: 'nowrap' }}>
            <SmallIconTypography label={`${resourcesCount} Resources`} sx={{ flexGrow: 0, flexShrink: 0 }} startElement={<ResourceIcon />} />
            <StackColumn sx={{ paddingLeft: 40, alignItems: 'flex-end', width: '100%' }}>
              <SmallIconTypography label={`${availableResourcesCount} Available Today`} />
              <LinearProgress value={availablePercentage} variant="determinate" sx={{ width: '100%' }} />
            </StackColumn>
          </StackRow>
          <Divider />

          {locationDetails.organization?.customDomain === 'skedularpubliclocations' && (
            <>
              <StackRow>
                <BodyIconTypography label="Email" />
                <Switch defaultChecked={contactedViaEmail} onChange={handleContactedViaEmailChange} />

                <BodyIconTypography label="Call" />
                <Switch defaultChecked={contactedViaCall} onChange={handleContactedViaCallChange} />

                <BodyIconTypography label="Sms" />
                <Switch defaultChecked={contactedViaSms} onChange={handleContactedViaSmsChange} />

                <BodyIconTypography label="Whatsapp" />
                <Switch defaultChecked={contactedViaWhatsapp} onChange={handleContactedViaWhatsappChange} />
              </StackRow>
            </>
          )}

          <Zones zones={zones} sx={{ paddingTop: 1, paddingBottom: 1, flexWrap: 'nowrap' }} />
          <Divider />

          {contactPeople.length !== 0 && (
            <>
              <ContactPeople contactPeople={contactPeople} sx={{ paddingTop: 1, paddingBottom: 1, flexWrap: 'nowrap' }} />
              <Divider />
            </>
          )}

          {contactEmails.length !== 0 && (
            <>
              <ContactEmails contactEmails={contactEmails} sx={{ paddingTop: 1, paddingBottom: 1, flexWrap: 'nowrap' }} />
              <Divider />
            </>
          )}

          {contactPhones.length !== 0 && (
            <>
              <ContactPhones contactPhones={contactPhones} sx={{ paddingTop: 1, paddingBottom: 1, flexWrap: 'nowrap' }} />
              <Divider />
            </>
          )}

          <StackRow sx={{ paddingTop: 1, paddingBottom: 1, flexWrap: 'nowrap' }}>
            <StackColumn>
              <SmallIconTypography label="Shared with teammates" />
              <StackRow>
                <AvatarGroup max={5}>
                  {sharedWithTeammates.map((item) => (
                    <CustomerAvatar key={item?.id} name={item} photo={{ url: item?.photoUrl }} size="medium" showFullName />
                  ))}
                </AvatarGroup>
              </StackRow>
            </StackColumn>

            <Divider orientation="vertical" flexItem />

            <SmallIconTypography
              label={locationDetails.physicalAddress?.multilinesFormattedAddress ? locationDetails.physicalAddress?.multilinesFormattedAddress : 'N/A'}
              sx={{ whiteSpace: 'pre-line' }}
            />

            <Divider orientation="vertical" flexItem />

            {locationDetails.physicalAddress && locationDetails.physicalAddress.latitude && locationDetails.physicalAddress.longitude && (
              <Box sx={{ height: '25vh', width: '25vh' }}>
                <MapContainer
                  center={[locationDetails.physicalAddress.latitude, locationDetails.physicalAddress.longitude]}
                  zoom={13}
                  scrollWheelZoom
                  style={{ height: '100%', width: '100%' }}
                >
                  <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                  />
                  <Marker position={[locationDetails.physicalAddress.latitude, locationDetails.physicalAddress.longitude]} />
                </MapContainer>
              </Box>
            )}
          </StackRow>
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
