import { OrganizationMultipleChoicesIndustries, OrganizationTermsOfUse } from '@/components/organization';
import type { addOrganization_addOrganizationMutation } from '@/queries/__generated__/addOrganization_addOrganizationMutation.graphql';
import type { addOrganization_query$key } from '@/queries/__generated__/addOrganization_query.graphql';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { useSnackbar } from 'notistack';
import { memo } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { array, boolean, object, string } from 'yup';

type Props = {
  rootDataRelay: addOrganization_query$key;
};

interface OrganizationDetails {
  name: string;
  about: string | null;
  website: string | null;
  agreedToTermsOfUse: boolean;
  industrySubCategoryIds: string[];
}

const organizationSchema = object({
  name: string().min(3, 'Organization name must be at least three charcters long.').required('Organization name is required'),
  about: string().nullable(),
  website: string().nullable(),
  industrySubCategoryIds: array().nullable(),
  agreedToTermsOfUse: boolean().oneOf([true], 'Please accept the terms').required('Please accept the terms'),
});

const AddOrganization = ({ rootDataRelay }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment addOrganization_query on Query {
        activeOrganizationTermsOfUse {
          id
        }
        ...organizationMultipleChoicesIndustries_query
        ...organizationTermsOfUse_query
      }
    `,
    rootDataRelay,
  );

  const [commitAddOrganization] = useMutation<addOrganization_addOrganizationMutation>(graphql`
    mutation addOrganization_addOrganizationMutation($input: AddOrganizationInput!) @raw_response_type {
      addOrganization(input: $input) {
        organization {
          id
          name
          about
          website
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const router = useRouter();
  const validate = makeValidate(organizationSchema);
  const requiredFields = makeRequired(organizationSchema);

  const handleCancelClick = () => {
    router.back();
  };

  const handleOrganizationCreateClick = ({ name, about, website, industrySubCategoryIds }: OrganizationDetails) => {
    const id = nanoid();

    commitAddOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
          name,
          about,
          website,
          agreedToTermsOfUse: true,
          termsOfUseId: rootData.activeOrganizationTermsOfUse.id,
          industrySubCategoryIds: industrySubCategoryIds ?? [],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to add new organization '${name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          router.back();
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to add new organization '${name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addOrganization: {
          organization: {
            id,
            name,
            about,
            website,
          },
        },
      },
    });
  };

  return (
    <Paper elevation={24} sx={{ padding: 2 }}>
      <Form
        onSubmit={handleOrganizationCreateClick}
        initialValues={{
          name: '',
          about: null,
          website: null,
        }}
        validate={validate}
        render={({ handleSubmit }) => (
          <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
            <TextField label="Name" name="name" required={requiredFields.name} />
            <TextField label="About" name="about" required={requiredFields.about} multiline={true} />
            <TextField label="Website" name="website" required={requiredFields.about} helperText="https://" />
            <OrganizationMultipleChoicesIndustries
              rootDataRelay={rootData}
              name="industrySubCategoryIds"
              required={requiredFields.industrySubCategoryIds}
            />
            <OrganizationTermsOfUse rootDataRelay={rootData} name="agreedToTermsOfUse" required={requiredFields.agreedToTermsOfUse} />

            <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
              <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                Cancel
              </Button>
              <Button color="primary" variant="contained" type="submit">
                Create
              </Button>
            </Stack>
          </Stack>
        )}
      />
    </Paper>
  );
};

export default memo(AddOrganization);
