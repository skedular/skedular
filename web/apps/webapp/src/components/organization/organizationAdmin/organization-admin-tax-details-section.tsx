import { FormFieldLabel, FormStackColumn, StackColumn } from '@skedular/ui';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { TaxDetails, taxDetailsSchema } from '@/components/organization/organizationAdmin/organization-admin-shared';
import { PaletteModeContext } from '@skedular/shared';
import { keyboardTextFieldDebounceTimeout } from '@skedular/shared';
import { getRelayErrorMessage } from '@skedular/shared';
import type { organizationAdminTaxDetailsSectionQuery } from '@/queries/__generated__/organizationAdminTaxDetailsSectionQuery.graphql';
import type { organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation } from '@/queries/__generated__/organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation.graphql';
import type { organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation } from '@/queries/__generated__/organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Switch from '@mui/material/Switch';
import { EditorActionBar, SettingsSectionCard } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
};

type InnerProps = {
  organizationCustomDomain: string;
  queryReference: PreloadedQuery<organizationAdminTaxDetailsSectionQuery>;
};

const RootQuery = graphql`
  query organizationAdminTaxDetailsSectionQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
      taxDetails {
        taxId
        taxRatePercentage
      }
    }
  }
`;

const OrganizationAdminTaxDetailsSectionContent = ({ organizationCustomDomain, queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminTaxDetailsSectionQuery>(RootQuery, queryReference);
  const [commitUpdateOrganizationTaxDetails] = useMutation<organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation>(graphql`
    mutation organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation($input: UpdateOrganizationTaxDetailsInput!) @raw_response_type {
      updateOrganizationTaxDetails(input: $input) {
        organization {
          id
          taxDetails {
            taxId
            taxRatePercentage
          }
        }
      }
    }
  `);
  const [commitRemoveOrganizationTaxDetails] = useMutation<organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation>(graphql`
    mutation organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation($input: RemoveOrganizationTaxDetailsInput!) @raw_response_type {
      removeOrganizationTaxDetails(input: $input) {
        organization {
          id
          taxDetails {
            taxId
            taxRatePercentage
          }
        }
      }
    }
  `);

  const organization = rootData.organization;
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateTaxDetails = makeValidate(taxDetailsSchema);
  const requiredTaxDetailsFields = makeRequired(taxDetailsSchema);
  const formColumnSx = {
    width: '100%',
    maxWidth: 760,
  };

  const [taxDetailsEnabled, setTaxDetailsEnabled] = useState(!!organization?.taxDetails);
  const [taxDetailsTaxId, setTaxDetailsTaxId] = useState<string>(organization?.taxDetails?.taxId ?? '');
  const debounceSetTaxDetailsTaxId = useDebounceCallback(setTaxDetailsTaxId, keyboardTextFieldDebounceTimeout);
  const [taxDetailsTaxRatePercentage, setTaxDetailsTaxRatePercentage] = useState<string>(organization?.taxDetails?.taxRatePercentage ?? '');
  const debounceSetTaxDetailsTaxRatePercentage = useDebounceCallback(setTaxDetailsTaxRatePercentage, keyboardTextFieldDebounceTimeout);

  if (!organization) {
    return null;
  }

  const handleEnableOrganizationTaxDetailsClick = ({ taxId, taxRatePercentage }: TaxDetails) => {
    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' tax details...`} />, infoNotificationOptions);

    commitUpdateOrganizationTaxDetails({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
          taxId,
          taxRatePercentage: parseFloat(taxRatePercentage),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization.name}' tax details. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization.name} tax details details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization '${organization.name}' tax details. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganizationTaxDetails: {
          organization: {
            id: organization.id,
            taxDetails: {
              id: '',
              taxId,
              taxRatePercentage,
            },
          },
        },
      },
    });
  };

  const handleEnableTaxDetailsChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setTaxDetailsEnabled(event.target.checked);

    if (event.target.checked) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing organization '${organization.name}' tax details...`} />, infoNotificationOptions);

    commitRemoveOrganizationTaxDetails({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove organization '${organization.name}' tax details. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization.name} tax details removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove organization '${organization.name}' tax details. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        removeOrganizationTaxDetails: {
          organization: {
            id: organization.id,
            taxDetails: null,
          },
        },
      },
    });
  };

  return (
    <Form
      onSubmit={handleEnableOrganizationTaxDetailsClick}
      initialValues={{
        taxId: taxDetailsTaxId,
        taxRatePercentage: taxDetailsTaxRatePercentage,
      }}
      validate={validateTaxDetails}
      render={({ handleSubmit, values }) => {
        const formValues = values!;

        debounceSetTaxDetailsTaxId(formValues.taxId);
        debounceSetTaxDetailsTaxRatePercentage(formValues.taxRatePercentage);

        return (
          <FormStackColumn onSubmit={handleSubmit}>
            <Box sx={{ pb: 2 }}>
              <SettingsSectionCard title="Tax details" description="Control how tax identity and default rates are applied to invoices and marketplace billing.">
                <StackColumn sx={formColumnSx}>
                  <FormFieldLabel label="Is this business registered for tax (GST/VAT)?">
                    <Switch checked={taxDetailsEnabled} onChange={handleEnableTaxDetailsChange} />
                  </FormFieldLabel>

                  {taxDetailsEnabled && (
                    <>
                      <FormFieldLabel label="Tax ID / VAT / GST Number">
                        <TextField name="taxId" required={requiredTaxDetailsFields.taxId} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Tax Rate (%)">
                        <TextField name="taxRatePercentage" required={requiredTaxDetailsFields.taxRatePercentage} />
                      </FormFieldLabel>
                    </>
                  )}

                  {taxDetailsEnabled ? <EditorActionBar primaryAction="Update" /> : null}
                </StackColumn>
              </SettingsSectionCard>
            </Box>
          </FormStackColumn>
        );
      }}
    />
  );
};

const OrganizationAdminTaxDetailsSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationAdminTaxDetailsSectionQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      { organizationCustomDomain },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return <Loading />;
  }

  return <OrganizationAdminTaxDetailsSectionContent organizationCustomDomain={organizationCustomDomain} queryReference={queryReference} />;
};

export default memo(OrganizationAdminTaxDetailsSection);
