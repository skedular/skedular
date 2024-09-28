import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Grid from '@mui/material/Grid2';
import MuiLink from '@mui/material/Link';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { EditIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationMultipleChoicesIndustries } from 'components/organization';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { array, object, string } from 'yup';
import type { organizationAboutTab_query$key } from './__generated__/organizationAboutTab_query.graphql';
import type { organizationAboutTab_updateOrganizationMutation } from './__generated__/organizationAboutTab_updateOrganizationMutation.graphql';

type Props = {
  rootDataRelay: organizationAboutTab_query$key;
};

interface OrganizationDetails {
  name: string;
  about: string | null;
  website: string | null;
  industrySubCategoryIds: string[];
}

const maxChipTextWidthToDisplay = 200;

const organizationSchema = object({
  name: string().min(3, 'Organization name must be at least three charcters long.').required('Organization name is required'),
  about: string().nullable(),
  website: string().nullable(),
  industrySubCategoryIds: array().nullable(),
});

const OrganizationAboutTab = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<organizationAboutTab_query$key>(
    graphql`
      fragment organizationAboutTab_query on Query {
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
    `,
    rootDataRelay,
  );

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
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(organizationSchema);
  const requiredFields = makeRequired(organizationSchema);

  const handleEditClick = () => {
    setEditing(true);
  };

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
        } else {
          setEditing(false);
        }
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

  const handleCancelClick = () => {
    setEditing(false);
  };

  if (!organization) {
    return <></>;
  }

  return (
    <>
      <Stack direction="row" sx={{ justifyContent: 'flex-end' }} spacing={1}>
        {!editing && rootData.organization.canModify && (
          <Button size="small" color="primary" onClick={handleEditClick}>
            <EditIcon />
          </Button>
        )}
      </Stack>
      {!editing && (
        <Stack direction="column" spacing={1}>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <Typography variant="h6">About</Typography>
            <Typography variant="body1">{organization.about}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <Typography variant="h6">Website</Typography>
            {organization.website && (
              <MuiLink href={organization?.website} target="_blank" rel="noopener noreferrer">
                {organization.website}
              </MuiLink>
            )}
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <Typography variant="h6">Industry</Typography>
            <Grid sx={{ marginLeft: 1 }}>
              {organization.industrySubCategories.map(({ id, name }) => (
                <Tooltip key={id} title={name}>
                  <Chip
                    label={name}
                    sx={{
                      marginRight: 1,
                      maxWidth: maxChipTextWidthToDisplay,
                    }}
                  />
                </Tooltip>
              ))}
            </Grid>
          </Stack>
        </Stack>
      )}
      {editing && (
        <Paper elevation={24} sx={{ padding: 2 }}>
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
              <Stack direction="column" component="form" noValidate onSubmit={handleSubmit} spacing={2}>
                <TextField label="Name" name="name" required={requiredFields.name} />
                <TextField label="About" name="about" required={requiredFields.about} multiline={true} />
                <TextField label="Website" name="website" required={requiredFields.about} helperText="https://" />
                <OrganizationMultipleChoicesIndustries
                  rootDataRelay={rootData}
                  name="industrySubCategoryIds"
                  required={requiredFields.industrySubCategoryIds}
                />

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

export default memo(OrganizationAboutTab);
