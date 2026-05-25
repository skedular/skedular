import {
  ColorPicker,
  EditorActionBar,
  FormFieldLabel,
  FormStackColumn,
  PageHeaderPanel,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  StickyReviewRail,
} from '@skedular/ui';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { PaletteModeContext } from '@skedular/shared';
import { getRelayErrorMessage } from '@skedular/shared';
import type { editOrganizationZoneDialog_rootQuery } from '@/queries/__generated__/editOrganizationZoneDialog_rootQuery.graphql';
import type { editOrganizationZoneDialog_updateZoneMutation } from '@/queries/__generated__/editOrganizationZoneDialog_updateZoneMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
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

const zoneSchema = object({
  name: string().required('Zone name is required'),
  description: string().nullable(),
});

const EditOrganizationZonePageComponent = ({ queryReference, zoneId, onSaved, onCancel }: Props) => {
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

  const handleAddClick = ({ name, description }: ZoneDetails) => {
    if (!rootData.zone) {
      return;
    }

    const oldName = rootData.zone.name;
    const fieldsToUpdate: TagPatchField[] = [];
    if (rootData.zone.name !== name) {
      fieldsToUpdate.push('NAME');
    }
    if (rootData.zone.description !== description) {
      fieldsToUpdate.push('DESCRIPTION');
    }
    if (rootData.zone.color !== selectedColor) {
      fieldsToUpdate.push('COLOR');
    }
    if (fieldsToUpdate.length === 0) {
      onSaved();
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating zone '${oldName}'...`} />, infoNotificationOptions);

    commitUpdateZonePatch({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: zoneId,
          fieldsToUpdate,
          name,
          description,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update zone '${oldName}'. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone ${name} updated.`} />,
        });

        onSaved();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update zone '${oldName}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateZone: {
          organizationTag: {
            id: zoneId,
            name,
            description,
            color: selectedColor,
          },
        },
      },
    });
  };

  if (!rootData.zone) {
    return null;
  }

  return (
    <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
      <Box sx={{ maxWidth: 1320, mx: 'auto', display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', xl: 'minmax(0, 2fr) 320px' }, gap: 2 }}>
        <StackColumn spacing={2.5} sx={{ minWidth: 0 }}>
          <PageHeaderPanel title="Edit zone" description="Update the zone name, description, and colour used across resources and availability filters." />

          <Form
            onSubmit={handleAddClick}
            initialValues={{ name: rootData.zone.name, description: rootData.zone.description }}
            validate={validate}
            render={({ handleSubmit }) => (
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

                <EditorActionBar
                  secondaryActions={
                    <Button type="button" variant="text" onClick={onCancel} sx={{ textTransform: 'none' }}>
                      Cancel
                    </Button>
                  }
                  primaryAction="Save zone"
                />
              </FormStackColumn>
            )}
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
