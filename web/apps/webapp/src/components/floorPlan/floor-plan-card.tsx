import { BodyIconTypography, LeadIconTypography, SmallIconTypography, StackRow } from '@/components/commons';
import { EllipseMenuIcon, LocationIcon } from '@/components/icons';
import { getOrganizationLocationFloorPlanAdminEditLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext, UpdateGlobalReloadIdContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { floorPlanCard_FloorPlanDetails$key } from '@/queries/__generated__/floorPlanCard_FloorPlanDetails.graphql';
import type { floorPlanCard_deleteFloorPlanMutation } from '@/queries/__generated__/floorPlanCard_deleteFloorPlanMutation.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import CardMedia from '@mui/material/CardMedia';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import { nanoid } from 'nanoid';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  floorPlanDetailsRelay: floorPlanCard_FloorPlanDetails$key;
  connectionIds: string[];
  organizationId: string;
  locationId: string;
};

const FloorPlanCard = ({ floorPlanDetailsRelay, connectionIds, organizationId, locationId }: Props) => {
  const floorPlanDetails = useFragment(
    graphql`
      fragment floorPlanCard_FloorPlanDetails on FloorPlanDetails {
        id
        name
        image {
          thumbnail {
            url
            height
            width
          }
        }
        resourceCount
      }
    `,
    floorPlanDetailsRelay,
  );

  const [commitDeleteBooking] = useMutation<floorPlanCard_deleteFloorPlanMutation>(graphql`
    mutation floorPlanCard_deleteFloorPlanMutation($connectionIds: [ID!]!, $input: DeleteFloorPlanInput!) {
      deleteFloorPlan(input: $input) {
        floorPlan {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditFloorPlan],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteFloorPlan],
  ];

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditFloorPlan:
        if (floorPlanDetails) {
          router.push(getOrganizationLocationFloorPlanAdminEditLink(integratedPlatrform, organizationId, locationId, floorPlanDetails.id));
        }

        break;

      case MoreActionsMenuOptionType.DeleteFloorPlan:
        handleRemoveBookingClick();
        break;
    }
  };

  const handleRemoveBookingClick = () => {
    const toastId = themedToast(<NotificationContent content={`Removing floor plan '${floorPlanDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: floorPlanDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove floor plan ${floorPlanDetails.name}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Floor plan ${floorPlanDetails.name} removed.`} />,
        });
        UpdateGlobalReloadId();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove floor plan ${floorPlanDetails.name}.`} />,
        });
      },
    });
  };

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 380 } }}>
        {floorPlanDetails.image && floorPlanDetails.image.thumbnail && <CardMedia component="img" image={floorPlanDetails.image.thumbnail.url} />}
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={getOrganizationLocationFloorPlanAdminEditLink(integratedPlatrform, organizationId, locationId, floorPlanDetails.id)}>
                <LeadIconTypography label={floorPlanDetails.name} startElement={<LocationIcon />} sx={{ flexWrap: undefined }} invertDefaultColor />
              </Link>
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
        <CardContent>
          <StackRow>
            <BodyIconTypography label="Resource Count:" />
            <SmallIconTypography label={`${floorPlanDetails.resourceCount}`} />
          </StackRow>
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(FloorPlanCard);
