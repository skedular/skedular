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
import { MultipleChoicesCustomTags, MultipleChoicesProductTags, MultipleChoicesZones, SingleChoiceResourceType } from '@/components/organization';
import { RelayError, toRootError } from '@/components/relayError';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { addResourceDialog_addResourceMutation } from '@/queries/__generated__/addResourceDialog_addResourceMutation.graphql';
import type { addResourceDialog_rootQuery } from '@/queries/__generated__/addResourceDialog_rootQuery.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete, makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { array, number, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<addResourceDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  organizationUniqueAlphanumericName: string;
  locationId?: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: (locationId: string) => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query addResourceDialog_rootQuery(
    $organizationUniqueAlphanumericName: String!
    $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
      type {
        type
      }
    }
    locations(where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName }, orderBy: $locationsSortingValues) {
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
    ...multipleChoicesProductTags_query
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
  productTagIds: string[];
  capacity: number;
};

const ResourceSchema = object({
  location: string().required(),
  resourceTypeId: string().required('Resource type is required'),
  name: string().required('Resource name is required'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
  productTagIds: array().nullable(),
  capacity: number().required('Capacity is required').min(1, 'Capacity must be greater than 0'),
});

const AddResourceDialog = ({ queryReference, organizationUniqueAlphanumericName, locationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: Props) => {
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
            id
            name
            color
          }
          zones {
            id
            name
            color
          }
          productTags {
            id
            name
            color
          }
          resourceType {
            id
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

  const handleAddClick = ({ location: locationId, resourceTypeId, name, customTagIds, zoneIds, productTagIds, capacity: capacityStr }: ResourceDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding resource '${name}'...`} />, infoNotificationOptions);
    const capacity = parseInt(capacityStr.toString(), 10);

    commitAddResource({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          locationId,
          name,
          customTagIds,
          zoneIds,
          productTagIds,
          inactive: false,
          requireBookingApproval: false,
          color: selectedColor,
          capacity,
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
            productTags: [],
            color: selectedColor,
            capacity,
            resourceType: {
              id: resourceTypeId,
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
            productTagIds: [],
            capacity: 1,
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
                        <li {...props} key={castedOption.id}>
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
                <SingleChoiceResourceType rootDataRelay={rootData} name="resourceTypeId" required={requiredFields.resourceTypeId} />
              </FormFieldLabel>

              <FormFieldLabel label="Name" useWiderSpace>
                <TextField name="name" required={requiredFields.name} helperText="Add your resource name" />
              </FormFieldLabel>

              <FormFieldLabel label="Tags" useWiderSpace>
                <MultipleChoicesCustomTags
                  rootDataRelay={rootData}
                  name="customTagIds"
                  required={requiredFields.customTagIds}
                  organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                />
              </FormFieldLabel>

              <FormFieldLabel label="Zones" useWiderSpace>
                <MultipleChoicesZones
                  rootDataRelay={rootData}
                  name="zoneIds"
                  required={requiredFields.zoneIds}
                  organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                />
              </FormFieldLabel>

              {rootData.organization?.type.type === 'MARKETPLACE' && (
                <FormFieldLabel label="Product Tags" useWiderSpace>
                  <MultipleChoicesProductTags
                    rootDataRelay={rootData}
                    name="productTagIds"
                    required={requiredFields.productTagIds}
                    organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                  />
                </FormFieldLabel>
              )}

              <FormFieldLabel label="Color" useWiderSpace>
                <ColorPicker onChange={handleColorChange} />
              </FormFieldLabel>

              <FormFieldLabel label="Capacity" useWiderSpace>
                <TextField name="capacity" required={requiredFields.capacity} />
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
  organizationUniqueAlphanumericName: string;
  locationId?: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: (locationId: string) => void;
  onCancel: () => void;
};

const AddResourceDialogWithRelay = ({ onReloadRequired, organizationUniqueAlphanumericName, locationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addResourceDialog_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationUniqueAlphanumericName,
        multipleChoicesCustomTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        multipleChoicesZonesSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        multipleChoicesProductTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        locationsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());

      if (onReloadRequired) {
        onReloadRequired();
      }
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoAddResourceDialog
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
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
