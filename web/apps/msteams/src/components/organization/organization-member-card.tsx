import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Paper from '@mui/material/Paper';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { EditIcon } from '@repo/shared/components/icons';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { convertStringToLowercaseExceptFirstLetter, getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationSingleChoiceMembershipType } from 'components/organization';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import type { organizationMemberCard_OrganizationMemberDetails$key } from './__generated__/organizationMemberCard_OrganizationMemberDetails.graphql';
import type {
  OrganizationMembershipType,
  organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation,
} from './__generated__/organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation.graphql';
import type { organizationSingleChoiceMembershipType_query$key } from './__generated__/organizationSingleChoiceMembershipType_query.graphql';

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
    const membershipType = membershipTypeStr as unknown as OrganizationMembershipType;
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

  return (
    <>
      {!editing && (
        <Card sx={{ minWidth: 200, height: '100%' }}>
          <CardHeader
            title={
              <BodyIconTypography
                label={getCustomerFullName(organizationMemberDetails.customer)}
                startElement={
                  <CustomerAvatar
                    name={organizationMemberDetails.customer}
                    photo={{ url: organizationMemberDetails.customer?.photoUrl }}
                    showFullName
                  />
                }
                invertDefaultColor
              />
            }
          />

          <CardContent>
            {organizationMemberDetails.membershipType && (
              <BodyIconTypography label={convertStringToLowercaseExceptFirstLetter(organizationMemberDetails.membershipType)} />
            )}

            <CardActions sx={{ justifyContent: 'flex-end' }}>
              <Button size="small" color="primary" onClick={handleEditClick}>
                <EditIcon />
              </Button>
            </CardActions>
          </CardContent>
        </Card>
      )}

      {editing && (
        <Paper sx={{ padding: 2 }}>
          <Form
            onSubmit={handleSaveClick}
            initialValues={{
              membershipType: organizationMemberDetails.membershipType,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <BodyIconTypography
                  label={getCustomerFullName(organizationMemberDetails.customer)}
                  startElement={
                    <CustomerAvatar
                      name={organizationMemberDetails.customer}
                      photo={{ url: organizationMemberDetails.customer?.photoUrl }}
                      showFullName
                    />
                  }
                />
                <OrganizationSingleChoiceMembershipType rootDataRelay={data} name="membershipType" required={requiredFields.membershipType} />
                <TwoButtonsDialogActions onSecondaryClicked={handleCancelClick} primaryLabel="Update" secondaryLabel="Cancel" />
              </FormStackColumn>
            )}
          />
        </Paper>
      )}
    </>
  );
};

export default memo(OrganizationMemberCard);
