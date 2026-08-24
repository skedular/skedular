import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { Address, PhysicalAddress } from '@/components/address';
import { SingleChoinceTimezone } from '@/components/forms';
import { DeleteIcon } from '@/components/icons';
import {
  getOrganizationLocationFloorPlansBaseLink,
  getOrganizationLocationManageLocationBaseLink,
  getOrganizationLocationManageResourcesBaseLink,
  getOrganizationLocationOpeningHoursBaseLink,
  getOrganizationLocationPhysicalAddressSetupBaseLink,
  getOrganizationLocationRestrictedInformationBaseLink,
  getOrganizationLocationsBaseLink,
  getOrganizationLocationSetupBaseLink,
} from '@/components/links';
import { ListingMetadata, listingMetadataSchemaShape } from '@/components/listingMetadata';
import { Loading } from '@/components/loading';
import { MultipleChoicesLocationSpaceTypes, SingleChoiceLocationRestrictedInformationCategory } from '@/components/location';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { MultipleChoicesAmenities } from '@/components/organization';
import OrganizationLocationFloorPlansSection from '@/components/organization/organizationLocation/organization-location-floor-plans-section';
import OrganizationLocationManageResourcesSection from '@/components/organization/organizationLocation/organization-location-manage-resources-section';
import { OrganizationLocationSection } from '@/components/organization/organizationLocation/organization-location-section-nav';
import { WeekOpeningHours, WeekOpeningHoursDetails } from '@/components/weekOpeningHours';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import type { organizationLocation_addLocationPhysicalAddressMutation } from '@/queries/__generated__/organizationLocation_addLocationPhysicalAddressMutation.graphql';
import type {
  LocationRestrictedInformationCategory,
  organizationLocation_addLocationRestrictedInformationMutation,
} from '@/queries/__generated__/organizationLocation_addLocationRestrictedInformationMutation.graphql';
import type { organizationLocation_deleteLocationMutation } from '@/queries/__generated__/organizationLocation_deleteLocationMutation.graphql';
import type { organizationLocation_deleteLocationRestrictedInformationMutation } from '@/queries/__generated__/organizationLocation_deleteLocationRestrictedInformationMutation.graphql';
import type { organizationLocation_query$key } from '@/queries/__generated__/organizationLocation_query.graphql';
import type { LocationPatchField, LocationType, organizationLocation_updateLocationMutation } from '@/queries/__generated__/organizationLocation_updateLocationMutation.graphql';
import type { organizationLocation_updateLocationOpeningHoursMutation } from '@/queries/__generated__/organizationLocation_updateLocationOpeningHoursMutation.graphql';
import type { organizationLocation_updateLocationPhysicalAddressMutation } from '@/queries/__generated__/organizationLocation_updateLocationPhysicalAddressMutation.graphql';
import type {
  LocationRestrictedInformationPatchField,
  organizationLocation_updateLocationRestrictedInformationMutation,
} from '@/queries/__generated__/organizationLocation_updateLocationRestrictedInformationMutation.graphql';
import AddPhotoAlternateRoundedIcon from '@mui/icons-material/AddPhotoAlternateRounded';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import { getRelayErrorMessage, keyboardTextFieldDebounceTimeout, PaletteModeContext, stringCollectionToString, stringToMultiLines, useIntegratedPlatform } from '@skedular/shared';
import {
  BodyIconTypography,
  defaultButtonStyle,
  defaultPadding,
  EditorActionBar,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  PageHeaderPanel,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@skedular/ui';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import Image from 'next/image';
import NextLink from 'next/link';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo, PropsWithChildren, Suspense, useContext, useEffect, useMemo, useRef, useState, type MouseEvent } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, object, string } from 'yup';

type Props = {
  rootDataRelay: organizationLocation_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  locationId: string;
};

type EditorSectionProps = { title: string; description: string; summary: string; expanded: boolean; onChange: () => void };
const EditorSection = ({ title, description, summary, expanded, onChange, children }: PropsWithChildren<EditorSectionProps>) => (
  <Accordion
    disableGutters
    elevation={0}
    expanded={expanded}
    onChange={onChange}
    sx={{
      margin: 0,
      border: 1,
      borderColor: 'divider',
      borderRadius: '16px !important',
      overflow: 'hidden',
      backgroundColor: 'background.paper',
      '&::before': { display: 'none' },
    }}
  >
    <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />} sx={{ px: 2.5, py: 0.75, minHeight: 72, '& .MuiAccordionSummary-content': { my: 1 } }}>
      <StackColumn spacing={0.35} sx={{ minWidth: 0 }}>
        <LeadIconTypography label={title} />
        <SmallIconTypography label={expanded ? description : summary} />
      </StackColumn>
    </AccordionSummary>
    <AccordionDetails sx={{ borderTop: 1, borderColor: 'divider', p: { xs: 2, sm: 2.5 } }}>
      <StackColumn spacing={2}>{children}</StackColumn>
    </AccordionDetails>
  </Accordion>
);

type LocationDetails = {
  name: string;
  title: string | null;
  subTitle: string | null;
  includedFeatures: string | null;
  timezone: string;
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
  spaceTypeIds: string[];
  amenityIds: string[];
};

type PhysicalAddressDetails = {
  osmType: string | null | undefined;
  osmId: string | null | undefined;
  placeId: string | null | undefined;
  longitude: number | null | undefined;
  latitude: number | null | undefined;
  formattedAddress: string | null | undefined;
  country: string;
  addressLine1: string;
  addressLine2: string | null;
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string;
  countryCode: string;
};

type RestrictedInformationDetails = {
  title: string;
  category: LocationRestrictedInformationCategory;
  content: string;
  active: boolean;
  sortOrder: number;
};

type RestrictedInformationItem = RestrictedInformationDetails & {
  id: string;
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  ...listingMetadataSchemaShape,
  timezone: string().required('Timezone is required'),
  contactPeople: string().nullable(),
  contactEmails: string().nullable(),
  contactPhones: string().nullable(),
  areaRangeFromInSqm: string().nullable(),
  areaRangeToInSqm: string().nullable(),
  peopleCapacityFrom: string().nullable(),
  peopleCapacityTo: string().nullable(),
  website: string().url('Website must be a valid Url').nullable(),
  relatedImageLinks: string().nullable(),
  relatedVideoLinks: string().nullable(),
  otherLinks: string().nullable(),
  spaceTypeIds: array().nullable(),
  amenityIds: array().nullable(),
});

const restrictedInformationSchema = object({
  title: string().required('Title is required'),
  category: string().required('Category is required'),
  content: string().required('Content is required'),
});

const physicalAddressSchema = object({
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string().nullable(),
  city: string().nullable(),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  countryCode: string().required('Country is required'),
});

const validSections: OrganizationLocationSection[] = [
  'setup',
  'physical-address-setup',
  'opening-hours',
  'floor-plans',
  'manage-resources',
  'restricted-information',
  'manage-location',
];

const getActiveSection = (value: string | null): OrganizationLocationSection | null => {
  if (value && validSections.includes(value as OrganizationLocationSection)) {
    return value as OrganizationLocationSection;
  }

  return null;
};

const sectionLabels: Record<OrganizationLocationSection, string> = {
  setup: 'Location profile',
  'physical-address-setup': 'Physical address',
  'opening-hours': 'Opening hours',
  'floor-plans': 'Floor plans',
  'manage-resources': 'Resources',
  'restricted-information': 'Restricted info',
  'manage-location': 'Manage location',
};
const locationAutosaveDebounceTimeout = 1000;

type LocationFormField = keyof LocationDetails;
const locationFieldGroups: ReadonlyArray<[LocationPatchField, ReadonlyArray<LocationFormField>]> = [
  ['NAME', ['name']],
  ['LISTING_METADATA', ['title', 'subTitle', 'includedFeatures']],
  ['TIMEZONE', ['timezone']],
  [
    'EXTRA_METADATA',
    [
      'contactPeople',
      'contactEmails',
      'contactPhones',
      'areaRangeFromInSqm',
      'areaRangeToInSqm',
      'peopleCapacityFrom',
      'peopleCapacityTo',
      'website',
      'relatedImageLinks',
      'relatedVideoLinks',
      'otherLinks',
    ],
  ],
  ['TAGS', ['spaceTypeIds', 'amenityIds']],
];

const getChangedLocationFields = (left: LocationDetails | null, right: LocationDetails): LocationPatchField[] => {
  if (!left) return [];
  return locationFieldGroups.filter(([, formFields]) => formFields.some((field) => JSON.stringify(left[field]) !== JSON.stringify(right[field]))).map(([patchField]) => patchField);
};

const restrictedInformationPatchFields: Record<keyof RestrictedInformationDetails, LocationRestrictedInformationPatchField> = {
  title: 'TITLE',
  category: 'CATEGORY',
  content: 'CONTENT',
  active: 'ACTIVE',
  sortOrder: 'SORT_ORDER',
};

const getChangedRestrictedInformationFields = (left: RestrictedInformationDetails, right: RestrictedInformationDetails): LocationRestrictedInformationPatchField[] =>
  (Object.keys(restrictedInformationPatchFields) as (keyof RestrictedInformationDetails)[])
    .filter((field) => JSON.stringify(left[field]) !== JSON.stringify(right[field]))
    .map((field) => restrictedInformationPatchFields[field]);

const getValidRestrictedInformationPatchFields = (
  fieldsToUpdate: LocationRestrictedInformationPatchField[],
  values: RestrictedInformationDetails,
): LocationRestrictedInformationPatchField[] =>
  fieldsToUpdate.filter((patchField) => {
    const formField = (Object.entries(restrictedInformationPatchFields) as [keyof RestrictedInformationDetails, LocationRestrictedInformationPatchField][]).find(
      ([, field]) => field === patchField,
    )?.[0];
    if (!formField) {
      return false;
    }

    try {
      restrictedInformationSchema.validateSyncAt(formField, values);
      return true;
    } catch {
      return false;
    }
  });

const OrganizationLocation = ({ rootDataRelay, onReloadRequired, organizationCustomDomain, locationId }: Props) => {
  const rootData = useFragment<organizationLocation_query$key>(
    graphql`
      fragment organizationLocation_query on Query {
        emailsToShowLatestCapabilities
        me {
          id
          emails
        }
        organization(customDomain: $organizationCustomDomain) {
          type {
            type
          }
        }
        location(id: $locationId) {
          id
          name
          listingMetadata {
            title
            subTitle
            includedFeatures
          }
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
          spaceTypes {
            id
            name
            color
          }
          amenities {
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
          restrictedInformation {
            id
            title
            category
            content
            active
            sortOrder
          }
        }
        ...weekOpeningHours_query
        ...singleChoiceLocationRestrictedInformationCategory_query
        ...multipleChoicesLocationSpaceTypes_query
        ...multipleChoicesAmenities_query
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
          listingMetadata {
            title
            subTitle
            includedFeatures
          }
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
          spaceTypes {
            id
            name
            color
          }
          amenities {
            id
            name
            color
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
  const [commitAddLocationRestrictedInformation] = useMutation<organizationLocation_addLocationRestrictedInformationMutation>(graphql`
    mutation organizationLocation_addLocationRestrictedInformationMutation($input: AddLocationRestrictedInformationInput!) {
      addLocationRestrictedInformation(input: $input) {
        location {
          id
          restrictedInformation {
            id
            title
            category
            content
            active
            sortOrder
          }
        }
      }
    }
  `);
  const [commitUpdateLocationRestrictedInformation] = useMutation<organizationLocation_updateLocationRestrictedInformationMutation>(graphql`
    mutation organizationLocation_updateLocationRestrictedInformationMutation($input: UpdateLocationRestrictedInformationInput!) {
      updateLocationRestrictedInformation(input: $input) {
        location {
          id
          restrictedInformation {
            id
            title
            category
            content
            active
            sortOrder
          }
        }
      }
    }
  `);
  const [commitDeleteLocationRestrictedInformation] = useMutation<organizationLocation_deleteLocationRestrictedInformationMutation>(graphql`
    mutation organizationLocation_deleteLocationRestrictedInformationMutation($input: DeleteLocationRestrictedInformationInput!) {
      deleteLocationRestrictedInformation(input: $input) {
        location {
          id
          restrictedInformation {
            id
            title
            category
            content
            active
            sortOrder
          }
        }
      }
    }
  `);

  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const theme = useTheme();
  const isMobileLocationNav = useMediaQuery(theme.breakpoints.down('sm'), { noSsr: true });
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const activeSection = useMemo(() => getActiveSection(searchParams.get('section')), [searchParams]);
  const [activeLandingTab, setActiveLandingTab] = useState(
    searchParams.get('tab') === 'resources'
      ? 'Resources'
      : searchParams.get('tab') === 'floor-plans'
        ? 'Floor Plans'
        : searchParams.get('tab') === 'operations'
          ? 'Operations'
          : 'Profile',
  );
  const [locationTabsMenuAnchor, setLocationTabsMenuAnchor] = useState<HTMLElement | null>(null);
  const [expandedSetupSection, setExpandedSetupSection] = useState(
    activeSection === 'physical-address-setup'
      ? 'physical-address'
      : activeSection === 'opening-hours'
        ? 'opening-hours'
        : activeSection === 'restricted-information'
          ? 'restricted-information'
          : 'presentation',
  );
  const [stickyTop, setStickyTop] = useState(0);
  const updateLocationUrl = (updates: { tab?: string; section?: string }) => {
    const params = new URLSearchParams(searchParams.toString());
    if (updates.tab) params.set('tab', updates.tab.toLowerCase().replaceAll(' ', '-'));
    if (updates.section !== undefined) {
      if (updates.section) params.set('section', updates.section);
      else params.delete('section');
    }
    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
  };
  const setLocationTab = (tab: string) => {
    setActiveLandingTab(tab);
    updateLocationUrl({ tab, section: tab === 'Profile' ? expandedSetupSection : '' });
  };
  const openLocationTabsMenu = (event: MouseEvent<HTMLElement>) => {
    setLocationTabsMenuAnchor(event.currentTarget);
  };
  const closeLocationTabsMenu = () => {
    setLocationTabsMenuAnchor(null);
  };
  const toggleSetupSection = (section: string) => {
    const next = expandedSetupSection === section ? '' : section;
    setExpandedSetupSection(next);
    updateLocationUrl({ section: next });
  };
  /* eslint-disable react-hooks/set-state-in-effect -- restore the view from browser URL navigation. */
  useEffect(() => {
    const tab = searchParams.get('tab');
    setActiveLandingTab(tab === 'resources' ? 'Resources' : tab === 'floor-plans' ? 'Floor Plans' : tab === 'operations' ? 'Operations' : 'Profile');
    const section = searchParams.get('section');
    setExpandedSetupSection(section ? (section === 'physical-address-setup' ? 'physical-address' : section) : 'presentation');
  }, [searchParams]);
  /* eslint-enable react-hooks/set-state-in-effect */
  const validateLocationDetails = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);
  const validateRestrictedInformation = makeValidate(restrictedInformationSchema);
  const requiredRestrictedInformationFields = makeRequired(restrictedInformationSchema);
  const validatePhysicalAddress = makeValidate(physicalAddressSchema);
  const requiredPhysicalAddressFields = makeRequired(physicalAddressSchema);

  const [locationName, setLocationName] = useState<string>(rootData.location?.name ?? '');
  const debounceSetLocationName = useDebounceCallback(setLocationName, keyboardTextFieldDebounceTimeout);
  const [locationTitle, setLocationTitle] = useState(rootData.location?.listingMetadata.title ?? null);
  const debounceSetLocationTitle = useDebounceCallback(setLocationTitle, keyboardTextFieldDebounceTimeout);
  const [locationSubTitle, setLocationSubTitle] = useState(rootData.location?.listingMetadata.subTitle ?? null);
  const debounceSetLocationSubTitle = useDebounceCallback(setLocationSubTitle, keyboardTextFieldDebounceTimeout);
  const [locationIncludedFeatures, setLocationIncludedFeatures] = useState<string | null>(rootData.location?.listingMetadata.includedFeatures?.join('\n') ?? null);
  const debounceSetLocationIncludedFeatures = useDebounceCallback(setLocationIncludedFeatures, keyboardTextFieldDebounceTimeout);
  const [locationTimezone, setLocationTimezone] = useState<string>(rootData.location?.timezone ?? '');
  const debounceSetLocationTimezone = useDebounceCallback(setLocationTimezone, keyboardTextFieldDebounceTimeout);
  const [spaceTypeIds, setSpaceTypeIds] = useState<string[]>(rootData.location?.spaceTypes.map((item) => item.id) ?? []);
  const debounceSetSpaceTypeIds = useDebounceCallback(setSpaceTypeIds, keyboardTextFieldDebounceTimeout);
  const [amenityIds, setAmenityIds] = useState<string[]>(rootData.location?.amenities.map((item) => item.id) ?? []);
  const debounceSetAmenityIds = useDebounceCallback(setAmenityIds, keyboardTextFieldDebounceTimeout);
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
  const previousLocationValues = useRef<LocationDetails | null>(null);
  const previousLocationFeatureImages = useRef<FileUploadResponse[] | null>(null);
  const submittedPhysicalAddressKey = useRef<string | null>(null);
  const previousRestrictedInformationValues = useRef<Record<string, RestrictedInformationDetails>>({});
  const debouncedLocationDetailsUpdate = useDebounceCallback((save: () => void) => save(), locationAutosaveDebounceTimeout);
  const debouncedPhysicalAddressUpdate = useDebounceCallback((save: () => void) => save(), locationAutosaveDebounceTimeout);
  const debouncedRestrictedInformationUpdate = useDebounceCallback((save: () => void) => save(), locationAutosaveDebounceTimeout);

  useEffect(() => {
    const updateStickyTop = () => {
      setStickyTop(document.querySelector('.app-bar')?.clientHeight ?? 0);
    };

    updateStickyTop();
    window.addEventListener('resize', updateStickyTop);

    return () => {
      window.removeEventListener('resize', updateStickyTop);
    };
  }, []);

  if (!rootData.location) {
    return null;
  }

  const location = rootData.location;
  const restrictedInformation = location.restrictedInformation as RestrictedInformationItem[];

  const formColumnSx = {
    width: '100%',
  };

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setFeatureImages((current) => [response, ...current]);
    setPrimaryFeatureImage((current) => current ?? response);
  };

  const handleRemoveFeatureImage = (image: FileUploadResponse) => {
    setFeatureImages((current) => {
      const next = current.filter((item) => item.original?.url !== image.original?.url);
      if (primaryFeatureImage?.original?.url === image.original?.url) {
        setPrimaryFeatureImage(next[0] ?? null);
      }
      return next;
    });
  };

  const handleSetPrimaryFeatureImage = (image: FileUploadResponse) => {
    setPrimaryFeatureImage(image);
    setFeatureImages((current) => [image, ...current.filter((item) => item.original?.url !== image.original?.url)]);
  };

  function handleLocationDetailUpdateClick(
    fieldsToUpdate: LocationPatchField[],
    {
      name,
      title,
      subTitle,
      includedFeatures,
      timezone,
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
      spaceTypeIds: nextSpaceTypeIds,
      amenityIds: nextAmenityIds,
    }: LocationDetails,
  ) {
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    commitUpdateLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: location.id,
          fieldsToUpdate,
          name,
          listingMetadata: {
            about: '',
            title: title ?? '',
            subTitle: subTitle ?? '',
            includedFeatures: (includedFeatures ?? '')
              .split('\n')
              .map((feature) => feature.trim())
              .filter((feature) => feature !== ''),
          },
          timezone,
          type: location.type.type as LocationType,
          extraMetadata: {
            contactDetails: {
              contactPeople: stringToMultiLines(contactPeople),
              contactEmails: stringToMultiLines(contactEmails),
              contactPhones: stringToMultiLines(contactPhones),
            },
            areaRange: areaRangeFromInSqm && areaRangeToInSqm ? { fromInSqm: areaRangeFromInSqm, toInSqm: areaRangeToInSqm } : null,
            peopleCapacity: peopleCapacityFrom && peopleCapacityTo ? { from: peopleCapacityFrom, to: peopleCapacityTo } : null,
            website: website ?? null,
            relatedImageLinks: stringToMultiLines(relatedImageLinks),
            relatedVideoLinks: stringToMultiLines(relatedVideoLinks),
            otherLinks: stringToMultiLines(otherLinks),
          },
          featureImages: finalFeatureImages,
          tagIds: nextSpaceTypeIds.concat(nextAmenityIds),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to update location '${location.name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to update location '${location.name}'. Error: ${error.message}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        updateLocation: {
          location: {
            id: location.id,
            name,
            listingMetadata: {
              title: title ?? '',
              subTitle: subTitle ?? '',
              includedFeatures: (includedFeatures ?? '')
                .split('\n')
                .map((feature) => feature.trim())
                .filter((feature) => feature !== ''),
            },
            timezone,
            type: {
              type: location.type.type as LocationType,
              name: '',
            },
            extraMetadata: {
              contactDetails: {
                contactPeople: stringToMultiLines(contactPeople),
                contactEmails: stringToMultiLines(contactEmails),
                contactPhones: stringToMultiLines(contactPhones),
              },
              areaRange: areaRangeFromInSqm && areaRangeToInSqm ? { fromInSqm: areaRangeFromInSqm, toInSqm: areaRangeToInSqm } : null,
              peopleCapacity: peopleCapacityFrom && peopleCapacityTo ? { from: peopleCapacityFrom, to: peopleCapacityTo } : null,
              website: website ?? null,
              relatedImageLinks: stringToMultiLines(relatedImageLinks),
              relatedVideoLinks: stringToMultiLines(relatedVideoLinks),
              otherLinks: stringToMultiLines(otherLinks),
            },
            featureImages: finalFeatureImages,
            spaceTypes: location.spaceTypes,
            amenities: location.amenities,
          },
        },
      },
    });
  }

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

  function handlePhysicalAddressUpdateClick({
    osmType,
    osmId,
    placeId,
    longitude,
    latitude,
    formattedAddress,
    country: lookupCountry,
    addressLine1,
    addressLine2,
    suburb,
    city,
    province,
    zipcode,
    countryCode,
  }: PhysicalAddressDetails) {
    if (!physicalAddressSchema.isValidSync({ addressLine1, addressLine2, suburb, city, province, zipcode, countryCode })) {
      return;
    }

    const countryData = getCountryData(countryCode as TCountryCode);
    let country = lookupCountry;
    if (countryData) {
      country = countryData.name;
    }

    const physicalAddress = location.physicalAddress;
    if (physicalAddress) {
      commitUpdateLocationPhysicalAddress({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: physicalAddress.id,
            fieldsToUpdate: ['ADDRESS'],
            osmType,
            osmId,
            placeId,
            longitude,
            latitude,
            formattedAddress,
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
            themedToast(
              <NotificationContent content={`Failed to update location '${location.name}' physical address. Error: ${getRelayErrorMessage(errors)}.`} />,
              errorNotificationOptions,
            );
            return;
          }
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to update location '${location.name}' physical address. Error: ${error.message}.`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateLocationPhysicalAddress: {
            location: {
              id: location.id,
              physicalAddress: {
                id: physicalAddress.id,
                osmType,
                osmId,
                placeId,
                longitude,
                latitude,
                formattedAddress,
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
      return;
    }

    const newAddressId = uuid();
    commitAddLocationPhysicalAddress({
      variables: {
        input: {
          clientMutationId: uuid(),
          locationId: location.id,
          id: newAddressId,
          osmType,
          osmId,
          placeId,
          longitude,
          latitude,
          formattedAddress,
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
          themedToast(
            <NotificationContent content={`Failed to add location '${location.name}' physical address. Error: ${getRelayErrorMessage(errors)}.`} />,
            errorNotificationOptions,
          );
          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add location '${location.name}' physical address. Error: ${error.message}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addLocationPhysicalAddress: {
          location: {
            id: location.id,
            physicalAddress: {
              id: newAddressId,
              osmType,
              osmId,
              placeId,
              longitude,
              latitude,
              formattedAddress,
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

  const handleLocationOpeningHoursUpdateClick = (weekOpeningHours: WeekOpeningHoursDetails) => {
    commitUpdateLocationOpeningHours({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: location.id,
          fieldsToUpdate: ['WEEK_OPENING_HOURS'],
          weekOpeningHours,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`Failed to update location '${location.name}' opening hours. Error: ${getRelayErrorMessage(errors)}.`} />,
            errorNotificationOptions,
          );
          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to update location '${location.name}' opening hours. Error: ${error.message}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        updateLocationOpeningHours: {
          location: {
            id: location.id,
            openingHours: {
              weekOpeningHours,
            },
          },
        },
      },
    });
  };

  const handleAddRestrictedInformationClick = ({ title, category, content, active, sortOrder }: RestrictedInformationDetails) => {
    commitAddLocationRestrictedInformation({
      variables: {
        input: {
          clientMutationId: uuid(),
          locationId: location.id,
          title,
          category,
          content,
          active,
          sortOrder: Number(sortOrder) || 0,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to add restricted information. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
          return;
        }
        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add restricted information. Error: ${error.message}.`} />, errorNotificationOptions);
      },
    });
  };

  function handleUpdateRestrictedInformationClick(id: string, fieldsToUpdate: LocationRestrictedInformationPatchField[], values: RestrictedInformationDetails) {
    const { title, category, content, active, sortOrder } = values;
    const validFieldsToUpdate = getValidRestrictedInformationPatchFields(fieldsToUpdate, values);
    if (validFieldsToUpdate.length === 0) {
      return;
    }

    commitUpdateLocationRestrictedInformation({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          fieldsToUpdate: validFieldsToUpdate,
          title,
          category,
          content,
          active,
          sortOrder: Number(sortOrder) || 0,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to update restricted information. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
          return;
        }
        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to update restricted information. Error: ${error.message}.`} />, errorNotificationOptions);
      },
    });
  }

  const handleDeleteRestrictedInformationClick = (item: RestrictedInformationItem) => {
    commitDeleteLocationRestrictedInformation({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: item.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to remove restricted information. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
          return;
        }

        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to remove restricted information. Error: ${error.message}.`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveLocationClicked = () => {
    commitDeleteLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: location.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to remove the location '${location.name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
          return;
        }

        router.push(getOrganizationLocationsBaseLink(integratedPlatform, organizationCustomDomain));
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to remove the location '${location.name}'. Error: ${error.message}.`} />, errorNotificationOptions);
      },
    });
  };

  const renderLocationSummaryRail = () => (
    <StackColumn spacing={2} sx={{ position: { xl: 'sticky' }, top: { xl: stickyTop + 24 }, alignSelf: 'start', pl: { xs: 0, xl: 0 }, pr: 0, pt: 0 }}>
      <SettingsSectionCard title="Summary" description="A compact snapshot of the location currently being managed.">
        <StackColumn>
          <StackRow sx={{ flexWrap: 'wrap', gap: 1 }}>
            <Chip size="small" label={location.type.name} />
            <Chip size="small" label={location.timezone} />
            <Chip size="small" label={`${location.spaceTypes.length} space types`} />
            <Chip size="small" label={`${location.amenities.length} amenities`} />
          </StackRow>
          <BodyIconTypography label={location.name} />
          {location.listingMetadata.title && <BodyIconTypography label={`Title: ${location.listingMetadata.title}`} />}
          {location.physicalAddress?.formattedAddress && <BodyIconTypography label={location.physicalAddress.formattedAddress} />}
        </StackColumn>
      </SettingsSectionCard>
    </StackColumn>
  );

  const renderSetupSection = () => (
    <Form<LocationDetails>
      onSubmit={() => undefined}
      initialValues={{
        name: locationName,
        title: locationTitle,
        subTitle: locationSubTitle,
        includedFeatures: locationIncludedFeatures,
        timezone: locationTimezone,
        spaceTypeIds,
        amenityIds,
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
        debounceSetLocationTimezone(values!.timezone);
        debounceSetSpaceTypeIds(values!.spaceTypeIds ?? []);
        debounceSetAmenityIds(values!.amenityIds ?? []);
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
        debounceSetLocationTitle(values!.title ?? null);
        debounceSetLocationSubTitle(values!.subTitle ?? null);
        debounceSetLocationIncludedFeatures(values!.includedFeatures ?? null);
        const locationValues = values as LocationDetails;
        const changedFormFields = getChangedLocationFields(previousLocationValues.current, locationValues);
        const extraFields: LocationPatchField[] =
          previousLocationFeatureImages.current !== null && featureImages !== previousLocationFeatureImages.current ? ['FEATURE_IMAGES'] : [];
        const fieldsToUpdate: LocationPatchField[] = [...changedFormFields, ...extraFields];
        if (previousLocationValues.current === null) {
          previousLocationValues.current = locationValues;
          previousLocationFeatureImages.current = featureImages;
        } else if (fieldsToUpdate.length > 0) {
          previousLocationValues.current = locationValues;
          previousLocationFeatureImages.current = featureImages;
          debouncedLocationDetailsUpdate(() => handleLocationDetailUpdateClick(fieldsToUpdate, locationValues));
        }

        return (
          <Box sx={{ display: 'grid', gridTemplateColumns: 'minmax(0, 1fr)', gap: { xs: 2, xl: 2 } }}>
            <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
              <StackColumn spacing={1.5}>
                <EditorSection
                  title="Presentation"
                  description="Shape the customer-facing identity and visual presentation of this location."
                  summary={`${featureImages.length} image${featureImages.length === 1 ? '' : 's'} · ${locationName || 'Unnamed location'}`}
                  expanded={expandedSetupSection === 'presentation'}
                  onChange={() => toggleSetupSection('presentation')}
                >
                  <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'minmax(0, 1fr) minmax(0, 1fr)' }, gap: { xs: 2, md: 3 } }}>
                    <StackColumn spacing={1.5}>
                      <LeadIconTypography label="Cover and gallery" />
                      <SmallIconTypography label="Use a strong cover image to help customers recognize this location." />
                      <FormFieldLabel label="Feature Images">
                        <StackColumn>
                          <Box sx={{ display: 'grid', gridTemplateColumns: 'minmax(0, 1fr)', gap: 1.5 }}>
                            {[primaryFeatureImage ?? featureImages[0]]
                              .filter((image): image is FileUploadResponse => !!image)
                              .map((image, index) => (
                                <Box
                                  key={index}
                                  sx={{
                                    position: 'relative',
                                    aspectRatio: '16 / 9',
                                    borderRadius: 3,
                                    overflow: 'hidden',
                                    border: 1,
                                    borderColor: 'divider',
                                    backgroundColor: paletteMode === 'dark' ? 'grey.900' : 'grey.50',
                                  }}
                                >
                                  <Image
                                    width={800}
                                    height={600}
                                    unoptimized
                                    alt=""
                                    src={image.original?.url ?? image.thumbnail?.url ?? ''}
                                    style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                                  />
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
                          <Box
                            sx={{
                              position: 'relative',
                              overflow: 'hidden',
                              border: 1,
                              borderStyle: 'dashed',
                              borderColor: 'success.main',
                              borderRadius: 2.5,
                              p: 2,
                              backgroundColor: 'action.hover',
                              '& .MuiFormControl-root': { position: 'absolute', inset: 0, width: '100%', height: '100%', opacity: 0, zIndex: 1 },
                              '& .MuiInput-root, & input': { width: '100%', height: '100%', cursor: 'pointer' },
                            }}
                          >
                            <StackRow sx={{ alignItems: 'center', justifyContent: 'center', gap: 1 }}>
                              <AddPhotoAlternateRoundedIcon color="success" />
                              <BodyIconTypography label={featureImages.length === 0 ? 'Choose a cover image' : 'Add another image'} />
                            </StackRow>
                            <ImageFileUploaderWithCropper onUploadCompleted={handleFeatureImageUploadCompleted} />
                          </Box>
                        </StackColumn>
                      </FormFieldLabel>
                    </StackColumn>
                    <StackColumn spacing={1.5}>
                      <LeadIconTypography label="Customer-facing details" />
                      <SmallIconTypography label="Use concise language customers can scan before choosing this location." />
                      <FormFieldLabel label="Name">
                        <TextField name="name" required={requiredFields.name} />
                      </FormFieldLabel>
                      <ListingMetadata
                        fields={['title', 'subTitle', 'includedFeatures']}
                        onChange={({ title, subTitle, includedFeatures }) => {
                          debounceSetLocationTitle(title);
                          debounceSetLocationSubTitle(subTitle);
                          debounceSetLocationIncludedFeatures(includedFeatures);
                        }}
                        requiredFields={requiredFields}
                      />
                    </StackColumn>
                  </Box>
                </EditorSection>

                <EditorSection
                  title="Classification"
                  description="Set how this location is categorized and presented in availability searches."
                  summary={`${locationTimezone || 'No timezone'} · ${spaceTypeIds.length} space types · ${amenityIds.length} amenities`}
                  expanded={expandedSetupSection === 'classification'}
                  onChange={() => toggleSetupSection('classification')}
                >
                  <StackColumn>
                    <FormFieldLabel label="Timezone">
                      <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Space Type">
                      <MultipleChoicesLocationSpaceTypes rootDataRelay={rootData} name="spaceTypeIds" required={requiredFields.spaceTypeIds} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Amenities">
                      <MultipleChoicesAmenities rootDataRelay={rootData} name="amenityIds" required={requiredFields.amenityIds} />
                    </FormFieldLabel>
                  </StackColumn>
                </EditorSection>

                <EditorSection
                  title="Capacity & Links"
                  description="Add capacity, website, and supporting media links for customers and operators."
                  summary={`${locationAreaRangeFromSqm || 'No area'} · ${locationPeopleCapacityFrom || 'No capacity'} · ${locationWebsite || 'No website'}`}
                  expanded={expandedSetupSection === 'capacity'}
                  onChange={() => toggleSetupSection('capacity')}
                >
                  <StackColumn>
                    {rootData.me.emails.some((item) => !!rootData.emailsToShowLatestCapabilities.find((email) => email.toLocaleLowerCase() === item.toLocaleLowerCase())) && (
                      <>
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
                  </StackColumn>
                </EditorSection>

                <EditorSection
                  title="Contact Details"
                  description="Keep the public contact points for this location accurate and easy to maintain."
                  summary={`${locationContactEmail || 'No email'} · ${locationContactPhone || 'No phone'}`}
                  expanded={expandedSetupSection === 'contact'}
                  onChange={() => toggleSetupSection('contact')}
                >
                  <StackColumn>
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
                </EditorSection>
              </StackColumn>
            </FormStackColumn>
          </Box>
        );
      }}
    />
  );

  const renderPhysicalAddressSection = () => (
    <Form<PhysicalAddressDetails>
      onSubmit={() => undefined}
      initialValues={{
        osmType: physicalAddressOsmType,
        osmId: physicalAddressOsmId,
        placeId: physicalAddressPlaceId,
        longitude: physicalAddressLongitude,
        latitude: physicalAddressLatitude,
        formattedAddress: physicalAddressFormattedAddress,
        country: physicalAddressCountry,
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
        const physicalAddressValues = values as PhysicalAddressDetails;
        const nextPhysicalAddressKey = JSON.stringify(physicalAddressValues);
        if (submittedPhysicalAddressKey.current === null) {
          submittedPhysicalAddressKey.current = nextPhysicalAddressKey;
        } else if (nextPhysicalAddressKey !== submittedPhysicalAddressKey.current) {
          submittedPhysicalAddressKey.current = nextPhysicalAddressKey;
          debouncedPhysicalAddressUpdate(() => handlePhysicalAddressUpdateClick(physicalAddressValues));
        }

        return (
          <Box sx={{ display: 'grid', gridTemplateColumns: 'minmax(0, 1fr)', gap: { xs: 2, xl: 2 } }}>
            <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
              <EditorSection
                title="Physical Address"
                description="Use the exact address members and customers should navigate to."
                summary={physicalAddressFormattedAddress || physicalAddressAddressLine1 || 'No address set'}
                expanded={expandedSetupSection === 'physical-address'}
                onChange={() => toggleSetupSection('physical-address')}
              >
                <StackColumn>
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
                        form.change('osmType', address.osmType);
                        form.change('osmId', address.osmId);
                        form.change('placeId', address.placeId);
                        form.change('longitude', address.longitude);
                        form.change('latitude', address.latitude);
                        form.change('formattedAddress', address.formattedAddress);
                        form.change('country', address.country ?? '');
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
              </EditorSection>
            </FormStackColumn>
          </Box>
        );
      }}
    />
  );

  const renderOpeningHoursSection = () => (
    <EditorSection
      title="Opening Hours"
      description="Manage the standard opening hours that bookings and availability will follow."
      summary="Weekly availability schedule"
      expanded={expandedSetupSection === 'opening-hours'}
      onChange={() => toggleSetupSection('opening-hours')}
    >
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
    </EditorSection>
  );

  const renderRestrictedInformationFields = () => (
    <StackColumn>
      <FormFieldLabel label="Title" required={requiredRestrictedInformationFields.title}>
        <TextField name="title" required={requiredRestrictedInformationFields.title} />
      </FormFieldLabel>
      <FormFieldLabel label="Category" required={requiredRestrictedInformationFields.category}>
        <SingleChoiceLocationRestrictedInformationCategory rootDataRelay={rootData} name="category" required={requiredRestrictedInformationFields.category} />
      </FormFieldLabel>
      <FormFieldLabel label="Details" required={requiredRestrictedInformationFields.content}>
        <TextField name="content" required={requiredRestrictedInformationFields.content} multiline rows={6} />
      </FormFieldLabel>
      <FormFieldLabel label="Sort Order">
        <TextField name="sortOrder" type="number" />
      </FormFieldLabel>
    </StackColumn>
  );

  const renderRestrictedInformationSection = () => (
    <EditorSection
      title="Restricted Information"
      description="Manage private details shown only to organization members or customers with bookings at this location."
      summary={`${restrictedInformation.length} private entr${restrictedInformation.length === 1 ? 'y' : 'ies'}`}
      expanded={expandedSetupSection === 'restricted-information'}
      onChange={() => toggleSetupSection('restricted-information')}
    >
      <StackColumn spacing={2}>
        {restrictedInformation.length === 0 ? (
          <BodyIconTypography label="No restricted information has been added for this location." />
        ) : (
          restrictedInformation.map((item) => (
            <Form<RestrictedInformationDetails>
              key={item.id}
              onSubmit={() => undefined}
              initialValues={{
                title: item.title,
                category: item.category,
                content: item.content,
                active: item.active,
                sortOrder: item.sortOrder,
              }}
              validate={validateRestrictedInformation}
              render={({ handleSubmit, values }) => {
                const restrictedInformationValues = values as RestrictedInformationDetails;
                const previousValues = previousRestrictedInformationValues.current[item.id];
                if (!previousValues) {
                  previousRestrictedInformationValues.current[item.id] = restrictedInformationValues;
                } else {
                  const changedFields = getChangedRestrictedInformationFields(previousValues, restrictedInformationValues);
                  if (changedFields.length > 0) {
                    previousRestrictedInformationValues.current[item.id] = restrictedInformationValues;
                    debouncedRestrictedInformationUpdate(() => handleUpdateRestrictedInformationClick(item.id, changedFields, restrictedInformationValues));
                  }
                }

                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <Box sx={{ border: 1, borderColor: 'divider', borderRadius: 2, p: 2 }}>
                      <StackColumn>
                        {renderRestrictedInformationFields()}
                        <StackRow sx={{ justifyContent: 'flex-end', flexWrap: 'wrap' }}>
                          <Button color="error" variant="outlined" onClick={() => handleDeleteRestrictedInformationClick(item)} sx={defaultButtonStyle}>
                            Remove
                          </Button>
                        </StackRow>
                      </StackColumn>
                    </Box>
                  </FormStackColumn>
                );
              }}
            />
          ))
        )}
        <SettingsSectionCard
          title="Add Restricted Information"
          description="Create a new private note for access, Wi-Fi, cleaning, security, parking, or other operational details."
        >
          <Form<RestrictedInformationDetails>
            onSubmit={(values) => handleAddRestrictedInformationClick(values as RestrictedInformationDetails)}
            initialValues={{
              title: '',
              category: 'OTHER',
              content: '',
              active: true,
              sortOrder: restrictedInformation.length,
            }}
            validate={validateRestrictedInformation}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                {renderRestrictedInformationFields()}
                <EditorActionBar
                  primaryAction={
                    <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                      Add
                    </Button>
                  }
                />
              </FormStackColumn>
            )}
          />
        </SettingsSectionCard>
      </StackColumn>
    </EditorSection>
  );

  const renderManageLocationSection = () => (
    <Box sx={{ pb: defaultPadding }}>
      <SettingsSectionCard title="Manage Location" description="Use destructive actions here only when the location should be removed permanently.">
        <EditorActionBar
          primaryAction={
            <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveLocationClicked} sx={{ textTransform: 'none' }}>
              Remove Location
            </Button>
          }
        />
      </SettingsSectionCard>
    </Box>
  );

  const sectionLinks: Record<OrganizationLocationSection, string> = {
    setup: getOrganizationLocationSetupBaseLink(integratedPlatform, organizationCustomDomain, locationId),
    'physical-address-setup': getOrganizationLocationPhysicalAddressSetupBaseLink(integratedPlatform, organizationCustomDomain, locationId),
    'opening-hours': getOrganizationLocationOpeningHoursBaseLink(integratedPlatform, organizationCustomDomain, locationId),
    'floor-plans': getOrganizationLocationFloorPlansBaseLink(integratedPlatform, organizationCustomDomain, locationId),
    'manage-resources': getOrganizationLocationManageResourcesBaseLink(integratedPlatform, organizationCustomDomain, locationId),
    'restricted-information': getOrganizationLocationRestrictedInformationBaseLink(integratedPlatform, organizationCustomDomain, locationId),
    'manage-location': getOrganizationLocationManageLocationBaseLink(integratedPlatform, organizationCustomDomain, locationId),
  };
  const locationCards = [
    {
      title: 'Profile',
      description: 'Location identity, listing content, images, contact details, amenities, and space types.',
      sections: ['setup'] satisfies OrganizationLocationSection[],
    },
    {
      title: 'Resources',
      description: 'Manage the resources and capacity available at this location.',
      sections: ['manage-resources'] satisfies OrganizationLocationSection[],
    },
    {
      title: 'Floor Plans',
      description: 'Manage the floor plans available for this location.',
      sections: ['floor-plans'] satisfies OrganizationLocationSection[],
    },
    {
      title: 'Operations',
      description: 'Private operating notes and location lifecycle controls.',
      sections: ['manage-location'] satisfies OrganizationLocationSection[],
    },
  ];

  const renderLocationTabs = () =>
    isMobileLocationNav ? (
      <Box sx={{ borderTop: 1, borderColor: 'divider', pt: 1.5 }}>
        <Button
          fullWidth
          variant="outlined"
          color="inherit"
          onClick={openLocationTabsMenu}
          aria-haspopup="menu"
          aria-expanded={locationTabsMenuAnchor ? 'true' : undefined}
          aria-controls={locationTabsMenuAnchor ? 'location-settings-sections-menu' : undefined}
          endIcon={<ExpandMoreRoundedIcon />}
          sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
        >
          {`Section: ${activeLandingTab}`}
        </Button>
        <Menu anchorEl={locationTabsMenuAnchor} open={Boolean(locationTabsMenuAnchor)} onClose={closeLocationTabsMenu} id="location-settings-sections-menu">
          {locationCards.map((card) => (
            <MenuItem
              key={card.title}
              selected={activeLandingTab === card.title}
              onClick={() => {
                setLocationTab(card.title);
                closeLocationTabsMenu();
              }}
            >
              {card.title}
            </MenuItem>
          ))}
        </Menu>
      </Box>
    ) : (
      <Tabs
        value={activeLandingTab}
        onChange={(_, tab: string) => setLocationTab(tab)}
        variant="scrollable"
        scrollButtons="auto"
        aria-label="Location settings sections"
        sx={{
          mb: -2,
          borderTop: 1,
          borderColor: 'divider',
          '& .MuiTabs-indicator': {
            height: 3,
            borderRadius: '3px 3px 0 0',
          },
        }}
      >
        {locationCards.map((card) => (
          <Tab
            key={card.title}
            value={card.title}
            label={card.title}
            disableRipple
            sx={{
              minWidth: 112,
              minHeight: 52,
              px: 2.5,
              textTransform: 'none',
              whiteSpace: 'nowrap',
              color: 'text.secondary',
              fontWeight: 500,
              '&.Mui-selected': {
                color: 'primary.main',
                fontWeight: 600,
              },
              '&:hover': {
                color: 'text.primary',
                backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(46, 125, 50, 0.08)' : 'rgba(255, 255, 255, 0.06)'),
              },
            }}
          />
        ))}
      </Tabs>
    );

  const renderOverview = () => (
    <StackColumn spacing={2}>
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' },
          gap: 2,
        }}
      >
        {activeLandingTab === 'Profile' ? (
          <Box sx={{ gridColumn: '1 / -1' }}>
            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: 'minmax(0, 1fr) 320px' }, gap: 2 }}>
              <StackColumn spacing={1.5}>
                {renderSetupSection()}
                {renderPhysicalAddressSection()}
                {renderOpeningHoursSection()}
                {renderRestrictedInformationSection()}
              </StackColumn>
              {renderLocationSummaryRail()}
            </Box>
          </Box>
        ) : activeLandingTab === 'Floor Plans' ? (
          <Box sx={{ gridColumn: '1 / -1' }}>
            <Suspense fallback={<Loading />}>
              <OrganizationLocationFloorPlansSection organizationCustomDomain={organizationCustomDomain} locationId={locationId} />
            </Suspense>
          </Box>
        ) : activeLandingTab === 'Resources' ? (
          <Box sx={{ gridColumn: '1 / -1' }}>
            <Suspense fallback={<Loading />}>
              <OrganizationLocationManageResourcesSection onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} locationId={locationId} />
            </Suspense>
          </Box>
        ) : (
          <Box sx={{ gridColumn: '1 / -1' }}>{renderManageLocationSection()}</Box>
        )}
        {false &&
          locationCards
            .filter((card) => card.title === activeLandingTab)
            .map((card) => {
              const primarySection = card.sections[0];

              return (
                <Card
                  key={card.title}
                  variant="outlined"
                  sx={{
                    borderRadius: 3,
                    borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
                    boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.06)' : theme.shadows[1]),
                    overflow: 'hidden',
                  }}
                >
                  <CardContent>
                    <StackColumn spacing={1.5}>
                      <StackColumn spacing={0.5}>
                        <LeadIconTypography label={card.title} />
                        <BodyIconTypography label={card.description} />
                      </StackColumn>
                      <Divider />
                      <StackRow sx={{ flexWrap: 'wrap', gap: 1 }}>
                        {card.sections.map((item) => (
                          <Button
                            key={item}
                            component={NextLink}
                            href={sectionLinks[item]}
                            variant="outlined"
                            size="small"
                            sx={{
                              borderRadius: 999,
                              textTransform: 'none',
                              fontWeight: 700,
                              ...(item === primarySection
                                ? {
                                    bgcolor: (theme) => (theme.palette.mode === 'light' ? 'grey.900' : 'grey.100'),
                                    borderColor: (theme) => (theme.palette.mode === 'light' ? 'grey.900' : 'grey.100'),
                                    color: (theme) => (theme.palette.mode === 'light' ? 'common.white' : 'grey.900'),
                                    '&:hover': {
                                      bgcolor: (theme) => (theme.palette.mode === 'light' ? 'grey.800' : 'common.white'),
                                      borderColor: (theme) => (theme.palette.mode === 'light' ? 'grey.800' : 'common.white'),
                                    },
                                  }
                                : {
                                    bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : 'transparent'),
                                    borderColor: (theme) => (theme.palette.mode === 'light' ? 'grey.400' : 'grey.500'),
                                    color: 'text.primary',
                                    '&:hover': {
                                      bgcolor: (theme) => (theme.palette.mode === 'light' ? 'grey.50' : 'rgba(255, 255, 255, 0.08)'),
                                      borderColor: 'text.primary',
                                    },
                                  }),
                            }}
                          >
                            {sectionLabels[item]}
                          </Button>
                        ))}
                      </StackRow>
                    </StackColumn>
                  </CardContent>
                </Card>
              );
            })}
      </Box>
    </StackColumn>
  );

  const renderSection = () => {
    switch (activeSection) {
      case 'physical-address-setup':
      case 'setup':
      case 'opening-hours':
      case 'restricted-information':
        return (
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: 'minmax(0, 1fr) 320px' }, gap: { xs: 2, xl: 2 }, pb: defaultPadding }}>
            <StackColumn spacing={1.5}>
              {renderSetupSection()}
              {renderPhysicalAddressSection()}
              {renderOpeningHoursSection()}
              {renderRestrictedInformationSection()}
            </StackColumn>
            {renderLocationSummaryRail()}
          </Box>
        );
      case 'floor-plans':
        return (
          <Suspense fallback={<Loading />}>
            <OrganizationLocationFloorPlansSection organizationCustomDomain={organizationCustomDomain} locationId={locationId} />
          </Suspense>
        );
      case 'manage-resources':
        return (
          <Suspense fallback={<Loading />}>
            <OrganizationLocationManageResourcesSection onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} locationId={locationId} />
          </Suspense>
        );
      case 'manage-location':
        return renderManageLocationSection();
      default:
        return renderOverview();
    }
  };

  return (
    <Box
      sx={{
        width: '100%',
        maxWidth: '100vw',
        minWidth: 0,
        display: 'flex',
        justifyContent: 'center',
        overflowX: 'hidden',
        boxSizing: 'border-box',
        px: { xs: 0, sm: 1, md: 2 },
        pt: { xs: 1, sm: 1, md: 2 },
        pb: defaultPadding,
      }}
    >
      <StackColumn
        sx={{
          width: '100%',
          maxWidth: 1200,
          minWidth: 0,
          mx: 'auto',
          overflowX: 'hidden',
          backgroundColor: 'transparent',
          gap: 2,
        }}
      >
        <PageHeaderPanel
          eyebrow="Location settings"
          title={location.name}
          description="Setup, address, opening hours, floor plans, resources, and lifecycle controls."
          sx={{ width: '100%', minWidth: 0, maxWidth: '100%' }}
        >
          {activeSection ? null : renderLocationTabs()}
        </PageHeaderPanel>
        {renderSection()}
      </StackColumn>
    </Box>
  );
};

export default memo(OrganizationLocation);
