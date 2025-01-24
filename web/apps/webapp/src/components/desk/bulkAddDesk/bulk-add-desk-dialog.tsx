import { MultipleChoicesCustomTags, MultipleChoicesZones } from '@/components/organization';
import type { bulkAddDeskDialog_bulkAddDeskMutation } from '@/queries/__generated__/bulkAddDeskDialog_bulkAddDeskMutation.graphql';
import type { bulkAddDeskDialog_rootQuery } from '@/queries/__generated__/bulkAddDeskDialog_rootQuery.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { DefaultDialogTitle, FormFieldLabel, FormStackColumn, LeadIconTypography, SmallIconTypography, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, number, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<bulkAddDeskDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  organizationId: string;
  locationId: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query bulkAddDeskDialog_rootQuery(
    $organizationId: String!
    $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]
  ) {
    ...multipleChoicesCustomTags_query
    ...multipleChoicesZones_query
  }
`;

type DeskDetails = {
  namePrefix: string;
  count: number;
  customTagIds: string[];
  zoneIds: string[];
};

const deskSchema = object({
  namePrefix: string(),
  count: number().positive().integer().required('Desk count is required'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
});

const BulkAddDeskDialog = ({ queryReference, organizationId, locationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const rootData = usePreloadedQuery<bulkAddDeskDialog_rootQuery>(RootQuery, queryReference);

  const [commitAddDesk] = useMutation<bulkAddDeskDialog_bulkAddDeskMutation>(graphql`
    mutation bulkAddDeskDialog_bulkAddDeskMutation($connectionIds: [ID!]!, $input: BulkAddDeskInput!) @raw_response_type {
      bulkAddDesk(input: $input) {
        desks @appendNode(connections: $connectionIds, edgeTypeName: "DeskDetails") {
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

  const handleAddClick = ({ namePrefix, count, customTagIds, zoneIds }: DeskDetails) => {
    const ids = Array.from(Array(count).keys()).map((_) => nanoid());
    const toastId = themedToast(<NotificationContent content={`Adding desks...`} />, infoNotificationOptions);

    commitAddDesk({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          namePrefix,
          locationId,
          count: parseInt(count.toString()),
          deactivated: false,
          requireBookingApproval: false,
          customTagIds,
          zoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add desks. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desks added.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add desk. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        bulkAddDesk: {
          desks: ids.map((id) => ({ id, name: namePrefix, customTags: [], zones: [] })),
        },
      },
    });
  };

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Add Desk" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            namePrefix: '',
            count: 0,
            customTagIds: [],
            zoneIds: [],
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <LeadIconTypography label="Add desks to this location" />
              <SmallIconTypography label="Enter the name of the desks to add to this location." />

              <FormFieldLabel label="Optional name prefix" useWiderSpace>
                <TextField name="namePrefix" required={requiredFields.namePrefix} helperText="Add your desk name prefix" />
              </FormFieldLabel>

              <FormFieldLabel label="Count" useWiderSpace>
                <TextField name="count" required={requiredFields.count} helperText="Add number of the desks to add" />
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

const MemoBulkAddDeskDialog = memo(BulkAddDeskDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  organizationId: string;
  locationId: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const BulkAddDeskDialogWithRelay = ({ onReloadRequired, organizationId, locationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<bulkAddDeskDialog_rootQuery>(RootQuery);
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
      <MemoBulkAddDeskDialog
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

export default memo(BulkAddDeskDialogWithRelay);
