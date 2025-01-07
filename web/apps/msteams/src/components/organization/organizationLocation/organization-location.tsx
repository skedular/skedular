import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import {
  BodyIconTypography,
  FormFieldLabel,
  SectionIconTypography,
  StackColumn,
  StackColumnWithSaveExitCancelAppBar,
} from '@repo/shared/components/commons';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultPadding } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useRef } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import { getModernOrganizationLocationsBaseLink } from '../organization-link';
import type { organizationLocation_query$key } from './__generated__/organizationLocation_query.graphql';
import type { organizationLocation_updateLocationMutation } from './__generated__/organizationLocation_updateLocationMutation.graphql';
import { expandedDrawerWidthPx } from './commons';
import OrganizationLocationLeftSideNavigationMenuContent from './organization-location-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationLocation_query$key;
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
};

type LocationDetails = {
  name: string;
  about: string | null;
  timezone?: string;
  physicalAddress: string;
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().nullable(),
  physicalAddress: string().nullable(),
});

const OrganizationLocation = ({ rootDataRelay, organizationId, locationId }: Props) => {
  const rootData = useFragment<organizationLocation_query$key>(
    graphql`
      fragment organizationLocation_query on Query {
        location(id: $locationId) {
          id
          name
          about
          timezone
          physicalAddress {
            formattedAddress
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateLocation] = useMutation<organizationLocation_updateLocationMutation>(graphql`
    mutation organizationLocation_updateLocationMutation($input: UpdateLocationInput!) @raw_response_type {
      updateLocation(input: $input) {
        location {
          id
          name
          about
          timezone
          physicalAddress {
            formattedAddress
          }
        }
      }
    }
  `);

  const navigate = useNavigate();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [searchParams] = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validate = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);

  useEffect(() => {
    if (!section || section === 'setup') {
      return;
    }

    const element = sectionRefs.current[section];
    if (!element) {
      return;
    }

    const appBarHeight = document.querySelector('.app-bar')?.clientHeight || 0;
    const elementTop = element.getBoundingClientRect().top + window.scrollY;
    window.scrollTo({
      top: elementTop - appBarHeight,
      behavior: 'smooth',
    });
  }, [section]);

  const handleDetailUpdateClick = ({ name, about, timezone, physicalAddress }: LocationDetails) => {
    if (!rootData.location) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating location '${rootData.location.name}'...`} />, infoNotificationOptions);

    commitUpdateLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.location.id,
          name,
          about,
          timezone,
          organizationId,
          physicalAddress: {
            formattedAddress: physicalAddress,
          },
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update location '${rootData.location?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location ${name} details updated.`} />,
        });

        navigate(getModernOrganizationLocationsBaseLink(organizationId));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update location '${rootData.location?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateLocation: {
          location: {
            id: rootData.location.id,
            name,
            about,
            timezone,
            physicalAddress: {
              formattedAddress: physicalAddress,
            },
          },
        },
      },
    });
  };

  const handleCancelClick = () => {
    navigate(getModernOrganizationLocationsBaseLink(organizationId));
  };

  if (!rootData.location) {
    return <></>;
  }

  const location = rootData.location;

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationLocationLeftSideNavigationMenuContent organizationId={organizationId} locationId={locationId} hideIcons />
        <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
          <Form
            onSubmit={handleDetailUpdateClick}
            initialValues={{
              name: location.name,
              about: location.about,
              timezone: location.timezone,
              physicalAddress: location.physicalAddress?.formattedAddress,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <StackColumnWithSaveExitCancelAppBar onSubmit={handleSubmit} onCancel={handleCancelClick} label="Edit Location Information">
                <StackColumn
                  sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                  ref={(divElement) => {
                    sectionRefs.current['setup'] = divElement;
                  }}
                >
                  <SectionIconTypography label="Location Setup" />
                  <BodyIconTypography label="Edit your location name and details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredFields.name} />
                  </FormFieldLabel>

                  <FormFieldLabel label="About">
                    <TextField name="about" required={requiredFields.about} multiline rows={3} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Timezone">
                    <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Physical Address">
                    <TextField name="physicalAddress" required={requiredFields.physicalAddress} multiline rows={5} />
                  </FormFieldLabel>
                </StackColumn>
              </StackColumnWithSaveExitCancelAppBar>
            )}
          />
        </Box>
      </Box>
    </>
  );
};

export default memo(OrganizationLocation);
