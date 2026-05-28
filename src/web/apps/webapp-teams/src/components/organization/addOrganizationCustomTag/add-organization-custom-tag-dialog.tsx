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
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext } from '@skedular/shared';
import { getRelayErrorMessage } from '@skedular/shared';
import type { addOrganizationCustomTagDialog_addCustomTagMutation } from '@/queries/__generated__/addOrganizationCustomTagDialog_addCustomTagMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  organizationCustomDomain: string;
  connectionIds?: string[];
  onAddClicked: () => void;
  onCancel: () => void;
};

type CustomTagDetails = {
  name: string;
  description: string | null | undefined;
};

const customTagSchema = object({
  name: string().required('Tag name is required'),
  description: string().nullable(),
});

const AddOrganizationCustomTagPageComponent = ({ organizationCustomDomain, connectionIds = [], onAddClicked, onCancel }: Props) => {
  const [commitAddCustomTag] = useMutation<addOrganizationCustomTagDialog_addCustomTagMutation>(graphql`
    mutation addOrganizationCustomTagDialog_addCustomTagMutation($connectionIds: [ID!]!, $input: AddCustomTagInput!) @raw_response_type {
      addCustomTag(input: $input) {
        organizationTag @appendNode(connections: $connectionIds, edgeTypeName: "OrganizationTagDetails") {
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
  const [selectedColor, setSelectedColor] = useState('');

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: CustomTagDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding tag '${name}'...`} />, infoNotificationOptions);

    commitAddCustomTag({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          organizationCustomDomain,
          name,
          description,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add tag '${name}'. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag ${name} added.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add tag '${name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addCustomTag: {
          organizationTag: {
            id,
            name,
            description,
            color: selectedColor,
          },
        },
      },
    });
  };

  return (
    <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
      <Box sx={{ maxWidth: 1320, mx: 'auto', display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', xl: 'minmax(0, 2fr) 320px' }, gap: 2 }}>
        <StackColumn spacing={2.5} sx={{ minWidth: 0 }}>
          <PageHeaderPanel title="Add tag" description="Create a reusable tag for filtering resources, bookings, and organisation preferences." />

          <Form
            onSubmit={handleAddClick}
            initialValues={{}}
            validate={validate}
            render={({ handleSubmit }) => {
              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <SettingsSectionCard title="Tag details" description="Set the label and description operators will use when assigning this tag.">
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
                      <ColorPicker onChange={handleColorChange} />
                    </FormFieldLabel>
                  </SettingsSectionCard>

                  <EditorActionBar
                    secondaryActions={
                      <Button type="button" variant="text" onClick={onCancel} sx={{ textTransform: 'none' }}>
                        Cancel
                      </Button>
                    }
                    primaryAction="Add tag"
                  />
                </FormStackColumn>
              );
            }}
          />
        </StackColumn>

        <StickyReviewRail title="Tag help" description="Tags make resource and booking lists easier to scan.">
          <SettingsSectionCard title="Suggested setup" description="Keep the tag library compact and meaningful.">
            <StackColumn spacing={1}>
              <SmallIconTypography label="Use names that match how operators search or group resources." />
              <SmallIconTypography label="Add a description when the tag needs a clear usage rule." />
              <SmallIconTypography label="Pick colours that differ from existing tags." />
            </StackColumn>
          </SettingsSectionCard>

          <SettingsSectionCard title="After adding" description="The tag can be assigned from resource and preference setup screens.">
            <StackColumn spacing={1}>
              <SmallIconTypography label="Return to the previous page to apply it where needed." />
            </StackColumn>
          </SettingsSectionCard>
        </StickyReviewRail>
      </Box>
    </Box>
  );
};

export const AddOrganizationCustomTagPage = memo(AddOrganizationCustomTagPageComponent);

export default AddOrganizationCustomTagPage;
