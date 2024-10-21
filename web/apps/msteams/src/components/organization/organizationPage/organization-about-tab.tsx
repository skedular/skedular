import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationMultipleChoicesIndustries } from 'components/organization';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { array, object, string } from 'yup';
import type { organizationAboutTab_rootQuery } from './__generated__/organizationAboutTab_rootQuery.graphql';
import type { organizationAboutTab_updateOrganizationMutation } from './__generated__/organizationAboutTab_updateOrganizationMutation.graphql';

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
  name: string().min(3, 'Organization name must be at least three charcters long.').required('Organization name is required'),
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

  const { enqueueSnackbar } = useSnackbar();
  const validate = makeValidate(organizationSchema);
  const requiredFields = makeRequired(organizationSchema);
  const organization = rootData.organization;

  const handleUpdateClick = ({ name, about, website, industrySubCategoryIds }: OrganizationDetails) => {
    if (!organization) {
      return;
    }

    const selectedIndustrySubCategoryIds = industrySubCategoryIds ?? [];

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
          enqueueSnackbar(`Failed to update organization '${name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        enqueueSnackbar(`Organization details updated.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update organization '${name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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
        <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
          <TextField label="Name" name="name" required={requiredFields.name} />
          <TextField label="About" name="about" required={requiredFields.about} multiline={true} />
          <TextField label="Website" name="website" required={requiredFields.about} helperText="https://" />
          <OrganizationMultipleChoicesIndustries
            rootDataRelay={rootData}
            name="industrySubCategoryIds"
            required={requiredFields.industrySubCategoryIds}
          />

          <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
            <Button color="primary" variant="contained" type="submit">
              Update
            </Button>
          </Stack>
        </Stack>
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
