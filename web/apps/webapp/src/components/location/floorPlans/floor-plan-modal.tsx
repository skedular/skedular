import { DefaultDialogTitle } from '@/components/commons';
import { DialogTransition } from '@/components/transitions';
import CloseIcon from '@mui/icons-material/Close';
import Box from '@mui/material/Box';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import IconButton from '@mui/material/IconButton';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import type { floorPlanModal_query$key } from '@/queries/__generated__/floorPlanModal_query.graphql';
import CustomerFloorPlanView from './customer-floor-plan-view';

type Props = {
  rootDataRelay: floorPlanModal_query$key;
  organizationId: string;
  locationId: string;
  locationName: string;
  isOpen: boolean;
  onClose: () => void;
  onBookResource?: (resourceId: string) => void;
  platform?: string;
};

const FloorPlanModal = ({ rootDataRelay, organizationId, locationId, locationName, isOpen, onClose, onBookResource, platform = 'web' }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment floorPlanModal_query on Query @argumentDefinitions(locationId: { type: "String!" }) {
        location(id: $locationId) {
          id
          name
        }
        floorPlansByLocation(locationId: $locationId) {
          id
          name
          floorLevel
          floorName
          imagePath
          thumbnailPath
          width
          height
          isActive
          resourcePositions {
            id
            x
            y
            width
            height
            shape
            metadata
            resource {
              id
              name
            }
          }
        }
        resources(where: { locationId: $locationId }) {
          edges {
            node {
              id
              name
              inactive
              color
              capacity
              requireBookingApproval
              resourceType {
                uniqueId
                name
                tagType
                color
              }
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
            }
          }
        }
        ...customerFloorPlanView_availableResources_query
      }
    `,
    rootDataRelay,
  );

  return (
    <Dialog open={isOpen} onClose={onClose} maxWidth="xl" fullWidth slots={{ transition: DialogTransition }}>
      <Box sx={{ position: 'relative' }}>
        <DefaultDialogTitle title={`${rootData.location?.name || locationName} - Floor Plan`} />
        <IconButton
          aria-label="close"
          onClick={onClose}
          sx={{
            position: 'absolute',
            right: 8,
            top: 8,
            color: (theme) => theme.palette.grey[500],
          }}
        >
          <CloseIcon />
        </IconButton>
      </Box>
      <DialogContent sx={{ p: 0, height: '70vh', overflow: 'hidden' }}>
        <CustomerFloorPlanView
          organizationId={organizationId}
          locationId={locationId}
          locationName={rootData.location?.name || locationName}
          floorPlans={rootData.floorPlansByLocation || []}
          resources={rootData.resources?.edges?.map((edge: any) => edge?.node).filter(Boolean) || []}
          rootDataAvailableResourcesRelay={rootData}
          onBookResource={onBookResource}
          platform={platform}
        />
      </DialogContent>
    </Dialog>
  );
};

export { FloorPlanModal };
export default memo(FloorPlanModal);
