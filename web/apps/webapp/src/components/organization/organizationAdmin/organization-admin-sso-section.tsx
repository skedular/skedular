import { FormFieldLabel, FormStackColumn, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { SsoSettingsDetails, ssoSettingsSchema } from '@/components/organization/organizationAdmin/organization-admin-shared';
import { PaletteModeContext } from '@/libs/providers';
import { keyboardTextFieldDebounceTimeout } from '@/libs/utils';
import { getRelayErrorMessage } from '@/libs/utils';
import type { organizationAdminSsoSectionQuery } from '@/queries/__generated__/organizationAdminSsoSectionQuery.graphql';
import type { organizationAdminSsoSection_removeOrganizationSsoSettingsMutation } from '@/queries/__generated__/organizationAdminSsoSection_removeOrganizationSsoSettingsMutation.graphql';
import type { organizationAdminSsoSection_updateOrganizationSsoSettingsMutation } from '@/queries/__generated__/organizationAdminSsoSection_updateOrganizationSsoSettingsMutation.graphql';
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
  queryReference: PreloadedQuery<organizationAdminSsoSectionQuery>;
};

const RootQuery = graphql`
  query organizationAdminSsoSectionQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
      customDomain
      ssoSettings {
        id
        isActive
        entityId
        loginUrl
        appFederationMetadataUrl
      }
    }
  }
`;

const OrganizationAdminSsoSectionContent = ({ organizationCustomDomain, queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminSsoSectionQuery>(RootQuery, queryReference);
  const [commitUpdateOrganizationSsoSettings] = useMutation<organizationAdminSsoSection_updateOrganizationSsoSettingsMutation>(graphql`
    mutation organizationAdminSsoSection_updateOrganizationSsoSettingsMutation($input: UpdateOrganizationSsoSettingsInput!) @raw_response_type {
      updateOrganizationSsoSettings(input: $input) {
        organization {
          id
          ssoSettings {
            id
            isActive
            entityId
            loginUrl
            appFederationMetadataUrl
          }
        }
      }
    }
  `);
  const [commitRemoveOrganizationSsoSettings] = useMutation<organizationAdminSsoSection_removeOrganizationSsoSettingsMutation>(graphql`
    mutation organizationAdminSsoSection_removeOrganizationSsoSettingsMutation($input: RemoveOrganizationSsoSettingsInput!) @raw_response_type {
      removeOrganizationSsoSettings(input: $input) {
        organization {
          id
          ssoSettings {
            id
            isActive
            entityId
            loginUrl
            appFederationMetadataUrl
          }
        }
      }
    }
  `);

  const organization = rootData.organization;
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateSsoSettings = makeValidate(ssoSettingsSchema);
  const requiredSsoSettingsFields = makeRequired(ssoSettingsSchema);
  const formColumnSx = {
    width: '100%',
    maxWidth: 760,
  };

  const [ssoSettingsEnabled, setSsoSettingsEnabled] = useState(organization?.ssoSettings?.isActive);
  const [ssoSettingsEntityId, setSsoSettingsEntityId] = useState<string>(organization?.ssoSettings?.entityId ?? '');
  const debounceSetSsoSettingsEntityId = useDebounceCallback(setSsoSettingsEntityId, keyboardTextFieldDebounceTimeout);
  const [ssoSettingsLoginUrl, setSsoSettingsLoginUrl] = useState<string>(organization?.ssoSettings?.loginUrl ?? '');
  const debounceSetSsoSettingsLoginUrl = useDebounceCallback(setSsoSettingsLoginUrl, keyboardTextFieldDebounceTimeout);
  const [ssoSettingsAppFederationMetadataUrl, setSsoSettingsAppFederationMetadataUrl] = useState<string>(organization?.ssoSettings?.appFederationMetadataUrl ?? '');
  const debounceSetSsoSettingsAppFederationMetadataUrl = useDebounceCallback(setSsoSettingsAppFederationMetadataUrl, keyboardTextFieldDebounceTimeout);

  if (!organization) {
    return null;
  }

  const handleEnableOrganizationSsoSettingsClick = ({ entityId, loginUrl, appFederationMetadataUrl }: SsoSettingsDetails) => {
    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' SSO settings...`} />, infoNotificationOptions);

    commitUpdateOrganizationSsoSettings({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain: organization.customDomain,
          entityId,
          loginUrl,
          appFederationMetadataUrl,
          isActive: true,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization.name}' SSO settings. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization.name} SSO settings details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization '${organization.name}' SSO settings. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganizationSsoSettings: {
          organization: {
            id: organization.id,
            ssoSettings: {
              id: organization.ssoSettings?.id ?? '',
              isActive: true,
              entityId,
              loginUrl,
              appFederationMetadataUrl,
            },
          },
        },
      },
    });
  };

  const handleEnableSsoChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSsoSettingsEnabled(event.target.checked);

    if (event.target.checked) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing organization '${organization.name}' SSO settings...`} />, infoNotificationOptions);

    commitRemoveOrganizationSsoSettings({
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
            render: <NotificationContent content={`Failed to remove organization '${organization.name}' SSO settings. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization.name} SSO settings removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove organization '${organization.name}' SSO settings. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        removeOrganizationSsoSettings: {
          organization: {
            id: organization.id,
            ssoSettings: organization.ssoSettings
              ? {
                  id: organization.ssoSettings.id,
                  isActive: false,
                  entityId: organization.ssoSettings.entityId,
                  loginUrl: organization.ssoSettings.loginUrl,
                  appFederationMetadataUrl: organization.ssoSettings.appFederationMetadataUrl,
                }
              : null,
          },
        },
      },
    });
  };

  return (
    <Form
      onSubmit={handleEnableOrganizationSsoSettingsClick}
      initialValues={{
        entityId: ssoSettingsEntityId,
        loginUrl: ssoSettingsLoginUrl,
        appFederationMetadataUrl: ssoSettingsAppFederationMetadataUrl,
      }}
      validate={validateSsoSettings}
      render={({ handleSubmit, values }) => {
        const formValues = values!;

        debounceSetSsoSettingsEntityId(formValues.entityId);
        debounceSetSsoSettingsLoginUrl(formValues.loginUrl);
        debounceSetSsoSettingsAppFederationMetadataUrl(formValues.appFederationMetadataUrl);

        return (
          <FormStackColumn onSubmit={handleSubmit}>
            <Box sx={{ pb: 2 }}>
              <SettingsSectionCard title="SSO setup" description="Configure enterprise sign-in and identity federation for organization members.">
                <StackColumn sx={formColumnSx}>
                  <FormFieldLabel label="Enable Sign sign-on">
                    <Switch checked={!!ssoSettingsEnabled} onChange={handleEnableSsoChange} />
                  </FormFieldLabel>

                  {ssoSettingsEnabled && (
                    <>
                      <FormFieldLabel label="Entity Id">
                        <TextField name="entityId" required={requiredSsoSettingsFields.entityId} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Login Url">
                        <TextField name="loginUrl" required={requiredSsoSettingsFields.loginUrl} />
                      </FormFieldLabel>

                      <FormFieldLabel label="App Federation Metadata Url">
                        <TextField name="appFederationMetadataUrl" required={requiredSsoSettingsFields.appFederationMetadataUrl} />
                      </FormFieldLabel>
                    </>
                  )}

                  {ssoSettingsEnabled ? <EditorActionBar primaryAction="Update" /> : null}
                </StackColumn>
              </SettingsSectionCard>
            </Box>
          </FormStackColumn>
        );
      }}
    />
  );
};

const OrganizationAdminSsoSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationAdminSsoSectionQuery>(RootQuery);

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

  return <OrganizationAdminSsoSectionContent organizationCustomDomain={organizationCustomDomain} queryReference={queryReference} />;
};

export default memo(OrganizationAdminSsoSection);
