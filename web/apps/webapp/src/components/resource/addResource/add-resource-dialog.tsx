import {
  BodyIconTypography,
  ColorPicker,
  DefaultDialogTitle,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  SmallIconTypography,
  TwoButtonsDialogActions,
} from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesCustomTags, MultipleChoicesZones, SingleChoicesResourceType } from '@/components/organization';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { addResourceDialog_addResourceMutation } from '@/queries/__generated__/addResourceDialog_addResourceMutation.graphql';
import type { addResourceDialog_rootQuery } from '@/queries/__generated__/addResourceDialog_rootQuery.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete, makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<addResourceDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  organizationId: string;
  locationId?: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: (locationId: string) => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query addResourceDialog_rootQuery(
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
    ...singleChoiceResourceType_query
    ...multipleChoicesCustomTags_query
    ...multipleChoicesZones_query
  }
`;

type LocationDetails = {
  id: string;
  name: string;
};

type ResourceDetails = {
  location: string;
  resourceTypeId: string;
  name: string;
  customTagIds: string[];
  zoneIds: string[];
};

const ResourceSchema = object({
  location: string().required(),
  resourceTypeId: string().required('Resource type is required'),
  name: string().required('Resource name is required'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
});

const AddResourceDialog = ({ queryReference, organizationId, locationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const rootData = usePreloadedQuery<addResourceDialog_rootQuery>(RootQuery, queryReference);

  const [commitAddResource] = useMutation<addResourceDialog_addResourceMutation>(graphql`
    mutation addResourceDialog_addResourceMutation($connectionIds: [ID!]!, $input: AddResourceInput!) @raw_response_type {
      addResource(input: $input) {
        resource @appendNode(connections: $connectionIds, edgeTypeName: "ResourceDetails") {
          id
          name
          inactive
          requireBookingApproval
          color
          capacity
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
          resourceType {
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
  const validate = makeValidate(ResourceSchema);
  const requiredFields = makeRequired(ResourceSchema);
  const filterLocation = createFilterOptions<LocationDetails>();
  const [selectedColor, setSelectedColor] = useState('');
  const locations = useMemo<LocationDetails[]>(() => (rootData.locations ? rootData.locations.edges.map(({ node }) => node) : []), [rootData.locations]);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ location: locationId, resourceTypeId, name, customTagIds, zoneIds }: ResourceDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding resource '${name}'...`} />, infoNotificationOptions);

    commitAddResource({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          locationId,
          name,
          customTagIds,
          zoneIds,
          inactive: false,
          requireBookingApproval: false,
          color: selectedColor,
          capacity: 1,
          organizationResourceTypeId: resourceTypeId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add resource '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Resource ${name} added.`} />,
        });

        onAddClicked(locationId);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add resource '${name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addResource: {
          resource: {
            id,
            name,
            inactive: false,
            requireBookingApproval: false,
            customTags: [],
            zones: [],
            color: selectedColor,
            capacity: 1,
            resourceType: {
              uniqueId: resourceTypeId,
              name: '',
              color: '',
            },
          },
        },
      },
    });
  };

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Add Resource" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            location: locationId,
            resourceTypeId: '',
            name: '',
            customTagIds: [],
            zoneIds: [],
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <LeadIconTypography label="Add resource to this location" />
              <SmallIconTypography label="Enter the name of the resource to add to this location." />

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

              <FormFieldLabel label="Resource Type" useWiderSpace>
                <SingleChoicesResourceType rootDataRelay={rootData} name="resourceTypeId" required={requiredFields.resourceTypeId} />
              </FormFieldLabel>

              <FormFieldLabel label="Name" useWiderSpace>
                <TextField name="name" required={requiredFields.name} helperText="Add your resource name" />
              </FormFieldLabel>

              <FormFieldLabel label="Tags" useWiderSpace>
                <MultipleChoicesCustomTags rootDataRelay={rootData} name="customTagIds" required={requiredFields.customTagIds} organizationId={organizationId} />
              </FormFieldLabel>

              <FormFieldLabel label="Zones" useWiderSpace>
                <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredFields.zoneIds} organizationId={organizationId} />
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

const MemoAddResourceDialog = memo(AddResourceDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  organizationId: string;
  locationId?: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: (locationId: string) => void;
  onCancel: () => void;
};

const AddResourceDialogWithRelay = ({ onReloadRequired, organizationId, locationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addResourceDialog_rootQuery>(RootQuery);
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
      <MemoAddResourceDialog
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

export default memo(AddResourceDialogWithRelay);
