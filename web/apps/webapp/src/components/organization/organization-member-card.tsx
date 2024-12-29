import { OrganizationSingleChoiceMembershipType } from '@/components/organization';
import type { organizationMemberCard_OrganizationMemberDetails$key } from '@/queries/__generated__/organizationMemberCard_OrganizationMemberDetails.graphql';
import type {
  OrganizationMembershipType,
  organizationMemberCard_changeOrganizationMembershipMutation,
} from '@/queries/__generated__/organizationMemberCard_changeOrganizationMembershipMutation.graphql';
import type { organizationSingleChoiceMembershipType_query$key } from '@/queries/__generated__/organizationSingleChoiceMembershipType_query.graphql';
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

  const [commitChangeOrganizationMembership] = useMutation<organizationMemberCard_changeOrganizationMembershipMutation>(graphql`
    mutation organizationMemberCard_changeOrganizationMembershipMutation($input: ChangeOrganizationMembershipTypeInput!) @raw_response_type {
      changeOrganizationMembership(input: $input) {
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

    commitChangeOrganizationMembership({
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
        changeOrganizationMembership: {
          member: {
            id: organizationMemberDetails.id,
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
        membershipType: organizationMemberDetails.membershipType,
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
            {!editing && organizationMemberDetails.membershipType && (
              <BodyIconTypography label={convertStringToLowercaseExceptFirstLetter(organizationMemberDetails.membershipType)} />
            )}
            {editing && (
              <FormFieldLabel label="Role" useWiderSpace>
                <OrganizationSingleChoiceMembershipType rootDataRelay={data} name="membershipType" required={requiredFields.membershipType} />
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
