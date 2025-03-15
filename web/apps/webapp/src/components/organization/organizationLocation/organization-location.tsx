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
import { Desk } from '@/components/desk';
import { AddDeskButton } from '@/components/desk/addDesk';
import { BulkAddDeskButton } from '@/components/desk/bulkAddDesk';
import { SingleChoinceTimezone } from '@/components/forms';
import { BookingIcon, DeleteIcon, EllipseMenuIcon, NotPreferredIcon, PreferredIcon } from '@/components/icons';
import {
  getOrganizationBookingsBaseLink,
  getOrganizationLocationDeskBaseLink,
  getOrganizationLocationResourceBaseLink,
  getOrganizationLocationRoomBaseLink,
  getOrganizationLocationsBaseLink,
} from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { CustomTagSelector } from '@/components/organization/customTagSelector/';
import { ZoneSelector } from '@/components/organization/zoneSelector';
import { Resource } from '@/components/resource';
import { AddResourceButton } from '@/components/resource/addResource';
import { ResourceType } from '@/components/resourceType';
import { Room } from '@/components/room';
import AddRoomButton from '@/components/room/addRoom/add-room-button';
import { Search } from '@/components/search';
import { WeekOpeningHours, WeekOpeningHoursDetails } from '@/components/weekOpeningHours';
import { Zones } from '@/components/zone';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, flame, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { organizationLocation_activateDesksMutation } from '@/queries/__generated__/organizationLocation_activateDesksMutation.graphql';
import type { organizationLocation_activateResourcesMutation } from '@/queries/__generated__/organizationLocation_activateResourcesMutation.graphql';
import type { organizationLocation_activateRoomsMutation } from '@/queries/__generated__/organizationLocation_activateRoomsMutation.graphql';
import type { organizationLocation_addCustomerPreferredDeskMutation } from '@/queries/__generated__/organizationLocation_addCustomerPreferredDeskMutation.graphql';
import type { organizationLocation_addCustomerPreferredResourceMutation } from '@/queries/__generated__/organizationLocation_addCustomerPreferredResourceMutation.graphql';
import type { organizationLocation_addCustomerPreferredRoomMutation } from '@/queries/__generated__/organizationLocation_addCustomerPreferredRoomMutation.graphql';
import type { organizationLocation_deactivateDesksMutation } from '@/queries/__generated__/organizationLocation_deactivateDesksMutation.graphql';
import type { organizationLocation_deactivateResourcesMutation } from '@/queries/__generated__/organizationLocation_deactivateResourcesMutation.graphql';
import type { organizationLocation_deactivateRoomsMutation } from '@/queries/__generated__/organizationLocation_deactivateRoomsMutation.graphql';
import type { organizationLocation_deleteDesksMutation } from '@/queries/__generated__/organizationLocation_deleteDesksMutation.graphql';
import type { organizationLocation_deleteLocationMutation } from '@/queries/__generated__/organizationLocation_deleteLocationMutation.graphql';
import type { organizationLocation_deleteResourcesMutation } from '@/queries/__generated__/organizationLocation_deleteResourcesMutation.graphql';
import type { organizationLocation_deleteRoomsMutation } from '@/queries/__generated__/organizationLocation_deleteRoomsMutation.graphql';
import type { organizationLocation_desks_query$key } from '@/queries/__generated__/organizationLocation_desks_query.graphql';
import type { organizationLocation_desks_refetchableFragment } from '@/queries/__generated__/organizationLocation_desks_refetchableFragment.graphql';
import type { organizationLocation_query$key } from '@/queries/__generated__/organizationLocation_query.graphql';
import type { organizationLocation_removeCustomerPreferredDeskMutation } from '@/queries/__generated__/organizationLocation_removeCustomerPreferredDeskMutation.graphql';
import type { organizationLocation_removeCustomerPreferredResourceMutation } from '@/queries/__generated__/organizationLocation_removeCustomerPreferredResourceMutation.graphql';
import type { organizationLocation_removeCustomerPreferredRoomMutation } from '@/queries/__generated__/organizationLocation_removeCustomerPreferredRoomMutation.graphql';
import type { organizationLocation_resources_query$key } from '@/queries/__generated__/organizationLocation_resources_query.graphql';
import type { organizationLocation_resources_refetchableFragment } from '@/queries/__generated__/organizationLocation_resources_refetchableFragment.graphql';
import type { organizationLocation_rooms_query$key } from '@/queries/__generated__/organizationLocation_rooms_query.graphql';
import type { organizationLocation_rooms_refetchableFragment } from '@/queries/__generated__/organizationLocation_rooms_refetchableFragment.graphql';
import type { organizationLocation_updateLocationMutation } from '@/queries/__generated__/organizationLocation_updateLocationMutation.graphql';
import type { organizationLocation_updateLocationOpeningHoursMutation } from '@/queries/__generated__/organizationLocation_updateLocationOpeningHoursMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
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
import { object, string } from 'yup';
import OrganizationLocationLeftSideNavigationMenuContent from './organization-location-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationLocation_query$key;
  rootDataResourcesRelay: organizationLocation_resources_query$key;
  rootDataDesksRelay: organizationLocation_desks_query$key;
  rootDataRoomsRelay: organizationLocation_rooms_query$key;
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
};

type LocationDetails = {
  name: string;
  about: string | null;
  timezone: string;
  physicalAddress: string;
};

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

type DeskDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type RoomDetails = {
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

type ResourceRowType = {
  id: string;
  resource: ResourceDetails;
  resourceType: ResourceTypeDetails;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
  status: boolean;
  preferred: boolean;
};

type DeskRowType = {
  id: string;
  desk: DeskDetails;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
  status: boolean;
  preferred: boolean;
};

type RoomRowType = {
  id: string;
  room: RoomDetails;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
  status: boolean;
  preferred: boolean;
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
  physicalAddress: string().nullable(),
});

const OrganizationLocation = ({ rootDataRelay, rootDataResourcesRelay, rootDataDesksRelay, rootDataRoomsRelay, onReloadRequired, organizationId, locationId }: Props) => {
  const rootData = useFragment<organizationLocation_query$key>(
    graphql`
      fragment organizationLocation_query on Query {
        me {
          id
          preferredResources {
            uniqueId
          }
          preferredDesks {
            uniqueId
          }
          preferredRooms {
            uniqueId
          }
        }
        location(id: $locationId) {
          id
          name
          about
          timezone
          physicalAddress {
            formattedAddress
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

  const [rootDataDesks, refetchDesks] = useRefetchableFragment<organizationLocation_desks_refetchableFragment, organizationLocation_desks_query$key>(
    graphql`
      fragment organizationLocation_desks_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationLocation_desks_refetchableFragment") {
        desks(first: $count, after: $cursor, where: { locationId: $locationId, nameContains: $deskNameSearchText, customTagIds: $deskCustomTagIds, zoneIds: $deskZoneIds })
          @connection(key: "organizationLocation_desks") {
          __id
          totalCount
          edges {
            node {
              id
              name
              deactivated
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
            }
          }
        }
      }
    `,
    rootDataDesksRelay,
  );

  const [rootDataRooms, refetchRooms] = useRefetchableFragment<organizationLocation_rooms_refetchableFragment, organizationLocation_rooms_query$key>(
    graphql`
      fragment organizationLocation_rooms_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationLocation_rooms_refetchableFragment") {
        rooms(first: $count, after: $cursor, where: { locationId: $locationId, nameContains: $roomNameSearchText, customTagIds: $roomCustomTagIds, zoneIds: $roomZoneIds })
          @connection(key: "organizationLocation_rooms") {
          __id
          totalCount
          edges {
            node {
              id
              name
              deactivated
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
            }
          }
        }
      }
    `,
    rootDataRoomsRelay,
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

  const [commitDeleteDesks] = useMutation<organizationLocation_deleteDesksMutation>(graphql`
    mutation organizationLocation_deleteDesksMutation($connectionIds: [ID!]!, $input: DeleteDesksInput!) {
      deleteDesks(input: $input) {
        desks {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitActivateDesks] = useMutation<organizationLocation_activateDesksMutation>(graphql`
    mutation organizationLocation_activateDesksMutation($input: ActivateDesksInput!) {
      activateDesks(input: $input) {
        desks {
          id
          name
          deactivated
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
        }
      }
    }
  `);

  const [commitDeactivateDesks] = useMutation<organizationLocation_deactivateDesksMutation>(graphql`
    mutation organizationLocation_deactivateDesksMutation($input: DeactivateDesksInput!) {
      deactivateDesks(input: $input) {
        desks {
          id
          name
          deactivated
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
        }
      }
    }
  `);

  const [commitAddCustomerPreferredDesk] = useMutation<organizationLocation_addCustomerPreferredDeskMutation>(graphql`
    mutation organizationLocation_addCustomerPreferredDeskMutation($input: AddCustomerPreferredDeskInput!) {
      addCustomerPreferredDesk(input: $input) {
        customer {
          id
          preferredDesks {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerPreferredDesk] = useMutation<organizationLocation_removeCustomerPreferredDeskMutation>(graphql`
    mutation organizationLocation_removeCustomerPreferredDeskMutation($input: RemoveCustomerPreferredDeskInput!) {
      removeCustomerPreferredDesk(input: $input) {
        customer {
          id
          preferredDesks {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitDeleteRooms] = useMutation<organizationLocation_deleteRoomsMutation>(graphql`
    mutation organizationLocation_deleteRoomsMutation($connectionIds: [ID!]!, $input: DeleteRoomsInput!) {
      deleteRooms(input: $input) {
        rooms {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitActivateRooms] = useMutation<organizationLocation_activateRoomsMutation>(graphql`
    mutation organizationLocation_activateRoomsMutation($input: ActivateRoomsInput!) {
      activateRooms(input: $input) {
        rooms {
          id
          name
          deactivated
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
        }
      }
    }
  `);

  const [commitDeactivateRooms] = useMutation<organizationLocation_deactivateRoomsMutation>(graphql`
    mutation organizationLocation_deactivateRoomsMutation($input: DeactivateRoomsInput!) {
      deactivateRooms(input: $input) {
        rooms {
          id
          name
          deactivated
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
        }
      }
    }
  `);

  const [commitAddCustomerPreferredRoom] = useMutation<organizationLocation_addCustomerPreferredRoomMutation>(graphql`
    mutation organizationLocation_addCustomerPreferredRoomMutation($input: AddCustomerPreferredRoomInput!) {
      addCustomerPreferredRoom(input: $input) {
        customer {
          id
          preferredRooms {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerPreferredRoom] = useMutation<organizationLocation_removeCustomerPreferredRoomMutation>(graphql`
    mutation organizationLocation_removeCustomerPreferredRoomMutation($input: RemoveCustomerPreferredRoomInput!) {
      removeCustomerPreferredRoom(input: $input) {
        customer {
          id
          preferredRooms {
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
          physicalAddress {
            formattedAddress
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

  const [, startTransition] = useTransition();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validateLocationDetails = makeValidate(locationSchema);
  const requiredLocationDetailsFields = makeRequired(locationSchema);

  const [resourceNameSearchText, setResourceNameSearchText] = useState<string>('');
  const [resourceCustomTagIds, setResourceCustomTagIds] = useState<string[]>([]);
  const [resourceZoneIds, setResourceZoneIds] = useState<string[]>([]);
  const [selectedResourceId, setSelectedResourceId] = useState<null | string>(null);
  const [seledctedResources, setSeledctedResources] = useState<GridRowSelectionModel>([]);
  const [resourceMoreActionsAnchorEl, setResourceMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const resourceMoreActionsMenuOpen = Boolean(resourceMoreActionsAnchorEl);
  const [preferredResources, setPreferredResources] = useState(rootData.me?.preferredResources.map(({ uniqueId }) => uniqueId) ?? []);

  const resourceMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteResource],
  ];

  const resourcesConnectionIds = useMemo(() => (rootDataResources.resources ? [rootDataResources.resources.__id] : []), [rootDataResources.resources]);
  const resources = useMemo(() => (rootDataResources.resources ? rootDataResources.resources.edges.map(({ node }) => node) : []), [rootDataResources.resources]);
  const resourceDetails = useMemo(() => resources.find((item) => item.id === selectedResourceId), [selectedResourceId, resources]);

  const [deskNameSearchText, setDeskNameSearchText] = useState<string>('');
  const [deskCustomTagIds, setDeskCustomTagIds] = useState<string[]>([]);
  const [deskZoneIds, setDeskZoneIds] = useState<string[]>([]);
  const [selectedDeskId, setSelectedDeskId] = useState<null | string>(null);
  const [seledctedDesks, setSeledctedDesks] = useState<GridRowSelectionModel>([]);
  const [deskMoreActionsAnchorEl, setDeskMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const deskMoreActionsMenuOpen = Boolean(deskMoreActionsAnchorEl);
  const [preferredDesks, setPreferredDesks] = useState(rootData.me?.preferredDesks.map(({ uniqueId }) => uniqueId) ?? []);

  const deskMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditDesk],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateDesk],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateDesk],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteDesk],
  ];

  const desksConnectionIds = useMemo(() => (rootDataDesks.desks ? [rootDataDesks.desks.__id] : []), [rootDataDesks.desks]);
  const desks = useMemo(() => (rootDataDesks.desks ? rootDataDesks.desks.edges.map(({ node }) => node) : []), [rootDataDesks.desks]);
  const deskDetails = useMemo(() => desks.find((item) => item.id === selectedDeskId), [selectedDeskId, desks]);

  const [roomNameSearchText, setRoomNameSearchText] = useState<string>('');
  const [roomCustomTagIds, setRoomCustomTagIds] = useState<string[]>([]);
  const [roomZoneIds, setRoomZoneIds] = useState<string[]>([]);
  const [selectedRoomId, setSelectedRoomId] = useState<null | string>(null);
  const [seledctedRooms, setSeledctedRooms] = useState<GridRowSelectionModel>([]);
  const [roomMoreActionsAnchorEl, setRoomMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const roomMoreActionsMenuOpen = Boolean(roomMoreActionsAnchorEl);
  const [preferredRooms, setPreferredRooms] = useState(rootData.me?.preferredRooms.map(({ uniqueId }) => uniqueId) ?? []);

  const roomMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditRoom],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateRoom],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateRoom],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteRoom],
  ];

  const roomsConnectionIds = useMemo(() => (rootDataRooms.rooms ? [rootDataRooms.rooms.__id] : []), [rootDataRooms.rooms]);
  const rooms = useMemo(() => (rootDataRooms.rooms ? rootDataRooms.rooms.edges.map(({ node }) => node) : []), [rootDataRooms.rooms]);
  const roomDetails = useMemo(() => rooms.find((item) => item.id === selectedRoomId), [selectedRoomId, rooms]);

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

  const handleRefetchDesks = useCallback(
    (deskNameSearchText: string, deskCustomTagIds: string[], deskZoneIds: string[]) => {
      startTransition(() => {
        refetchDesks(
          {
            deskNameSearchText,
            deskCustomTagIds,
            deskZoneIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchDesks],
  );

  const handleRefetchRooms = useCallback(
    (roomNameSearchText: string, roomCustomTagIds: string[], roomZoneIds: string[]) => {
      startTransition(() => {
        refetchRooms(
          {
            roomNameSearchText,
            roomCustomTagIds,
            roomZoneIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchRooms],
  );

  const handleLocationDetailUpdateClick = ({ name, about, timezone, physicalAddress }: LocationDetails) => {
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
            physicalAddress: {
              formattedAddress: physicalAddress,
            },
            openingHours: location.openingHours,
          },
        },
      },
    });
  };

  const handleCloseClick = () => {
    router.push(getOrganizationLocationsBaseLink(organizationId));
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
          router.push(getOrganizationLocationResourceBaseLink(organizationId, locationId, resourceDetails.id));
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
          ids: seledctedResources.map((id) => id as string),
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
        setSeledctedResources([]);
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
          ids: seledctedResources.map((id) => id as string),
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
        setSeledctedResources([]);
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
          ids: seledctedResources.map((id) => id as string),
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
        setSeledctedResources([]);
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
        setSeledctedResources([]);
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
        setSeledctedResources([]);
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
        setSeledctedResources([]);
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

  const handleDeskNameSearchTextChange = (str: string) => {
    setDeskNameSearchText(str);

    handleRefetchDesks(str, deskZoneIds, deskCustomTagIds);
  };

  const handleDeskCustomTagChanged = (id?: string) => {
    const newIds = id ? [id] : [];
    setDeskCustomTagIds(newIds);

    handleRefetchDesks(deskNameSearchText, newIds, deskZoneIds);
  };

  const handleDeskZoneTypeChanged = (id?: string) => {
    const newIds = id ? [id] : [];
    setDeskZoneIds(newIds);

    handleRefetchDesks(deskNameSearchText, deskCustomTagIds, newIds);
  };

  const handleSelectedDesksChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedDesks(newRowSelectionModel);
  };

  const handleDeskMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setDeskMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditDesk:
        if (deskDetails) {
          router.push(getOrganizationLocationDeskBaseLink(organizationId, locationId, deskDetails.id));
          return;
        }

        break;

      case MoreActionsMenuOptionType.DeactivateDesk:
        handleDeactivateDeskClick();
        break;

      case MoreActionsMenuOptionType.ActivateDesk:
        handleActivateDeskClick();
        break;

      case MoreActionsMenuOptionType.DeleteDesk:
        handleRemoveDeskClick();
        break;
    }
  };

  const handleDeactivateDesksClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating desks...'} />, infoNotificationOptions);

    commitDeactivateDesks({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedDesks.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate desks. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desks deactivated.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate desks. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateDesksClick = () => {
    const toastId = themedToast(<NotificationContent content={'Activating desks...'} />, infoNotificationOptions);

    commitActivateDesks({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedDesks.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate desks. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desks activated.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate desks. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveDesksClick = () => {
    const toastId = themedToast(<NotificationContent content={'Removing desks...'} />, infoNotificationOptions);

    commitDeleteDesks({
      variables: {
        connectionIds: desksConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedDesks.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove desks. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desks removed.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove desks. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleDeactivateDeskClick = () => {
    if (!deskDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Deactivating desk...'} />, infoNotificationOptions);

    commitDeactivateDesks({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [deskDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate desk. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desk deactivated.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate desk. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateDeskClick = () => {
    if (!deskDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Activating desk...'} />, infoNotificationOptions);

    commitActivateDesks({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [deskDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate desk. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desk activated.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate desk. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveDeskClick = () => {
    if (!deskDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Removing desk...'} />, infoNotificationOptions);

    commitDeleteDesks({
      variables: {
        connectionIds: desksConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [deskDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove desk. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desk removed.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove desk. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSetAsPreferredDeskClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const deskDetails = desks.find((item) => item.id === id);
    if (!deskDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting desk '${deskDetails.name}' as your preferred desk...`} />, infoNotificationOptions);

    commitAddCustomerPreferredDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          deskId: deskDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set desk '${deskDetails.name}' as your preferred desk. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk '${deskDetails.name}' has been set as the preferred desk.`} />,
        });

        setPreferredDesks(preferredDesks.concat([deskDetails.id]));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set desk '${deskDetails.name}' as your preferred desk. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredDeskClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const deskDetails = desks.find((item) => item.id === id);
    if (!deskDetails) {
      return;
    }
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing desk '${deskDetails.name}' as your preferred desk...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          deskId: deskDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the desk '${deskDetails.name}' as your preferred desk. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk '${deskDetails.name}' has been removed as your preferred desk.`} />,
        });

        setPreferredDesks(preferredDesks.filter((item) => item !== deskDetails.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the desk '${deskDetails.name}' as your preferred desk. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRoomNameSearchTextChange = (str: string) => {
    setRoomNameSearchText(str);

    handleRefetchRooms(str, roomZoneIds, roomCustomTagIds);
  };

  const handleRoomCustomTagChanged = (id?: string) => {
    const newIds = id ? [id] : [];
    setRoomCustomTagIds(newIds);

    handleRefetchRooms(roomNameSearchText, newIds, roomZoneIds);
  };

  const handleRoomZoneTypeChanged = (id?: string) => {
    const newIds = id ? [id] : [];
    setRoomZoneIds(newIds);

    handleRefetchRooms(roomNameSearchText, roomCustomTagIds, newIds);
  };

  const handleSelectedRoomsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedRooms(newRowSelectionModel);
  };

  const handleRoomMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setRoomMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditRoom:
        if (roomDetails) {
          router.push(getOrganizationLocationRoomBaseLink(organizationId, locationId, roomDetails.id));
          return;
        }

        break;

      case MoreActionsMenuOptionType.DeactivateRoom:
        handleDeactivateRoomClick();
        break;

      case MoreActionsMenuOptionType.ActivateRoom:
        handleActivateRoomClick();
        break;

      case MoreActionsMenuOptionType.DeleteRoom:
        handleRemoveRoomClick();
        break;
    }
  };

  const handleDeactivateRoomsClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating rooms...'} />, infoNotificationOptions);

    commitDeactivateRooms({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedRooms.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate rooms. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Rooms deactivated.'} />,
        });
        setSeledctedRooms([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate rooms. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateRoomsClick = () => {
    const toastId = themedToast(<NotificationContent content={'Activating rooms...'} />, infoNotificationOptions);

    commitActivateRooms({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedRooms.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate rooms. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Rooms activated.'} />,
        });
        setSeledctedRooms([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate rooms. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveRoomsClick = () => {
    const toastId = themedToast(<NotificationContent content={'Removing rooms...'} />, infoNotificationOptions);

    commitDeleteRooms({
      variables: {
        connectionIds: roomsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedRooms.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove rooms. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Rooms removed.'} />,
        });
        setSeledctedRooms([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove rooms. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleDeactivateRoomClick = () => {
    if (!roomDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Deactivating room...'} />, infoNotificationOptions);

    commitDeactivateRooms({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [roomDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate room. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Room deactivated.'} />,
        });
        setSeledctedRooms([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate room. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateRoomClick = () => {
    if (!roomDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Activating room...'} />, infoNotificationOptions);

    commitActivateRooms({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [roomDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate room. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Room activated.'} />,
        });
        setSeledctedRooms([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate room. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveRoomClick = () => {
    if (!roomDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Removing room...'} />, infoNotificationOptions);

    commitDeleteRooms({
      variables: {
        connectionIds: roomsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [roomDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove room. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Room removed.'} />,
        });
        setSeledctedRooms([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove room. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSetAsPreferredRoomClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const roomDetails = rooms.find((item) => item.id === id);
    if (!roomDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting room '${roomDetails.name}' as your preferred room...`} />, infoNotificationOptions);

    commitAddCustomerPreferredRoom({
      variables: {
        input: {
          clientMutationId: nanoid(),
          roomId: roomDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set room '${roomDetails.name}' as your preferred room. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Room '${roomDetails.name}' has been set as the preferred room.`} />,
        });

        setPreferredRooms(preferredRooms.concat([roomDetails.id]));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set room '${roomDetails.name}' as your preferred room. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredRoomClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const roomDetails = rooms.find((item) => item.id === id);
    if (!roomDetails) {
      return;
    }
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing room '${roomDetails.name}' as your preferred room...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredRoom({
      variables: {
        input: {
          clientMutationId: nanoid(),
          roomId: roomDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the room '${roomDetails.name}' as your preferred room. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Room '${roomDetails.name}' has been removed as your preferred room.`} />,
        });

        setPreferredRooms(preferredRooms.filter((item) => item !== roomDetails.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the room '${roomDetails.name}' as your preferred room. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleViewDeskBookingsClick = () => {
    router.push(getOrganizationBookingsBaseLink(organizationId, { locationId }));
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

        router.push(getOrganizationLocationsBaseLink(organizationId));
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
            physicalAddress: location.physicalAddress,
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
    status: !resource.inactive,
    preferred: preferredResources.includes(resource.id),
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
      field: 'moreActions',
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

  const deskRows: DeskRowType[] = desks.map((desk) => ({
    id: desk.id,
    desk,
    customTags: desk.customTags.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color })),
    zones: desk.zones.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color })),
    status: !desk.deactivated,
    preferred: preferredDesks.includes(desk.id),
  }));

  const deskColumns: GridColDef<(typeof deskRows)[number]>[] = [
    {
      field: 'desk',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <Desk desk={params.value} />,
      display: 'flex',
      minWidth: 200,
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
            <IconButton onClick={() => handleRemoveAsPreferredDeskClicked(id)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredDeskClicked(id)}>
            <NotPreferredIcon />
          </IconButton>
        );
      },
      display: 'flex',
    },
    {
      field: 'moreActions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedDeskId(params.id as string);
              setDeskMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  const roomRows: RoomRowType[] = rooms.map((room) => ({
    id: room.id,
    room,
    customTags: room.customTags.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color })),
    zones: room.zones.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color })),
    status: !room.deactivated,
    preferred: preferredRooms.includes(room.id),
  }));

  const roomColumns: GridColDef<(typeof roomRows)[number]>[] = [
    {
      field: 'room',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <Room room={params.value} />,
      display: 'flex',
      minWidth: 200,
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
            <IconButton onClick={() => handleRemoveAsPreferredRoomClicked(id)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredRoomClicked(id)}>
            <NotPreferredIcon />
          </IconButton>
        );
      },
      display: 'flex',
    },
    {
      field: 'moreActions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedRoomId(params.id as string);
              setRoomMoreActionsAnchorEl(event.currentTarget);
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
                physicalAddress: location.physicalAddress?.formattedAddress,
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
                        <Button variant="contained" sx={defaultButtonStyle} startIcon={<BookingIcon />} onClick={handleViewDeskBookingsClick}>
                          View Location Bookings
                        </Button>
                      </Grid>
                    </GridContainer>
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredLocationDetailsFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="About">
                      <TextField name="about" required={requiredLocationDetailsFields.about} multiline rows={3} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Timezone">
                      <SingleChoinceTimezone name="timezone" required={requiredLocationDetailsFields.timezone} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Physical Address">
                      <TextField name="physicalAddress" required={requiredLocationDetailsFields.physicalAddress} multiline rows={5} />
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
                  <AddResourceButton onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} connectionIds={desksConnectionIds} />
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

            {seledctedResources.length > 0 && (
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
                    <SmallIconTypography label={`${seledctedResources.length} records selected`} />
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

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['manage-desks'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Manage Desks" />
                  <BodyIconTypography label="Manage your location desks details" />
                </Grid>

                <Grid>
                  <AddDeskButton onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} connectionIds={desksConnectionIds} />
                  <BulkAddDeskButton onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} connectionIds={desksConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <ZoneSelector rootDataRelay={rootData} onChange={handleDeskZoneTypeChanged} />
              <CustomTagSelector rootDataRelay={rootData} onChange={handleDeskCustomTagChanged} />
              <PushToRight />
              <Search size="small" placeholder="Search for desks" defaultValue={deskNameSearchText} onChange={handleDeskNameSearchTextChange} />
            </GridContainer>

            {seledctedDesks.length > 0 && (
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
                    <SmallIconTypography label={`${seledctedDesks.length} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="secondary" onClick={handleDeactivateDesksClick} sx={defaultButtonStyle}>
                      Deactivate Desk
                    </Button>
                    <Button size="medium" variant="contained" color="secondary" onClick={handleActivateDesksClick} sx={defaultButtonStyle}>
                      Activate Desk
                    </Button>
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveDesksClick} sx={{ textTransform: 'none' }}>
                      Remove Desk
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedDesks}
                onRowSelectionModelChange={handleSelectedDesksChanged}
                rows={deskRows}
                columns={deskColumns}
                hideFooterPagination={deskRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: deskRows.length,
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
                localeText={{ noRowsLabel: 'No desk found' }}
              />
            </StackRow>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['manage-rooms'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Manage Rooms" />
                  <BodyIconTypography label="Manage your location rooms details" />
                </Grid>

                <Grid>
                  <AddRoomButton onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} connectionIds={roomsConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <ZoneSelector rootDataRelay={rootData} onChange={handleRoomZoneTypeChanged} />
              <CustomTagSelector rootDataRelay={rootData} onChange={handleRoomCustomTagChanged} />
              <PushToRight />
              <Search size="small" placeholder="Search for rooms" defaultValue={roomNameSearchText} onChange={handleRoomNameSearchTextChange} />
            </GridContainer>

            {seledctedRooms.length > 0 && (
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
                    <SmallIconTypography label={`${seledctedRooms.length} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="secondary" onClick={handleDeactivateRoomsClick} sx={defaultButtonStyle}>
                      Deactivate Room
                    </Button>
                    <Button size="medium" variant="contained" color="secondary" onClick={handleActivateRoomsClick} sx={defaultButtonStyle}>
                      Activate Room
                    </Button>
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveRoomsClick} sx={{ textTransform: 'none' }}>
                      Remove Room
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedRooms}
                onRowSelectionModelChange={handleSelectedRoomsChanged}
                rows={roomRows}
                columns={roomColumns}
                hideFooterPagination={roomRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: roomRows.length,
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
                localeText={{ noRowsLabel: 'No room found' }}
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
      <MoreActionsMenu anchorEl={deskMoreActionsAnchorEl} open={deskMoreActionsMenuOpen} onMenuItemClick={handleDeskMoreActionsMenuItemClick} options={deskMoreActionsOption} />
      <MoreActionsMenu anchorEl={roomMoreActionsAnchorEl} open={roomMoreActionsMenuOpen} onMenuItemClick={handleRoomMoreActionsMenuItemClick} options={roomMoreActionsOption} />
    </>
  );
};

export default memo(OrganizationLocation);
