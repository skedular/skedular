import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { Loading } from '@/components/loading';
import { SingleChoiceLocation } from '@/components/location/locationSelector';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { OrganizationMemberSelector } from '@/components/organization';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { ImageFileUploader } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { addTeam_addTeamMutation } from '@/queries/__generated__/addTeam_addTeamMutation.graphql';
import type { addTeam_rootQuery } from '@/queries/__generated__/addTeam_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import Image from 'next/image';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { array, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<addTeam_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  onAdded: (teamId: string) => void;
  onCancel: () => void;
  addLabel?: string;
  showDismiss: boolean;
};

const RootQuery = graphql`
  query addTeam_rootQuery(
    $organizationUniqueAlphanumericName: String!
    $organizationExists: Boolean!
    $bookingPeopleNameSearchText: String
    $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    me {
      id
    }
    organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) @include(if: $organizationExists) {
      id
      name
    }
    ...organizationMemberSelector_query
    ...singleChoiceLocation_locations_query
  }
`;

type TeamDetails = {
  name: string;
  about: string | null;
  timezone: string | null;
  organizationMemberIds: string[];
  primaryLocationId?: string;
};

const teamSchema = object({
  name: string().min(3, 'Team name must be at least three characters long.').required('Team name is required'),
  about: string().nullable(),
  timezone: string().nullable(),
  organizationMemberIds: array().nullable(),
  primaryLocationId: string().nullable(),
});

const AddTeam = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName, onAdded, onCancel, addLabel, showDismiss }: Props) => {
  const rootData = usePreloadedQuery<addTeam_rootQuery>(RootQuery, queryReference);
  const [commitAddTeam] = useMutation<addTeam_addTeamMutation>(graphql`
    mutation addTeam_addTeamMutation($input: AddTeamInput!) @raw_response_type {
      addTeam(input: $input) {
        team {
          id
          name
          about
          timezone
          primaryFeatureImage {
            original {
              url
              height
              width
            }
            thumbnail {
              url
              height
              width
            }
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateTeamDetails = makeValidate(teamSchema);
  const requiredTeamDetailsFields = makeRequired(teamSchema);
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse>();

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
  };

  const handleTeamAddClick = ({ name, about, timezone, organizationMemberIds, primaryLocationId }: TeamDetails) => {
    const id = uuid();
    const customerIds = !organizationUniqueAlphanumericName ? [rootData.me.id] : [];
    const toastId = themedToast(<NotificationContent content={`Adding team '${name}'...`} />, infoNotificationOptions);
    const finalPrimaryFeatureImage = primaryFeatureImage
      ? {
          original: primaryFeatureImage.original
            ? { url: primaryFeatureImage.original.url, height: primaryFeatureImage.original.height, width: primaryFeatureImage.original.width }
            : null,
          thumbnail: primaryFeatureImage.thumbnail
            ? { url: primaryFeatureImage.thumbnail.url, height: primaryFeatureImage.thumbnail.height, width: primaryFeatureImage.thumbnail.width }
            : null,
        }
      : null;

    commitAddTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          name,
          about,
          timezone,
          primaryFeatureImage: finalPrimaryFeatureImage,
          customerIds,
          organizationUniqueAlphanumericName,
          organizationMemberIds: [...new Set(organizationMemberIds)],
          primaryLocationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add new team '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Team ${name} added.`} />,
        });

        onAdded(id);
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add new team '${name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addTeam: {
          team: {
            id,
            name,
            about,
            timezone,
            primaryFeatureImage: finalPrimaryFeatureImage,
          },
        },
      },
    });
  };

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setPrimaryFeatureImage(response);
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Add Team">
          <Form
            onSubmit={handleTeamAddClick}
            initialValues={{
              organizationMemberIds: [],
            }}
            validate={validateTeamDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Team Setup" />
                  <BodyIconTypography label="Add your team name and details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Feature image">
                    <StackColumn>
                      {primaryFeatureImage?.thumbnail && primaryFeatureImage.original.height && primaryFeatureImage.original.width && (
                        <Image src={primaryFeatureImage.original.url} height={primaryFeatureImage.original.height} width={primaryFeatureImage.original.width} alt="" />
                      )}
                      <ImageFileUploader onUploadCompleted={handleFeatureImageUploadCompleted} />
                    </StackColumn>
                  </FormFieldLabel>

                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredTeamDetailsFields.name} />
                  </FormFieldLabel>

                  <FormFieldLabel label="About">
                    <TextField name="about" required={requiredTeamDetailsFields.about} multiline rows={3} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Timezone">
                    <SingleChoinceTimezone name="timezone" required={requiredTeamDetailsFields.timezone} />
                  </FormFieldLabel>
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Location Settings" />
                  <BodyIconTypography label="Assign team to locations" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Primary Location">
                    <SingleChoiceLocation rootDataRelay={rootData} id="primaryLocationId" required={requiredTeamDetailsFields.primaryLocationId} />
                  </FormFieldLabel>
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Team Members" />
                  <BodyIconTypography label="Manage your team members" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Organization Users">
                    <OrganizationMemberSelector
                      rootDataRelay={rootData}
                      organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                      name="organizationMemberIds"
                      required={requiredTeamDetailsFields.organizationMemberIds}
                      multiple={true}
                      useMemberId={true}
                    />
                  </FormFieldLabel>
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <StackRow>
                    {showDismiss && (
                      <Button variant="contained" sx={defaultButtonStyle} onClick={handleCloseClick}>
                        <BodyIconTypography label="Dismiss" invertDefaultColor={paletteMode === 'dark'} />
                      </Button>
                    )}
                    <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                      <BodyIconTypography label={addLabel ?? 'Add'} invertDefaultColor={paletteMode === 'dark'} />
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

const MemoAddTeam = memo(AddTeam);

type RelayProps = {
  organizationUniqueAlphanumericName: string;
  onReloadRequired: () => void;
  onAdded: (teamId: string) => void;
  onCancel: () => void;
  addLabel?: string;
  showDismiss: boolean;
};

const AddTeamWithRelay = ({ organizationUniqueAlphanumericName, onReloadRequired, onAdded, onCancel, addLabel, showDismiss }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addTeam_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationUniqueAlphanumericName,
        organizationExists: !!organizationUniqueAlphanumericName,
        organizationMemberSelectorOrganizationMembersSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoAddTeam
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
        onAdded={onAdded}
        onCancel={onCancel}
        addLabel={addLabel}
        showDismiss={showDismiss}
      />
    </ErrorBoundary>
  );
};

export default memo(AddTeamWithRelay);
