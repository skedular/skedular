import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import { BodyIconTypography, FormStackColumn, LeadIconTypography, StackRow } from '@repo/shared/components/commons';
import { SingleChoiceCountry } from '@repo/shared/components/forms';
import { EditIcon } from '@repo/shared/components/icons';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import type { organizationBillingInfo_query$key } from './__generated__/organizationBillingInfo_query.graphql';
import type { organizationBillingInfo_refetchableFragment } from './__generated__/organizationBillingInfo_refetchableFragment.graphql';
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
  const [rootData, refetch] = useRefetchableFragment<organizationBillingInfo_refetchableFragment, organizationBillingInfo_query$key>(
    graphql`
      fragment organizationBillingInfo_query on Query @refetchable(queryName: "organizationBillingInfo_refetchableFragment") {
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [, startTransition] = useTransition();
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

    const toastId = themedToast(
      <NotificationContent content={`Updating organization '${rootData.organization.name} billing contact info'...`} />,
      infoNotificationOptions,
    );

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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to update organization '${rootData.organization?.name}' billing contact info. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${rootData.organization?.name} billing contact info updated.`} />,
        });

        setEditing(false);
        startTransition(() => {
          refetch({ organizationId: rootData.organization?.id }, { fetchPolicy: 'store-and-network' });
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent
              content={`Failed to update organization '${rootData.organization?.name}' billing contact info. Error: ${error.message}.`}
            />
          ),
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
      {!editing && (
        <>
          <StackRow>
            <LeadIconTypography label="Email" />
            <BodyIconTypography label={email} />
          </StackRow>

          <StackRow>
            <LeadIconTypography label="Address Line 1" />
            <BodyIconTypography label={addressLine1} />
          </StackRow>

          <StackRow>
            <LeadIconTypography label="Address Line 2" />
            <BodyIconTypography label={addressLine2} />
          </StackRow>

          <StackRow>
            <LeadIconTypography label="Suburb" />
            <BodyIconTypography label={suburb} />
          </StackRow>

          <StackRow>
            <LeadIconTypography label="City" />
            <BodyIconTypography label={city} />
          </StackRow>

          <StackRow>
            <LeadIconTypography label="Province" />
            <BodyIconTypography label={province} />
          </StackRow>

          <StackRow>
            <LeadIconTypography label="Zipcode" />
            <BodyIconTypography label={zipcode} />
          </StackRow>

          <StackRow>
            <LeadIconTypography label="Country" />
            <BodyIconTypography label={country} />
          </StackRow>

          <StackRow>
            <Button variant="contained" size="small" color="primary" startIcon={<EditIcon />} onClick={handleEditClick}>
              Edit
            </Button>
          </StackRow>
        </>
      )}
      {editing && (
        <Paper sx={{ padding: 2 }}>
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
              <FormStackColumn onSubmit={handleSubmit}>
                <TextField label="Email" name="email" required={requiredFields.email} helperText="Email to send invoice to" />
                <TextField label="Address line 1" name="addressLine1" required={requiredFields.addressLine1} />
                <TextField label="Address line 2" name="addressLine2" required={requiredFields.addressLine2} />
                <TextField label="Suburb" name="suburb" required={requiredFields.suburb} />
                <TextField label="City" name="city" required={requiredFields.city} />
                <TextField label="Province" name="province" required={requiredFields.province} />
                <TextField label="Zipcode" name="zipcode" required={requiredFields.zipcode} />
                <SingleChoiceCountry name="country" required={requiredFields.country} />

                <StackRow sx={{ justifyContent: 'flex-end' }}>
                  <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                    Cancel
                  </Button>
                  <Button color="primary" variant="contained" type="submit">
                    Update
                  </Button>
                </StackRow>
              </FormStackColumn>
            )}
          />
        </Paper>
      )}
    </>
  );
};

export default memo(OrganizationBillingInfo);
