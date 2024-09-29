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
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { convertStringToLowercaseExceptFirstLetter, getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { LocationSingleChoiceMembershipType } from 'components/location';
import type { locationSingleChoiceMembershipType_query$key } from 'components/location/__generated__/locationSingleChoiceMembershipType_query.graphql';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { object, string } from 'yup';
import type { locationMemberCard_LocationMemberDetails$key } from './__generated__/locationMemberCard_LocationMemberDetails.graphql';
import type {
  LocationMemberMembershipType,
  locationMemberCard_changeLocationMemberOwnershipTypeMutation,
} from './__generated__/locationMemberCard_changeLocationMemberOwnershipTypeMutation.graphql';

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

  const avatar = (
    <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
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
      <Typography variant="body1">{getCustomerFullName(locationMemberDetails.customer)}</Typography>
    </Stack>
  );

  return (
    <>
      {!editing && (
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Card>
            <CardHeader title={<>{avatar}</>} />
            <CardContent>
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                {locationMemberDetails.membershipType && (
                  <Typography variant="body1">{convertStringToLowercaseExceptFirstLetter(locationMemberDetails.membershipType)}</Typography>
                )}
              </Stack>

              <CardActions sx={{ justifyContent: 'flex-end' }}>
                <Button size="small" color="primary" onClick={handleEditClick}>
                  <EditIcon />
                </Button>
              </CardActions>
            </CardContent>
          </Card>
        </Paper>
      )}

      {editing && (
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Form
            onSubmit={handleSaveClick}
            initialValues={{
              membershipType: locationMemberDetails.membershipType,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <Stack direction="column" spacing={1} component="form" noValidate onSubmit={handleSubmit}>
                {avatar}

                <LocationSingleChoiceMembershipType rootDataRelay={data} name="membershipType" required={requiredFields.membershipType} />

                <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
                  <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                    Cancel
                  </Button>
                  <Button color="primary" variant="contained" type="submit">
                    Save
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

export default memo(LocationMemberCard);
