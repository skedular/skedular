import { CustomerAvatar } from '@/components/customer';
import { OrganizationSingleChoiceMembershipType } from '@/components/organization';
import { EditIcon } from '@repo/shared/components/icons';
import type { organizationMemberCard_OrganizationMemberDetails$key } from '@/queries/__generated__/organizationMemberCard_OrganizationMemberDetails.graphql';
import type {
  OrganizationMemberMembershipType,
  organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation,
} from '@/queries/__generated__/organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation.graphql';
import type { organizationSingleChoiceMembershipType_query$key } from '@/queries/__generated__/organizationSingleChoiceMembershipType_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { convertStringToLowercaseExceptFirstLetter, getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate } from 'mui-rff';
import { v4 as uuidv4 } from 'uuid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { object, string } from 'yup';

type Props = {
  data: organizationSingleChoiceMembershipType_query$key;
  organizationMemberDetailsRelay: organizationMemberCard_OrganizationMemberDetails$key;
  connectionIds: string[];
};

interface OrganizationMemberDetails {
  membershipType: string;
}

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

  const { enqueueSnackbar } = useSnackbar();
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
    setEditing(false);

    const membershipType = membershipTypeStr as unknown as OrganizationMemberMembershipType;

    commitChangeOrganizationMemberOwnershipType({
      variables: {
        input: {
          clientMutationId: uuidv4(),
          id: organizationMemberDetails.id,
          membershipType,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to update membership to ${membershipType}. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update membership to '${membershipType}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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
        <Paper
          elevation={24}
          sx={{
            minWidth: 300,
            maxWidth: 300,
          }}
        >
          <Card
            sx={{
              minWidth: 300,
              maxWidth: 300,
            }}
          >
            <CardContent>
              <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
                <CustomerAvatar
                  name={{
                    name: organizationMemberDetails.customer?.name,
                    givenName: organizationMemberDetails.customer?.givenName,
                    middleName: organizationMemberDetails.customer?.middleName,
                    familyName: organizationMemberDetails.customer?.familyName,
                  }}
                  photo={{
                    url: organizationMemberDetails.customer?.photoUrl,
                  }}
                />
              </Stack>

              <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
                <Typography gutterBottom variant="body1">
                  {getCustomerFullName(organizationMemberDetails.customer)}
                </Typography>
              </Stack>

              {organizationMemberDetails.membershipType && (
                <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
                  <Typography gutterBottom variant="body1">
                    {convertStringToLowercaseExceptFirstLetter(organizationMemberDetails.membershipType)}
                  </Typography>
                </Stack>
              )}

              <CardActions>
                <Button size="small" color="primary" onClick={handleEditClick}>
                  <EditIcon />
                </Button>
              </CardActions>
            </CardContent>
          </Card>
        </Paper>
      )}
      {editing && (
        <Paper
          elevation={24}
          sx={{
            minWidth: 300,
            maxWidth: 300,
          }}
        >
          <Form
            onSubmit={handleSaveClick}
            initialValues={{
              membershipType: organizationMemberDetails.membershipType,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <Box
                component="form"
                sx={{
                  '& > :not(style)': { m: 1 },
                }}
                autoComplete="off"
                noValidate
                onSubmit={handleSubmit}
              >
                <Stack sx={{ flex: 1 }} direction="row" spacing={2} />

                <OrganizationSingleChoiceMembershipType rootDataRelay={data} name="membershipType" required={requiredFields.membershipType} />

                <Stack sx={{ flex: 1 }} direction="row" spacing={2}>
                  <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                    Cancel
                  </Button>
                  <Button color="primary" variant="contained" type="submit">
                    Save
                  </Button>
                </Stack>
                <Stack sx={{ flex: 1 }} direction="row" spacing={2} />
              </Box>
            )}
          />
        </Paper>
      )}
    </>
  );
};

export default memo(OrganizationMemberCard);
