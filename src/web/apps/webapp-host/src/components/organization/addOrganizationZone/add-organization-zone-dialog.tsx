import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import type { addOrganizationZoneDialog_addZoneMutation } from '@/queries/__generated__/addOrganizationZoneDialog_addZoneMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
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

type ZoneDetails = {
  name: string;
  description: string | null | undefined;
};

const zoneSchema = object({
  name: string().required('Zone name is required'),
  description: string().nullable(),
});

const AddOrganizationZonePageComponent = ({ organizationCustomDomain, connectionIds = [], onAddClicked, onCancel }: Props) => {
  const [commitAddZone] = useMutation<addOrganizationZoneDialog_addZoneMutation>(graphql`
    mutation addOrganizationZoneDialog_addZoneMutation($connectionIds: [ID!]!, $input: AddZoneInput!) @raw_response_type {
      addZone(input: $input) {
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
  const validate = makeValidate(zoneSchema);
  const requiredFields = makeRequired(zoneSchema);
  const [selectedColor, setSelectedColor] = useState('');

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: ZoneDetails) => {
    const id = uuid();

    commitAddZone({
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
          themedToast(<NotificationContent content={`Failed to add zone '${name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);

          return;
        }

        onAddClicked();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add zone '${name}'. Error: ${error.message}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addZone: {
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
          <PageHeaderPanel title="Add zone" description="Create a place-based zone used to group resources, floor plans, and availability filters." />

          <Form
            onSubmit={handleAddClick}
            initialValues={{}}
            validate={validate}
            render={({ handleSubmit }) => {
              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <SettingsSectionCard title="Zone details" description="Set the name and description operators will see when assigning this zone.">
                    <StackColumn spacing={2}>
                      <FormFieldLabel label="Name">
                        <TextField name="name" required={requiredFields.name} helperText="Use a clear place name such as Level 2 or North Wing." />
                      </FormFieldLabel>

                      <FormFieldLabel label="Description">
                        <TextField name="description" required={requiredFields.description} multiline rows={3} />
                      </FormFieldLabel>
                    </StackColumn>
                  </SettingsSectionCard>

                  <SettingsSectionCard title="Appearance" description="Choose a colour so this zone is easy to recognise in resource lists.">
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
                    primaryAction="Add zone"
                  />
                </FormStackColumn>
              );
            }}
          />
        </StackColumn>

        <StickyReviewRail title="Zone help" description="Zones work best when they map to real areas users recognise.">
          <SettingsSectionCard title="Suggested setup" description="Keep zones broad enough to help filtering without creating noise.">
            <StackColumn spacing={1}>
              <SmallIconTypography label="Use zones for floors, wings, neighbourhoods, or resource clusters." />
              <SmallIconTypography label="Avoid duplicating tags that are not location-based." />
              <SmallIconTypography label="Pick colours that make adjacent zones easy to tell apart." />
            </StackColumn>
          </SettingsSectionCard>

          <SettingsSectionCard title="After adding" description="The zone can be assigned from resource and floor-plan setup screens.">
            <StackColumn spacing={1}>
              <SmallIconTypography label="Return to the previous page to apply it where needed." />
            </StackColumn>
          </SettingsSectionCard>
        </StickyReviewRail>
      </Box>
    </Box>
  );
};

export const AddOrganizationZonePage = memo(AddOrganizationZonePageComponent);

export default AddOrganizationZonePage;
