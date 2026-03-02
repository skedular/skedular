import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { Address, PhysicalAddress } from '@/components/address';
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
import { FloorPlanCard } from '@/components/floorPlan';
import { NewFloorplanButton } from '@/components/floorPlan/addFloorPlan';
import { SingleChoinceTimezone } from '@/components/forms';
import { BookingIcon, DeleteIcon, EllipseMenuIcon, NotPreferredIcon, PreferredIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationLocationResourceBaseLink, getOrganizationLocationsBaseLink } from '@/components/links';
import { MultipleChoicesLocationSpaceTypes, SingleChoiceLocationType } from '@/components/location';
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
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { defaultGridRowSelectionModelValue } from '@/libs/mui';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, flame, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { joinErrors, keyboardTextFieldDebounceTimeout, stringCollectionToString, stringToMultiLines } from '@/libs/utils';
import type { organizationLocation_activateResourcesMutation } from '@/queries/__generated__/organizationLocation_activateResourcesMutation.graphql';
import type { organizationLocation_addCustomerPreferredResourceMutation } from '@/queries/__generated__/organizationLocation_addCustomerPreferredResourceMutation.graphql';
import type { organizationLocation_addLocationPhysicalAddressMutation } from '@/queries/__generated__/organizationLocation_addLocationPhysicalAddressMutation.graphql';
import type { organizationLocation_deactivateResourcesMutation } from '@/queries/__generated__/organizationLocation_deactivateResourcesMutation.graphql';
import type { organizationLocation_deleteLocationMutation } from '@/queries/__generated__/organizationLocation_deleteLocationMutation.graphql';
import type { organizationLocation_deleteResourcesMutation } from '@/queries/__generated__/organizationLocation_deleteResourcesMutation.graphql';
import type { organizationLocation_floorPlans_query$key } from '@/queries/__generated__/organizationLocation_floorPlans_query.graphql';
import type { organizationLocation_floorPlans_refetchableFragment } from '@/queries/__generated__/organizationLocation_floorPlans_refetchableFragment.graphql';
import type { organizationLocation_query$key } from '@/queries/__generated__/organizationLocation_query.graphql';
import type { organizationLocation_removeCustomerPreferredResourceMutation } from '@/queries/__generated__/organizationLocation_removeCustomerPreferredResourceMutation.graphql';
import type { organizationLocation_resources_query$key } from '@/queries/__generated__/organizationLocation_resources_query.graphql';
import type { organizationLocation_resources_refetchableFragment } from '@/queries/__generated__/organizationLocation_resources_refetchableFragment.graphql';
import type { LocationType, organizationLocation_updateLocationMutation } from '@/queries/__generated__/organizationLocation_updateLocationMutation.graphql';
import type { organizationLocation_updateLocationOpeningHoursMutation } from '@/queries/__generated__/organizationLocation_updateLocationOpeningHoursMutation.graphql';
import type { organizationLocation_updateLocationPhysicalAddressMutation } from '@/queries/__generated__/organizationLocation_updateLocationPhysicalAddressMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, object, string } from 'yup';
import OrganizationLocationLeftSideNavigationMenuContent from './organization-location-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationLocation_query$key;
  rootDataResourcesRelay: organizationLocation_resources_query$key;
  rootDataFloorPlansRelay: organizationLocation_floorPlans_query$key;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  locationId: string;
};

type LocationDetails = {
  name: string;
  about: string | null;
  timezone: string;
  type: string;
  locationTagIds: string[];
  contactPeople: string | null;
  contactEmails: string | null;
  contactPhones: string | null;
  areaRangeFromInSqm: string | null;
  areaRangeToInSqm: string | null;
  peopleCapacityFrom: string | null;
  peopleCapacityTo: string | null;
  website: string | null;
  relatedImageLinks: string | null;
  relatedVideoLinks: string | null;
  otherLinks: string | null;
  locationSpaceTypeIds: string[];
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
  type: string().required('Type is required'),
  locationTagIds: array().nullable(),
  contactPeople: string().nullable(),
  contactEmails: string().nullable(),
  contactPhones: string().nullable(),
  areaRangeFromInSqm: string().nullable(),
  areaRangeToInSqm: string().nullable(),
  peopleCapacityFrom: string().nullable(),
  peopleCapacityTo: string().nullable(),
  website: string().nullable(),
  relatedImageLinks: string().nullable(),
  relatedVideoLinks: string().nullable(),
  otherLinks: string().nullable(),
  locationSpaceTypeIds: array().nullable(),
});

type PhysicalAddress = {
  addressLine1: string;
  addressLine2: string | null;
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string;
  countryCode: string;
};

const physicalAddressSchema = object({
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string(),
  city: string(),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  countryCode: string().required('Country is required'),
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

const OrganizationLocation = ({ rootDataRelay, rootDataResourcesRelay, rootDataFloorPlansRelay, onReloadRequired, organizationUniqueAlphanumericName, locationId }: Props) => {
  const rootData = useFragment<organizationLocation_query$key>(
    graphql`
      fragment organizationLocation_query on Query {
        emailsToShowLatestCapabilities
        me {
          id
          emails
          preferredResources {
            id
          }
        }
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
          type {
            type
          }
        }
        location(id: $locationId) {
          id
          name
          about
          timezone
          type {
            type
            name
          }
          extraMetadata {
            contactDetails {
              contactPeople
              contactEmails
              contactPhones
            }
            areaRange {
              fromInSqm
              toInSqm
            }
            peopleCapacity {
              from
              to
            }
            website
            relatedImageLinks
            relatedVideoLinks
            otherLinks
          }
          featureImages {
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
          physicalAddress {
            id
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
          }
          locationTags {
            id
            name
            color
          }
          locationSpaceTypes {
            id
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
        ...multipleChoicesLocationTags_query
        ...weekOpeningHours_query
        ...customTagSelector_allCustomTags_query
        ...zoneSelector_allZones_query
        ...singleChoiceLocationType_query
        ...multipleChoicesLocationSpaceTypes_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataResources, refetchResources] = useRefetchableFragment<organizationLocation_resources_refetchableFragment, organizationLocation_resources_query$key>(
    graphql`
      fragment organizationLocation_resources_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationLocation_resources_refetchableFragment") {
        location(id: $locationId) {
          resources(
            first: $count
            after: $cursor
            where: { nameContains: $resourceNameSearchText, customTagIds: $resourceCustomTagIds, zoneIds: $resourceZoneIds }
            orderBy: $resourcesSortingValues
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
                  id
                  name
                  color
                }
                zones {
                  id
                  name
                  color
                }
                productTags {
                  id
                  name
                  color
                }
                resourceType {
                  id
                  name
                  color
                }
              }
            }
          }
        }
      }
    `,
    rootDataResourcesRelay,
  );

  const [rootDataFloorPlans] = useRefetchableFragment<organizationLocation_floorPlans_refetchableFragment, organizationLocation_floorPlans_query$key>(
    graphql`
      fragment organizationLocation_floorPlans_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationLocation_floorPlans_refetchableFragment") {
        floorPlans(first: $count, after: $cursor, where: { locationId: $locationId }, orderBy: $floorPlansSortingValues) @connection(key: "organizationLocation_floorPlans") {
          __id
          totalCount
          edges {
            node {
              id
              name
              ...floorPlanCard_FloorPlanDetails
            }
          }
        }
      }
    `,
    rootDataFloorPlansRelay,
  );

  const [commitUpdateLocation] = useMutation<organizationLocation_updateLocationMutation>(graphql`
    mutation organizationLocation_updateLocationMutation($input: UpdateLocationInput!) @raw_response_type {
      updateLocation(input: $input) {
        location {
          id
          name
          about
          timezone
          type {
            type
            name
          }
          extraMetadata {
            contactDetails {
              contactPeople
              contactEmails
              contactPhones
            }
            areaRange {
              fromInSqm
              toInSqm
            }
            peopleCapacity {
              from
              to
            }
            website
            relatedImageLinks
            relatedVideoLinks
            otherLinks
          }
          featureImages {
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
          locationTags {
            id
            name
            color
          }
          locationSpaceTypes {
            id
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
            id
            name
            color
          }
          zones {
            id
            name
            color
          }
          productTags {
            id
            name
            color
          }
          resourceType {
            id
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
            id
            name
            color
          }
          zones {
            id
            name
            color
          }
          productTags {
            id
            name
            color
          }
          resourceType {
            id
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
            id
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
            id
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
          extraMetadata {
            contactDetails {
              contactPeople
              contactEmails
              contactPhones
            }
            areaRange {
              fromInSqm
              toInSqm
            }
            peopleCapacity {
              from
              to
            }
            website
            relatedImageLinks
            relatedVideoLinks
            otherLinks
          }
          physicalAddress {
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
          }
          locationTags {
            id
            name
            color
          }
          locationSpaceTypes {
            id
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

  const [commitAddLocationPhysicalAddress] = useMutation<organizationLocation_addLocationPhysicalAddressMutation>(graphql`
    mutation organizationLocation_addLocationPhysicalAddressMutation($input: AddLocationPhysicalAddressInput!) @raw_response_type {
      addLocationPhysicalAddress(input: $input) {
        location {
          id
          physicalAddress {
            id
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
          }
        }
      }
    }
  `);

  const [commitUpdateLocationPhysicalAddress] = useMutation<organizationLocation_updateLocationPhysicalAddressMutation>(graphql`
    mutation organizationLocation_updateLocationPhysicalAddressMutation($input: UpdateLocationPhysicalAddressInput!) @raw_response_type {
      updateLocationPhysicalAddress(input: $input) {
        location {
          id
          physicalAddress {
            id
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
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

  const [locationName, setLocationName] = useState<string>(rootData.location?.name ?? '');
  const debounceSetLocationName = useDebounceCallback(setLocationName, keyboardTextFieldDebounceTimeout);
  const [locationAbout, setLocationAbout] = useState(rootData.location?.about);
  const debounceSetLocationAbout = useDebounceCallback(setLocationAbout, keyboardTextFieldDebounceTimeout);
  const [locationTimezone, setLocationTimezone] = useState<string>(rootData.location?.timezone ?? '');
  const debounceSetLocationTimezone = useDebounceCallback(setLocationTimezone, keyboardTextFieldDebounceTimeout);
  const [locationType, setLocationType] = useState<string>(rootData.location?.type.type ?? '');
  const debounceSetLocationType = useDebounceCallback(setLocationType, keyboardTextFieldDebounceTimeout);
  const [locationTagIds, setLocationTagIds] = useState<string[]>(rootData.location ? rootData.location.locationTags.map((item) => item.id) : []);
  const debounceSetLocationTagIds = useDebounceCallback(setLocationTagIds, keyboardTextFieldDebounceTimeout);
  const [locationSpaceTypeIds, setLocationSpaceTypeIds] = useState<string[]>(rootData.location?.locationSpaceTypes.map((item) => item.id) ?? []);
  const debounceSetLocationSpaceTypeIds = useDebounceCallback(setLocationSpaceTypeIds, keyboardTextFieldDebounceTimeout);

  const [locationContactPerson, setLocationContactPerson] = useState<string | null | undefined>(
    stringCollectionToString(rootData.location?.extraMetadata?.contactDetails?.contactPeople),
  );
  const debounceSetLocationContactPerson = useDebounceCallback(setLocationContactPerson, keyboardTextFieldDebounceTimeout);
  const [locationContactEmail, setLocationContactEmail] = useState<string | null | undefined>(
    stringCollectionToString(rootData.location?.extraMetadata?.contactDetails?.contactEmails),
  );
  const debounceSetLocationContactEmail = useDebounceCallback(setLocationContactEmail, keyboardTextFieldDebounceTimeout);
  const [locationContactPhone, setLocationContactPhone] = useState<string | null | undefined>(
    stringCollectionToString(rootData.location?.extraMetadata?.contactDetails?.contactPhones),
  );
  const debounceSetLocationContactPhone = useDebounceCallback(setLocationContactPhone, keyboardTextFieldDebounceTimeout);

  const [locationAreaRangeFromSqm, setLocationAreaRangeFromSqm] = useState<string | null | undefined>(rootData.location?.extraMetadata?.areaRange?.fromInSqm);
  const debounceSetLocationAreaRangeFromSqm = useDebounceCallback(setLocationAreaRangeFromSqm, keyboardTextFieldDebounceTimeout);
  const [locationAreaRangeToSqm, setLocationAreaRangeToSqm] = useState<string | null | undefined>(rootData.location?.extraMetadata?.areaRange?.toInSqm);
  const debounceSetLocationAreaRangeToSqm = useDebounceCallback(setLocationAreaRangeToSqm, keyboardTextFieldDebounceTimeout);

  const [locationPeopleCapacityFrom, setLocationPeopleCapacityFrom] = useState<string | null | undefined>(rootData.location?.extraMetadata?.peopleCapacity?.from);
  const debounceSetLocationPeopleCapacityFrom = useDebounceCallback(setLocationPeopleCapacityFrom, keyboardTextFieldDebounceTimeout);
  const [locationPeopleCapacityTo, setLocationPeopleCapacityTo] = useState<string | null | undefined>(rootData.location?.extraMetadata?.peopleCapacity?.to);
  const debounceSetLocationPeopleCapacityTo = useDebounceCallback(setLocationPeopleCapacityTo, keyboardTextFieldDebounceTimeout);

  const [locationWebsite, setLocationWebsite] = useState<string | null | undefined>(rootData.location?.extraMetadata?.website);
  const debounceSetLocationWebsite = useDebounceCallback(setLocationWebsite, keyboardTextFieldDebounceTimeout);
  const [locationRelatedImageLinks, setLocationRelatedImageLinks] = useState<string | null | undefined>(
    stringCollectionToString(rootData.location?.extraMetadata?.relatedImageLinks),
  );
  const debounceSetLocationRelatedImageLinks = useDebounceCallback(setLocationRelatedImageLinks, keyboardTextFieldDebounceTimeout);
  const [locationRelatedVideoLinks, setLocationRelatedVideoLinks] = useState<string | null | undefined>(
    stringCollectionToString(rootData.location?.extraMetadata?.relatedVideoLinks),
  );
  const debounceSetLocationRelatedVideoLinks = useDebounceCallback(setLocationRelatedVideoLinks, keyboardTextFieldDebounceTimeout);
  const [locationOtherLinks, setLocationOtherLinks] = useState<string | null | undefined>(stringCollectionToString(rootData.location?.extraMetadata?.otherLinks));
  const debounceSetLocationOtherLinks = useDebounceCallback(setLocationOtherLinks, keyboardTextFieldDebounceTimeout);

  const [featureImages, setFeatureImages] = useState<FileUploadResponse[]>(
    rootData.location
      ? rootData.location.featureImages
          .filter((item) => !!item.original)
          .map((item) => ({
            id: '',
            original: {
              url: item.original!.url,
              height: item.original!.height,
              width: item.original!.width,
            },
            thumbnail: item.thumbnail
              ? {
                  url: item.thumbnail.url,
                  height: item.thumbnail.height,
                  width: item.thumbnail.width,
                }
              : null,
          }))
      : [],
  );
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(featureImages[0] ?? null);

  const [resourceNameSearchText, setResourceNameSearchText] = useState<string>('');
  const [resourceCustomTagIds, setResourceCustomTagIds] = useState<string[]>([]);
  const [resourceZoneIds, setResourceZoneIds] = useState<string[]>([]);
  const [selectedResourceId, setSelectedResourceId] = useState<null | string>(null);
  const [seledctedResources, setSeledctedResources] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [resourceMoreActionsAnchorEl, setResourceMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const resourceMoreActionsMenuOpen = Boolean(resourceMoreActionsAnchorEl);
  const [preferredResources, setPreferredResources] = useState(rootData.me?.preferredResources.map(({ id }) => id) ?? []);

  const resourceMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteResource],
  ];

  const resources = useMemo(() => (rootDataResources.location ? rootDataResources.location.resources.edges.map(({ node }) => node) : []), [rootDataResources.location]);
  const resourcesConnectionIds = useMemo(() => (rootDataResources.location ? [rootDataResources.location.resources.__id] : []), [rootDataResources.location]);
  const resourceDetails = useMemo(() => resources.find((item) => item.id === selectedResourceId), [selectedResourceId, resources]);

  const floorPlans = useMemo(() => rootDataFloorPlans.floorPlans.edges.map((edge) => edge.node), [rootDataFloorPlans.floorPlans]);
  const floorPlansConnectionIds = useMemo(() => [rootDataFloorPlans.floorPlans.__id], [rootDataFloorPlans.floorPlans]);

  const validatePhysicalAddress = makeValidate(physicalAddressSchema);
  const requiredPhysicalAddressFields = makeRequired(physicalAddressSchema);

  const [physicalAddressOsmType, setPhysicalAddressOsmType] = useState(rootData.location?.physicalAddress?.osmType);
  const [physicalAddressOsmId, setPhysicalAddressOsmId] = useState(rootData.location?.physicalAddress?.osmId);
  const [physicalAddressPlaceId, setPhysicalAddressPlaceId] = useState(rootData.location?.physicalAddress?.placeId);
  const [physicalAddressLongitude, setPhysicalAddressLongitude] = useState(rootData.location?.physicalAddress?.longitude);
  const [physicalAddressLatitude, setPhysicalAddressLatitude] = useState(rootData.location?.physicalAddress?.latitude);
  const [physicalAddressFormattedAddress, setPhysicalAddressFormattedAddress] = useState(rootData.location?.physicalAddress?.formattedAddress);
  const [physicalAddressAddressLine1, setPhysicalAddressAddressLine1] = useState<string>(rootData.location?.physicalAddress?.addressLine1 ?? '');
  const debounceSetPhysicalAddressAddressLine1 = useDebounceCallback(setPhysicalAddressAddressLine1, keyboardTextFieldDebounceTimeout);
  const [physicalAddressAddressLine2, setPhysicalAddressAddressLine2] = useState(rootData.location?.physicalAddress?.addressLine2);
  const debounceSetPhysicalAddressAddressLine2 = useDebounceCallback(setPhysicalAddressAddressLine2, keyboardTextFieldDebounceTimeout);
  const [physicalAddressSuburb, setPhysicalAddressSuburb] = useState(rootData.location?.physicalAddress?.suburb);
  const debounceSetPhysicalAddressSuburb = useDebounceCallback(setPhysicalAddressSuburb, keyboardTextFieldDebounceTimeout);
  const [physicalAddressCity, setPhysicalAddressCity] = useState(rootData.location?.physicalAddress?.city);
  const debounceSetPhysicalAddressCity = useDebounceCallback(setPhysicalAddressCity, keyboardTextFieldDebounceTimeout);
  const [physicalAddressProvince, setPhysicalAddressProvince] = useState(rootData.location?.physicalAddress?.province);
  const debounceSetPhysicalAddressProvince = useDebounceCallback(setPhysicalAddressProvince, keyboardTextFieldDebounceTimeout);
  const [physicalAddressZipcode, setPhysicalAddressZipcode] = useState<string>(rootData.location?.physicalAddress?.zipcode ?? '');
  const debounceSetPhysicalAddressZipcode = useDebounceCallback(setPhysicalAddressZipcode, keyboardTextFieldDebounceTimeout);
  const [physicalAddressCountry, setPhysicalAddressCountry] = useState<string>(rootData.location?.physicalAddress?.country ?? '');
  const [physicalAddressCountryCode, setPhysicalAddressCountryCode] = useState<string>(rootData.location?.physicalAddress?.countryCode ?? '');
  const debounceSetPhysicalAddressCountryCode = useDebounceCallback(setPhysicalAddressCountryCode, keyboardTextFieldDebounceTimeout);

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
    [startTransition, refetchResources],
  );

  const handleLocationDetailUpdateClick = ({
    name,
    about,
    timezone,
    type,
    contactPeople,
    contactEmails,
    contactPhones,
    areaRangeFromInSqm,
    areaRangeToInSqm,
    peopleCapacityFrom,
    peopleCapacityTo,
    website,
    relatedImageLinks,
    relatedVideoLinks,
    otherLinks,
    locationTagIds,
    locationSpaceTypeIds,
  }: LocationDetails) => {
    const location = rootData.location;
    if (!location) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating location '${location.name}'...`} />, infoNotificationOptions);
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    commitUpdateLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: location.id,
          name,
          about,
          timezone,
          type: type as LocationType,
          extraMetadata: {
            contactDetails: {
              contactPeople: stringToMultiLines(contactPeople),
              contactEmails: stringToMultiLines(contactEmails),
              contactPhones: stringToMultiLines(contactPhones),
            },
            areaRange:
              areaRangeFromInSqm && areaRangeToInSqm
                ? {
                    fromInSqm: areaRangeFromInSqm,
                    toInSqm: areaRangeToInSqm,
                  }
                : null,
            peopleCapacity:
              peopleCapacityFrom && peopleCapacityTo
                ? {
                    from: peopleCapacityFrom,
                    to: peopleCapacityTo,
                  }
                : null,
            website: website ?? null,
            relatedImageLinks: stringToMultiLines(relatedImageLinks),
            relatedVideoLinks: stringToMultiLines(relatedVideoLinks),
            otherLinks: stringToMultiLines(otherLinks),
          },
          featureImages: finalFeatureImages,
          locationTagIds: locationTagIds.concat(locationSpaceTypeIds),
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
            type: {
              type: type as LocationType,
              name: '',
            },
            extraMetadata: {
              contactDetails: {
                contactPeople: stringToMultiLines(contactPeople),
                contactEmails: stringToMultiLines(contactEmails),
                contactPhones: stringToMultiLines(contactPhones),
              },
              areaRange:
                areaRangeFromInSqm && areaRangeToInSqm
                  ? {
                      fromInSqm: areaRangeFromInSqm,
                      toInSqm: areaRangeToInSqm,
                    }
                  : null,
              peopleCapacity:
                peopleCapacityFrom && peopleCapacityTo
                  ? {
                      from: peopleCapacityFrom,
                      to: peopleCapacityTo,
                    }
                  : null,
              website: website ?? null,
              relatedImageLinks: stringToMultiLines(relatedImageLinks),
              relatedVideoLinks: stringToMultiLines(relatedVideoLinks),
              otherLinks: stringToMultiLines(otherLinks),
            },
            featureImages: finalFeatureImages,
            locationTags: location.locationTags,
            openingHours: location.openingHours,
            locationSpaceTypes: [],
          },
        },
      },
    });
  };

  const handlePhysicalAddressSelect = (address: Address) => {
    setPhysicalAddressOsmType(address.osmType);
    setPhysicalAddressOsmId(address.osmId);
    setPhysicalAddressPlaceId(address.placeId);
    setPhysicalAddressLongitude(address.longitude);
    setPhysicalAddressLatitude(address.latitude);
    setPhysicalAddressFormattedAddress(address.formattedAddress);
    setPhysicalAddressAddressLine1(address.addressLine1 ?? '');
    setPhysicalAddressAddressLine2(address.addressLine2 ?? '');
    setPhysicalAddressSuburb(address.suburb ?? '');
    setPhysicalAddressCity(address.city ?? '');
    setPhysicalAddressProvince(address.province ?? '');
    setPhysicalAddressZipcode(address.zipcode ?? '');
    setPhysicalAddressCountry(address.country ?? '');
    setPhysicalAddressCountryCode(address.countryCode ?? '');
  };

  const handlePhysicalAddressUpdateClick = ({ addressLine1, addressLine2, suburb, city, province, zipcode, countryCode }: PhysicalAddress) => {
    const location = rootData.location;
    if (!location) {
      return;
    }

    const countryData = getCountryData(countryCode as TCountryCode);
    let country = physicalAddressCountry;
    if (countryData) {
      country = countryData.name;
    }

    const physicalAddress = location.physicalAddress;

    if (physicalAddress) {
      const toastId = themedToast(<NotificationContent content={`Updating location '${location.name}' physical address...`} />, infoNotificationOptions);

      commitUpdateLocationPhysicalAddress({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: physicalAddress.id,
            osmType: physicalAddressOsmType,
            osmId: physicalAddressOsmId,
            placeId: physicalAddressPlaceId,
            longitude: physicalAddressLongitude,
            latitude: physicalAddressLatitude,
            formattedAddress: physicalAddressFormattedAddress,
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
            countryCode,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to update location '${location?.name}' physical address. Error: ${joinErrors(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Location '${location?.name}' physical address updated.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update location '${location?.name}' physical address. Error: ${error.message}.`} />,
          });
        },
        optimisticResponse: {
          updateLocationPhysicalAddress: {
            location: {
              id: location.id,
              physicalAddress: {
                id: physicalAddress.id,
                osmType: physicalAddressOsmType,
                osmId: physicalAddressOsmId,
                placeId: physicalAddressPlaceId,
                longitude: physicalAddressLongitude,
                latitude: physicalAddressLatitude,
                formattedAddress: physicalAddressFormattedAddress,
                addressLine1,
                addressLine2,
                suburb,
                city,
                province,
                zipcode,
                country,
                countryCode,
              },
            },
          },
        },
      });
    } else {
      const id = uuid();
      const toastId = themedToast(<NotificationContent content={`Adding location '${location.name}' physical address...`} />, infoNotificationOptions);

      commitAddLocationPhysicalAddress({
        variables: {
          input: {
            clientMutationId: uuid(),
            locationId: location.id,
            id,
            osmType: physicalAddressOsmType,
            osmId: physicalAddressOsmId,
            placeId: physicalAddressPlaceId,
            longitude: physicalAddressLongitude,
            latitude: physicalAddressLatitude,
            formattedAddress: physicalAddressFormattedAddress,
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
            countryCode,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to add location '${location?.name}' physical address. Error: ${joinErrors(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Location '${location?.name}' physical address added.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add location '${location?.name}' physical address. Error: ${error.message}.`} />,
          });
        },
        optimisticResponse: {
          addLocationPhysicalAddress: {
            location: {
              id: location.id,
              physicalAddress: {
                id,
                osmType: physicalAddressOsmType,
                osmId: physicalAddressOsmId,
                placeId: physicalAddressPlaceId,
                longitude: physicalAddressLongitude,
                latitude: physicalAddressLatitude,
                formattedAddress: physicalAddressFormattedAddress,
                addressLine1,
                addressLine2,
                suburb,
                city,
                province,
                zipcode,
                country,
                countryCode,
              },
            },
          },
        },
      });
    }
  };

  const handleCloseClick = () => {
    router.push(getOrganizationLocationsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName));
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
          router.push(getOrganizationLocationResourceBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, locationId, resourceDetails.id));
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
          clientMutationId: uuid(),
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
          clientMutationId: uuid(),
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
          clientMutationId: uuid(),
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
          clientMutationId: uuid(),
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
          clientMutationId: uuid(),
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
          clientMutationId: uuid(),
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
    const resourceDetails = resources.find((item) => item.id === id);
    if (!resourceDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting resource '${resourceDetails.name}' as your preferred resource...`} />, infoNotificationOptions);

    commitAddCustomerPreferredResource({
      variables: {
        input: {
          clientMutationId: uuid(),
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
    const resourceDetails = resources.find((item) => item.id === id);
    if (!resourceDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing resource '${resourceDetails.name}' as your preferred resource...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredResource({
      variables: {
        input: {
          clientMutationId: uuid(),
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
    router.push(getOrganizationBookingsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, { locationId }));
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
          clientMutationId: uuid(),
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

        router.push(getOrganizationLocationsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName));
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
          clientMutationId: uuid(),
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
            extraMetadata: location.extraMetadata,
            physicalAddress: location.physicalAddress,
            locationTags: location.locationTags,
            locationSpaceTypes: location.locationSpaceTypes,
            openingHours: {
              weekOpeningHours,
            },
          },
        },
      },
    });
  };

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setFeatureImages((prev) => [response, ...prev]);
    setPrimaryFeatureImage((prevPrimary) => prevPrimary ?? response);
  };

  const handleRemoveFeatureImage = (image: FileUploadResponse) => {
    setFeatureImages((prev) => {
      const next = prev.filter((item) => item.original?.url !== image.original?.url);

      if (primaryFeatureImage?.original?.url === image.original?.url) {
        setPrimaryFeatureImage(next[0] ?? null);
      }

      return next;
    });
  };

  const handleSetPrimaryFeatureImage = (image: FileUploadResponse) => {
    setPrimaryFeatureImage(image);
    setFeatureImages((prev) => [image, ...prev.filter((item) => item.original?.url !== image.original?.url)]);
  };

  if (!rootData.location) {
    return null;
  }

  const resourceRows: ResourceRowType[] = resources.map((resource) => ({
    id: resource.id,
    resource,
    resourceType: {
      id: resource.resourceType.id,
      name: resource.resourceType.name,
      color: resource.resourceType.color,
    },
    customTags: resource.customTags.map((item) => ({ id: item.id, name: item.name, color: item.color })),
    zones: resource.zones.map((item) => ({ id: item.id, name: item.name, color: item.color })),
    productTags: resource.productTags.map((item) => ({ id: item.id, name: item.name, color: item.color })),
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
        <OrganizationLocationLeftSideNavigationMenuContent organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} locationId={locationId} hideIcons />
        <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Location Information">
            <Form
              onSubmit={handleLocationDetailUpdateClick}
              initialValues={{
                name: locationName,
                about: locationAbout,
                timezone: locationTimezone,
                type: locationType,
                locationTagIds,
                locationSpaceTypeIds,
                contactPeople: locationContactPerson,
                contactEmails: locationContactEmail,
                contactPhones: locationContactPhone,
                areaRangeFromInSqm: locationAreaRangeFromSqm,
                areaRangeToInSqm: locationAreaRangeToSqm,
                peopleCapacityFrom: locationPeopleCapacityFrom,
                peopleCapacityTo: locationPeopleCapacityTo,
                website: locationWebsite,
                relatedImageLinks: locationRelatedImageLinks,
                relatedVideoLinks: locationRelatedVideoLinks,
                otherLinks: locationOtherLinks,
              }}
              validate={validateLocationDetails}
              render={({ handleSubmit, values }) => {
                debounceSetLocationName(values!.name);
                debounceSetLocationAbout(values!.about);
                debounceSetLocationTimezone(values!.timezone);
                debounceSetLocationType(values!.type);
                debounceSetLocationTagIds(values!.locationTagIds);
                debounceSetLocationSpaceTypeIds(values!.locationSpaceTypeIds);

                debounceSetLocationContactPerson(values!.contactPeople);
                debounceSetLocationContactEmail(values!.contactEmails);
                debounceSetLocationContactPhone(values!.contactPhones);
                debounceSetLocationAreaRangeFromSqm(values!.areaRangeFromInSqm);
                debounceSetLocationAreaRangeToSqm(values!.areaRangeToInSqm);
                debounceSetLocationPeopleCapacityFrom(values!.peopleCapacityFrom);
                debounceSetLocationPeopleCapacityTo(values!.peopleCapacityTo);

                debounceSetLocationWebsite(values!.website);
                debounceSetLocationRelatedImageLinks(values!.relatedImageLinks);
                debounceSetLocationRelatedVideoLinks(values!.relatedVideoLinks);
                debounceSetLocationOtherLinks(values!.otherLinks);
                return (
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
                      <FormFieldLabel label="Feature Images">
                        <StackColumn>
                          <Box
                            sx={{
                              display: 'grid',
                              gridTemplateColumns: { xs: 'repeat(auto-fill, minmax(140px, 1fr))', sm: 'repeat(auto-fill, minmax(180px, 1fr))' },
                              gap: 2,
                            }}
                          >
                            {featureImages.map((image, index) => (
                              <Box
                                key={index}
                                sx={{
                                  position: 'relative',
                                  borderRadius: 2,
                                  overflow: 'hidden',
                                  border: 1,
                                  borderColor: 'divider',
                                  backgroundColor: paletteMode === 'dark' ? 'grey.900' : 'grey.50',
                                }}
                              >
                                {/* eslint-disable-next-line @next/next/no-img-element */}
                                <img src={image.original?.url ?? image.thumbnail?.url ?? ''} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                                <StackRow sx={{ position: 'absolute', top: 8, right: 8 }}>
                                  <IconButton size="small" aria-label="Remove feature image" onClick={() => handleRemoveFeatureImage(image)}>
                                    <DeleteIcon fontSize="small" />
                                  </IconButton>
                                </StackRow>
                                <StackRow sx={{ position: 'absolute', left: 8, bottom: 8 }}>
                                  {primaryFeatureImage?.original?.url === image.original?.url ? (
                                    <Chip size="small" color="success" label="Cover image" />
                                  ) : (
                                    <Button variant="contained" size="small" onClick={() => handleSetPrimaryFeatureImage(image)} sx={{ textTransform: 'none' }}>
                                      Make cover
                                    </Button>
                                  )}
                                </StackRow>
                              </Box>
                            ))}
                          </Box>

                          <ImageFileUploaderWithCropper defaultAspectRatio={1} onUploadCompleted={handleFeatureImageUploadCompleted} />
                        </StackColumn>
                      </FormFieldLabel>

                      <FormFieldLabel label="Name">
                        <TextField name="name" required={requiredFields.name} />
                      </FormFieldLabel>

                      <FormFieldLabel label="About">
                        <TextField name="about" required={requiredFields.about} multiline rows={3} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Timezone">
                        <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Space Type">
                        <MultipleChoicesLocationSpaceTypes rootDataRelay={rootData} name="locationSpaceTypeIds" required={requiredFields.locationSpaceTypeIds} />
                      </FormFieldLabel>

                      {rootData.me.emails.some((item) => !!rootData.emailsToShowLatestCapabilities.find((email) => email.toLocaleLowerCase() === item.toLocaleLowerCase())) && (
                        <>
                          <FormFieldLabel label="Type">
                            <SingleChoiceLocationType rootDataRelay={rootData} name="type" required={requiredFields.type} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Area From(sqm)" required={requiredFields.areaRangeFromInSqm}>
                            <TextField name="areaRangeFromInSqm" required={requiredFields.areaRangeFromInSqm} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Area To(sqm)" required={requiredFields.areaRangeToInSqm}>
                            <TextField name="areaRangeToInSqm" required={requiredFields.areaRangeToInSqm} />
                          </FormFieldLabel>

                          <FormFieldLabel label="People Capacity From" required={requiredFields.peopleCapacityFrom}>
                            <TextField name="peopleCapacityFrom" required={requiredFields.peopleCapacityFrom} />
                          </FormFieldLabel>

                          <FormFieldLabel label="People Capacity To" required={requiredFields.peopleCapacityTo}>
                            <TextField name="peopleCapacityTo" required={requiredFields.peopleCapacityTo} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Website" required={requiredFields.website}>
                            <TextField name="website" required={requiredFields.website} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Image Links" required={requiredFields.relatedImageLinks}>
                            <TextField name="relatedImageLinks" required={requiredFields.relatedImageLinks} multiline rows={5} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Video Links" required={requiredFields.relatedVideoLinks}>
                            <TextField name="relatedVideoLinks" required={requiredFields.relatedVideoLinks} multiline rows={5} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Other Links" required={requiredFields.otherLinks}>
                            <TextField name="otherLinks" required={requiredFields.otherLinks} multiline rows={5} />
                          </FormFieldLabel>
                        </>
                      )}

                      {rootData.organization?.type.type === 'MARKETPLACE' && (
                        <FormFieldLabel label="Location Tags">
                          <MultipleChoicesLocationTags
                            rootDataRelay={rootData}
                            name="locationTagIds"
                            required={requiredFields.locationTagIds}
                            organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                          />
                        </FormFieldLabel>
                      )}

                      <SectionIconTypography label="Contact Details" />
                      <BodyIconTypography label="Edit your location contact details" />
                      <Divider />

                      <FormFieldLabel label="Contact People" required={requiredFields.contactPeople}>
                        <TextField name="contactPeople" required={requiredFields.contactPeople} multiline rows={2} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Emails">
                        <TextField name="contactEmails" required={requiredFields.contactEmails} multiline rows={2} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Phone Numbers">
                        <TextField name="contactPhones" required={requiredFields.contactPhones} multiline rows={2} />
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
                );
              }}
            />

            <Form
              onSubmit={handlePhysicalAddressUpdateClick}
              initialValues={{
                addressLine1: physicalAddressAddressLine1,
                addressLine2: physicalAddressAddressLine2,
                suburb: physicalAddressSuburb,
                city: physicalAddressCity,
                province: physicalAddressProvince,
                zipcode: physicalAddressZipcode,
                countryCode: physicalAddressCountryCode,
              }}
              validate={validatePhysicalAddress}
              render={({ handleSubmit, values, form }) => {
                debounceSetPhysicalAddressAddressLine1(values!.addressLine1);
                debounceSetPhysicalAddressAddressLine2(values!.addressLine2);
                debounceSetPhysicalAddressSuburb(values!.suburb);
                debounceSetPhysicalAddressCity(values!.city);
                debounceSetPhysicalAddressProvince(values!.province);
                debounceSetPhysicalAddressZipcode(values!.zipcode);
                debounceSetPhysicalAddressCountryCode(values!.countryCode);

                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <StackColumn
                      sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                      ref={(divElement) => {
                        sectionRefs.current['physical-address-setup'] = divElement;
                      }}
                    >
                      <SectionIconTypography label="Physical Address Setup" />
                      <BodyIconTypography label="Edit your organization physical address" />
                      <Divider />
                    </StackColumn>

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                      <PhysicalAddress
                        addressLine1Name="addressLine1"
                        addressLine1Required={requiredPhysicalAddressFields.addressLine1}
                        addressLine2Name="addressLine2"
                        addressLine2Required={requiredPhysicalAddressFields.addressLine2}
                        suburbName="suburb"
                        suburbRequired={requiredPhysicalAddressFields.suburb}
                        cityName="city"
                        cityRequired={requiredPhysicalAddressFields.city}
                        provinceName="province"
                        provinceRequired={requiredPhysicalAddressFields.province}
                        zipcodeName="zipcode"
                        zipcodeRequired={requiredPhysicalAddressFields.zipcode}
                        countryName="countryCode"
                        countryRequired={requiredPhysicalAddressFields.countryCode}
                        onSelect={(address) => {
                          handlePhysicalAddressSelect(address);
                          form.batch(() => {
                            form.change('addressLine1', address.addressLine1 ?? '');
                            form.change('addressLine2', address.addressLine2 ?? '');
                            form.change('suburb', address.suburb ?? '');
                            form.change('city', address.city ?? '');
                            form.change('province', address.province ?? '');
                            form.change('zipcode', address.zipcode ?? '');
                            form.change('countryCode', address.countryCode ?? '');
                          });
                        }}
                      />
                    </StackColumn>

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                      <StackRow>
                        <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                          Update
                        </Button>
                      </StackRow>
                    </StackColumn>
                  </FormStackColumn>
                );
              }}
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
                sectionRefs.current['floor-plans'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Manage Floor Plans" />
                  <BodyIconTypography label="Manage your location floor plans details" />
                </Grid>

                <Grid>
                  <NewFloorplanButton organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} locationId={locationId} />
                </Grid>
              </GridContainer>
              <Divider />

              <GridContainer>
                {floorPlans.map((floorPlan) => (
                  <Grid key={floorPlan.id}>
                    <FloorPlanCard
                      floorPlanDetailsRelay={floorPlan}
                      connectionIds={floorPlansConnectionIds}
                      organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                      locationId={locationId}
                    />
                  </Grid>
                ))}
              </GridContainer>
            </StackColumn>

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
                  <AddResourceButton
                    onReloadRequired={onReloadRequired}
                    organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                    locationId={locationId}
                    connectionIds={resourcesConnectionIds}
                  />
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
