import { PaletteModeContext, getRelayErrorMessage, keyboardTextFieldDebounceTimeout } from '@skedular/shared';
import { FormFieldLabel, FormStackColumn, StackColumn } from '@skedular/ui';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { SsoSettingsDetails, ssoSettingsSchema } from '@/components/organization/organizationSettings/organization-settings-shared';

import type { organizationSettingsSsoSectionQuery } from '@/queries/__generated__/organizationSettingsSsoSectionQuery.graphql';
import type { organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation } from '@/queries/__generated__/organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation.graphql';
import Box from '@mui/material/Box';
import Switch from '@mui/material/Switch';
import { SettingsSectionCard } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useCallback, useContext, useEffect, useRef, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
};

type InnerProps = {
  queryReference: PreloadedQuery<organizationSettingsSsoSectionQuery>;
};

type SubmittedSsoSettings = SsoSettingsDetails & {
  isActive: boolean;
};

const RootQuery = graphql`
  query organizationSettingsSsoSectionQuery($organizationCustomDomain: String!) {
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

const OrganizationSettingsSsoSectionContent = ({ queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationSettingsSsoSectionQuery>(RootQuery, queryReference);
  const [commitUpdateOrganizationSsoSettingsPatch] = useMutation<organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation>(graphql`
    mutation organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation($input: UpdateOrganizationSsoSettingsInput!) @raw_response_type {
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
  const submittedSsoSettings = useRef<SubmittedSsoSettings>({
    entityId: organization?.ssoSettings?.entityId ?? '',
    loginUrl: organization?.ssoSettings?.loginUrl ?? '',
    appFederationMetadataUrl: organization?.ssoSettings?.appFederationMetadataUrl ?? '',
    isActive: !!organization?.ssoSettings?.isActive,
  });

  const patchOrganizationSsoSettings = useCallback(
    ({ entityId, loginUrl, appFederationMetadataUrl, isActive }: SubmittedSsoSettings) => {
      if (!organization) {
        return;
      }

      commitUpdateOrganizationSsoSettingsPatch({
        variables: {
          input: {
            clientMutationId: uuid(),
            organizationCustomDomain: organization.customDomain,
            fieldsToUpdate: ['SSO_SETTINGS'],
            entityId,
            loginUrl,
            appFederationMetadataUrl,
            isActive,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            themedToast(
              <NotificationContent content={`Failed to update organization '${organization.name}' SSO settings. Error: ${getRelayErrorMessage(errors)}.`} />,
              errorNotificationOptions,
            );

            return;
          }

          submittedSsoSettings.current = { entityId, loginUrl, appFederationMetadataUrl, isActive };
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to update organization '${organization.name}' SSO settings. Error: ${error.message}.`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateOrganizationSsoSettings: {
            organization: {
              id: organization.id,
              ssoSettings: {
                id: organization.ssoSettings?.id ?? '',
                isActive,
                entityId,
                loginUrl,
                appFederationMetadataUrl,
              },
            },
          },
        },
      });
    },
    [commitUpdateOrganizationSsoSettingsPatch, organization, themedToast],
  );

  useEffect(() => {
    const nextSsoSettings = {
      entityId: ssoSettingsEntityId,
      loginUrl: ssoSettingsLoginUrl,
      appFederationMetadataUrl: ssoSettingsAppFederationMetadataUrl,
      isActive: !!ssoSettingsEnabled,
    };
    const previousSsoSettings = submittedSsoSettings.current;
    const unchanged =
      previousSsoSettings.entityId === nextSsoSettings.entityId &&
      previousSsoSettings.loginUrl === nextSsoSettings.loginUrl &&
      previousSsoSettings.appFederationMetadataUrl === nextSsoSettings.appFederationMetadataUrl &&
      previousSsoSettings.isActive === nextSsoSettings.isActive;

    if (unchanged || !ssoSettingsSchema.isValidSync(nextSsoSettings)) {
      return;
    }

    patchOrganizationSsoSettings(nextSsoSettings);
  }, [patchOrganizationSsoSettings, ssoSettingsAppFederationMetadataUrl, ssoSettingsEnabled, ssoSettingsEntityId, ssoSettingsLoginUrl]);

  const handleEnableSsoChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSsoSettingsEnabled(event.target.checked);
  };

  if (!organization) {
    return null;
  }

  return (
    <Form
      onSubmit={() => undefined}
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
                  <FormFieldLabel label="Entity Id">
                    <TextField name="entityId" required={requiredSsoSettingsFields.entityId} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Login Url">
                    <TextField name="loginUrl" required={requiredSsoSettingsFields.loginUrl} />
                  </FormFieldLabel>

                  <FormFieldLabel label="App Federation Metadata Url">
                    <TextField name="appFederationMetadataUrl" required={requiredSsoSettingsFields.appFederationMetadataUrl} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Enable SSO across the organisation">
                    <Switch checked={!!ssoSettingsEnabled} onChange={handleEnableSsoChange} />
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

const OrganizationSettingsSsoSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationSettingsSsoSectionQuery>(RootQuery);

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

  return <OrganizationSettingsSsoSectionContent queryReference={queryReference} />;
};

export default memo(OrganizationSettingsSsoSection);
