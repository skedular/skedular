import { PaletteModeContext, RelayError, getRelayErrorMessage, toRootError, useIntegratedPlatform } from '@skedular/shared';
import { Loading } from '@/components/loading';
import { getOrganizationLocationManageResourcesBaseLink } from '@/components/links';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';

import BulkAddResourceRowForm from '@/components/resource/bulkAddResources/bulk-add-resources-row';
import { DialogTransition } from '@/components/transitions';
import type { bulkAddResourcesDialog_bulkAddResourcesMutation } from '@/queries/__generated__/bulkAddResourcesDialog_bulkAddResourcesMutation.graphql';
import type { bulkAddResourcesDialog_rootQuery } from '@/queries/__generated__/bulkAddResourcesDialog_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';

import {
  BodyIconTypography,
  DefaultDialogTitle,
  EditorActionBar,
  FormStackColumn,
  LeadIconTypography,
  PageHeaderPanel,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  StackRow,
  StickyReviewRail,
  TwoButtonsDialogActions,
} from '@skedular/ui';
import type { FormApi } from 'final-form';
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

// ─── Types ───────────────────────────────────────────────────────────────────

type RowData = {
  resourceTypeId: string;
  baseName: string;
  quantity: number;
  customTagIds: string[];
  zoneIds: string[];
};

type RowResult = {
  rowIndex: number;
  createdResources: ReadonlyArray<{ readonly id: string; readonly name: string }>;
  failureReason: string | null | undefined;
};

type FormValues = {
  rows: RowData[];
};

type ChangeFormValue = FormApi<FormValues>['change'];

// ─── GraphQL ─────────────────────────────────────────────────────────────────

const RootQuery = graphql`
  query bulkAddResourcesDialog_rootQuery(
    $organizationCustomDomain: String!
    $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]
  ) {
    organization(customDomain: $organizationCustomDomain) {
      type {
        type
      }
    }
    ...singleChoiceResourceType_query
    ...multipleChoicesCustomTags_query
    ...multipleChoicesZones_query
  }
`;

// ─── Empty row factory ───────────────────────────────────────────────────────

const makeEmptyRow = (): RowData => ({
  resourceTypeId: '',
  baseName: '',
  quantity: 1,
  customTagIds: [],
  zoneIds: [],
});

// ─── Inner dialog (receives preloaded query) ─────────────────────────────────

type InnerProps = {
  queryReference: PreloadedQuery<bulkAddResourcesDialog_rootQuery, Record<string, unknown>>;
  locationId: string;
  organizationCustomDomain: string;
  onReloadRequired?: () => void;
  onCancel: () => void;
  presentation?: 'dialog' | 'page';
};

const BulkAddResourcesDialogInner = ({ queryReference, locationId, organizationCustomDomain, onReloadRequired, onCancel, presentation = 'dialog' }: InnerProps) => {
  const rootData = usePreloadedQuery<bulkAddResourcesDialog_rootQuery>(RootQuery, queryReference);

  const [commitBulkAdd, isMutationInFlight] = useMutation<bulkAddResourcesDialog_bulkAddResourcesMutation>(graphql`
    mutation bulkAddResourcesDialog_bulkAddResourcesMutation($input: BulkAddResourcesInput!) {
      bulkAddResources(input: $input) {
        clientMutationId
        results {
          rowIndex
          createdResources {
            id
            name
          }
          failureReason
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;

  // rowKeys keeps stable React keys for each row; form state holds the data
  const [rowKeys, setRowKeys] = useState<string[]>([uuid()]);
  const [results, setResults] = useState<RowResult[] | null>(null);
  const [submittedValues, setSubmittedValues] = useState<FormValues | null>(null);
  const [formKey, setFormKey] = useState(0);
  const [currentInitialValues, setCurrentInitialValues] = useState<FormValues>({ rows: [makeEmptyRow()] });

  const isResultsView = results !== null;

  const totalCreated = results?.reduce((sum, r) => sum + r.createdResources.length, 0) ?? 0;
  const totalFailed = results?.filter((r) => r.failureReason).length ?? 0;

  const handleAddRow = (formValues: FormValues, changeForm: ChangeFormValue) => {
    const newRows = [...formValues.rows, makeEmptyRow()];
    changeForm('rows', newRows);
    setRowKeys((prev) => [...prev, uuid()]);
  };

  const handleRemoveRow = (index: number, formValues: FormValues, changeForm: ChangeFormValue) => {
    const newRows = formValues.rows.filter((_, i) => i !== index);
    changeForm('rows', newRows);
    setRowKeys((prev) => prev.filter((_, i) => i !== index));
  };

  const handleRetryFailed = () => {
    const failedRows = results?.filter((r) => r.failureReason).map((r) => submittedValues?.rows[r.rowIndex] ?? makeEmptyRow()) ?? [];
    const retryValues: FormValues = { rows: failedRows.length > 0 ? failedRows : [makeEmptyRow()] };
    setCurrentInitialValues(retryValues);
    setRowKeys(retryValues.rows.map(() => uuid()));
    setFormKey((k) => k + 1);
    setResults(null);
    setSubmittedValues(null);
  };

  const handleSubmit = (values: FormValues) => {
    // Store submitted values for potential retry
    setSubmittedValues(values);

    commitBulkAdd({
      variables: {
        input: {
          clientMutationId: uuid(),
          locationId,
          rows: values.rows.map((row) => ({
            organizationResourceTypeTagId: row.resourceTypeId,
            baseName: row.baseName || null,
            quantity: row.quantity,
            customTagIds: row.customTagIds ?? [],
            zoneIds: row.zoneIds ?? [],
            productTagIds: [],
          })),
        },
      },
      onCompleted: (data, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Add failed. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }

        const rowResults = data.bulkAddResources?.results ?? [];
        const createdCount = rowResults.reduce((sum, r) => sum + r.createdResources.length, 0);
        const failedCount = rowResults.filter((r) => r.failureReason).length;

        if (failedCount === 0) {
        } else {
          themedToast(<NotificationContent content={`${createdCount} imported, ${failedCount} failed. See details below.`} />, errorNotificationOptions);
        }

        setResults(rowResults as RowResult[]);

        if (createdCount > 0 && onReloadRequired) {
          onReloadRequired();
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Add failed. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  // ─── Results view ─────────────────────────────────────────────────────────

  if (isResultsView) {
    const resultsContent = (
      <StackColumn>
        <StackRow sx={{ gap: 2 }}>
          <Chip label={`${totalCreated} created`} color="success" variant="outlined" />
          {totalFailed > 0 && <Chip label={`${totalFailed} failed`} color="error" variant="outlined" />}
        </StackRow>

        <StackColumn spacing={1}>
          {results.map((result) => (
            <Box
              key={result.rowIndex}
              sx={{
                border: 1,
                borderColor: result.failureReason ? 'error.main' : 'success.main',
                borderRadius: 2,
                p: 1.5,
                backgroundColor: 'background.default',
              }}
            >
              <SmallIconTypography label={`Row ${result.rowIndex + 1}`} />
              {result.failureReason ? (
                <BodyIconTypography label={`Failed: ${result.failureReason}`} />
              ) : (
                <BodyIconTypography label={`Created: ${result.createdResources.map((r) => r.name).join(', ')}`} />
              )}
            </Box>
          ))}
        </StackColumn>

        <StackRow sx={{ gap: 1, justifyContent: 'flex-end' }}>
          {totalFailed > 0 && (
            <Button
              variant="outlined"
              onClick={() => {
                handleRetryFailed();
              }}
              sx={{ textTransform: 'none' }}
            >
              <SmallIconTypography label="Retry failed rows" />
            </Button>
          )}
          <Button variant="contained" onClick={onCancel} sx={{ textTransform: 'none' }}>
            <SmallIconTypography label="Done" />
          </Button>
        </StackRow>
      </StackColumn>
    );

    if (presentation === 'page') {
      return (
        <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
          <Box sx={{ maxWidth: 1320, mx: 'auto', display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', xl: 'minmax(0, 2fr) 320px' }, gap: 2 }}>
            <StackColumn spacing={2.5} sx={{ minWidth: 0 }}>
              <PageHeaderPanel title="Bulk add resources" description="Review the result of the resource import and retry any rows that need attention." />
              <SettingsSectionCard title="Import results" description="Each row is processed independently, so successful rows can stay created while failed rows are corrected.">
                {resultsContent}
              </SettingsSectionCard>
            </StackColumn>

            <StickyReviewRail title="Result summary" description="Use retry for failed rows, or finish when the created resources look correct.">
              <SettingsSectionCard title="Outcome" description="Resources created from this import.">
                <StackColumn spacing={1}>
                  <SmallIconTypography label={`${totalCreated} resource${totalCreated === 1 ? '' : 's'} created.`} />
                  <SmallIconTypography label={`${totalFailed} row${totalFailed === 1 ? '' : 's'} need attention.`} />
                </StackColumn>
              </SettingsSectionCard>
            </StickyReviewRail>
          </Box>
        </Box>
      );
    }

    return (
      <Dialog slots={{ transition: DialogTransition }} open={true} onClose={onCancel} fullWidth>
        <DefaultDialogTitle title="Results" />
        <DialogContent sx={{ marginTop: 2 }}>{resultsContent}</DialogContent>
      </Dialog>
    );
  }

  // ─── Compose view ─────────────────────────────────────────────────────────

  const form = (
    <Form<FormValues>
      key={formKey}
      onSubmit={handleSubmit}
      initialValues={currentInitialValues}
      render={({ handleSubmit: submitForm, values, form }) => (
        <FormStackColumn onSubmit={submitForm}>
          {presentation === 'dialog' ? (
            <>
              <LeadIconTypography label="Add multiple resources at once" />
              <SmallIconTypography label="Each row generates one or more resources with auto-assigned names." />
            </>
          ) : (
            <PageHeaderPanel title="Bulk add resources" description="Create several resources in one pass by adding rows for each type, naming pattern, and quantity." />
          )}

          {presentation === 'page' ? (
            <SettingsSectionCard
              title="Resource rows"
              description="Each row can create one or more resources with the same classification. Add another row for a different resource type, zone, or tag set."
            >
              <StackColumn spacing={2}>
                {values.rows.map((_, index) => (
                  <BulkAddResourceRowForm
                    key={rowKeys[index] ?? index}
                    rowIndex={index}
                    rootDataRelay={rootData}
                    organizationCustomDomain={organizationCustomDomain}
                    onRemove={() => handleRemoveRow(index, values, form.change)}
                  />
                ))}

                <Button variant="outlined" onClick={() => handleAddRow(values, form.change)} disabled={isMutationInFlight} sx={{ alignSelf: 'flex-start', textTransform: 'none' }}>
                  <SmallIconTypography label="Add row" />
                </Button>
              </StackColumn>
            </SettingsSectionCard>
          ) : (
            <>
              {values.rows.map((_, index) => (
                <BulkAddResourceRowForm
                  key={rowKeys[index] ?? index}
                  rowIndex={index}
                  rootDataRelay={rootData}
                  organizationCustomDomain={organizationCustomDomain}
                  onRemove={() => handleRemoveRow(index, values, form.change)}
                />
              ))}

              <Button variant="outlined" onClick={() => handleAddRow(values, form.change)} disabled={isMutationInFlight} sx={{ alignSelf: 'flex-start', textTransform: 'none' }}>
                <SmallIconTypography label="Add row" />
              </Button>
            </>
          )}

          {presentation === 'page' ? (
            <EditorActionBar
              secondaryActions={
                <Button type="button" variant="text" onClick={onCancel} sx={{ textTransform: 'none' }}>
                  Cancel
                </Button>
              }
              primaryAction="Add resources"
            />
          ) : (
            <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Add" secondaryLabel="Cancel" primaryDisabled={isMutationInFlight || values.rows.length === 0} />
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

          <StickyReviewRail title="Bulk add help" description="Use this when several resources share the same setup.">
            <SettingsSectionCard title="How rows work" description="Rows are independent and can be retried individually if one fails.">
              <StackColumn spacing={1}>
                <SmallIconTypography label="Quantity controls how many resources are generated from that row." />
                <SmallIconTypography label="Use a base name that can produce clear resource names." />
                <SmallIconTypography label="Create separate rows when resources need different tags, zones, or booking groups." />
              </StackColumn>
            </SettingsSectionCard>

            <SettingsSectionCard title="Before submitting" description="Check the generated set matches the real location layout.">
              <StackColumn spacing={1}>
                <SmallIconTypography label="Start with one row, then add more rows for different resource groups." />
                <SmallIconTypography label="You can retry failed rows after the import completes." />
              </StackColumn>
            </SettingsSectionCard>
          </StickyReviewRail>
        </Box>
      </Box>
    );
  }

  return (
    <Dialog slots={{ transition: DialogTransition }} open={true} onClose={onCancel} fullWidth maxWidth="md">
      <DefaultDialogTitle title="Bulk Add Resources" />
      <DialogContent sx={{ marginTop: 2 }}>{form}</DialogContent>
    </Dialog>
  );
};

const MemoBulkAddResourcesDialogInner = memo(BulkAddResourcesDialogInner);

// ─── Outer wrapper with query loader ─────────────────────────────────────────

type Props = {
  locationId: string;
  organizationCustomDomain: string;
  isDialogOpen: boolean;
  onReloadRequired?: () => void;
  onCancel: () => void;
  presentation?: 'dialog' | 'page';
};

const BulkAddResourcesDialog = ({ locationId, organizationCustomDomain, isDialogOpen, onReloadRequired, onCancel, presentation = 'dialog' }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<bulkAddResourcesDialog_rootQuery>(RootQuery);

  useEffect(() => {
    if (!isDialogOpen) {
      return;
    }

    loadQuery(
      {
        organizationCustomDomain,
        multipleChoicesCustomTagsSortingValues: [{ direction: 'ASCENDING', field: 'NAME' }],
        multipleChoicesZonesSortingValues: [{ direction: 'ASCENDING', field: 'NAME' }],
      },
      { fetchPolicy: 'store-and-network' },
    );
  }, [isDialogOpen, loadQuery, organizationCustomDomain]);

  if (!isDialogOpen) {
    return null;
  }

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoBulkAddResourcesDialogInner
        queryReference={queryReference}
        locationId={locationId}
        organizationCustomDomain={organizationCustomDomain}
        onReloadRequired={onReloadRequired}
        onCancel={onCancel}
        presentation={presentation}
      />
    </ErrorBoundary>
  );
};

type PageProps = {
  organizationCustomDomain: string;
  locationId: string;
};

const BulkAddResourcesPageComponent = ({ organizationCustomDomain, locationId }: PageProps) => {
  const router = useRouter();
  const { integratedPlatform } = useIntegratedPlatform();

  const handleDone = () => {
    router.push(getOrganizationLocationManageResourcesBaseLink(integratedPlatform, organizationCustomDomain, locationId));
  };

  return <BulkAddResourcesDialog locationId={locationId} organizationCustomDomain={organizationCustomDomain} isDialogOpen={true} onCancel={handleDone} presentation="page" />;
};

export const BulkAddResourcesPage = memo(BulkAddResourcesPageComponent);

export default memo(BulkAddResourcesDialog);
