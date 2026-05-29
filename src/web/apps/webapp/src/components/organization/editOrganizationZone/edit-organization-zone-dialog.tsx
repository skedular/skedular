import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import type { editOrganizationZoneDialog_rootQuery } from '@/queries/__generated__/editOrganizationZoneDialog_rootQuery.graphql';
import type { editOrganizationZoneDialog_updateZoneMutation } from '@/queries/__generated__/editOrganizationZoneDialog_updateZoneMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import { ColorPicker, FormFieldLabel, FormStackColumn, PageHeaderPanel, SettingsSectionCard, SmallIconTypography, StackColumn, StickyReviewRail } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<editOrganizationZoneDialog_rootQuery, Record<string, unknown>>;
  zoneId: string;
  onSaved: () => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query editOrganizationZoneDialog_rootQuery($zoneId: String!) {
    zone(id: $zoneId) {
      id
      name
      description
      color
    }
  }
`;

type ZoneDetails = {
  name: string;
  description: string | null | undefined;
};

type TagPatchField = 'NAME' | 'DESCRIPTION' | 'COLOR';

type ZonePatchDetails = ZoneDetails & {
  color: string | null | undefined;
};

const inlinePatchDebounceTimeout = 1000;

const getChangedZoneFields = (left: ZonePatchDetails, right: ZonePatchDetails): TagPatchField[] => {
  const fieldsToUpdate: TagPatchField[] = [];
  if (left.name !== right.name) {
    fieldsToUpdate.push('NAME');
  }
  if (left.description !== right.description) {
    fieldsToUpdate.push('DESCRIPTION');
  }
  if (left.color !== right.color) {
    fieldsToUpdate.push('COLOR');
  }

  return fieldsToUpdate;
};

const zoneSchema = object({
  name: string().required('Zone name is required'),
  description: string().nullable(),
});

const EditOrganizationZonePageComponent = ({ queryReference, zoneId, onCancel }: Props) => {
  const rootData = usePreloadedQuery<editOrganizationZoneDialog_rootQuery>(RootQuery, queryReference);
  const [commitUpdateZonePatch] = useMutation<editOrganizationZoneDialog_updateZoneMutation>(graphql`
    mutation editOrganizationZoneDialog_updateZoneMutation($input: UpdateOrganizationTagInput!) @raw_response_type {
      updateZone(input: $input) {
        organizationTag {
          id
          name
          description
          color
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(zoneSchema);
  const requiredFields = makeRequired(zoneSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.zone?.color);
  const initialZoneValues = useMemo<ZonePatchDetails>(
    () => ({
      name: rootData.zone?.name ?? '',
      description: rootData.zone?.description,
      color: rootData.zone?.color,
    }),
    [rootData.zone],
  );
  const draftZoneValues = useRef(initialZoneValues);
  const submittedZoneValues = useRef(initialZoneValues);

  const commitZonePatch = useCallback(
    (fieldsToUpdate: TagPatchField[], values: ZonePatchDetails) => {
      const zone = rootData.zone;
      if (!zone || fieldsToUpdate.length === 0 || !zoneSchema.isValidSync({ name: values.name, description: values.description })) {
        return;
      }

      const previousValues = submittedZoneValues.current;
      if (getChangedZoneFields(previousValues, values).length === 0) {
        return;
      }
      submittedZoneValues.current = values;

      commitUpdateZonePatch({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: zoneId,
            fieldsToUpdate,
            name: values.name,
            description: values.description,
            color: values.color,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            submittedZoneValues.current = previousValues;
            themedToast(<NotificationContent content={`Failed to update zone '${zone.name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
          }
        },
        onError: (error) => {
          submittedZoneValues.current = previousValues;
          themedToast(<NotificationContent content={`Failed to update zone '${zone.name}'. Error: ${error.message}.`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateZone: {
            organizationTag: {
              id: zoneId,
              name: values.name,
              description: values.description,
              color: values.color,
            },
          },
        },
      });
    },
    [commitUpdateZonePatch, rootData.zone, themedToast, zoneId],
  );
  const debouncedCommitZonePatch = useDebounceCallback(commitZonePatch, inlinePatchDebounceTimeout);

  if (!rootData.zone) {
    return null;
  }

  return (
    <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
      <Box sx={{ maxWidth: 1320, mx: 'auto', display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', xl: 'minmax(0, 2fr) 320px' }, gap: 2 }}>
        <StackColumn spacing={2.5} sx={{ minWidth: 0 }}>
          <PageHeaderPanel
            title="Edit zone"
            description="Update the zone name, description, and colour used across resources and availability filters."
            actions={
              <Button type="button" variant="text" onClick={onCancel} sx={{ textTransform: 'none' }}>
                Cancel
              </Button>
            }
          />

          <Form
            onSubmit={() => undefined}
            initialValues={{ name: rootData.zone.name, description: rootData.zone.description }}
            validate={validate}
            render={({ handleSubmit, values }) => {
              const zoneValues = values as ZoneDetails;
              const nextZoneValues = { ...zoneValues, color: selectedColor };
              const changedFields = getChangedZoneFields(draftZoneValues.current, nextZoneValues);
              if (changedFields.length > 0) {
                draftZoneValues.current = nextZoneValues;
                debouncedCommitZonePatch(changedFields, nextZoneValues);
              }

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <SettingsSectionCard title="Zone details" description="Keep the zone label clear for operators applying it across the organisation.">
                    <StackColumn spacing={2}>
                      <FormFieldLabel label="Name">
                        <TextField name="name" required={requiredFields.name} helperText="Use a short, recognisable zone name." />
                      </FormFieldLabel>

                      <FormFieldLabel label="Description">
                        <TextField name="description" required={requiredFields.description} multiline rows={3} />
                      </FormFieldLabel>
                    </StackColumn>
                  </SettingsSectionCard>

                  <SettingsSectionCard title="Appearance" description="Choose a colour so this zone is easy to recognise in lists and filters.">
                    <FormFieldLabel label="Colour">
                      <ColorPicker onChange={setSelectedColor} defaultColor={rootData.zone?.color} />
                    </FormFieldLabel>
                  </SettingsSectionCard>
                </FormStackColumn>
              );
            }}
          />
        </StackColumn>

        <StickyReviewRail title="Zone help" description="Changes apply wherever this zone is already used.">
          <SettingsSectionCard title="Before saving" description="Avoid renaming tags in a way that changes their operational meaning unexpectedly.">
            <StackColumn spacing={1}>
              <SmallIconTypography label="Keep names consistent with how operators search and filter." />
              <SmallIconTypography label="Use the description for usage rules or edge cases." />
            </StackColumn>
          </SettingsSectionCard>
        </StickyReviewRail>
      </Box>
    </Box>
  );
};

const MemoEditOrganizationZonePage = memo(EditOrganizationZonePageComponent);

type RelayProps = {
  zoneId: string;
  onSaved: () => void;
  onCancel: () => void;
};

const EditOrganizationZonePageWithRelay = ({ zoneId, onSaved, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<editOrganizationZoneDialog_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery({ zoneId }, { fetchPolicy: 'store-and-network' });
  }, [zoneId, loadQuery]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoEditOrganizationZonePage queryReference={queryReference} zoneId={zoneId} onSaved={onSaved} onCancel={onCancel} />
    </ErrorBoundary>
  );
};

export const EditOrganizationZonePage = memo(EditOrganizationZonePageWithRelay);

export default EditOrganizationZonePage;
