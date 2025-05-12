import {
  AppBarWithStackColumn,
  BodyIconTypography,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { SingleChoiceCountry, SingleChoinceTimezone } from '@/components/forms';
import { BookingIcon, DeleteIcon, EllipseMenuIcon, NotPreferredIcon, PreferredIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationLocationResourceBaseLink, getOrganizationLocationsBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesLocationTags } from '@/components/organization';
import { CustomTagSelector } from '@/components/organization/customTagSelector/';
import { ZoneSelector } from '@/components/organization/zoneSelector';
import { ProductTags } from '@/components/productTag';
import { Resource } from '@/components/resource';
import { AddResourceButton } from '@/components/resource/addResource';
import { ResourceType } from '@/components/resourceType';
import { Search } from '@/components/search';
import { WeekOpeningHours, WeekOpeningHoursDetails } from '@/components/weekOpeningHours';
import { Zones } from '@/components/zone';
import { defaultGridRowSelectionModelValue } from '@/libs/mui';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, flame, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { organizationLocation_activateResourcesMutation } from '@/queries/__generated__/organizationLocation_activateResourcesMutation.graphql';
import type { organizationLocation_addCustomerPreferredResourceMutation } from '@/queries/__generated__/organizationLocation_addCustomerPreferredResourceMutation.graphql';
import type { organizationLocation_deactivateResourcesMutation } from '@/queries/__generated__/organizationLocation_deactivateResourcesMutation.graphql';
import type { organizationLocation_deleteLocationMutation } from '@/queries/__generated__/organizationLocation_deleteLocationMutation.graphql';
import type { organizationLocation_deleteResourcesMutation } from '@/queries/__generated__/organizationLocation_deleteResourcesMutation.graphql';
import type { organizationLocation_query$key } from '@/queries/__generated__/organizationLocation_query.graphql';
import type { organizationLocation_removeCustomerPreferredResourceMutation } from '@/queries/__generated__/organizationLocation_removeCustomerPreferredResourceMutation.graphql';
import type { organizationLocation_resources_query$key } from '@/queries/__generated__/organizationLocation_resources_query.graphql';
import type { organizationLocation_resources_refetchableFragment } from '@/queries/__generated__/organizationLocation_resources_refetchableFragment.graphql';
import type { organizationLocation_updateLocationMutation } from '@/queries/__generated__/organizationLocation_updateLocationMutation.graphql';
import type { organizationLocation_updateLocationOpeningHoursMutation } from '@/queries/__generated__/organizationLocation_updateLocationOpeningHoursMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';
import OrganizationLocationLeftSideNavigationMenuContent from './organization-location-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationLocation_query$key;
  rootDataResourcesRelay: organizationLocation_resources_query$key;
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
};

type LocationDetails = {
  name: string;
  about: string | null;
  timezone: string;
  locationTagIds: string[];
  contactEmail: string | null;
  contactPhone: string | null;
  addressLine1: string;
  addressLine2: string | null;
  suburb: string;
  city: string;
  province: string | null;
  zipcode: string;
  country: string;
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
  locationTagIds: array().nullable(),
  contactEmail: string()
    .nullable()
    .email(({ value }) => `${value} is not a valid email`),
  contactPhone: string().nullable(),
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string().required('Suburb is required'),
  city: string().required('City is required'),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  country: string().required('Country is required'),
});

type ResourceTypeDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ResourceDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type CustomTagDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ProductTagDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ResourceRowType = {
  id: string;
  resource: ResourceDetails;
  resourceType: ResourceTypeDetails;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
  productTags: ProductTagDetails[];
  status: boolean;
  preferred: boolean;
  capacity: number;
};

const OrganizationLocation = ({ rootDataRelay, rootDataResourcesRelay, onReloadRequired, organizationId, locationId }: Props) => {
  const rootData = useFragment<organizationLocation_query$key>(
    graphql`
      fragment organizationLocation_query on Query {
        me {
          id
          preferredResources {
            uniqueId
          }
        }
        organization(id: $organizationId) {
          type {
            type
          }
        }
        location(id: $locationId) {
          id
          name
          about
          timezone
          contactEmail
          contactPhone
          physicalAddress {
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
          }
          locationTags {
            uniqueId
            name
            color
          }
          openingHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
        openingHoursMinutesStep
        ...multipleChoicesLocationTags_query
        ...weekOpeningHours_query
        ...customTagSelector_allCustomTags_query
        ...zoneSelector_allZones_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataResources, refetchResources] = useRefetchableFragment<organizationLocation_resources_refetchableFragment, organizationLocation_resources_query$key>(
    graphql`
      fragment organizationLocation_resources_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationLocation_resources_refetchableFragment") {
        resources(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $resourceNameSearchText, customTagIds: $resourceCustomTagIds, zoneIds: $resourceZoneIds }
        ) @connection(key: "organizationLocation_resources") {
          __id
          totalCount
          edges {
            node {
              id
              name
              inactive
              requireBookingApproval
              color
              capacity
              customTags {
                uniqueId
                name
                color
              }
              zones {
                uniqueId
                name
                color
              }
              productTags {
                uniqueId
                name
                color
              }
              resourceType {
                uniqueId
                name
                color
              }
            }
          }
        }
      }
    `,
    rootDataResourcesRelay,
  );

  const [commitUpdateLocation] = useMutation<organizationLocation_updateLocationMutation>(graphql`
    mutation organizationLocation_updateLocationMutation($input: UpdateLocationInput!) @raw_response_type {
      updateLocation(input: $input) {
        location {
          id
          name
          about
          timezone
          contactEmail
          contactPhone
          physicalAddress {
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
          }
          locationTags {
            uniqueId
            name
            color
          }
          openingHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
      }
    }
  `);

  const [commitDeleteResources] = useMutation<organizationLocation_deleteResourcesMutation>(graphql`
    mutation organizationLocation_deleteResourcesMutation($connectionIds: [ID!]!, $input: DeleteResourcesInput!) {
      deleteResources(input: $input) {
        resources {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitActivateResources] = useMutation<organizationLocation_activateResourcesMutation>(graphql`
    mutation organizationLocation_activateResourcesMutation($input: ActivateResourcesInput!) {
      activateResources(input: $input) {
        resources {
          id
          name
          color
          inactive
          requireBookingApproval
          color
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
          productTags {
            uniqueId
            name
            color
          }
          resourceType {
            uniqueId
            name
            color
          }
        }
      }
    }
  `);

  const [commitDeactivateResources] = useMutation<organizationLocation_deactivateResourcesMutation>(graphql`
    mutation organizationLocation_deactivateResourcesMutation($input: DeactivateResourcesInput!) {
      deactivateResources(input: $input) {
        resources {
          id
          name
          color
          inactive
          requireBookingApproval
          color
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
          productTags {
            uniqueId
            name
            color
          }
          resourceType {
            uniqueId
            name
            color
          }
        }
      }
    }
  `);

  const [commitAddCustomerPreferredResource] = useMutation<organizationLocation_addCustomerPreferredResourceMutation>(graphql`
    mutation organizationLocation_addCustomerPreferredResourceMutation($input: AddCustomerPreferredResourceInput!) {
      addCustomerPreferredResource(input: $input) {
        customer {
          id
          preferredResources {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerPreferredResource] = useMutation<organizationLocation_removeCustomerPreferredResourceMutation>(graphql`
    mutation organizationLocation_removeCustomerPreferredResourceMutation($input: RemoveCustomerPreferredResourceInput!) {
      removeCustomerPreferredResource(input: $input) {
        customer {
          id
          preferredResources {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitDeleteLocation] = useMutation<organizationLocation_deleteLocationMutation>(graphql`
    mutation organizationLocation_deleteLocationMutation($input: DeleteLocationInput!) {
      deleteLocation(input: $input) {
        location {
          id
        }
      }
    }
  `);

  const [commitUpdateLocationOpeningHours] = useMutation<organizationLocation_updateLocationOpeningHoursMutation>(graphql`
    mutation organizationLocation_updateLocationOpeningHoursMutation($input: UpdateLocationOpeningHoursInput!) @raw_response_type {
      updateLocationOpeningHours(input: $input) {
        location {
          id
          name
          about
          timezone
          contactEmail
          contactPhone
          physicalAddress {
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
          }
          locationTags {
            uniqueId
            name
            color
          }
          openingHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const [, startTransition] = useTransition();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validateLocationDetails = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);

  const [resourceNameSearchText, setResourceNameSearchText] = useState<string>('');
  const [resourceCustomTagIds, setResourceCustomTagIds] = useState<string[]>([]);
  const [resourceZoneIds, setResourceZoneIds] = useState<string[]>([]);
  const [selectedResourceId, setSelectedResourceId] = useState<null | string>(null);
  const [seledctedResources, setSeledctedResources] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [resourceMoreActionsAnchorEl, setResourceMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const resourceMoreActionsMenuOpen = Boolean(resourceMoreActionsAnchorEl);
  const [preferredResources, setPreferredResources] = useState(rootData.me?.preferredResources.map(({ uniqueId }) => uniqueId) ?? []);

  const resourceMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteResource],
  ];

  const resources = useMemo(() => rootDataResources.resources.edges.map(({ node }) => node), [rootDataResources.resources]);
  const resourcesConnectionIds = useMemo(() => [rootDataResources.resources.__id], [rootDataResources.resources]);
  const resourceDetails = useMemo(() => resources.find((item) => item.id === selectedResourceId), [selectedResourceId, resources]);

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

  const handleRefetchResources = useCallback(
    (resourceNameSearchText: string, resourceCustomTagIds: string[], resourceZoneIds: string[]) => {
      startTransition(() => {
        refetchResources(
          {
            resourceNameSearchText,
            resourceCustomTagIds,
            resourceZoneIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchResources],
  );

  const handleLocationDetailUpdateClick = ({
    name,
    about,
    timezone,
    contactEmail,
    contactPhone,
    addressLine1,
    addressLine2,
    suburb,
    city,
    province,
    zipcode,
    country,
    locationTagIds,
  }: LocationDetails) => {
    const location = rootData.location;
    if (!location) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating location '${location.name}'...`} />, infoNotificationOptions);

    commitUpdateLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: location.id,
          name,
          about,
          timezone,
          contactEmail,
          contactPhone,
          physicalAddress: {
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
          },
          locationTagIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update location '${location?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location ${name} details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update location '${location?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateLocation: {
          location: {
            id: location.id,
            name,
            about,
            timezone,
            contactEmail,
            contactPhone,
            physicalAddress: {
              addressLine1,
              addressLine2,
              suburb,
              city,
              province,
              zipcode,
              country,
            },
            locationTags: location.locationTags,
            openingHours: location.openingHours,
          },
        },
      },
    });
  };

  const handleCloseClick = () => {
    router.push(getOrganizationLocationsBaseLink(integratedPlatrform, organizationId));
  };

  const handleResourceNameSearchTextChange = (str: string) => {
    setResourceNameSearchText(str);

    handleRefetchResources(str, resourceZoneIds, resourceCustomTagIds);
  };

  const handleResourceCustomTagChanged = (id?: string) => {
    const newIds = id ? [id] : [];
    setResourceCustomTagIds(newIds);

    handleRefetchResources(resourceNameSearchText, newIds, resourceZoneIds);
  };

  const handleResourceZoneTypeChanged = (id?: string) => {
    const newIds = id ? [id] : [];
    setResourceZoneIds(newIds);

    handleRefetchResources(resourceNameSearchText, resourceCustomTagIds, newIds);
  };

  const handleSelectedResourcesChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedResources(newRowSelectionModel);
  };

  const handleResourceMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setResourceMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditResource:
        if (resourceDetails) {
          router.push(getOrganizationLocationResourceBaseLink(integratedPlatrform, organizationId, locationId, resourceDetails.id));
          return;
        }

        break;

      case MoreActionsMenuOptionType.DeactivateResource:
        handleDeactivateResourceClick();
        break;

      case MoreActionsMenuOptionType.ActivateResource:
        handleActivateResourceClick();
        break;

      case MoreActionsMenuOptionType.DeleteResource:
        handleRemoveResourceClick();
        break;
    }
  };

  const handleDeactivateResourcesClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating resources...'} />, infoNotificationOptions);

    commitDeactivateResources({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedResources.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate resources. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Resources deactivated.'} />,
        });
        setSeledctedResources(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate resources. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateResourcesClick = () => {
    const toastId = themedToast(<NotificationContent content={'Activating resources...'} />, infoNotificationOptions);

    commitActivateResources({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedResources.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate resources. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Resources activated.'} />,
        });
        setSeledctedResources(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate resources. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveResourcesClick = () => {
    const toastId = themedToast(<NotificationContent content={'Removing resources...'} />, infoNotificationOptions);

    commitDeleteResources({
      variables: {
        connectionIds: resourcesConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedResources.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove resources. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Resources removed.'} />,
        });
        setSeledctedResources(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove resources. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleDeactivateResourceClick = () => {
    if (!resourceDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Deactivating resource...'} />, infoNotificationOptions);

    commitDeactivateResources({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [resourceDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate resource. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Resource deactivated.'} />,
        });
        setSeledctedResources(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate resource. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateResourceClick = () => {
    if (!resourceDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Activating resource...'} />, infoNotificationOptions);

    commitActivateResources({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [resourceDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate resource. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Resource activated.'} />,
        });
        setSeledctedResources(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate resource. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveResourceClick = () => {
    if (!resourceDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Removing resource...'} />, infoNotificationOptions);

    commitDeleteResources({
      variables: {
        connectionIds: resourcesConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [resourceDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove resource. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Resource removed.'} />,
        });
        setSeledctedResources(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove resource. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSetAsPreferredResourceClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const resourceDetails = resources.find((item) => item.id === id);
    if (!resourceDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting resource '${resourceDetails.name}' as your preferred resource...`} />, infoNotificationOptions);

    commitAddCustomerPreferredResource({
      variables: {
        input: {
          clientMutationId: nanoid(),
          resourceId: resourceDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set resource '${resourceDetails.name}' as your preferred resource. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Resource '${resourceDetails.name}' has been set as the preferred resource.`} />,
        });

        setPreferredResources(preferredResources.concat([resourceDetails.id]));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set resource '${resourceDetails.name}' as your preferred resource. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredResourceClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const resourceDetails = resources.find((item) => item.id === id);
    if (!resourceDetails) {
      return;
    }
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing resource '${resourceDetails.name}' as your preferred resource...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredResource({
      variables: {
        input: {
          clientMutationId: nanoid(),
          resourceId: resourceDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the resource '${resourceDetails.name}' as your preferred resource. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Resource '${resourceDetails.name}' has been removed as your preferred resource.`} />,
        });

        setPreferredResources(preferredResources.filter((item) => item !== resourceDetails.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the resource '${resourceDetails.name}' as your preferred resource. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleViewLocationBookingsClick = () => {
    router.push(getOrganizationBookingsBaseLink(integratedPlatrform, organizationId, { locationId }));
  };

  const handleRemoveLocationClicked = () => {
    const location = rootData.location;
    if (!location) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing location '${location.name}'...`} />, infoNotificationOptions);

    commitDeleteLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: location.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the location '${location.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${location.name}' removed.`} />,
        });

        router.push(getOrganizationLocationsBaseLink(integratedPlatrform, organizationId));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the location '${location.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleLocationOpeningHoursUpdateClick = (weekOpeningHours: WeekOpeningHoursDetails) => {
    const location = rootData.location;
    if (!location) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating location '${location.name}' opening hours...`} />, infoNotificationOptions);

    commitUpdateLocationOpeningHours({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: location.id,
          weekOpeningHours,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update location '${location?.name}' opening hours . Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location ${location.name} opening hours updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update location '${location?.name}' opening hours. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateLocationOpeningHours: {
          location: {
            id: location.id,
            name: location.name,
            about: location.about,
            timezone: location.timezone,
            contactEmail: location.contactEmail,
            contactPhone: location.contactPhone,
            physicalAddress: location.physicalAddress,
            locationTags: location.locationTags,
            openingHours: {
              weekOpeningHours,
            },
          },
        },
      },
    });
  };

  if (!rootData.location) {
    return <></>;
  }

  const resourceRows: ResourceRowType[] = resources.map((resource) => ({
    id: resource.id,
    resource,
    resourceType: { id: resource.resourceType.uniqueId, name: resource.resourceType.name, color: resource.resourceType.color },
    customTags: resource.customTags.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color })),
    zones: resource.zones.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color })),
    productTags: resource.productTags.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color })),
    status: !resource.inactive,
    preferred: preferredResources.includes(resource.id),
    capacity: resource.capacity,
  }));

  const resourceColumns: GridColDef<(typeof resourceRows)[number]>[] = [
    {
      field: 'resource',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <Resource resource={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'resourceType',
      headerName: 'Type',
      editable: false,
      renderCell: (params) => <ResourceType resourceType={params.value} />,
      display: 'flex',
      minWidth: 50,
    },
    {
      field: 'capacity',
      headerName: 'Capacity',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'customTags',
      headerName: 'Tags',
      editable: false,
      renderCell: (params) => <CustomTags customTags={params.value} hideIcon />,
      display: 'flex',
      minWidth: 250,
    },
    {
      field: 'zones',
      headerName: 'Zones',
      editable: false,
      renderCell: (params) => <Zones zones={params.value} hideIcon />,
      display: 'flex',
      minWidth: 250,
    },
    {
      field: 'productTags',
      headerName: 'Product Tags',
      editable: false,
      renderCell: (params) => <ProductTags productTags={params.value} hideIcon />,
      display: 'flex',
      minWidth: 250,
    },
    {
      field: 'status',
      headerName: 'Status',
      editable: false,
      renderCell: (params) => (
        <StackRow>
          {params.value && (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="Active" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: emerald }} />
            </StackRow>
          )}
          {!params.value && (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="Inactive" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: flame }} />
            </StackRow>
          )}
        </StackRow>
      ),
      display: 'flex',
    },
    {
      field: 'preferred',
      headerName: 'Preferred?',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        if (params.value) {
          return (
            <IconButton onClick={() => handleRemoveAsPreferredResourceClicked(id)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredResourceClicked(id)}>
            <NotPreferredIcon />
          </IconButton>
        );
      },
      display: 'flex',
    },
    {
      field: 'More Actions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedResourceId(params.id as string);
              setResourceMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  const location = rootData.location;

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationLocationLeftSideNavigationMenuContent organizationId={organizationId} locationId={locationId} hideIcons />
        <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Location Information">
            <Form
              onSubmit={handleLocationDetailUpdateClick}
              initialValues={{
                name: location.name,
                about: location.about,
                timezone: location.timezone ?? '',
                locationTagIds: location.locationTags.map((item) => item.uniqueId),
                contactEmail: location.contactEmail,
                contactPhone: location.contactPhone,
                addressLine1: location.physicalAddress.addressLine1,
                addressLine2: location.physicalAddress.addressLine2,
                suburb: location.physicalAddress.suburb,
                city: location.physicalAddress.city,
                province: location.physicalAddress.province,
                zipcode: location.physicalAddress.zipcode,
                country: location.physicalAddress.country,
              }}
              validate={validateLocationDetails}
              render={({ handleSubmit }) => (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['setup'] = divElement;
                    }}
                  >
                    <GridContainer sx={{ justifyContent: 'space-between' }}>
                      <Grid>
                        <SectionIconTypography label="Location Setup" />
                        <BodyIconTypography label="Edit your location name and details" />
                      </Grid>

                      <Grid>
                        <Button variant="contained" sx={defaultButtonStyle} startIcon={<BookingIcon />} onClick={handleViewLocationBookingsClick}>
                          View Location Bookings
                        </Button>
                      </Grid>
                    </GridContainer>
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

                    {rootData.organization?.type.type === 'MARKETPLACE' && (
                      <FormFieldLabel label="Location Tags">
                        <MultipleChoicesLocationTags rootDataRelay={rootData} name="locationTagIds" required={requiredFields.locationTagIds} organizationId={organizationId} />
                      </FormFieldLabel>
                    )}

                    <SectionIconTypography label="Contact Details" />
                    <BodyIconTypography label="Edit your location contact details" />
                    <Divider />

                    <FormFieldLabel label="Email">
                      <TextField name="contactEmail" required={requiredFields.contactEmail} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Phone Number">
                      <TextField name="contactPhone" required={requiredFields.contactPhone} />
                    </FormFieldLabel>

                    <SectionIconTypography label="Address" />
                    <BodyIconTypography label="Edit your location address" />
                    <Divider />

                    <FormFieldLabel label="Address Line 1">
                      <TextField name="addressLine1" required={requiredFields.addressLine1} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Address Line 2">
                      <TextField name="addressLine2" required={requiredFields.addressLine2} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Suburb">
                      <TextField name="suburb" required={requiredFields.suburb} />
                    </FormFieldLabel>

                    <FormFieldLabel label="City">
                      <TextField name="city" required={requiredFields.city} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Province">
                      <TextField name="province" required={requiredFields.province} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Zipcode">
                      <TextField name="zipcode" required={requiredFields.zipcode} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Country">
                      <SingleChoiceCountry name="country" required={requiredFields.country} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              )}
            />

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['opening-hours'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Opening Hours" />
                  <BodyIconTypography label="Manage your location opening hours" />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <WeekOpeningHours
              rootDataRelay={rootData}
              defaultValue={{
                monday: location.openingHours.weekOpeningHours.monday,
                tuesday: location.openingHours.weekOpeningHours.tuesday,
                wednesday: location.openingHours.weekOpeningHours.wednesday,
                thursday: location.openingHours.weekOpeningHours.thursday,
                friday: location.openingHours.weekOpeningHours.friday,
                saturday: location.openingHours.weekOpeningHours.saturday,
                sunday: location.openingHours.weekOpeningHours.sunday,
              }}
              onWeekOpeningHoursDetailUpdateClick={handleLocationOpeningHoursUpdateClick}
            />

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['manage-resources'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Manage Resources" />
                  <BodyIconTypography label="Manage your location resources details" />
                </Grid>

                <Grid>
                  <AddResourceButton onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} connectionIds={resourcesConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <ZoneSelector rootDataRelay={rootData} onChange={handleResourceZoneTypeChanged} />
              <CustomTagSelector rootDataRelay={rootData} onChange={handleResourceCustomTagChanged} />
              <PushToRight />
              <Search size="small" placeholder="Search for resources" defaultValue={resourceNameSearchText} onChange={handleResourceNameSearchTextChange} />
            </GridContainer>

            {seledctedResources.ids.size > 0 && (
              <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                <Box
                  sx={{
                    backgroundColor: 'white',
                    padding: defaultGridActionPadding,
                    border: 1,
                    borderColor: (theme) => theme.palette.divider,
                    borderRadius: 2,
                    flexGrow: 1,
                  }}
                >
                  <StackRow sx={{ alignItems: 'center' }}>
                    <SmallIconTypography label={`${seledctedResources.ids.size} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="secondary" onClick={handleDeactivateResourcesClick} sx={defaultButtonStyle}>
                      Deactivate Resource
                    </Button>
                    <Button size="medium" variant="contained" color="secondary" onClick={handleActivateResourcesClick} sx={defaultButtonStyle}>
                      Activate Resource
                    </Button>
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveResourcesClick} sx={{ textTransform: 'none' }}>
                      Remove Resource
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedResources}
                onRowSelectionModelChange={handleSelectedResourcesChanged}
                rows={resourceRows}
                columns={resourceColumns}
                hideFooterPagination={resourceRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: resourceRows.length,
                    paginationModel: {
                      pageSize: 10,
                    },
                  },
                }}
                pageSizeOptions={[10]}
                ignoreDiacritics
                disableRowSelectionOnClick
                getRowHeight={() => 'auto'}
                rowSpacingType="margin"
                getRowSpacing={() => ({ top: 3, bottom: 3 })}
                sx={defaultGridStyle}
                localeText={{ noRowsLabel: 'No resource found' }}
              />
            </StackRow>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['manage-location'] = divElement;
              }}
            >
              <SectionIconTypography label="Manage" />
              <BodyIconTypography label="Remove your location" />
              <Divider />
            </StackColumn>

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveLocationClicked} sx={{ textTransform: 'none' }}>
                Remove Location
              </Button>
            </StackRow>
          </AppBarWithStackColumn>
        </Box>
      </Box>

      <MoreActionsMenu
        anchorEl={resourceMoreActionsAnchorEl}
        open={resourceMoreActionsMenuOpen}
        onMenuItemClick={handleResourceMoreActionsMenuItemClick}
        options={resourceMoreActionsOption}
      />
    </>
  );
};

export default memo(OrganizationLocation);
