import { EllipseMenuIcon, LocationIcon } from '@/components/icons';
import { getOrganizationLocationFloorPlanAdminEditLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import type { floorPlanCard_FloorPlanDetails$key } from '@/queries/__generated__/floorPlanCard_FloorPlanDetails.graphql';
import type { floorPlanCard_deleteFloorPlanMutation } from '@/queries/__generated__/floorPlanCard_deleteFloorPlanMutation.graphql';
import Box from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import { getRelayErrorMessage, PaletteModeContext, useIntegratedPlatform } from '@skedular/shared';
import { LeadIconTypography, StackRow } from '@skedular/ui';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  floorPlanDetailsRelay: floorPlanCard_FloorPlanDetails$key;
  connectionIds: string[];
  organizationCustomDomain: string;
  locationId: string;
};

const FloorPlanCard = ({ floorPlanDetailsRelay, connectionIds, organizationCustomDomain, locationId }: Props) => {
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

  const [commitDeleteFloorPlan] = useMutation<floorPlanCard_deleteFloorPlanMutation>(graphql`
    mutation floorPlanCard_deleteFloorPlanMutation($connectionIds: [ID!]!, $input: DeleteFloorPlanInput!) {
      deleteFloorPlan(input: $input) {
        floorPlan {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
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
          router.push(getOrganizationLocationFloorPlanAdminEditLink(integratedPlatform, organizationCustomDomain, locationId, floorPlanDetails.id));
        }
        break;

      case MoreActionsMenuOptionType.DeleteFloorPlan:
        commitDeleteFloorPlan({
          variables: {
            connectionIds,
            input: { clientMutationId: uuid(), id: floorPlanDetails.id },
          },
          onCompleted: (_, errors) => {
            if (errors && errors.length > 0) {
              themedToast(
                <NotificationContent content={`Failed to remove floor plan ${floorPlanDetails.name}. Error: ${getRelayErrorMessage(errors)}.`} />,
                errorNotificationOptions,
              );
            }
          },
          onError: (error) => {
            themedToast(<NotificationContent content={`Failed to remove floor plan ${floorPlanDetails.name}. Error: ${error.message}.`} />, errorNotificationOptions);
          },
        });
        break;
    }
  };

  const editLink = getOrganizationLocationFloorPlanAdminEditLink(integratedPlatform, organizationCustomDomain, locationId, floorPlanDetails.id);

  return (
    <>
      <Box
        sx={{
          border: 1,
          borderColor: 'divider',
          borderRadius: 2.5,
          px: 1,
          py: 0.75,
          backgroundColor: 'background.paper',
          boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 2px 10px rgba(15, 23, 42, 0.04)' : theme.shadows[1]),
        }}
      >
        <StackRow sx={{ alignItems: 'center', gap: 1, flexWrap: 'nowrap', minWidth: 0 }}>
          <Link component={NextLink} href={editLink} underline="none" sx={{ minWidth: 0, flex: '1 1 auto', overflow: 'hidden' }}>
            <LeadIconTypography label={floorPlanDetails.name} startElement={<LocationIcon excludeTooltip />} sx={{ flexWrap: undefined }} invertDefaultColor />
          </Link>

          <Chip size="small" label={`${floorPlanDetails.resourceCount} resource${floorPlanDetails.resourceCount === 1 ? '' : 's'}`} variant="outlined" sx={{ flexShrink: 0 }} />

          <IconButton size="small" onClick={handleMoreActionsMenuClick} aria-label={`More actions for ${floorPlanDetails.name}`}>
            <EllipseMenuIcon />
          </IconButton>
        </StackRow>
      </Box>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(FloorPlanCard);
