import { PaletteModeContext, getRelayErrorMessage, useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, TwoButtonsDialogActions } from '@skedular/ui';
import { AreaIcon, CloseIcon, FavouriteIcon, LocationIcon, NotFavouriteIcon, PersonIcon, ShareIcon } from '@/components/icons';
import { getMarketplaceLocationLink, getSignInLink } from '@/components/links';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';

import { emerald } from '@skedular/ui';

import type { marketplaceLocationCard_addCustomerFavouriteLocationMutation } from '@/queries/__generated__/marketplaceLocationCard_addCustomerFavouriteLocationMutation.graphql';
import type { marketplaceLocationCard_LocationDetails$key } from '@/queries/__generated__/marketplaceLocationCard_LocationDetails.graphql';
import type { marketplaceLocationCard_query$key } from '@/queries/__generated__/marketplaceLocationCard_query.graphql';
import type { marketplaceLocationCard_removeCustomerFavouriteLocationMutation } from '@/queries/__generated__/marketplaceLocationCard_removeCustomerFavouriteLocationMutation.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import IconButton from '@mui/material/IconButton';
import Tooltip from '@mui/material/Tooltip';
import type { SxProps, Theme } from '@mui/system';
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

const cardSx: SxProps<Theme> = {
  width: '100%',
  height: '100%',
  textDecoration: 'none',
  color: 'text.primary',
  textAlign: 'left',
  display: 'flex',
  flexDirection: 'column',
  borderRadius: 4,
  overflow: 'hidden',
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => theme.palette.background.paper,
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 10px 28px rgba(15, 23, 42, 0.08)' : '0 2px 12px rgba(0, 0, 0, 0.32)'),
  transition: 'transform 120ms ease, box-shadow 120ms ease, border-color 120ms ease',
  '&:hover': {
    transform: 'translateY(-1px)',
    borderColor: emerald,
    boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 16px 36px rgba(15, 23, 42, 0.12)' : '0 6px 18px rgba(0, 0, 0, 0.36)'),
  },
  '&:visited': {
    color: 'text.primary',
  },
};

const mediaSx: SxProps<Theme> = {
  position: 'relative',
  minHeight: 132,
  borderBottom: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  background: 'linear-gradient(135deg, rgba(16, 185, 129, 0.12), rgba(15, 23, 42, 0.08))',
};

const detailsPanelSx: SxProps<Theme> = {
  borderRadius: 3,
  px: 1.25,
  py: 1,
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.03)' : theme.palette.action.hover),
};

const headerIconButtonSx: SxProps<Theme> = {
  width: 32,
  height: 32,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => theme.palette.background.paper,
  color: 'text.primary',
  '&:hover': {
    backgroundColor: 'action.hover',
  },
};

const imageOverlayIconButtonSx: SxProps<Theme> = {
  width: 32,
  height: 32,
  backgroundColor: 'rgba(255,255,255,0.88)',
  color: 'text.primary',
  boxShadow: '0 8px 20px rgba(15, 23, 42, 0.16)',
  '&:hover': {
    backgroundColor: 'rgba(255,255,255,0.98)',
  },
};

const closeOverlayIconButtonSx: SxProps<Theme> = {
  ...imageOverlayIconButtonSx,
  backgroundColor: 'rgba(255,255,255,0.92)',
  color: 'rgba(15, 23, 42, 0.92)',
  '& .MuiSvgIcon-root': {
    color: 'rgba(15, 23, 42, 0.92)',
    opacity: 1,
  },
  '&:hover': {
    backgroundColor: '#ffffff',
  },
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

  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const { user, loading } = useAuth();
  const isFavoured = useMemo(() => rootData.me?.favouriteLocations.some((item) => item.id === locationDetails.id), [locationDetails.id, rootData.me?.favouriteLocations]);
  const shareUrl = useMemo(
    () => `${typeof window !== 'undefined' ? window.location.origin : ''}${getMarketplaceLocationLink(integratedPlatform, locationDetails.id)}`,
    [integratedPlatform, locationDetails.id],
  );
  const imageUrl = useMemo(() => locationDetails.featureImages.find((item) => !!item.thumbnail?.url)?.thumbnail?.url ?? null, [locationDetails.featureImages]);
  const addressLabel = useMemo(
    () => locationDetails.physicalAddress?.multilinesFormattedAddress?.replace(/\s*\n\s*/g, ', ').trim() ?? '',
    [locationDetails.physicalAddress?.multilinesFormattedAddress],
  );
  const canShare = typeof navigator !== 'undefined' && typeof navigator.canShare === 'function' && navigator.canShare({ url: shareUrl });
  const isPopupCard = Boolean(onClose);

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

    commitAddCustomerFavouriteLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
          locationId: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`Failed to set location '${locationDetails.name}' as your favourite location. Error: ${getRelayErrorMessage(errors)}.`} />,
            errorNotificationOptions,
          );

          return;
        }
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`Failed to set location '${locationDetails.name}' as your favourite location. Error: ${error.message}.`} />,
          errorNotificationOptions,
        );
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

    commitRemoveCustomerFavouriteLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
          locationId: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`Failed to remove the location '${locationDetails.name}' as your favourite location. Error: ${getRelayErrorMessage(errors)}.`} />,
            errorNotificationOptions,
          );

          return;
        }
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`Failed to remove the location '${locationDetails.name}' as your favourite location. Error: ${error.message}.`} />,
          errorNotificationOptions,
        );
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
        sx={{
          ...cardSx,
          ...(isPopupCard
            ? {
                width: 320,
                maxWidth: 'calc(100vw - 32px)',
                height: 'auto',
              }
            : null),
        }}
        component={NextLink}
        href={getMarketplaceLocationLink(integratedPlatform, locationDetails.id)}
      >
        <Box sx={mediaSx}>
          {imageUrl ? (
            <Box
              sx={{
                position: 'absolute',
                inset: 0,
                backgroundImage: `url(${imageUrl})`,
                backgroundSize: 'cover',
                backgroundPosition: 'center',
              }}
            />
          ) : (
            <Box
              sx={{
                position: 'absolute',
                inset: 0,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
              }}
            >
              <Box
                sx={{
                  width: 52,
                  height: 52,
                  borderRadius: '50%',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255,255,255,0.75)' : 'rgba(15,23,42,0.6)'),
                  boxShadow: '0 8px 20px rgba(15, 23, 42, 0.12)',
                }}
              >
                <LocationIcon fontSize="medium" excludeTooltip sx={{ color: emerald }} />
              </Box>
            </Box>
          )}

          <Box
            sx={{
              position: 'absolute',
              inset: 0,
              background: imageUrl ? 'linear-gradient(180deg, rgba(15, 23, 42, 0.08), rgba(15, 23, 42, 0.34))' : 'transparent',
            }}
          />

          {onClose ? (
            <Box
              sx={{
                position: 'absolute',
                top: 12,
                right: 12,
              }}
            >
              <Tooltip title="Close">
                <IconButton onClick={handleCloseClick} size="small" sx={closeOverlayIconButtonSx}>
                  <CloseIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            </Box>
          ) : null}
        </Box>

        <CardContent
          sx={{
            flexGrow: isPopupCard ? 0 : 1,
            display: 'flex',
            flexDirection: 'column',
            gap: isPopupCard ? 0.75 : 1.25,
            p: 2,
          }}
        >
          <StackColumn spacing={isPopupCard ? 0.5 : 0.75}>
            <StackRow sx={{ alignItems: 'flex-start', justifyContent: 'space-between', gap: 1 }}>
              <Box title={locationDetails.name} sx={{ minWidth: 0, flex: 1 }}>
                <LeadIconTypography
                  label={locationDetails.name}
                  noWrap
                  sx={{
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap',
                  }}
                />
              </Box>

              <StackRow sx={{ flexShrink: 0, gap: 0.75 }}>
                <Tooltip title={isFavoured ? 'Remove as Favourite' : 'Set as Favourite'}>
                  <IconButton
                    onClick={isFavoured ? handleRemoveAsFavouriteLocationClicked : handleSetAsFavouriteLocationClicked}
                    size="small"
                    sx={{
                      ...headerIconButtonSx,
                      ...(isFavoured
                        ? {
                            color: emerald,
                            borderColor: 'rgba(16, 185, 129, 0.28)',
                            backgroundColor: 'rgba(16, 185, 129, 0.08)',
                          }
                        : null),
                    }}
                  >
                    {isFavoured ? <FavouriteIcon fontSize="small" /> : <NotFavouriteIcon fontSize="small" />}
                  </IconButton>
                </Tooltip>

                {canShare ? (
                  <Tooltip title="Share">
                    <IconButton onClick={handleShareClick} size="small" sx={headerIconButtonSx}>
                      <ShareIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                ) : null}
              </StackRow>
            </StackRow>

            {addressLabel ? (
              <Box title={addressLabel}>
                <SmallIconTypography
                  label={addressLabel}
                  noWrap
                  sx={{
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap',
                  }}
                />
              </Box>
            ) : null}
          </StackColumn>

          {capacity || areaSize ? (
            <Box sx={{ mt: isPopupCard ? 0 : 'auto' }}>
              <Box sx={detailsPanelSx}>
                <StackColumn spacing={0.75}>
                  {capacity ? <BodyIconTypography label={capacity} startElement={<PersonIcon fontSize="small" />} /> : null}
                  {areaSize ? <BodyIconTypography label={areaSize} startElement={<AreaIcon fontSize="small" />} /> : null}
                </StackColumn>
              </Box>
            </Box>
          ) : null}
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
