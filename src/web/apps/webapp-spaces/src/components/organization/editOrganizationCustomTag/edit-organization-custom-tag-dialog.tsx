import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import type { editOrganizationCustomTagDialog_rootQuery } from '@/queries/__generated__/editOrganizationCustomTagDialog_rootQuery.graphql';
import type { editOrganizationCustomTagDialog_updateCustomTagMutation } from '@/queries/__generated__/editOrganizationCustomTagDialog_updateCustomTagMutation.graphql';
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
  queryReference: PreloadedQuery<editOrganizationCustomTagDialog_rootQuery, Record<string, unknown>>;
  customTagId: string;
  onSaved: () => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query editOrganizationCustomTagDialog_rootQuery($customTagId: String!) {
    customTag(id: $customTagId) {
      id
      name
      description
      color
    }
  }
`;

type CustomTagDetails = {
  name: string;
  description: string | null | undefined;
};

type TagPatchField = 'NAME' | 'DESCRIPTION' | 'COLOR';

type CustomTagPatchDetails = CustomTagDetails & {
  color: string | null | undefined;
};

const inlinePatchDebounceTimeout = 1000;

const getChangedTagFields = (left: CustomTagPatchDetails, right: CustomTagPatchDetails): TagPatchField[] => {
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

const customTagSchema = object({
  name: string().required('Tag name is required'),
  description: string().nullable(),
});

const EditOrganizationCustomTagPageComponent = ({ queryReference, customTagId, onCancel }: Props) => {
  const rootData = usePreloadedQuery<editOrganizationCustomTagDialog_rootQuery>(RootQuery, queryReference);
  const [commitUpdateCustomTagPatch] = useMutation<editOrganizationCustomTagDialog_updateCustomTagMutation>(graphql`
    mutation editOrganizationCustomTagDialog_updateCustomTagMutation($input: UpdateOrganizationTagInput!) @raw_response_type {
      updateCustomTag(input: $input) {
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
  const validate = makeValidate(customTagSchema);
  const requiredFields = makeRequired(customTagSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.customTag?.color);
  const initialCustomTagValues = useMemo<CustomTagPatchDetails>(
    () => ({
      name: rootData.customTag?.name ?? '',
      description: rootData.customTag?.description,
      color: rootData.customTag?.color,
    }),
    [rootData.customTag],
  );
  const draftCustomTagValues = useRef(initialCustomTagValues);
  const submittedCustomTagValues = useRef(initialCustomTagValues);

  const commitCustomTagPatch = useCallback(
    (fieldsToUpdate: TagPatchField[], values: CustomTagPatchDetails) => {
      const customTag = rootData.customTag;
      if (!customTag || fieldsToUpdate.length === 0 || !customTagSchema.isValidSync({ name: values.name, description: values.description })) {
        return;
      }

      const previousValues = submittedCustomTagValues.current;
      if (getChangedTagFields(previousValues, values).length === 0) {
        return;
      }
      submittedCustomTagValues.current = values;

      commitUpdateCustomTagPatch({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: customTagId,
            fieldsToUpdate,
            name: values.name,
            description: values.description,
            color: values.color,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            submittedCustomTagValues.current = previousValues;
            themedToast(<NotificationContent content={`Failed to update tag '${customTag.name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
          }
        },
        onError: (error) => {
          submittedCustomTagValues.current = previousValues;
          themedToast(<NotificationContent content={`Failed to update tag '${customTag.name}'. Error: ${error.message}.`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateCustomTag: {
            organizationTag: {
              id: customTagId,
              name: values.name,
              description: values.description,
              color: values.color,
            },
          },
        },
      });
    },
    [commitUpdateCustomTagPatch, customTagId, rootData.customTag, themedToast],
  );
  const debouncedCommitCustomTagPatch = useDebounceCallback(commitCustomTagPatch, inlinePatchDebounceTimeout);

  if (!rootData.customTag) {
    return null;
  }

  return (
    <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
      <Box sx={{ maxWidth: 1320, mx: 'auto', display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', xl: 'minmax(0, 2fr) 320px' }, gap: 2 }}>
        <StackColumn spacing={2.5} sx={{ minWidth: 0 }}>
          <PageHeaderPanel
            title="Edit tag"
            description="Update the tag name, description, and colour used across resources, bookings, and preferences."
            actions={
              <Button type="button" variant="text" onClick={onCancel} sx={{ textTransform: 'none' }}>
                Cancel
              </Button>
            }
          />

          <Form
            onSubmit={() => undefined}
            initialValues={{ name: rootData.customTag.name, description: rootData.customTag.description }}
            validate={validate}
            render={({ handleSubmit, values }) => {
              const customTagValues = values as CustomTagDetails;
              const nextCustomTagValues = { ...customTagValues, color: selectedColor };
              const changedFields = getChangedTagFields(draftCustomTagValues.current, nextCustomTagValues);
              if (changedFields.length > 0) {
                draftCustomTagValues.current = nextCustomTagValues;
                debouncedCommitCustomTagPatch(changedFields, nextCustomTagValues);
              }

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <SettingsSectionCard title="Tag details" description="Keep the tag label clear for operators applying it across the organisation.">
                    <StackColumn spacing={2}>
                      <FormFieldLabel label="Name">
                        <TextField name="name" required={requiredFields.name} helperText="Use a short, recognisable tag name." />
                      </FormFieldLabel>

                      <FormFieldLabel label="Description">
                        <TextField name="description" required={requiredFields.description} multiline rows={3} />
                      </FormFieldLabel>
                    </StackColumn>
                  </SettingsSectionCard>

                  <SettingsSectionCard title="Appearance" description="Choose a colour so this tag is easy to recognise in lists and filters.">
                    <FormFieldLabel label="Colour">
                      <ColorPicker onChange={setSelectedColor} defaultColor={rootData.customTag?.color} />
                    </FormFieldLabel>
                  </SettingsSectionCard>
                </FormStackColumn>
              );
            }}
          />
        </StackColumn>

        <StickyReviewRail title="Tag help" description="Changes apply wherever this tag is already used.">
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

const MemoEditOrganizationCustomTagPage = memo(EditOrganizationCustomTagPageComponent);

type RelayProps = {
  customTagId: string;
  onSaved: () => void;
  onCancel: () => void;
};

const EditOrganizationCustomTagPageWithRelay = ({ customTagId, onSaved, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<editOrganizationCustomTagDialog_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery({ customTagId }, { fetchPolicy: 'store-and-network' });
  }, [customTagId, loadQuery]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoEditOrganizationCustomTagPage queryReference={queryReference} customTagId={customTagId} onSaved={onSaved} onCancel={onCancel} />
    </ErrorBoundary>
  );
};

export const EditOrganizationCustomTagPage = memo(EditOrganizationCustomTagPageWithRelay);

export default EditOrganizationCustomTagPage;
