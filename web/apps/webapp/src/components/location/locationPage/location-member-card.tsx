import { LocationSingleChoiceMembershipType } from '@/components/location';
import type { locationMemberCard_LocationMemberDetails$key } from '@/queries/__generated__/locationMemberCard_LocationMemberDetails.graphql';
import type {
  LocationMembershipType,
  locationMemberCard_changeLocationMemberOwnershipTypeMutation,
} from '@/queries/__generated__/locationMemberCard_changeLocationMemberOwnershipTypeMutation.graphql';
import type { locationSingleChoiceMembershipType_query$key } from '@/queries/__generated__/locationSingleChoiceMembershipType_query.graphql';
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
  data: locationSingleChoiceMembershipType_query$key;
  locationMemberDetailsRelay: locationMemberCard_LocationMemberDetails$key;
  connectionIds: string[];
};

type LocationMemberDetails = {
  membershipType: string;
};

const locationMemberSchema = object({
  membershipType: string().required(),
});

const LocationMemberCard = ({ data, locationMemberDetailsRelay, connectionIds }: Props) => {
  const locationMemberDetails = useFragment(
    graphql`
      fragment locationMemberCard_LocationMemberDetails on LocationMemberDetails {
        id
        membershipType
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

  const [commitChangeLocationMembershipType] = useMutation<locationMemberCard_changeLocationMemberOwnershipTypeMutation>(graphql`
    mutation locationMemberCard_changeLocationMemberOwnershipTypeMutation($input: ChangeLocationMembershipTypeInput!) @raw_response_type {
      changeLocationMembershipType(input: $input) {
        member {
          id
          membershipType
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

  const handleSaveClick = ({ membershipType: membershipTypeStr }: LocationMemberDetails) => {
    const membershipType = membershipTypeStr as unknown as LocationMembershipType;
    const toastId = themedToast(<NotificationContent content={`Updating location membership...`} />, infoNotificationOptions);

    commitChangeLocationMembershipType({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: locationMemberDetails.id,
          membershipType,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update membership to ${membershipType}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location membership updated.`} />,
        });

        setEditing(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update membership to '${membershipType}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        changeLocationMembershipType: {
          member: {
            id: locationMemberDetails.id,
            membershipType,
          },
        },
      },
    });
  };

  return (
    <Form
      onSubmit={handleSaveClick}
      initialValues={{
        membershipType: locationMemberDetails.membershipType,
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
            {!editing && locationMemberDetails.membershipType && (
              <BodyIconTypography label={convertStringToLowercaseExceptFirstLetter(locationMemberDetails.membershipType)} />
            )}
            {editing && (
              <FormFieldLabel label="Role" useWiderSpace>
                <LocationSingleChoiceMembershipType rootDataRelay={data} name="membershipType" required={requiredFields.membershipType} />
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
