import { CustomerAvatar } from '@/components/customer';
import { LocationSingleChoiceMembershipType } from '@/components/location';
import type { locationMemberCard_LocationMemberDetails$key } from '@/queries/__generated__/locationMemberCard_LocationMemberDetails.graphql';
import type {
  LocationMemberMembershipType,
  locationMemberCard_changeLocationMemberOwnershipTypeMutation,
} from '@/queries/__generated__/locationMemberCard_changeLocationMemberOwnershipTypeMutation.graphql';
import type { locationSingleChoiceMembershipType_query$key } from '@/queries/__generated__/locationSingleChoiceMembershipType_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { EditIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { convertStringToLowercaseExceptFirstLetter, getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { object, string } from 'yup';

type Props = {
  data: locationSingleChoiceMembershipType_query$key;
  locationMemberDetailsRelay: locationMemberCard_LocationMemberDetails$key;
  connectionIds: string[];
};

interface LocationMemberDetails {
  membershipType: string;
}

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

  const [commitChangeLocationMemberOwnershipType] = useMutation<locationMemberCard_changeLocationMemberOwnershipTypeMutation>(graphql`
    mutation locationMemberCard_changeLocationMemberOwnershipTypeMutation($input: ChangeLocationMemberOwnershipTypeInput!) @raw_response_type {
      changeLocationMemberOwnershipType(input: $input) {
        member {
          id
          membershipType
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
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
    setEditing(false);

    const membershipType = membershipTypeStr as unknown as LocationMemberMembershipType;

    commitChangeLocationMemberOwnershipType({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: locationMemberDetails.id,
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
        changeLocationMemberOwnershipType: {
          member: {
            id: locationMemberDetails.id,
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
                    name: locationMemberDetails.customer?.name,
                    givenName: locationMemberDetails.customer?.givenName,
                    middleName: locationMemberDetails.customer?.middleName,
                    familyName: locationMemberDetails.customer?.familyName,
                  }}
                  photo={{
                    url: locationMemberDetails.customer?.photoUrl,
                  }}
                />
              </Stack>

              <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
                <Typography gutterBottom variant="body1">
                  {getCustomerFullName(locationMemberDetails.customer)}
                </Typography>
              </Stack>

              {locationMemberDetails.membershipType && (
                <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
                  <Typography gutterBottom variant="body1">
                    {convertStringToLowercaseExceptFirstLetter(locationMemberDetails.membershipType)}
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
              membershipType: locationMemberDetails.membershipType,
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
                <LocationSingleChoiceMembershipType rootDataRelay={data} name="membershipType" required={requiredFields.membershipType} />

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

export default memo(LocationMemberCard);
