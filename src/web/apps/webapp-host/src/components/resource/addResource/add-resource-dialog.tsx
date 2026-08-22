import { PaletteModeContext, RelayError, getRelayErrorMessage, toRootError, useIntegratedPlatform } from '@skedular/shared';
import {
  BodyIconTypography,
  ColorPicker,
  DefaultDialogTitle,
  EditorActionBar,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  PageHeaderPanel,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  TwoButtonsDialogActions,
  StickyReviewRail,
} from '@skedular/ui';
import { Loading } from '@/components/loading';
import { getOrganizationLocationManageResourcesBaseLink, getOrganizationLocationsBaseLink } from '@/components/links';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { MultipleChoicesCustomTags, MultipleChoicesProductTags, MultipleChoicesZones, SingleChoiceResourceType } from '@/components/organization';

import { DialogTransition } from '@/components/transitions';

import type { addResourceDialog_addResourceMutation } from '@/queries/__generated__/addResourceDialog_addResourceMutation.graphql';
import type { addResourceDialog_rootQuery } from '@/queries/__generated__/addResourceDialog_rootQuery.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete, makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter } from 'next/navigation';
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
  organizationCustomDomain: string;
  locationId?: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: (locationId: string) => void;
  onCancel: () => void;
  presentation?: 'dialog' | 'page';
};

const RootQuery = graphql`
  query addResourceDialog_rootQuery(
    $organizationCustomDomain: String!
    $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    organization(customDomain: $organizationCustomDomain) {
      type {
        type
      }
    }
    locations(where: { organizationCustomDomain: $organizationCustomDomain }, orderBy: $locationsSortingValues) {
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
  resourceTypeId: string().required('Please choose a resource type.'),
  name: string().required('Please enter a resource name.'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
  productTagIds: array().nullable(),
  capacity: number().required('Please enter a capacity.').min(1, 'Capacity must be at least 1.'),
});

const AddResourceDialog = ({ queryReference, organizationCustomDomain, locationId, connectionIds, isDialogOpen, onAddClicked, onCancel, presentation = 'dialog' }: Props) => {
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
          themedToast(<NotificationContent content={`We couldn't add ${name}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        onAddClicked(locationId);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't add ${name}. ${error.message}`} />, errorNotificationOptions);
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

  const form = (
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
          {presentation === 'dialog' ? (
            <>
              <LeadIconTypography label="Add a resource" />
              <SmallIconTypography label="Create a new resource for this location." />
            </>
          ) : (
            <PageHeaderPanel title="Add resource" description="Create a bookable resource and attach the tags, zones, and capacity used by availability and product setup." />
          )}

          {presentation === 'page' ? (
            <>
              <SettingsSectionCard title="Resource basics" description="Set the core identity and capacity operators will see when managing bookings and availability.">
                <StackColumn spacing={2}>
                  <FormFieldLabel label="Resource Type">
                    <SingleChoiceResourceType rootDataRelay={rootData} name="resourceTypeId" required={requiredFields.resourceTypeId} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredFields.name} helperText="Enter a clear name, such as Desk A1 or Meeting Room 2." />
                  </FormFieldLabel>

                  <FormFieldLabel label="Capacity">
                    <TextField name="capacity" required={requiredFields.capacity} />
                  </FormFieldLabel>
                </StackColumn>
              </SettingsSectionCard>

              <SettingsSectionCard title="Classification" description="Use tags, zones, and colour to make this resource easy to filter and recognise.">
                <StackColumn spacing={2}>
                  <FormFieldLabel label="Tags">
                    <MultipleChoicesCustomTags
                      rootDataRelay={rootData}
                      name="customTagIds"
                      required={requiredFields.customTagIds}
                      organizationCustomDomain={organizationCustomDomain}
                    />
                  </FormFieldLabel>

                  <FormFieldLabel label="Zones">
                    <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredFields.zoneIds} organizationCustomDomain={organizationCustomDomain} />
                  </FormFieldLabel>

                  {rootData.organization?.type.type === 'MARKETPLACE' && (
                    <FormFieldLabel label="Booking Groups">
                      <MultipleChoicesProductTags
                        rootDataRelay={rootData}
                        name="productTagIds"
                        required={requiredFields.productTagIds}
                        organizationCustomDomain={organizationCustomDomain}
                      />
                    </FormFieldLabel>
                  )}

                  <FormFieldLabel label="Colour">
                    <ColorPicker onChange={handleColorChange} />
                  </FormFieldLabel>
                </StackColumn>
              </SettingsSectionCard>
            </>
          ) : (
            <>
              {!locationId && (
                <FormFieldLabel label="Location">
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

              <FormFieldLabel label="Resource Type">
                <SingleChoiceResourceType rootDataRelay={rootData} name="resourceTypeId" required={requiredFields.resourceTypeId} />
              </FormFieldLabel>

              <FormFieldLabel label="Name">
                <TextField name="name" required={requiredFields.name} helperText="Enter a clear name, such as Desk A1 or Meeting Room 2." />
              </FormFieldLabel>

              <FormFieldLabel label="Tags">
                <MultipleChoicesCustomTags
                  rootDataRelay={rootData}
                  name="customTagIds"
                  required={requiredFields.customTagIds}
                  organizationCustomDomain={organizationCustomDomain}
                />
              </FormFieldLabel>

              <FormFieldLabel label="Zones">
                <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredFields.zoneIds} organizationCustomDomain={organizationCustomDomain} />
              </FormFieldLabel>

              {rootData.organization?.type.type === 'MARKETPLACE' && (
                <FormFieldLabel label="Booking Groups">
                  <MultipleChoicesProductTags
                    rootDataRelay={rootData}
                    name="productTagIds"
                    required={requiredFields.productTagIds}
                    organizationCustomDomain={organizationCustomDomain}
                  />
                </FormFieldLabel>
              )}

              <FormFieldLabel label="Colour">
                <ColorPicker onChange={handleColorChange} />
              </FormFieldLabel>

              <FormFieldLabel label="Capacity">
                <TextField name="capacity" required={requiredFields.capacity} />
              </FormFieldLabel>
            </>
          )}

          {presentation === 'page' ? (
            <EditorActionBar
              secondaryActions={
                <Button type="button" variant="text" onClick={onCancel} sx={{ textTransform: 'none' }}>
                  Cancel
                </Button>
              }
              primaryAction="Add resource"
            />
          ) : (
            <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Add" secondaryLabel="Cancel" />
          )}
        </FormStackColumn>
      )}
    />
  );

  if (presentation === 'page') {
    return (
      <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
        <Box sx={{ maxWidth: 1320, mx: 'auto', display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', xl: 'minmax(0, 2fr) 320px' }, gap: 2 }}>
          <StackColumn spacing={2.5} sx={{ minWidth: 0 }}>
            {form}
          </StackColumn>

          <StickyReviewRail title="Resource help" description="Create the resource first, then fine-tune opening hours or booking rules from resource settings.">
            <SettingsSectionCard title="Suggested setup" description="Keep resource records consistent so bookings, floor plans, and reporting stay useful.">
              <StackColumn spacing={1}>
                <SmallIconTypography label="Use a recognisable name that matches signage or floor-plan labels." />
                <SmallIconTypography label="Set capacity to the number of people or units the resource can support." />
                <SmallIconTypography label="Apply zones and tags now so users can filter availability immediately." />
              </StackColumn>
            </SettingsSectionCard>

            <SettingsSectionCard title="After adding" description="The resource will be available in this location's resource list.">
              <StackColumn spacing={1}>
                <SmallIconTypography label="Open the resource settings to adjust opening hours." />
                <SmallIconTypography label="Use booking groups for resources that should be bookable through matching products." />
              </StackColumn>
            </SettingsSectionCard>
          </StickyReviewRail>
        </Box>
      </Box>
    );
  }

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Add Resource" />
      <DialogContent sx={{ marginTop: 2 }}>{form}</DialogContent>
    </Dialog>
  );
};

const MemoAddResourceDialog = memo(AddResourceDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  organizationCustomDomain: string;
  locationId?: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: (locationId: string) => void;
  onCancel: () => void;
  presentation?: 'dialog' | 'page';
};

const AddResourceDialogWithRelay = ({
  onReloadRequired,
  organizationCustomDomain,
  locationId,
  connectionIds,
  isDialogOpen,
  onAddClicked,
  onCancel,
  presentation = 'dialog',
}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addResourceDialog_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    if (!isDialogOpen) {
      return;
    }

    loadQuery(
      {
        organizationCustomDomain,
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
  }, [isDialogOpen, loadQuery, triggerReloadId, organizationCustomDomain]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());

      if (onReloadRequired) {
        onReloadRequired();
      }
    });
  };

  if (!isDialogOpen) {
    return null;
  }

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoAddResourceDialog
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        locationId={locationId}
        connectionIds={connectionIds}
        isDialogOpen={isDialogOpen}
        onAddClicked={onAddClicked}
        onCancel={onCancel}
        presentation={presentation}
      />
    </ErrorBoundary>
  );
};

type PageProps = {
  organizationCustomDomain: string;
  locationId?: string;
};

const AddResourcePageComponent = ({ organizationCustomDomain, locationId }: PageProps) => {
  const router = useRouter();
  const { integratedPlatform } = useIntegratedPlatform();

  const handleDone = (createdLocationId: string) => {
    router.push(getOrganizationLocationManageResourcesBaseLink(integratedPlatform, organizationCustomDomain, createdLocationId));
  };

  const handleCancel = () => {
    router.push(
      locationId
        ? getOrganizationLocationManageResourcesBaseLink(integratedPlatform, organizationCustomDomain, locationId)
        : getOrganizationLocationsBaseLink(integratedPlatform, organizationCustomDomain),
    );
  };

  return (
    <AddResourceDialogWithRelay
      organizationCustomDomain={organizationCustomDomain}
      locationId={locationId}
      connectionIds={[]}
      isDialogOpen={true}
      onAddClicked={handleDone}
      onCancel={handleCancel}
      presentation="page"
    />
  );
};

export const AddResourcePage = memo(AddResourcePageComponent);

export default memo(AddResourceDialogWithRelay);
