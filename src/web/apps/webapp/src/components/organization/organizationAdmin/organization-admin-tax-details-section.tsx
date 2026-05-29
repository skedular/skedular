import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { TaxDetails, taxDetailsSchema } from '@/components/organization/organizationAdmin/organization-admin-shared';
import type { organizationAdminTaxDetailsSectionQuery } from '@/queries/__generated__/organizationAdminTaxDetailsSectionQuery.graphql';
import type { organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation } from '@/queries/__generated__/organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Switch from '@mui/material/Switch';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import { FormFieldLabel, FormStackColumn, SettingsSectionCard, StackColumn } from '@skedular/ui';
import { makeRequired, makeValidate, showErrorOnChange, TextField } from 'mui-rff';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';
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

type TaxDetailsPatchField = 'IS_REGISTERED' | 'TAX_ID' | 'TAX_RATE_PERCENTAGE';

const inlinePatchDebounceTimeout = 1000;

const formatTaxRatePercentageFormValue = (taxRatePercentage: string | number | null | undefined) => {
  if (taxRatePercentage === null || taxRatePercentage === undefined) {
    return '';
  }

  const taxRatePercentageValue = taxRatePercentage.toString();
  return parseFloat(taxRatePercentageValue) === 0 ? '' : taxRatePercentageValue;
};

const RootQuery = graphql`
  query organizationAdminTaxDetailsSectionQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
      taxDetails {
        isRegistered
        taxId
        taxRatePercentage
      }
    }
  }
`;

const OrganizationAdminTaxDetailsSectionContent = ({ organizationCustomDomain, queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminTaxDetailsSectionQuery>(RootQuery, queryReference);
  const [commitUpdateOrganizationTaxDetailsPatch] = useMutation<organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation>(graphql`
    mutation organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation($input: UpdateOrganizationTaxDetailsInput!) @raw_response_type {
      updateOrganizationTaxDetails(input: $input) {
        organization {
          id
          taxDetails {
            isRegistered
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

  const [forceShowTaxDetailsFieldErrors, setForceShowTaxDetailsFieldErrors] = useState(false);
  const initialTaxDetailsValues = useMemo<TaxDetails>(
    () => ({
      isRegistered: organization?.taxDetails?.isRegistered ?? false,
      taxId: organization?.taxDetails?.taxId ?? '',
      taxRatePercentage: formatTaxRatePercentageFormValue(organization?.taxDetails?.taxRatePercentage),
    }),
    [organization],
  );
  const draftTaxDetailsValues = useRef(initialTaxDetailsValues);

  const commitTaxDetailsPatch = useCallback(
    (fieldsToUpdate: TaxDetailsPatchField[], { isRegistered, taxId, taxRatePercentage }: TaxDetails) => {
      if (!organization || fieldsToUpdate.length === 0) {
        return;
      }

      const hasValidTaxDetails = taxDetailsSchema.isValidSync({ isRegistered, taxId, taxRatePercentage });
      if (!hasValidTaxDetails) {
        return;
      }

      const taxRatePercentageValue = taxRatePercentage ?? '';
      const taxRatePercentageInput = taxRatePercentageValue.trim().length === 0 ? null : parseFloat(taxRatePercentageValue);

      commitUpdateOrganizationTaxDetailsPatch({
        variables: {
          input: {
            clientMutationId: uuid(),
            organizationCustomDomain,
            fieldsToUpdate,
            isRegistered,
            taxId,
            taxRatePercentage: taxRatePercentageInput,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            themedToast(
              <NotificationContent content={`Failed to update organization '${organization.name}' tax details. Error: ${getRelayErrorMessage(errors)}.`} />,
              errorNotificationOptions,
            );
          }
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to update organization '${organization.name}' tax details. Error: ${error.message}.`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateOrganizationTaxDetails: {
            organization: {
              id: organization.id,
              taxDetails: {
                id: '',
                isRegistered,
                taxId,
                taxRatePercentage: taxRatePercentageValue,
              },
            },
          },
        },
      });
    },
    [commitUpdateOrganizationTaxDetailsPatch, organization, organizationCustomDomain, themedToast],
  );
  const debouncedCommitTaxDetailsPatch = useDebounceCallback(commitTaxDetailsPatch, inlinePatchDebounceTimeout);

  if (!organization) {
    return null;
  }

  return (
    <Form<TaxDetails>
      onSubmit={() => undefined}
      initialValues={initialTaxDetailsValues}
      validate={validateTaxDetails}
      render={({ handleSubmit, values, form }) => {
        const formValues = values;
        const changedFields: TaxDetailsPatchField[] = [];
        if (draftTaxDetailsValues.current.isRegistered !== formValues.isRegistered) {
          changedFields.push('IS_REGISTERED');
        }
        if (draftTaxDetailsValues.current.taxId !== formValues.taxId) {
          changedFields.push('TAX_ID');
        }
        if (draftTaxDetailsValues.current.taxRatePercentage !== formValues.taxRatePercentage) {
          changedFields.push('TAX_RATE_PERCENTAGE');
        }
        if (changedFields.length > 0) {
          draftTaxDetailsValues.current = formValues;
          debouncedCommitTaxDetailsPatch(changedFields, formValues);
        }
        const shouldShowTaxDetailsFieldErrors = forceShowTaxDetailsFieldErrors || formValues.isRegistered;
        const showTaxDetailsFieldError = (props: Parameters<typeof showErrorOnChange>[0]) =>
          (shouldShowTaxDetailsFieldErrors && Boolean(props.meta.error)) || showErrorOnChange(props);
        const forcedTaxIdError = forceShowTaxDetailsFieldErrors && formValues.taxId.trim().length === 0;
        const forcedTaxRatePercentageError = forceShowTaxDetailsFieldErrors && (formValues.taxRatePercentage ?? '').trim().length === 0;

        const handleIsRegisteredChange = (event: React.ChangeEvent<HTMLInputElement>) => {
          const isRegistered = event.target.checked;
          const nextValues = { ...formValues, isRegistered };
          if (isRegistered && !taxDetailsSchema.isValidSync(nextValues)) {
            form.blur('taxId');
            form.blur('taxRatePercentage');
            setForceShowTaxDetailsFieldErrors(true);
            return;
          }

          setForceShowTaxDetailsFieldErrors(false);
          form.change('isRegistered', isRegistered);
        };

        return (
          <FormStackColumn onSubmit={handleSubmit}>
            <Box sx={{ pb: 2 }}>
              <SettingsSectionCard title="Tax details" description="Control how tax identity and default rates are applied to invoices and marketplace billing.">
                <StackColumn sx={formColumnSx}>
                  <FormFieldLabel label="Is this business registered for tax (GST/VAT)?">
                    <Switch checked={formValues.isRegistered} onChange={handleIsRegisteredChange} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Tax ID / VAT / GST Number">
                    <TextField
                      name="taxId"
                      required={formValues.isRegistered && requiredTaxDetailsFields.taxId}
                      showError={showTaxDetailsFieldError}
                      {...(forcedTaxIdError ? { error: true, helperText: 'Tax ID / VAT / GST Number is required.' } : {})}
                    />
                  </FormFieldLabel>

                  <FormFieldLabel label="Tax Rate (%)">
                    <TextField
                      name="taxRatePercentage"
                      required={formValues.isRegistered && requiredTaxDetailsFields.taxRatePercentage}
                      showError={showTaxDetailsFieldError}
                      {...(forcedTaxRatePercentageError ? { error: true, helperText: 'Tax rate is required.' } : {})}
                    />
                  </FormFieldLabel>
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
