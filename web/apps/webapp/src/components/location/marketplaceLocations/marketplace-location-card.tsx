import { CardMediaCarousel } from '@/components/carousel';
import { LeadIconTypography, SmallIconTypography, StackRow, TwoButtonsDialogActions } from '@/components/commons';
import { AreaIcon, CloseIcon, FavouriteIcon, NotFavouriteIcon, PersonIcon, ShareIcon } from '@/components/icons';
import { getMarketplaceLocationLink, getSignInLink } from '@/components/links';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { marketplaceLocationCard_addCustomerFavouriteLocationMutation } from '@/queries/__generated__/marketplaceLocationCard_addCustomerFavouriteLocationMutation.graphql';
import type { marketplaceLocationCard_LocationDetails$key } from '@/queries/__generated__/marketplaceLocationCard_LocationDetails.graphql';
import type { marketplaceLocationCard_query$key } from '@/queries/__generated__/marketplaceLocationCard_query.graphql';
import type { marketplaceLocationCard_removeCustomerFavouriteLocationMutation } from '@/queries/__generated__/marketplaceLocationCard_removeCustomerFavouriteLocationMutation.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import IconButton from '@mui/material/IconButton';
import Tooltip from '@mui/material/Tooltip';
import Box from '@mui/system/Box';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  rootDataRelay: marketplaceLocationCard_query$key;
  locationDetailsRelay: marketplaceLocationCard_LocationDetails$key;
  onReloadRequired: () => void;
  onClose?: () => void;
};

const MarketplaceLocationCard = ({ rootDataRelay, locationDetailsRelay, onClose }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment marketplaceLocationCard_query on Query {
        me @include(if: $userSignedIn) {
          favouriteLocations {
            id
          }
        }
      }
    `,
    rootDataRelay,
  );

  const locationDetails = useFragment(
    graphql`
      fragment marketplaceLocationCard_LocationDetails on LocationDetails {
        id
        name
        extraMetadata {
          areaRange {
            fromInSqm
            toInSqm
          }
          peopleCapacity {
            from
            to
          }
        }
        physicalAddress {
          multilinesFormattedAddress
        }
        featureImages {
          thumbnail {
            url
            height
            width
          }
        }
      }
    `,
    locationDetailsRelay,
  );

  const [commitAddCustomerFavouriteLocation] = useMutation<marketplaceLocationCard_addCustomerFavouriteLocationMutation>(graphql`
    mutation marketplaceLocationCard_addCustomerFavouriteLocationMutation($input: AddCustomerFavouriteLocationInput!) {
      addCustomerFavouriteLocation(input: $input) {
        customer {
          id
          favouriteLocations {
            id
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerFavouriteLocation] = useMutation<marketplaceLocationCard_removeCustomerFavouriteLocationMutation>(graphql`
    mutation marketplaceLocationCard_removeCustomerFavouriteLocationMutation($input: RemoveCustomerFavouriteLocationInput!) {
      removeCustomerFavouriteLocation(input: $input) {
        customer {
          id
          favouriteLocations {
            id
          }
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const { user, loading } = useAuth();
  const isFavoured = useMemo(() => rootData.me?.favouriteLocations.some((item) => item.id === locationDetails.id), [locationDetails.id, rootData.me?.favouriteLocations]);
  const shareUrl = useMemo(
    () => `${typeof window !== 'undefined' ? window.location.origin : ''}${getMarketplaceLocationLink(integratedPlatrform, locationDetails.id)}`,
    [integratedPlatrform, locationDetails.id],
  );
  const canShare = typeof navigator !== 'undefined' && typeof navigator.canShare === 'function' && navigator.canShare({ url: shareUrl });

  const capacity = useMemo(() => {
    if (!locationDetails.extraMetadata?.peopleCapacity) {
      return '';
    }

    if (locationDetails.extraMetadata?.peopleCapacity.from === locationDetails.extraMetadata?.peopleCapacity.to) {
      return `${locationDetails.extraMetadata?.peopleCapacity.from} People`;
    } else {
      return `${locationDetails.extraMetadata?.peopleCapacity.from} - ${locationDetails.extraMetadata?.peopleCapacity.to} People`;
    }
  }, [locationDetails.extraMetadata?.peopleCapacity]);

  const areaSize = useMemo(() => {
    if (!locationDetails.extraMetadata?.areaRange) {
      return '';
    }

    if (locationDetails.extraMetadata?.areaRange.fromInSqm === locationDetails.extraMetadata?.areaRange.toInSqm) {
      return `${locationDetails.extraMetadata?.areaRange.fromInSqm} m2`;
    } else {
      return `${locationDetails.extraMetadata?.areaRange.fromInSqm} - ${locationDetails.extraMetadata?.areaRange.toInSqm} m2`;
    }
  }, [locationDetails.extraMetadata?.areaRange]);

  const [signInDialogOpen, setSignInDialogOpen] = useState(false);

  const handleShareClick = async (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    event.stopPropagation();

    try {
      if (navigator.share) {
        await navigator.share({ title: locationDetails.name, url: shareUrl });
      } else if (navigator.clipboard?.writeText) {
        await navigator.clipboard.writeText(shareUrl);
      }
    } catch {
      // user cancelled or share failed; ignore
    }
  };

  const handleCloseClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    event.stopPropagation();
    onClose?.();
  };

  const handleSetAsFavouriteLocationClicked = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    event.stopPropagation();

    if (!loading && !user) {
      setSignInDialogOpen(true);
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting location '${locationDetails.name}' as your favourite location...`} />, infoNotificationOptions);

    commitAddCustomerFavouriteLocation({
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
            render: <NotificationContent content={`Failed to set location '${locationDetails.name}' as your favourite location. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' has been set as the favourite location.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set location '${locationDetails.name}' as your favourite location. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsFavouriteLocationClicked = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    event.stopPropagation();

    if (!loading && !user) {
      setSignInDialogOpen(true);
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing location '${locationDetails.name}' as your favourite location...`} />, infoNotificationOptions);

    commitRemoveCustomerFavouriteLocation({
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
            render: <NotificationContent content={`Failed to remove the location '${locationDetails.name}' as your favourite location. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' has been removed as your favourite location.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the location '${locationDetails.name}' as your favourite location. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleCancelSignInClick = () => {
    setSignInDialogOpen(false);
  };

  const handleSignInClick = () => {
    router.push(getSignInLink());
  };

  return (
    <>
      <Card
        sx={{ width: '100%', textDecoration: 'none', display: 'flex', flexDirection: 'column' }}
        component={NextLink}
        href={getMarketplaceLocationLink(integratedPlatrform, locationDetails.id)}
      >
        <CardMediaCarousel images={locationDetails.featureImages} />
        <CardHeader
          sx={{ height: 60 }}
          title={
            <StackRow>
              {capacity && <SmallIconTypography label={capacity} startElement={<PersonIcon fontSize="small" />} invertDefaultColor />}
              {areaSize && <SmallIconTypography label={areaSize} startElement={<AreaIcon fontSize="small" />} invertDefaultColor />}
            </StackRow>
          }
          action={
            <StackRow>
              <Box color={paletteMode === 'dark' ? coal : sandstone}>
                {isFavoured && (
                  <Tooltip title="Remove as Favourite">
                    <IconButton onClick={handleRemoveAsFavouriteLocationClicked} color="inherit">
                      <FavouriteIcon fontSize="medium" />
                    </IconButton>
                  </Tooltip>
                )}
                {!isFavoured && (
                  <Tooltip title="Set as Favourite">
                    <IconButton onClick={handleSetAsFavouriteLocationClicked} color="inherit">
                      <NotFavouriteIcon fontSize="medium" />
                    </IconButton>
                  </Tooltip>
                )}
              </Box>
              {canShare && (
                <Tooltip title="Share">
                  <IconButton onClick={handleShareClick} sx={{ color: paletteMode === 'dark' ? coal : sandstone }}>
                    <ShareIcon fontSize="medium" />
                  </IconButton>
                </Tooltip>
              )}
              {onClose && (
                <Tooltip title="Close">
                  <IconButton onClick={handleCloseClick} sx={{ color: paletteMode === 'dark' ? coal : sandstone }}>
                    <CloseIcon fontSize="medium" />
                  </IconButton>
                </Tooltip>
              )}
            </StackRow>
          }
        />
        <CardContent sx={{ flexGrow: 1 }}>
          <LeadIconTypography label={locationDetails.name} />
          {locationDetails.physicalAddress?.multilinesFormattedAddress && <SmallIconTypography label={locationDetails.physicalAddress?.multilinesFormattedAddress} />}
        </CardContent>
      </Card>

      <Dialog open={signInDialogOpen} onClose={() => setSignInDialogOpen(false)}>
        <DialogTitle>Sign in to save this spot</DialogTitle>
        <DialogContent>Sign in to add this location to your favourites and find it instantly next time.</DialogContent>
        <DialogActions>
          <TwoButtonsDialogActions onPrimaryClicked={handleSignInClick} onSecondaryClicked={handleCancelSignInClick} primaryLabel="Sign In" secondaryLabel="Cancel" />
        </DialogActions>
      </Dialog>
    </>
  );
};

export default memo(MarketplaceLocationCard);
