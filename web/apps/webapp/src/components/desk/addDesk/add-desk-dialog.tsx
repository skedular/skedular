import { MultipleChoicesCustomTags, MultipleChoicesZones } from '@/components/organization';
import type { addDeskDialog_addDeskMutation } from '@/queries/__generated__/addDeskDialog_addDeskMutation.graphql';
import type { addDeskDialog_rootQuery } from '@/queries/__generated__/addDeskDialog_rootQuery.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import {
  BodyIconTypography,
  DefaultDialogTitle,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  SmallIconTypography,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import { Autocomplete, makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<addDeskDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  organizationId: string;
  locationId?: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: (locationId: string) => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query addDeskDialog_rootQuery(
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

type DeskDetails = {
  location: string;
  name: string;
  customTagIds: string[];
  zoneIds: string[];
};

const deskSchema = object({
  location: string().required(),
  name: string().required('Desk name is required'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
});

const AddDeskDialog = ({ queryReference, organizationId, locationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const rootData = usePreloadedQuery<addDeskDialog_rootQuery>(RootQuery, queryReference);

  const [commitAddDesk] = useMutation<addDeskDialog_addDeskMutation>(graphql`
    mutation addDeskDialog_addDeskMutation($connectionIds: [ID!]!, $input: AddDeskInput!) @raw_response_type {
      addDesk(input: $input) {
        desk @appendNode(connections: $connectionIds, edgeTypeName: "DeskDetails") {
          id
          name
          customTags {
            uniqueId
          }
          zones {
            uniqueId
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(deskSchema);
  const requiredFields = makeRequired(deskSchema);
  const filterLocation = createFilterOptions<LocationDetails>();
  const locations = useMemo<LocationDetails[]>(
    () => (rootData.locations ? rootData.locations.edges.map(({ node }) => node) : []),
    [rootData.locations],
  );

  const handleAddClick = ({ location: locationId, name, customTagIds, zoneIds }: DeskDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding desk '${name}'...`} />, infoNotificationOptions);

    commitAddDesk({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          locationId,
          name,
          customTagIds,
          zoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add desk '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk ${name} added.`} />,
        });

        onAddClicked(locationId);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add desk '${name}'. Error: ${error.message}.`} />,
        });
      },

      optimisticResponse: {
        addDesk: {
          desk: {
            id,
            name,
            customTags: [],
            zones: [],
          },
        },
      },
    });
  };

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Add Desk" />
      <DialogContent>
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
              <LeadIconTypography label="Add desk to this location" />
              <SmallIconTypography label="Enter the name of the desk to add to this location." />

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
                <TextField name="name" required={requiredFields.name} helperText="Add your desk name" />
              </FormFieldLabel>

              {organizationId && (
                <FormFieldLabel label="Tags" useWiderSpace>
                  <MultipleChoicesCustomTags rootDataRelay={rootData} name="customTagIds" required={requiredFields.customTagIds} />
                </FormFieldLabel>
              )}

              {organizationId && (
                <FormFieldLabel label="Zones" useWiderSpace>
                  <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredFields.zoneIds} />
                </FormFieldLabel>
              )}

              <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Add" secondaryLabel="Cancel" />
            </FormStackColumn>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

const MemoAddDeskDialog = memo(AddDeskDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  organizationId: string;
  locationId?: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: (locationId: string) => void;
  onCancel: () => void;
};

const AddDeskDialogWithRelay = ({
  onReloadRequired,
  organizationId,
  locationId,
  connectionIds,
  isDialogOpen,
  onAddClicked,
  onCancel,
}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addDeskDialog_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId: organizationId ?? '',
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
      <MemoAddDeskDialog
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

export default memo(AddDeskDialogWithRelay);
