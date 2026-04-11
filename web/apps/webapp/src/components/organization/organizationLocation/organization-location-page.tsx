import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { Address, PhysicalAddress } from '@/components/address';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, GridContainer, LeadIconTypography, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { DeleteIcon } from '@/components/icons';
import { getOrganizationLocationsBaseLink } from '@/components/links';
import { ListingMetadata, listingMetadataSchemaShape } from '@/components/listingMetadata';
import { Loading } from '@/components/loading';
import { MultipleChoicesLocationSpaceTypes, SingleChoiceLocationType } from '@/components/location';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesAmenities } from '@/components/organization';
import OrganizationLocationFloorPlansSection from '@/components/organization/organizationLocation/organization-location-floor-plans-section';
import OrganizationLocationManageResourcesSection from '@/components/organization/organizationLocation/organization-location-manage-resources-section';
import OrganizationLocationSectionNav, { OrganizationLocationSection } from '@/components/organization/organizationLocation/organization-location-section-nav';
import { WeekOpeningHours, WeekOpeningHoursDetails } from '@/components/weekOpeningHours';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getRelayErrorMessage, keyboardTextFieldDebounceTimeout, stringCollectionToString, stringToMultiLines } from '@/libs/utils';
import type { organizationLocationPage_addLocationPhysicalAddressMutation } from '@/queries/__generated__/organizationLocationPage_addLocationPhysicalAddressMutation.graphql';
import type { organizationLocationPage_deleteLocationMutation } from '@/queries/__generated__/organizationLocationPage_deleteLocationMutation.graphql';
import type { organizationLocationPage_query$key } from '@/queries/__generated__/organizationLocationPage_query.graphql';
import type { LocationType, organizationLocationPage_updateLocationMutation } from '@/queries/__generated__/organizationLocationPage_updateLocationMutation.graphql';
import type { organizationLocationPage_updateLocationOpeningHoursMutation } from '@/queries/__generated__/organizationLocationPage_updateLocationOpeningHoursMutation.graphql';
import type { organizationLocationPage_updateLocationPhysicalAddressMutation } from '@/queries/__generated__/organizationLocationPage_updateLocationPhysicalAddressMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, Suspense, useContext, useEffect, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, object, string } from 'yup';

type Props = {
  rootDataRelay: organizationLocationPage_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  locationId: string;
};

type LocationDetails = {
  name: string;
  title: string | null;
  subTitle: string | null;
  includedFeatures: string | null;
  timezone: string;
  type: string;
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
  addressLine1: string;
  addressLine2: string | null;
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string;
  countryCode: string;
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  ...listingMetadataSchemaShape,
  timezone: string().required('Timezone is required'),
  type: string().required('Type is required'),
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

const physicalAddressSchema = object({
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string(),
  city: string(),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  countryCode: string().required('Country is required'),
});

const validSections: OrganizationLocationSection[] = ['setup', 'physical-address-setup', 'opening-hours', 'floor-plans', 'manage-resources', 'manage-location'];

const getActiveSection = (value: string | null): OrganizationLocationSection => {
  if (value && validSections.includes(value as OrganizationLocationSection)) {
    return value as OrganizationLocationSection;
  }

  return 'setup';
};

const OrganizationLocationPage = ({ rootDataRelay, onReloadRequired, organizationCustomDomain, locationId }: Props) => {
  const rootData = useFragment<organizationLocationPage_query$key>(
    graphql`
      fragment organizationLocationPage_query on Query {
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
        }
        ...weekOpeningHours_query
        ...singleChoiceLocationType_query
        ...multipleChoicesLocationSpaceTypes_query
        ...multipleChoicesAmenities_query
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateLocation] = useMutation<organizationLocationPage_updateLocationMutation>(graphql`
    mutation organizationLocationPage_updateLocationMutation($input: UpdateLocationInput!) @raw_response_type {
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
  const [commitDeleteLocation] = useMutation<organizationLocationPage_deleteLocationMutation>(graphql`
    mutation organizationLocationPage_deleteLocationMutation($input: DeleteLocationInput!) {
      deleteLocation(input: $input) {
        location {
          id
        }
      }
    }
  `);
  const [commitUpdateLocationOpeningHours] = useMutation<organizationLocationPage_updateLocationOpeningHoursMutation>(graphql`
    mutation organizationLocationPage_updateLocationOpeningHoursMutation($input: UpdateLocationOpeningHoursInput!) @raw_response_type {
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
  const [commitAddLocationPhysicalAddress] = useMutation<organizationLocationPage_addLocationPhysicalAddressMutation>(graphql`
    mutation organizationLocationPage_addLocationPhysicalAddressMutation($input: AddLocationPhysicalAddressInput!) @raw_response_type {
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
  const [commitUpdateLocationPhysicalAddress] = useMutation<organizationLocationPage_updateLocationPhysicalAddressMutation>(graphql`
    mutation organizationLocationPage_updateLocationPhysicalAddressMutation($input: UpdateLocationPhysicalAddressInput!) @raw_response_type {
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
  const router = useRouter();
  const searchParams = useSearchParams();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const activeSection = useMemo(() => getActiveSection(searchParams.get('section')), [searchParams]);
  const [stickyTop, setStickyTop] = useState(0);
  const validateLocationDetails = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);
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
  const [locationType, setLocationType] = useState<string>(rootData.location?.type.type ?? '');
  const debounceSetLocationType = useDebounceCallback(setLocationType, keyboardTextFieldDebounceTimeout);
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

  const handleLocationDetailUpdateClick = ({
    name,
    title,
    subTitle,
    includedFeatures,
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
    spaceTypeIds: nextSpaceTypeIds,
    amenityIds: nextAmenityIds,
  }: LocationDetails) => {
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
          type: type as LocationType,
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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update location '${location.name}'. Error: ${getRelayErrorMessage(errors)}.`} />,
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
          render: <NotificationContent content={`Failed to update location '${location.name}'. Error: ${error.message}.`} />,
        });
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
              type: type as LocationType,
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

  const handlePhysicalAddressUpdateClick = ({ addressLine1, addressLine2, suburb, city, province, zipcode, countryCode }: PhysicalAddressDetails) => {
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
              render: <NotificationContent content={`Failed to update location '${location.name}' physical address. Error: ${getRelayErrorMessage(errors)}.`} />,
            });
            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Location '${location.name}' physical address updated.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update location '${location.name}' physical address. Error: ${error.message}.`} />,
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
      return;
    }

    const newAddressId = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding location '${location.name}' physical address...`} />, infoNotificationOptions);
    commitAddLocationPhysicalAddress({
      variables: {
        input: {
          clientMutationId: uuid(),
          locationId: location.id,
          id: newAddressId,
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
            render: <NotificationContent content={`Failed to add location '${location.name}' physical address. Error: ${getRelayErrorMessage(errors)}.`} />,
          });
          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${location.name}' physical address added.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add location '${location.name}' physical address. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addLocationPhysicalAddress: {
          location: {
            id: location.id,
            physicalAddress: {
              id: newAddressId,
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
  };

  const handleLocationOpeningHoursUpdateClick = (weekOpeningHours: WeekOpeningHoursDetails) => {
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
            render: <NotificationContent content={`Failed to update location '${location.name}' opening hours. Error: ${getRelayErrorMessage(errors)}.`} />,
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
          render: <NotificationContent content={`Failed to update location '${location.name}' opening hours. Error: ${error.message}.`} />,
        });
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

  const handleRemoveLocationClicked = () => {
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
            render: <NotificationContent content={`Failed to remove the location '${location.name}'. Error: ${getRelayErrorMessage(errors)}.`} />,
          });
          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${location.name}' removed.`} />,
        });
        router.push(getOrganizationLocationsBaseLink(integratedPlatrform, organizationCustomDomain));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the location '${location.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const renderSetupSection = () => (
    <Form
      onSubmit={handleLocationDetailUpdateClick}
      initialValues={{
        name: locationName,
        title: locationTitle,
        subTitle: locationSubTitle,
        includedFeatures: locationIncludedFeatures,
        timezone: locationTimezone,
        type: locationType,
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
        debounceSetLocationType(values!.type);
        debounceSetSpaceTypeIds(values!.spaceTypeIds);
        debounceSetAmenityIds(values!.amenityIds);
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
        debounceSetLocationTitle(values!.title);
        debounceSetLocationSubTitle(values!.subTitle);
        debounceSetLocationIncludedFeatures(values!.includedFeatures);

        return (
          <FormStackColumn onSubmit={handleSubmit}>
            <StackColumn sx={{ padding: defaultPadding }}>
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Location Setup" />
                  <BodyIconTypography label="Edit your location name and details" />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingBottom: defaultPadding }}>
              <FormFieldLabel label="Feature Images">
                <StackColumn>
                  <GridContainer
                    sx={{ display: 'grid', gridTemplateColumns: { xs: 'repeat(auto-fill, minmax(140px, 1fr))', sm: 'repeat(auto-fill, minmax(180px, 1fr))' }, gap: 2 }}
                  >
                    {featureImages.map((image, index) => (
                      <Grid
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
                      </Grid>
                    ))}
                  </GridContainer>
                  <ImageFileUploaderWithCropper onUploadCompleted={handleFeatureImageUploadCompleted} />
                </StackColumn>
              </FormFieldLabel>

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

              <FormFieldLabel label="Timezone">
                <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
              </FormFieldLabel>

              <FormFieldLabel label="Space Type">
                <MultipleChoicesLocationSpaceTypes rootDataRelay={rootData} name="spaceTypeIds" required={requiredFields.spaceTypeIds} />
              </FormFieldLabel>

              <FormFieldLabel label="Amenities">
                <MultipleChoicesAmenities rootDataRelay={rootData} name="amenityIds" required={requiredFields.amenityIds} />
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
  );

  const renderPhysicalAddressSection = () => (
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
            <StackColumn sx={{ padding: defaultPadding }}>
              <SectionIconTypography label="Physical Address Setup" />
              <BodyIconTypography label="Edit your organization physical address" />
              <Divider />
            </StackColumn>
            <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingBottom: defaultPadding }}>
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
  );

  const renderOpeningHoursSection = () => (
    <>
      <StackColumn sx={{ padding: defaultPadding }}>
        <SectionIconTypography label="Opening Hours" />
        <BodyIconTypography label="Manage your location opening hours" />
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
    </>
  );

  const renderManageLocationSection = () => (
    <StackColumn sx={{ padding: defaultPadding }}>
      <SectionIconTypography label="Manage" />
      <BodyIconTypography label="Remove your location" />
      <Divider />
      <StackRow sx={{ paddingTop: defaultPadding }}>
        <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveLocationClicked} sx={{ textTransform: 'none' }}>
          Remove Location
        </Button>
      </StackRow>
    </StackColumn>
  );

  const renderSection = () => {
    switch (activeSection) {
      case 'physical-address-setup':
        return renderPhysicalAddressSection();
      case 'opening-hours':
        return renderOpeningHoursSection();
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
      case 'setup':
      default:
        return renderSetupSection();
    }
  };

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn
        sx={{
          width: '100%',
          maxWidth: 1120,
          mx: 'auto',
          backgroundColor: 'background.paper',
        }}
      >
        <StackColumn sx={{ px: { xs: 2, sm: 3 }, pt: defaultPadding, pb: 1 }}>
          <StackColumn spacing={0.5}>
            <BodyIconTypography label="Location settings" />
            <LeadIconTypography label={location.name} />
          </StackColumn>
        </StackColumn>

        <OrganizationLocationSectionNav activeSection={activeSection} organizationCustomDomain={organizationCustomDomain} locationId={locationId} stickyTop={stickyTop} />
        {renderSection()}
      </StackColumn>
    </Box>
  );
};

export default memo(OrganizationLocationPage);
