import { LocationSingleChoiceMemberRole } from '@/components/location';
import type {
  LocationMemberRole,
  locationMemberCard_changeLocationMemberOwnershipTypeMutation,
} from '@/queries/__generated__/locationMemberCard_changeLocationMemberOwnershipTypeMutation.graphql';
import type { locationMemberCard_LocationMemberDetails$key } from '@/queries/__generated__/locationMemberCard_LocationMemberDetails.graphql';
import type { locationSingleChoiceMemberRole_query$key } from '@/queries/__generated__/locationSingleChoiceMemberRole_query.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { EditIcon } from '@repo/shared/components/icons';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { convertStringToLowercaseExceptFirstLetter, getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

type Props = {
  data: locationSingleChoiceMemberRole_query$key;
  locationMemberDetailsRelay: locationMemberCard_LocationMemberDetails$key;
  connectionIds: string[];
};

type LocationMemberDetails = {
  role: string;
};

const locationMemberSchema = object({
  role: string().required(),
});

const LocationMemberCard = ({ data, locationMemberDetailsRelay, connectionIds }: Props) => {
  const locationMemberDetails = useFragment(
    graphql`
      fragment locationMemberCard_LocationMemberDetails on LocationMemberDetails {
        id
        role
        customer {
          name
          givenName
          middleName
          familyName
          photoUrl
        }
      }
    `,
    locationMemberDetailsRelay,
  );

  const [commitChangeLocationMemberRole] = useMutation<locationMemberCard_changeLocationMemberOwnershipTypeMutation>(graphql`
    mutation locationMemberCard_changeLocationMemberOwnershipTypeMutation($input: ChangeLocationMemberRoleInput!) @raw_response_type {
      changeLocationMemberRole(input: $input) {
        member {
          id
          role
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(locationMemberSchema);
  const requiredFields = makeRequired(locationMemberSchema);

  const handleEditClick = () => {
    setEditing(true);
  };

  const handleCancelClick = () => {
    setEditing(false);
  };

  const handleSaveClick = ({ role: roleStr }: LocationMemberDetails) => {
    const role = roleStr as unknown as LocationMemberRole;
    const toastId = themedToast(<NotificationContent content={`Updating location role...`} />, infoNotificationOptions);

    commitChangeLocationMemberRole({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: locationMemberDetails.id,
          role,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update role to ${role}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location role updated.`} />,
        });

        setEditing(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update role to '${role}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        changeLocationMemberRole: {
          member: {
            id: locationMemberDetails.id,
            role,
          },
        },
      },
    });
  };

  return (
    <Form
      onSubmit={handleSaveClick}
      initialValues={{
        role: locationMemberDetails.role,
      }}
      validate={validate}
      render={({ handleSubmit }) => (
        <Card sx={{ minWidth: 300, height: '100%' }}>
          <CardHeader
            title={
              <BodyIconTypography
                label={getCustomerFullName(locationMemberDetails.customer)}
                startElement={
                  <CustomerAvatar name={locationMemberDetails.customer} photo={{ url: locationMemberDetails.customer?.photoUrl }} showFullName />
                }
                invertDefaultColor
              />
            }
          />

          <CardContent>
            {!editing && locationMemberDetails.role && (
              <BodyIconTypography label={convertStringToLowercaseExceptFirstLetter(locationMemberDetails.role)} />
            )}
            {editing && (
              <FormFieldLabel label="Role" useWiderSpace>
                <LocationSingleChoiceMemberRole rootDataRelay={data} name="role" required={requiredFields.role} />
              </FormFieldLabel>
            )}
          </CardContent>

          <CardActions sx={{ justifyContent: 'flex-end' }}>
            {!editing && (
              <Button size="small" color="primary" onClick={handleEditClick}>
                <EditIcon />
              </Button>
            )}
            {editing && (
              <FormStackColumn onSubmit={handleSubmit}>
                <TwoButtonsDialogActions onSecondaryClicked={handleCancelClick} primaryLabel="Update" secondaryLabel="Cancel" />
              </FormStackColumn>
            )}
          </CardActions>
        </Card>
      )}
    />
  );
};

export default memo(LocationMemberCard);
