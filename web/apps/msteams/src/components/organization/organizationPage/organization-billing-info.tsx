import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { SingleChoiceCountry } from '@repo/shared/components/forms';
import { EditIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { useMutation, useRefetchableFragment } from 'react-relay';
import { object, string } from 'yup';
import type { organizationBillingInfoQuery } from './__generated__/organizationBillingInfoQuery.graphql';
import type { organizationBillingInfo_query$key } from './__generated__/organizationBillingInfo_query.graphql';
import type { organizationBillingInfo_setOrganizationBillingInfoMutation } from './__generated__/organizationBillingInfo_setOrganizationBillingInfoMutation.graphql';

type Props = {
  rootDataRelay: organizationBillingInfo_query$key;
  onReloadRequired: () => void;
};

type OrganizationBillingInfoDetails = {
  email: string;
  addressLine1: string | null;
  addressLine2: string | null;
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string | null;
  country: string | null;
};

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

const OrganizationBillingInfo = ({ rootDataRelay, onReloadRequired }: Props) => {
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

  const [, startTransition] = useTransition();
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
          clientMutationId: nanoid(),
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

          return;
        }

        setEditing(false);
        startTransition(() => {
          refetch({ organizationId: rootData.organization?.id }, { fetchPolicy: 'store-and-network' });
        });
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
      <Stack direction="row" sx={{ justifyContent: 'flex-end' }} spacing={1}>
        {!editing && (
          <Button size="small" color="primary" onClick={handleEditClick}>
            <EditIcon />
          </Button>
        )}
      </Stack>
      {!editing && (
        <Stack direction="column" spacing={1}>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Email</Typography>
            <Typography variant="body1">{email}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Address Line 1</Typography>
            <Typography variant="body1">{addressLine1}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Address Line 2</Typography>
            <Typography variant="body1">{addressLine2}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Suburb</Typography>
            <Typography variant="body1">{suburb}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">City</Typography>
            <Typography variant="body1">{city}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Province</Typography>
            <Typography variant="body1">{province}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Zipcode</Typography>
            <Typography variant="body1">{zipcode}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Country</Typography>
            <Typography variant="body1">{country}</Typography>
          </Stack>
        </Stack>
      )}
      {editing && (
        <Paper elevation={24} sx={{ padding: 2 }}>
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
              <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
                <TextField label="Email" name="email" required={requiredFields.email} helperText="Email to send invoice to" />
                <TextField label="Address line 1" name="addressLine1" required={requiredFields.addressLine1} />
                <TextField label="Address line 2" name="addressLine2" required={requiredFields.addressLine2} />
                <TextField label="Suburb" name="suburb" required={requiredFields.suburb} />
                <TextField label="City" name="city" required={requiredFields.city} />
                <TextField label="Province" name="province" required={requiredFields.province} />
                <TextField label="Zipcode" name="zipcode" required={requiredFields.zipcode} />
                <SingleChoiceCountry name="country" required={requiredFields.country} />

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

export default memo(OrganizationBillingInfo);
