import { OrganizationMultipleChoicesIndustries, OrganizationTermsOfUse } from '@/components/organization';
import type { addOrganization_addOrganizationMutation } from '@/queries/__generated__/addOrganization_addOrganizationMutation.graphql';
import type { addOrganization_completeOrganizationOnboardingMutation } from '@/queries/__generated__/addOrganization_completeOrganizationOnboardingMutation.graphql';
import type { addOrganization_rootQuery } from '@/queries/__generated__/addOrganization_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  FormFieldLabel,
  FormStackColumn,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@repo/shared/components/commons';
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
import { defaultPadding } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, boolean, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<addOrganization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  showCancel: boolean;
  onAdded: (id: string) => void;
  onCancel?: () => void;
  addLabel?: string;
};

const RootQuery = graphql`
  query addOrganization_rootQuery {
    activeOrganizationTermsOfUse {
      id
    }
    ...organizationMultipleChoicesIndustries_query
    ...organizationTermsOfUse_query
  }
`;

type OrganizationDetails = {
  name: string;
  about: string | null;
  website: string | null;
  agreedToTermsOfUse: boolean;
  industrySubCategoryIds: string[];
};

const organizationSchema = object({
  name: string().min(3, 'Organization name must be at least three characters long.').required('Organization name is required'),
  about: string().nullable(),
  website: string().nullable(),
  industrySubCategoryIds: array().nullable(),
  agreedToTermsOfUse: boolean().oneOf([true], 'Please accept the terms').required('Please accept the terms'),
});

const AddOrganization = ({ queryReference, onReloadRequired, showCancel, onAdded, onCancel, addLabel }: Props) => {
  const rootData = usePreloadedQuery<addOrganization_rootQuery>(RootQuery, queryReference);
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

  const [commitCompleteOrganizationOnboarding] = useMutation<addOrganization_completeOrganizationOnboardingMutation>(graphql`
    mutation addOrganization_completeOrganizationOnboardingMutation($input: CompleteOrganizationOnboardingInput!) {
      completeOrganizationOnboarding(input: $input) {
        clientMutationId
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateOrganizationDetails = makeValidate(organizationSchema);
  const requiredOrganizationDetailsFields = makeRequired(organizationSchema);

  const handleOrganizationAddClick = ({ name, about, website, industrySubCategoryIds }: OrganizationDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding organization '${name}'...`} />, infoNotificationOptions);

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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add new organization '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

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
              toast.update(toastId, {
                ...errorNotificationOptions,
                render: <NotificationContent content={`Failed to complete organization onboarding. Error: ${joinErrors(errors)}.`} />,
              });
            } else {
              toast.update(toastId, {
                ...successNotificationOptions,
                render: <NotificationContent content={`Organization ${name} added.`} />,
              });

              onAdded(id);
              onReloadRequired();
            }
          },
          onError: (error) => {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to complete organization onboarding. Error: ${error.message}.`} />,
            });
          },
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add new organization '${name}'. Error: ${error.message}.`} />,
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
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={onCancel} label="Add Organization" hideClose={!showCancel}>
          <Form
            onSubmit={handleOrganizationAddClick}
            initialValues={{
              name: '',
              about: null,
              website: null,
            }}
            validate={validateOrganizationDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Organization Setup" />
                  <BodyIconTypography label="Edit your organization name and details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredOrganizationDetailsFields.name} />
                  </FormFieldLabel>

                  <FormFieldLabel label="About">
                    <TextField name="about" required={requiredOrganizationDetailsFields.about} multiline rows={3} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Website">
                    <TextField name="website" required={requiredOrganizationDetailsFields.about} helperText="https://" />
                  </FormFieldLabel>

                  <FormFieldLabel label="Industry">
                    <OrganizationMultipleChoicesIndustries
                      rootDataRelay={rootData}
                      name="industrySubCategoryIds"
                      required={requiredOrganizationDetailsFields.industrySubCategoryIds}
                    />
                  </FormFieldLabel>
                  <OrganizationTermsOfUse
                    rootDataRelay={rootData}
                    name="agreedToTermsOfUse"
                    required={requiredOrganizationDetailsFields.agreedToTermsOfUse}
                  />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <StackRow>
                    <Button variant="contained" color="primary" type="submit" sx={{ textTransform: 'none' }}>
                      <SmallIconTypography label={addLabel ?? 'Add'} />
                    </Button>
                  </StackRow>
                </StackColumn>
              </FormStackColumn>
            )}
          />
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

const MemoAddOrganization = memo(AddOrganization);

type RelayProps = {
  onReloadRequired: () => void;
  showCancel: boolean;
  onAdded: (id: string) => void;
  onCancel?: () => void;
  addLabel?: string;
};

const AddOrganizationWithRelay = ({ onReloadRequired, showCancel, onAdded, onCancel, addLabel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addOrganization_rootQuery>(RootQuery);
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

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoAddOrganization
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        showCancel={showCancel}
        onAdded={onAdded}
        onCancel={onCancel}
        addLabel={addLabel}
      />
    </ErrorBoundary>
  );
};

export default memo(AddOrganizationWithRelay);
