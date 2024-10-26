import { OrganizationSingleChoiceMembershipType } from '@/components/organization';
import type { organizationMemberCard_OrganizationMemberDetails$key } from '@/queries/__generated__/organizationMemberCard_OrganizationMemberDetails.graphql';
import type {
  OrganizationMemberMembershipType,
  organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation,
} from '@/queries/__generated__/organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation.graphql';
import type { organizationSingleChoiceMembershipType_query$key } from '@/queries/__generated__/organizationSingleChoiceMembershipType_query.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
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
  data: organizationSingleChoiceMembershipType_query$key;
  organizationMemberDetailsRelay: organizationMemberCard_OrganizationMemberDetails$key;
  connectionIds: string[];
};

type OrganizationMemberDetails = {
  membershipType: string;
};

const organizationMemberSchema = object({
  membershipType: string().required(),
});

const OrganizationMemberCard = ({ data, organizationMemberDetailsRelay, connectionIds }: Props) => {
  const organizationMemberDetails = useFragment(
    graphql`
      fragment organizationMemberCard_OrganizationMemberDetails on OrganizationMemberDetails {
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
    organizationMemberDetailsRelay,
  );

  const [commitChangeOrganizationMemberOwnershipType] = useMutation<organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation>(graphql`
    mutation organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation($input: ChangeOrganizationMemberOwnershipTypeInput!)
    @raw_response_type {
      changeOrganizationMemberOwnershipType(input: $input) {
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
  const validate = makeValidate(organizationMemberSchema);
  const requiredFields = makeRequired(organizationMemberSchema);

  const handleEditClick = () => {
    setEditing(true);
  };

  const handleCancelClick = () => {
    setEditing(false);
  };

  const handleSaveClick = ({ membershipType: membershipTypeStr }: OrganizationMemberDetails) => {
    const membershipType = membershipTypeStr as unknown as OrganizationMemberMembershipType;
    const toastId = themedToast(<NotificationContent content={`Updating organization membership...`} />, infoNotificationOptions);

    commitChangeOrganizationMemberOwnershipType({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: organizationMemberDetails.id,
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
          render: <NotificationContent content={`Organization membership updated.`} />,
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
        changeOrganizationMemberOwnershipType: {
          member: {
            id: organizationMemberDetails.id,
            membershipType,
          },
        },
      },
    });
  };

  const avatar = (
    <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
      <CustomerAvatar name={organizationMemberDetails.customer} photo={{ url: organizationMemberDetails.customer?.photoUrl }} showFullName />
      <Typography variant="body1">{getCustomerFullName(organizationMemberDetails.customer)}</Typography>
    </Stack>
  );

  return (
    <>
      {!editing && (
        <Card elevation={24} sx={{ minWidth: 200, height: '100%' }}>
          <CardHeader title={<>{avatar}</>} />

          <CardContent>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              {organizationMemberDetails.membershipType && (
                <Typography variant="body1">{convertStringToLowercaseExceptFirstLetter(organizationMemberDetails.membershipType)}</Typography>
              )}
            </Stack>

            <CardActions sx={{ justifyContent: 'flex-end' }}>
              <Button size="small" color="primary" onClick={handleEditClick}>
                <EditIcon />
              </Button>
            </CardActions>
          </CardContent>
        </Card>
      )}

      {editing && (
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Form
            onSubmit={handleSaveClick}
            initialValues={{
              membershipType: organizationMemberDetails.membershipType,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
                {avatar}

                <OrganizationSingleChoiceMembershipType rootDataRelay={data} name="membershipType" required={requiredFields.membershipType} />

                <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
                  <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                    Cancel
                  </Button>
                  <Button color="primary" variant="contained" type="submit">
                    Update
                  </Button>
                </Stack>
              </Stack>
            )}
          />
        </Paper>
      )}
    </>
  );
};

export default memo(OrganizationMemberCard);
