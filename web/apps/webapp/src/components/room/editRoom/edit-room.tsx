import { AppBarWithStackColumn, BodyIconTypography, ColorPicker, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesCustomTags, MultipleChoicesZones } from '@/components/organization';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { editRoom_query$key } from '@/queries/__generated__/editRoom_query.graphql';
import type { editRoom_updateRoomMutation } from '@/queries/__generated__/editRoom_updateRoomMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';

type Props = {
  rootDataRelay: editRoom_query$key;
  onReloadRequired?: () => void;
};

type RoomDetails = {
  name: string;
  customTagIds: string[];
  zoneIds: string[];
};

const roomSchema = object({
  name: string().required('Room name is required'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
});

const EditRoom = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<editRoom_query$key>(
    graphql`
      fragment editRoom_query on Query {
        room(id: $roomId) {
          id
          name
          deactivated
          requireBookingApproval
          color
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

  const [commitUpdateRoom] = useMutation<editRoom_updateRoomMutation>(graphql`
    mutation editRoom_updateRoomMutation($input: UpdateRoomInput!) @raw_response_type {
      updateRoom(input: $input) {
        room {
          id
          name
          deactivated
          requireBookingApproval
          color
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
  const validateRoomDetails = makeValidate(roomSchema);
  const requiredRoomDetailsFields = makeRequired(roomSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.room?.color);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleCloseClick = () => {
    router.back();
  };

  const handleRoomDetailUpdateClick = ({ name, customTagIds, zoneIds }: RoomDetails) => {
    if (!rootData.room) {
      return;
    }

    const oldName = rootData.room.name;
    const toastId = themedToast(<NotificationContent content={`Updating zone '${oldName}'...`} />, infoNotificationOptions);

    commitUpdateRoom({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.room.id,
          name,
          deactivated: rootData.room.deactivated,
          requireBookingApproval: rootData.room.requireBookingApproval,
          customTagIds,
          zoneIds,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update Room '${oldName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Room ${name} updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update Room '${oldName}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateRoom: {
          room: {
            id: rootData.room.id,
            name,
            deactivated: rootData.room.deactivated,
            requireBookingApproval: rootData.room.requireBookingApproval,
            customTags: [],
            zones: [],
            color: selectedColor,
          },
        },
      },
    });
  };

  if (!rootData.room) {
    return <></>;
  }

  const room = rootData.room;

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Room Information">
          <Form
            onSubmit={handleRoomDetailUpdateClick}
            initialValues={{
              name: room.name,
              customTagIds: room.customTags.map(({ uniqueId }) => uniqueId),
              zoneIds: room.zones.map(({ uniqueId }) => uniqueId),
            }}
            validate={validateRoomDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Room Setup" />
                  <BodyIconTypography label="Edit your room name and details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredRoomDetailsFields.name} helperText="Add your room name" />
                  </FormFieldLabel>

                  <FormFieldLabel label="Tags">
                    <MultipleChoicesCustomTags rootDataRelay={rootData} name="customTagIds" required={requiredRoomDetailsFields.customTagIds} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Zones">
                    <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredRoomDetailsFields.zoneIds} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Color">
                    <ColorPicker onChange={handleColorChange} defaultColor={rootData.room?.color} />
                  </FormFieldLabel>
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

export default memo(EditRoom);
