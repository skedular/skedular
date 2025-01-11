import type { organizationBillingInfo_query$key } from '@/queries/__generated__/organizationBillingInfo_query.graphql';
import type { organizationBillingInfo_refetchableFragment } from '@/queries/__generated__/organizationBillingInfo_refetchableFragment.graphql';
import type { organizationBillingInfo_setOrganizationBillingInfoMutation } from '@/queries/__generated__/organizationBillingInfo_setOrganizationBillingInfoMutation.graphql';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import {
  BodyIconTypography,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  StackRow,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
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
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

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
          id
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
          id
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

    if (!rootData.organizationBillingInfo) {
      return;
    }

    const organization = rootData.organization;
    const organizationBillingInfo = rootData.organizationBillingInfo;

    const toastId = themedToast(
      <NotificationContent content={`Updating organization '${organization.name} billing contact info'...`} />,
      infoNotificationOptions,
    );

    commitSetOrganizationBillingInfo({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId: organization.id,
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
                content={`Failed to update organization '${organization?.name}' billing contact info. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization?.name} billing contact info updated.`} />,
        });

        setEditing(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to update organization '${organization?.name}' billing contact info. Error: ${error.message}.`} />
          ),
        });
      },
      optimisticResponse: {
        setOrganizationBillingInfo: {
          organizationBillingInfo: {
            id: organizationBillingInfo.id,
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
                <FormFieldLabel label="Email">
                  <TextField name="email" required={requiredFields.email} helperText="Email to send invoice to" />
                </FormFieldLabel>

                <FormFieldLabel label="Address line 1">
                  <TextField name="addressLine1" required={requiredFields.addressLine1} />
                </FormFieldLabel>

                <FormFieldLabel label="Address line 2">
                  <TextField name="addressLine2" required={requiredFields.addressLine2} />
                </FormFieldLabel>

                <FormFieldLabel label="Suburb">
                  <TextField name="suburb" required={requiredFields.suburb} />
                </FormFieldLabel>

                <FormFieldLabel label="City">
                  <TextField name="city" required={requiredFields.city} />
                </FormFieldLabel>

                <FormFieldLabel label="Province">
                  <TextField name="province" required={requiredFields.province} />
                </FormFieldLabel>

                <FormFieldLabel label="Zipcode">
                  <TextField name="zipcode" required={requiredFields.zipcode} />
                </FormFieldLabel>

                <FormFieldLabel label="Country">
                  <SingleChoiceCountry name="country" required={requiredFields.country} />
                </FormFieldLabel>

                <TwoButtonsDialogActions onSecondaryClicked={handleCancelClick} primaryLabel="Update" secondaryLabel="Cancel" />
              </FormStackColumn>
            )}
          />
        </Paper>
      )}
    </>
  );
};

export default memo(OrganizationBillingInfo);
