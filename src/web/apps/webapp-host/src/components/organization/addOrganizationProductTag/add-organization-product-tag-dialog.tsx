import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import type { addOrganizationProductTagDialog_addProductTagMutation } from '@/queries/__generated__/addOrganizationProductTagDialog_addProductTagMutation.graphql';
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

type ProductTagDetails = {
  name: string;
  description: string | null | undefined;
};

const productTagSchema = object({
  name: string().required('Product tag name is required'),
  description: string().nullable(),
});

const AddOrganizationProductTagPageComponent = ({ organizationCustomDomain, connectionIds = [], onAddClicked, onCancel }: Props) => {
  const [commitAddProductTag] = useMutation<addOrganizationProductTagDialog_addProductTagMutation>(graphql`
    mutation addOrganizationProductTagDialog_addProductTagMutation($connectionIds: [ID!]!, $input: AddProductTagInput!) @raw_response_type {
      addProductTag(input: $input) {
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
  const validate = makeValidate(productTagSchema);
  const requiredFields = makeRequired(productTagSchema);
  const [selectedColor, setSelectedColor] = useState('');

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: ProductTagDetails) => {
    const id = uuid();

    commitAddProductTag({
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
          themedToast(<NotificationContent content={`We couldn't add the product tag '${name}'. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        onAddClicked();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't add the product tag '${name}'. ${error.message}`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addProductTag: {
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
          <PageHeaderPanel title="Add product tag" description="Create a customer-facing product tag for marketplace listings and resource-product matching." />

          <Form
            onSubmit={handleAddClick}
            initialValues={{}}
            validate={validate}
            render={({ handleSubmit }) => {
              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <SettingsSectionCard title="Product tag details" description="Set the label and description shown when products and resources are grouped.">
                    <StackColumn spacing={2}>
                      <FormFieldLabel label="Name">
                        <TextField name="name" required={requiredFields.name} helperText="Use a clear customer-facing category name." />
                      </FormFieldLabel>

                      <FormFieldLabel label="Description">
                        <TextField name="description" required={requiredFields.description} multiline rows={3} />
                      </FormFieldLabel>
                    </StackColumn>
                  </SettingsSectionCard>

                  <SettingsSectionCard title="Appearance" description="Choose a colour so this product tag is easy to recognise in marketplace setup.">
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
                    primaryAction="Add product tag"
                  />
                </FormStackColumn>
              );
            }}
          />
        </StackColumn>

        <StickyReviewRail title="Product tag help" description="Product tags connect marketplace products with the resources they can book.">
          <SettingsSectionCard title="Suggested setup" description="Use product tags as customer-facing categories.">
            <StackColumn spacing={1}>
              <SmallIconTypography label="Keep names aligned with product categories customers understand." />
              <SmallIconTypography label="Use descriptions to explain when operators should apply the tag." />
              <SmallIconTypography label="Assign the tag to matching resources after creating it." />
            </StackColumn>
          </SettingsSectionCard>

          <SettingsSectionCard title="After adding" description="The product tag can be used in product and resource setup.">
            <StackColumn spacing={1}>
              <SmallIconTypography label="Return to the previous page to apply it where needed." />
            </StackColumn>
          </SettingsSectionCard>
        </StickyReviewRail>
      </Box>
    </Box>
  );
};

export const AddOrganizationProductTagPage = memo(AddOrganizationProductTagPageComponent);

export default AddOrganizationProductTagPage;
