import type { organizationBillingInfoQuery } from '@/queries/__generated__/organizationBillingInfoQuery.graphql';
import type { organizationBillingInfo_query$key } from '@/queries/__generated__/organizationBillingInfo_query.graphql';
import type { organizationBillingInfo_setOrganizationBillingInfoMutation } from '@/queries/__generated__/organizationBillingInfo_setOrganizationBillingInfoMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { SingleChoiceCountry } from '@repo/shared/components/forms';
import { EditIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { useSnackbar } from 'notistack';
import { memo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation, useRefetchableFragment } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: organizationBillingInfo_query$key;
};

interface OrganizationBillingInfoDetails {
  email: string;
  addressLine1: string | null;
  addressLine2: string | null;
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string | null;
  country: string | null;
}

const organizationBillingInfoSchema = object({
  email: string().email(({ value }) => `${value} is not a valid email`),
  addressLine1: string().nullable(),
  addressLine2: string().nullable(),
  suburb: string().nullable(),
  city: string().nullable(),
  province: string().nullable(),
  zipcode: string().nullable(),
  country: string().nullable(),
});

const OrganizationBillingInfo = ({ rootDataRelay }: Props) => {
  const [rootData, refetch] = useRefetchableFragment<organizationBillingInfoQuery, organizationBillingInfo_query$key>(
    graphql`
      fragment organizationBillingInfo_query on Query @refetchable(queryName: "organizationBillingInfoQuery") {
        organization(id: $organizationId) {
          id
          name
        }
        organizationBillingInfo(organizationId: $organizationId) {
          organizationId
          email
          addressLine1
          addressLine2
          suburb
          city
          province
          zipcode
          country
        }
      }
    `,
    rootDataRelay,
  );

  const [commitSetOrganizationBillingInfo] = useMutation<organizationBillingInfo_setOrganizationBillingInfoMutation>(graphql`
    mutation organizationBillingInfo_setOrganizationBillingInfoMutation($input: SetOrganizationBillingInfoInput!) @raw_response_type {
      setOrganizationBillingInfo(input: $input) {
        organizationBillingInfo {
          organizationId
          email
          addressLine1
          addressLine2
          suburb
          city
          province
          zipcode
          country
        }
      }
    }
  `);

  const [isPending, startTransition] = useTransition();
  const { enqueueSnackbar } = useSnackbar();
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(organizationBillingInfoSchema);
  const requiredFields = makeRequired(organizationBillingInfoSchema);

  const handleEditClick = () => {
    setEditing(true);
  };

  const organizationBillingInfo = rootData.organizationBillingInfo;

  const handleUpdateClick = ({ email, addressLine1, addressLine2, suburb, city, province, zipcode, country }: OrganizationBillingInfoDetails) => {
    if (!rootData.organization) {
      return;
    }

    commitSetOrganizationBillingInfo({
      variables: {
        input: {
          clientMutationId: uuidv4(),
          organizationId: rootData.organization.id,
          email,
          addressLine1,
          addressLine2,
          suburb,
          city,
          province,
          zipcode,
          country,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to update organization '${rootData.organization?.name}' billing contact info. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          setEditing(false);
          startTransition(() => {
            refetch({ organizationId: rootData.organization?.id }, { fetchPolicy: 'store-and-network' });
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update organization '${rootData.organization?.name}' billing contact info. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        setOrganizationBillingInfo: {
          organizationBillingInfo: {
            organizationId: rootData.organization.id,
            email,
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
          },
        },
      },
    });
  };

  const handleCancelClick = () => {
    setEditing(false);
  };

  const email = organizationBillingInfo?.email ? organizationBillingInfo?.email : '';
  const addressLine1 = organizationBillingInfo?.addressLine1 ? organizationBillingInfo?.addressLine1 : '';
  const addressLine2 = organizationBillingInfo?.addressLine2 ? organizationBillingInfo?.addressLine2 : '';
  const suburb = organizationBillingInfo?.suburb ? organizationBillingInfo?.suburb : '';
  const city = organizationBillingInfo?.city ? organizationBillingInfo?.city : '';
  const province = organizationBillingInfo?.province ? organizationBillingInfo?.province : '';
  const zipcode = organizationBillingInfo?.zipcode ? organizationBillingInfo?.zipcode : '';
  const country = organizationBillingInfo?.country ? organizationBillingInfo?.country : '';

  return (
    <>
      <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
        {!editing && (
          <Button size="small" color="primary" onClick={handleEditClick}>
            <EditIcon />
          </Button>
        )}
      </Box>
      {!editing && (
        <Grid
          container
          sx={{
            display: 'flex',
            justifyContent: 'flex-start',
            alignItems: 'left',
            marginBottom: 1,
          }}
        >
          <Grid>
            <Stack direction={'row'}>
              <Typography gutterBottom variant="h6">
                Email
              </Typography>
              <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                {email}
              </Typography>
            </Stack>

            <Stack direction={'row'}>
              <Typography gutterBottom variant="h6">
                Address Line 1
              </Typography>
              <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                {addressLine1}
              </Typography>
            </Stack>

            <Stack direction={'row'}>
              <Typography gutterBottom variant="h6">
                Address Line 2
              </Typography>
              <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                {addressLine2}
              </Typography>
            </Stack>

            <Stack direction={'row'}>
              <Typography gutterBottom variant="h6">
                Suburb
              </Typography>
              <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                {suburb}
              </Typography>
            </Stack>

            <Stack direction={'row'}>
              <Typography gutterBottom variant="h6">
                City
              </Typography>
              <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                {city}
              </Typography>
            </Stack>

            <Stack direction={'row'}>
              <Typography gutterBottom variant="h6">
                Province
              </Typography>
              <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                {province}
              </Typography>
            </Stack>

            <Stack direction={'row'}>
              <Typography gutterBottom variant="h6">
                Zipcode
              </Typography>
              <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                {zipcode}
              </Typography>
            </Stack>

            <Stack direction={'row'}>
              <Typography gutterBottom variant="h6">
                Country
              </Typography>
              <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                {country}
              </Typography>
            </Stack>
          </Grid>
        </Grid>
      )}
      {editing && (
        <Grid
          container
          sx={{
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            marginBottom: 1,
          }}
        >
          <Paper elevation={24} sx={{ padding: 3 }}>
            <Form
              onSubmit={handleUpdateClick}
              initialValues={{
                email,
                addressLine1,
                addressLine2,
                suburb,
                city,
                province,
                zipcode,
                country,
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
                  <TextField label="Email" name="email" required={requiredFields.email} helperText="Email to send invoice to" />
                  <TextField label="Address line 1" name="addressLine1" required={requiredFields.addressLine1} />
                  <TextField label="Address line 2" name="addressLine2" required={requiredFields.addressLine2} />
                  <TextField label="Suburb" name="suburb" required={requiredFields.suburb} />
                  <TextField label="City" name="city" required={requiredFields.city} />
                  <TextField label="Province" name="province" required={requiredFields.province} />
                  <TextField label="Zipcode" name="zipcode" required={requiredFields.zipcode} />
                  <SingleChoiceCountry name="country" required={requiredFields.country} />
                  <Stack sx={{ flex: 1, justifyContent: 'flex-end' }} direction="row" spacing={2}>
                    <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                      Cancel
                    </Button>
                    <Button color="primary" variant="contained" type="submit">
                      Update
                    </Button>
                  </Stack>
                </Box>
              )}
            />
          </Paper>
        </Grid>
      )}
    </>
  );
};

export default memo(OrganizationBillingInfo);
