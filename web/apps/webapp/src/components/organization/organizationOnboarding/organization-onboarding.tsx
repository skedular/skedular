import { OrganizationMultipleChoicesIndustries, OrganizationTermsOfUse } from '@/components/organization';
import type { organizationOnboarding_addOrganizationMutation } from '@/queries/__generated__/organizationOnboarding_addOrganizationMutation.graphql';
import type { organizationOnboarding_completeOrganizationOnboardingMutation } from '@/queries/__generated__/organizationOnboarding_completeOrganizationOnboardingMutation.graphql';
import type { organizationOnboarding_rootQuery } from '@/queries/__generated__/organizationOnboarding_rootQuery.graphql';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Step from '@mui/material/Step';
import StepLabel from '@mui/material/StepLabel';
import Stepper from '@mui/material/Stepper';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, graphql, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { array, boolean, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<organizationOnboarding_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

type OrganizationDetails = {
  name: string;
  about: string | null;
  website: string | null;
  industrySubCategoryIds: string[] | null;
  agreedToTermsOfUse: boolean;
};

const RootQuery = graphql`
  query organizationOnboarding_rootQuery {
    me {
      id
      isOrganizationOnboardingDone
      isLocationOnboardingDone
    }
    activeOrganizationTermsOfUse {
      id
    }
    organizationIndustryMainCategoriesReferences {
      subCategories {
        id
        name
      }
    }
    ...organizationMultipleChoicesIndustries_query
    ...organizationTermsOfUse_query
  }
`;

const organizationSchema = object({
  name: string().min(3, 'Organization name must be at least three charcters long.').required('Organization name is required'),
  about: string().nullable(),
  website: string().nullable(),
  industrySubCategoryIds: array().of(string()).nullable(),
  agreedToTermsOfUse: boolean().oneOf([true], 'Please accept the terms').required('Please accept the terms'),
});

const OrganizationOnboarding = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<organizationOnboarding_rootQuery>(RootQuery, queryReference);
  const [commitAddOrganization] = useMutation<organizationOnboarding_addOrganizationMutation>(graphql`
    mutation organizationOnboarding_addOrganizationMutation($input: AddOrganizationInput!) @raw_response_type {
      addOrganization(input: $input) {
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

  const [commitCompleteOrganizationOnboarding] = useMutation<organizationOnboarding_completeOrganizationOnboardingMutation>(graphql`
    mutation organizationOnboarding_completeOrganizationOnboardingMutation($input: CompleteOrganizationOnboardingInput!) @raw_response_type {
      completeOrganizationOnboarding(input: $input) {
        customer {
          id
          isOrganizationOnboardingDone
          isLocationOnboardingDone
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const [activeStep, setActiveStep] = useState(0);
  const [isOnboardingOpen, setIsOnboardingOpen] = useState(!rootData.me?.isOrganizationOnboardingDone);
  const validate = makeValidate(organizationSchema);
  const requiredFields = makeRequired(organizationSchema);
  const handleOrganizationCreateClick = ({ name, about, website, industrySubCategoryIds }: OrganizationDetails) => {
    const id = nanoid();
    const selectedIndustrySubCategoryIds = industrySubCategoryIds ?? [];

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
          industrySubCategoryIds: selectedIndustrySubCategoryIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to add new organization '${name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        if (!rootData.me) {
          return;
        }

        commitCompleteOrganizationOnboarding({
          variables: {
            input: {
              clientMutationId: nanoid(),
            },
          },
          onCompleted: (_, errors) => {
            if (errors && errors.length > 0) {
              enqueueSnackbar(`Failed to complete organization onboarding. Error: ${joinErrors(errors)}`, {
                variant: 'error',
                anchorOrigin,
              });
            } else {
              setIsOnboardingOpen(false);
            }
          },
          onError: (error) => {
            enqueueSnackbar(`Failed to complete organization onboarding. Error: ${error.message}`, {
              variant: 'error',
              anchorOrigin,
            });
          },
          optimisticResponse: {
            completeOrganizationOnboarding: {
              customer: {
                id: rootData.me.id,
                isOrganizationOnboardingDone: true,
                isLocationOnboardingDone: rootData.me ? rootData.me.isLocationOnboardingDone : false,
              },
            },
          },
        });
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
            industrySubCategories: rootData.organizationIndustryMainCategoriesReferences
              .flatMap((mainCategory) => mainCategory.subCategories)
              .filter(({ id }) => selectedIndustrySubCategoryIds.find((selectedIndustrySubCategoryId) => selectedIndustrySubCategoryId === id))
              .map(({ id, name }) => ({ id, name })),
          },
        },
      },
    });
  };

  const handleDismissClick = () => {
    if (!rootData.me) {
      return;
    }

    commitCompleteOrganizationOnboarding({
      variables: {
        input: {
          clientMutationId: nanoid(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to dismiss organization onboarding. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        setIsOnboardingOpen(false);
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to dismiss organization onboarding. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        completeOrganizationOnboarding: {
          customer: {
            id: rootData.me.id,
            isOrganizationOnboardingDone: true,
            isLocationOnboardingDone: rootData.me ? rootData.me.isLocationOnboardingDone : false,
          },
        },
      },
    });
  };

  return (
    <>
      {isOnboardingOpen && (
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Stepper activeStep={activeStep}>
            <Step>
              <StepLabel>Create Organization</StepLabel>
            </Step>
          </Stepper>
          {activeStep === 0 && (
            <Form
              onSubmit={handleOrganizationCreateClick}
              initialValues={{
                name: '',
                about: null,
                website: null,
                agreedToTermsOfUse: false,
                industrySubCategoryIds: [],
                offeringFlexibilityIds: [],
                companyValueIds: [],
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
                  <OrganizationTermsOfUse rootDataRelay={rootData} name="agreedToTermsOfUse" required={requiredFields.agreedToTermsOfUse} />

                  <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
                    <Button color="secondary" variant="contained" onClick={handleDismissClick}>
                      Dismiss
                    </Button>
                    <Button color="primary" variant="contained" type="submit">
                      Create
                    </Button>
                  </Stack>
                </Stack>
              )}
            />
          )}
        </Paper>
      )}
    </>
  );
};

const MemoOrganizationOnboarding = memo(OrganizationOnboarding);

const OrganizationOnboardingWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<organizationOnboarding_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationOnboarding queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationOnboardingWithRelay);
