import { MultipleChoicesCustomTags, MultipleChoicesZones } from '@/components/organization';
import type { editDesk_query$key } from '@/queries/__generated__/editDesk_query.graphql';
import type { editDesk_updateDeskMutation } from '@/queries/__generated__/editDesk_updateDeskMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  FormFieldLabel,
  FormStackColumn,
  SectionIconTypography,
  StackColumn,
  StackRow,
} from '@repo/shared/components/commons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';

type Props = {
  rootDataRelay: editDesk_query$key;
  onReloadRequired?: () => void;
  organizationId: string;
};

type DeskDetails = {
  name: string;
  customTagIds: string[];
  zoneIds: string[];
};

const deskSchema = object({
  name: string().required('Desk name is required'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
});

const EditDesk = ({ rootDataRelay, organizationId }: Props) => {
  const rootData = useFragment<editDesk_query$key>(
    graphql`
      fragment editDesk_query on Query {
        desk(id: $deskId) {
          id
          name
          deactivated
          requireBookingApproval
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
        }
        ...multipleChoicesCustomTags_query
        ...multipleChoicesZones_query
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateDesk] = useMutation<editDesk_updateDeskMutation>(graphql`
    mutation editDesk_updateDeskMutation($input: UpdateDeskInput!) @raw_response_type {
      updateDesk(input: $input) {
        desk {
          id
          name
          deactivated
          requireBookingApproval
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateDeskDetails = makeValidate(deskSchema);
  const requiredDeskDetailsFields = makeRequired(deskSchema);

  const handleCloseClick = () => {
    router.back();
  };

  const handleDeskDetailUpdateClick = ({ name, customTagIds, zoneIds }: DeskDetails) => {
    if (!rootData.desk) {
      return;
    }

    const oldName = rootData.desk.name;
    const toastId = themedToast(<NotificationContent content={`Updating zone '${oldName}'...`} />, infoNotificationOptions);

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.desk.id,
          name,
          deactivated: rootData.desk.deactivated,
          requireBookingApproval: rootData.desk.requireBookingApproval,
          customTagIds,
          zoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update Desk '${oldName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk ${name} updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update Desk '${oldName}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: rootData.desk.id,
            name,
            deactivated: rootData.desk.deactivated,
            requireBookingApproval: rootData.desk.requireBookingApproval,
            customTags: [],
            zones: [],
          },
        },
      },
    });
  };

  if (!rootData.desk) {
    return <></>;
  }

  const desk = rootData.desk;

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Desk Information">
          <Form
            onSubmit={handleDeskDetailUpdateClick}
            initialValues={{
              name: desk.name,
              customTagIds: desk.customTags.map(({ uniqueId }) => uniqueId),
              zoneIds: desk.zones.map(({ uniqueId }) => uniqueId),
            }}
            validate={validateDeskDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Desk Setup" />
                  <BodyIconTypography label="Edit your desk name and details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredDeskDetailsFields.name} helperText="Add your desk name" />
                  </FormFieldLabel>

                  {organizationId && (
                    <FormFieldLabel label="Tags">
                      <MultipleChoicesCustomTags rootDataRelay={rootData} name="customTagIds" required={requiredDeskDetailsFields.customTagIds} />
                    </FormFieldLabel>
                  )}

                  {organizationId && (
                    <FormFieldLabel label="Zones">
                      <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredDeskDetailsFields.zoneIds} />
                    </FormFieldLabel>
                  )}
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <StackRow>
                    <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                      Update
                    </Button>
                  </StackRow>
                </StackColumn>
              </FormStackColumn>
            )}
          />
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(EditDesk);
