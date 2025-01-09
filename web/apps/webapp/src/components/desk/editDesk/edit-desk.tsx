import { MultipleChoicesDeskTypes, MultipleChoicesZones } from '@/components/organization';
import type { editDesk_query$key } from '@/queries/__generated__/editDesk_query.graphql';
import type { editDesk_updateDeskMutation } from '@/queries/__generated__/editDesk_updateDeskMutation.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import {
  BodyIconTypography,
  FormFieldLabel,
  SectionIconTypography,
  StackColumn,
  StackColumnWithSaveExitCancelAppBar,
} from '@repo/shared/components/commons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultPadding } from '@repo/shared/libs/theme';
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
  organizationId?: string;
};

type DeskDetails = {
  name: string;
  deskTypeIds: string[];
  zoneIds: string[];
};

const deskSchema = object({
  name: string().required('Desk name is required'),
  deskTypeIds: array().nullable(),
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
          deskTypes {
            uniqueId
            name
          }
          zones {
            uniqueId
            name
          }
        }
        ...multipleChoicesDeskTypes_query
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
          deskTypes {
            uniqueId
            name
          }
          zones {
            uniqueId
            name
          }
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(deskSchema);
  const requiredFields = makeRequired(deskSchema);

  const handleCancelClick = () => {
    router.back();
  };

  const handleSaveClick = ({ name, deskTypeIds, zoneIds }: DeskDetails) => {
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
          deskTypeIds,
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

        router.back();
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
            deskTypes: [],
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
        <Form
          onSubmit={handleSaveClick}
          initialValues={{
            name: desk.name,
            deskTypeIds: desk.deskTypes.map(({ uniqueId }) => uniqueId),
            zoneIds: desk.zones.map(({ uniqueId }) => uniqueId),
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <StackColumnWithSaveExitCancelAppBar onSubmit={handleSubmit} onCancel={handleCancelClick} label="Add Location">
              <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                <SectionIconTypography label="Desk Setup" />
                <BodyIconTypography label="Edit your desk name and details" />
                <Divider />
              </StackColumn>

              <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                <FormFieldLabel label="Name">
                  <TextField name="name" required={requiredFields.name} helperText="Add your desk name" />
                </FormFieldLabel>

                {organizationId && (
                  <FormFieldLabel label="Desk Types">
                    <MultipleChoicesDeskTypes rootDataRelay={rootData} name="deskTypeIds" required={requiredFields.deskTypeIds} />
                  </FormFieldLabel>
                )}

                {organizationId && (
                  <FormFieldLabel label="Zones">
                    <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredFields.zoneIds} />
                  </FormFieldLabel>
                )}
              </StackColumn>
            </StackColumnWithSaveExitCancelAppBar>
          )}
        />
      </Box>
    </Box>
  );
};

export default memo(EditDesk);
