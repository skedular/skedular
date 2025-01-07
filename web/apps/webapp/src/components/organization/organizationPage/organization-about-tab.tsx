import { OrganizationMultipleChoicesIndustries } from '@/components/organization';
import type { organizationAboutTab_rootQuery } from '@/queries/__generated__/organizationAboutTab_rootQuery.graphql';
import type { organizationAboutTab_updateOrganizationMutation } from '@/queries/__generated__/organizationAboutTab_updateOrganizationMutation.graphql';
import { FormFieldLabel, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<organizationAboutTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query organizationAboutTab_rootQuery($organizationId: String!) {
    organization(id: $organizationId) {
      id
      name
      logoUrl
      about
      website
      canModify
      industrySubCategories {
        id
        name
      }
    }
    organizationIndustryMainCategoriesReferences {
      subCategories {
        id
        name
      }
    }
    ...organizationMultipleChoicesIndustries_query
  }
`;

type OrganizationDetails = {
  name: string;
  about: string | null;
  website: string | null;
  industrySubCategoryIds: string[];
};

const organizationSchema = object({
  name: string().min(3, 'Organization name must be at least three characters long.').required('Organization name is required'),
  about: string().nullable(),
  website: string().nullable(),
  industrySubCategoryIds: array().nullable(),
});

const OrganizationAboutTab = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<organizationAboutTab_rootQuery>(RootQuery, queryReference);
  const [commitUpdateOrganization] = useMutation<organizationAboutTab_updateOrganizationMutation>(graphql`
    mutation organizationAboutTab_updateOrganizationMutation($input: UpdateOrganizationInput!) @raw_response_type {
      updateOrganization(input: $input) {
        organization {
          id
          name
          about
          website
          industrySubCategories {
            id
            name
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(organizationSchema);
  const requiredFields = makeRequired(organizationSchema);
  const organization = rootData.organization;

  const handleUpdateClick = ({ name, about, website, industrySubCategoryIds }: OrganizationDetails) => {
    if (!organization) {
      return;
    }

    const selectedIndustrySubCategoryIds = industrySubCategoryIds ?? [];
    const toastId = themedToast(
      <NotificationContent content={`Updating organization '${rootData.organization.name}'...`} />,
      infoNotificationOptions,
    );

    commitUpdateOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: organization.id,
          name,
          about,
          website,
          industrySubCategoryIds: selectedIndustrySubCategoryIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${rootData.organization?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${name} details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization '${rootData.organization?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganization: {
          organization: {
            id: organization.id,
            name,
            about,
            website,
            industrySubCategories: rootData.organizationIndustryMainCategoriesReferences
              .flatMap((mainCategory) => mainCategory.subCategories)
              .filter(({ id }) => selectedIndustrySubCategoryIds.find((selectedIndustrySubCategoryId) => selectedIndustrySubCategoryId === id))
              .map(({ id, name }) => ({ id, name })),
          },
        },
      },
    });
  };

  if (!organization) {
    return <></>;
  }

  return (
    <Form
      onSubmit={handleUpdateClick}
      initialValues={{
        name: organization.name,
        about: organization.about,
        website: organization.website,
        industrySubCategoryIds: organization.industrySubCategories.map(({ id }) => id),
      }}
      validate={validate}
      render={({ handleSubmit }) => (
        <FormStackColumn onSubmit={handleSubmit}>
          <FormFieldLabel label="Name">
            <TextField name="name" required={requiredFields.name} />
          </FormFieldLabel>

          <FormFieldLabel label="About">
            <TextField name="about" required={requiredFields.about} multiline rows={3} />
          </FormFieldLabel>

          <FormFieldLabel label="Industry">
            <TextField name="website" required={requiredFields.about} helperText="https://" />
          </FormFieldLabel>

          <FormFieldLabel label="Industry">
            <OrganizationMultipleChoicesIndustries
              rootDataRelay={rootData}
              name="industrySubCategoryIds"
              required={requiredFields.industrySubCategoryIds}
            />
          </FormFieldLabel>

          <TwoButtonsDialogActions primaryLabel="Update" hideSecondary />
        </FormStackColumn>
      )}
    />
  );
};

const MemoOrganizationAboutTab = memo(OrganizationAboutTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationAboutTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationAboutTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationAboutTab queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationAboutTabWithRelay);
