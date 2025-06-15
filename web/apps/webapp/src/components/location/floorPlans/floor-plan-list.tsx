import { BodyIconTypography, GridContainer, StackColumn, StackRow } from '@/components/commons';
import { AddIcon, DeleteIcon, EditIcon } from '@/components/icons';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Chip from '@mui/material/Chip';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import { memo, useState } from 'react';
import { AddFloorPlanDialog } from './add-floor-plan-dialog';
import { EditFloorPlanDialog } from './edit-floor-plan-dialog';
import { FloorPlanEditor } from './floor-plan-editor';

type FloorPlan = {
  readonly id: string;
  readonly name: string;
  readonly floorLevel: number;
  readonly floorName: string | null | undefined;
  readonly imagePath: string;
  readonly thumbnailPath: string | null | undefined;
  readonly width: number;
  readonly height: number;
  readonly isActive: boolean;
  readonly resourcePositions: ReadonlyArray<{
    readonly id: string;
    readonly x: number;
    readonly y: number;
    readonly width: number;
    readonly height: number;
    readonly shape: string | null | undefined;
    readonly metadata: string | null | undefined;
    readonly resource: {
      readonly id: string;
      readonly name: string;
    };
  }>;
};

type ResourceData = {
  id: string;
  name: string;
  inactive: boolean;
  color: string | null | undefined;
  resourceType: {
    uniqueId: string;
    name: string | null | undefined;
    color: string | null | undefined;
  };
};

type Props = {
  floorPlans: ReadonlyArray<FloorPlan>;
  locationId: string;
  organizationId: string;
  resources: ReadonlyArray<ResourceData>;
  onReloadRequired: () => void;
};

const FloorPlanList = ({ floorPlans, locationId, organizationId, resources, onReloadRequired }: Props) => {
  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [selectedFloorPlan, setSelectedFloorPlan] = useState<FloorPlan | null>(null);
  const [editorOpen, setEditorOpen] = useState(false);

  const handleAddClick = () => {
    setAddDialogOpen(true);
  };

  const handleEditClick = (floorPlan: FloorPlan) => {
    setSelectedFloorPlan(floorPlan);
    setEditDialogOpen(true);
  };

  const handleViewClick = (floorPlan: FloorPlan) => {
    setSelectedFloorPlan(floorPlan);
    setEditorOpen(true);
  };

  const handleDeleteClick = async (floorPlanId: string) => {
    if (!window.confirm('Are you sure you want to delete this floor plan? This action cannot be undone.')) {
      return;
    }

    try {
      const response = await fetch('/api/v1/graphql', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          query: `
            mutation DeleteFloorPlan($input: DeleteFloorPlanInput!) {
              deleteFloorPlan(input: $input) {
                clientMutationId
                success
              }
            }
          `,
          variables: {
            input: {
              clientMutationId: Math.random().toString(36).substring(7),
              id: floorPlanId,
            },
          },
        }),
      });

      const result = await response.json();

      if (result.errors) {
        console.error('Error deleting floor plan:', result.errors);
        alert('Failed to delete floor plan. Please try again.');
        return;
      }

      if (result.data?.deleteFloorPlan?.success) {
        onReloadRequired();
      }
    } catch (error) {
      console.error('Error deleting floor plan:', error);
      alert('Failed to delete floor plan. Please try again.');
    }
  };

  const sortedFloorPlans = [...floorPlans].sort((a, b) => a.floorLevel - b.floorLevel);

  return (
    <StackColumn spacing={3}>
      <GridContainer sx={{ justifyContent: 'flex-end', paddingX: defaultPadding }}>
        <Grid size="auto">
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAddClick} sx={defaultButtonStyle}>
            Add Floor Plan
          </Button>
        </Grid>
      </GridContainer>

      <Grid container spacing={3} sx={{ paddingX: defaultPadding }}>
        {sortedFloorPlans.map((floorPlan) => (
          <Grid key={floorPlan.id} size={{ xs: 12, sm: 6, md: 4 }}>
            <Card>
              {floorPlan.thumbnailPath && (
                <CardMedia
                  component="img"
                  height="200"
                  image={floorPlan.thumbnailPath}
                  alt={floorPlan.name}
                  sx={{ cursor: 'pointer' }}
                  onClick={() => handleViewClick(floorPlan)}
                />
              )}
              <CardContent>
                <StackColumn spacing={1}>
                  <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                    <Box>
                      <BodyIconTypography label={floorPlan.name} sx={{ fontWeight: 'bold' }} />
                      <BodyIconTypography label={floorPlan.floorName || `Floor ${floorPlan.floorLevel}`} sx={{ fontSize: '0.875rem', color: 'text.secondary' }} />
                    </Box>
                    <StackRow spacing={1}>
                      <IconButton size="small" onClick={() => handleEditClick(floorPlan)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" onClick={() => handleDeleteClick(floorPlan.id)}>
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </StackRow>
                  </StackRow>
                  <StackRow spacing={1}>
                    <Chip label={`${floorPlan.resourcePositions.length} resources`} size="small" />
                    {!floorPlan.isActive && <Chip label="Inactive" size="small" color="warning" />}
                  </StackRow>
                  <Button variant="outlined" fullWidth onClick={() => handleViewClick(floorPlan)} sx={{ mt: 1 }}>
                    Manage Resources
                  </Button>
                </StackColumn>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {addDialogOpen && <AddFloorPlanDialog open={addDialogOpen} onClose={() => setAddDialogOpen(false)} locationId={locationId} onReloadRequired={onReloadRequired} />}

      {editDialogOpen && selectedFloorPlan && (
        <EditFloorPlanDialog open={editDialogOpen} onClose={() => setEditDialogOpen(false)} floorPlan={selectedFloorPlan} onReloadRequired={onReloadRequired} />
      )}

      {editorOpen && selectedFloorPlan && (
        <FloorPlanEditor
          open={editorOpen}
          onClose={() => setEditorOpen(false)}
          floorPlan={selectedFloorPlan}
          locationId={locationId}
          organizationId={organizationId}
          resources={resources}
          onReloadRequired={onReloadRequired}
        />
      )}
    </StackColumn>
  );
};

export default memo(FloorPlanList);
