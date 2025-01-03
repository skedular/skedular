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
import graphql from 'babel-plugin-relay/macro';
import { OrganizationSingleChoiceMemberRole } from 'components/organization';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import type {
  OrganizationMemberRole,
  organizationMemberCard_changeOrganizationMemberRoleMutation,
} from './__generated__/organizationMemberCard_changeOrganizationMemberRoleMutation.graphql';
import type { organizationMemberCard_OrganizationMemberDetails$key } from './__generated__/organizationMemberCard_OrganizationMemberDetails.graphql';
import type { organizationSingleChoiceMemberRole_query$key } from './__generated__/organizationSingleChoiceMemberRole_query.graphql';

type Props = {
  data: organizationSingleChoiceMemberRole_query$key;
  organizationMemberDetailsRelay: organizationMemberCard_OrganizationMemberDetails$key;
  connectionIds: string[];
};

type OrganizationMemberDetails = {
  role: string;
};

const organizationMemberSchema = object({
  role: string().required(),
});

const OrganizationMemberCard = ({ data, organizationMemberDetailsRelay, connectionIds }: Props) => {
  const organizationMemberDetails = useFragment(
    graphql`
      fragment organizationMemberCard_OrganizationMemberDetails on OrganizationMemberDetails {
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
    organizationMemberDetailsRelay,
  );

  const [commitChangeOrganizationMemberRole] = useMutation<organizationMemberCard_changeOrganizationMemberRoleMutation>(graphql`
    mutation organizationMemberCard_changeOrganizationMemberRoleMutation($input: ChangeOrganizationMemberRoleInput!) @raw_response_type {
      changeOrganizationMemberRole(input: $input) {
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
  const validate = makeValidate(organizationMemberSchema);
  const requiredFields = makeRequired(organizationMemberSchema);

  const handleEditClick = () => {
    setEditing(true);
  };

  const handleCancelClick = () => {
    setEditing(false);
  };

  const handleSaveClick = ({ role: roleStr }: OrganizationMemberDetails) => {
    const role = roleStr as unknown as OrganizationMemberRole;
    const toastId = themedToast(<NotificationContent content={`Updating organization role...`} />, infoNotificationOptions);

    commitChangeOrganizationMemberRole({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: organizationMemberDetails.id,
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
          render: <NotificationContent content={`Organization role updated.`} />,
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
        changeOrganizationMemberRole: {
          member: {
            id: organizationMemberDetails.id,
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
        role: organizationMemberDetails.role,
      }}
      validate={validate}
      render={({ handleSubmit }) => (
        <Card sx={{ minWidth: 300, height: '100%' }}>
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
            {!editing && organizationMemberDetails.role && (
              <BodyIconTypography label={convertStringToLowercaseExceptFirstLetter(organizationMemberDetails.role)} />
            )}
            {editing && (
              <FormFieldLabel label="Role" useWiderSpace>
                <OrganizationSingleChoiceMemberRole rootDataRelay={data} name="role" required={requiredFields.role} />
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

export default memo(OrganizationMemberCard);
