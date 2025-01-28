import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import {
  BodyIconTypography,
  ColorPicker,
  DefaultDialogTitle,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  SmallIconTypography,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { MultipleChoicesCustomTags, MultipleChoicesZones } from 'components/organization';
import { Autocomplete, makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';
import type { addRoomDialog_addRoomMutation } from './__generated__/addRoomDialog_addRoomMutation.graphql';
import type { addRoomDialog_rootQuery } from './__generated__/addRoomDialog_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<addRoomDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  organizationId: string;
  locationId?: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: (locationId: string) => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query addRoomDialog_rootQuery(
    $organizationId: String!
    $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    locations(where: { organizationId: $organizationId }, orderBy: $locationsSortingValues) {
      __id
      totalCount
      edges {
        node {
          id
          name
        }
      }
    }
    ...multipleChoicesCustomTags_query
    ...multipleChoicesZones_query
  }
`;

type LocationDetails = {
  id: string;
  name: string;
};

type RoomDetails = {
  location: string;
  name: string;
  customTagIds: string[];
  zoneIds: string[];
};

const roomSchema = object({
  location: string().required(),
  name: string().required('Room name is required'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
});

const AddRoomDialog = ({ queryReference, organizationId, locationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const rootData = usePreloadedQuery<addRoomDialog_rootQuery>(RootQuery, queryReference);

  const [commitAddRoom] = useMutation<addRoomDialog_addRoomMutation>(graphql`
    mutation addRoomDialog_addRoomMutation($connectionIds: [ID!]!, $input: AddRoomInput!) @raw_response_type {
      addRoom(input: $input) {
        room @appendNode(connections: $connectionIds, edgeTypeName: "RoomDetails") {
          id
          name
          deactivated
          requireBookingApproval
          color
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
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(roomSchema);
  const requiredFields = makeRequired(roomSchema);
  const filterLocation = createFilterOptions<LocationDetails>();
  const [selectedColor, setSelectedColor] = useState('');
  const locations = useMemo<LocationDetails[]>(() => (rootData.locations ? rootData.locations.edges.map(({ node }) => node) : []), [rootData.locations]);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ location: locationId, name, customTagIds, zoneIds }: RoomDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding room '${name}'...`} />, infoNotificationOptions);

    commitAddRoom({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          locationId,
          name,
          customTagIds,
          zoneIds,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add room '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Room ${name} added.`} />,
        });

        onAddClicked(locationId);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add room '${name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addRoom: {
          room: {
            id,
            name,
            deactivated: false,
            requireBookingApproval: false,
            customTags: [],
            zones: [],
            color: selectedColor,
          },
        },
      },
    });
  };

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Add Room" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            location: locationId,
            name: '',
            customTagIds: [],
            zoneIds: [],
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <LeadIconTypography label="Add room to this location" />
              <SmallIconTypography label="Enter the name of the room to add to this location." />

              {!locationId && (
                <FormFieldLabel label="Location" useWiderSpace>
                  <Autocomplete
                    name="location"
                    multiple={false}
                    required={requiredFields.location}
                    options={locations}
                    getOptionValue={(option) => (option as LocationDetails).id}
                    getOptionLabel={(option: string | LocationDetails) => (option as LocationDetails).name}
                    renderOption={(props, option) => {
                      const castedOption = option as LocationDetails;

                      return (
                        <li {...props}>
                          <BodyIconTypography label={castedOption.name} />
                        </li>
                      );
                    }}
                    filterOptions={(options, params) => filterLocation(options as LocationDetails[], params)}
                    selectOnFocus
                    clearOnBlur
                    handleHomeEndKeys
                  />
                </FormFieldLabel>
              )}

              <FormFieldLabel label="Name" useWiderSpace>
                <TextField name="name" required={requiredFields.name} helperText="Add your room name" />
              </FormFieldLabel>

              <FormFieldLabel label="Tags" useWiderSpace>
                <MultipleChoicesCustomTags rootDataRelay={rootData} name="customTagIds" required={requiredFields.customTagIds} />
              </FormFieldLabel>

              <FormFieldLabel label="Zones" useWiderSpace>
                <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredFields.zoneIds} />
              </FormFieldLabel>

              <FormFieldLabel label="Color" useWiderSpace>
                <ColorPicker onChange={handleColorChange} />
              </FormFieldLabel>

              <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Add" secondaryLabel="Cancel" />
            </FormStackColumn>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

const MemoAddRoomDialog = memo(AddRoomDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  organizationId: string;
  locationId?: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: (locationId: string) => void;
  onCancel: () => void;
};

const AddRoomDialogWithRelay = ({ onReloadRequired, organizationId, locationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addRoomDialog_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        multipleChoicesCustomTagsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        multipleChoicesZonesSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        locationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());

      if (onReloadRequired) {
        onReloadRequired();
      }
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoAddRoomDialog
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
        locationId={locationId}
        connectionIds={connectionIds}
        isDialogOpen={isDialogOpen}
        onAddClicked={onAddClicked}
        onCancel={onCancel}
      />
    </ErrorBoundary>
  );
};

export default memo(AddRoomDialogWithRelay);
