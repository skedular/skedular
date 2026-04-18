import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { Address, PhysicalAddress } from '@/components/address';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, HelperText, StackColumn, StackRow } from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { DeleteIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { MultipleChoicesLocationSpaceTypes, SingleChoiceLocationType } from '@/components/location';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle } from '@/libs/theme';
import { getRelayErrorMessage, keyboardTextFieldDebounceTimeout, stringToMultiLines } from '@/libs/utils';
import type { addMarketplaceLocation_addLocationMutation, LocationType } from '@/queries/__generated__/addMarketplaceLocation_addLocationMutation.graphql';
import type { addMarketplaceLocation_rootQuery } from '@/queries/__generated__/addMarketplaceLocation_rootQuery.graphql';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import GridViewIcon from '@mui/icons-material/GridView';
import LocalCafeIcon from '@mui/icons-material/LocalCafe';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import MeetingRoomIcon from '@mui/icons-material/MeetingRoom';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import IconButton from '@mui/material/IconButton';
import { EditorActionBar, SettingsSectionCard, SetupFeatureCard, SetupSplitLayout } from '@skedular/ui';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, object, string } from 'yup';

const RootQuery = graphql`
  query addMarketplaceLocation_rootQuery($organizationCustomDomain: String!) {
    emailsToShowLatestCapabilities
    me {
      emails
    }
    organization(customDomain: $organizationCustomDomain) {
      type {
        type
      }
    }
    ...singleChoiceLocationType_query
    ...multipleChoicesLocationSpaceTypes_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<addMarketplaceLocation_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  onAdded: (id: string) => void;
  onCancel: () => void;
  cancelLabel?: string;
  createLabel?: string;
};

type LocationDetails = {
  name: string;
  timezone: string;
  type: string;
  spaceTypeIds: string[];
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
  timezone: string().required('Timezone is required'),
  type: string().required('Type is required'),
  spaceTypeIds: array().nullable(),
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
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string(),
  city: string(),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  countryCode: string().required('Country is required'),
});

const AddMarketplaceLocation = ({ queryReference, onReloadRequired, organizationCustomDomain, onAdded, onCancel, cancelLabel, createLabel }: Props) => {
  const rootData = usePreloadedQuery<addMarketplaceLocation_rootQuery>(RootQuery, queryReference);

  const [commitAddLocation] = useMutation<addMarketplaceLocation_addLocationMutation>(graphql`
    mutation addMarketplaceLocation_addLocationMutation($input: AddLocationInput!) @raw_response_type {
      addLocation(input: $input) {
        location {
          id
          name
          listingMetadata {
            about
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateLocationDetails = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);

  const [locationName, setLocationName] = useState<string>('');
  const debounceSetLocationName = useDebounceCallback(setLocationName, keyboardTextFieldDebounceTimeout);
  const [locationTimezone, setLocationTimezone] = useState<string>('');
  const debounceSetLocationTimezone = useDebounceCallback(setLocationTimezone, keyboardTextFieldDebounceTimeout);
  const [locationType, setLocationType] = useState<string>('MARKETPLACE');
  const debounceSetLocationType = useDebounceCallback(setLocationType, keyboardTextFieldDebounceTimeout);

  const [spaceTypeIds, setSpaceTypeIds] = useState<string[]>([]);
  const debounceSetSpaceTypeIds = useDebounceCallback(setSpaceTypeIds, keyboardTextFieldDebounceTimeout);

  const [locationContactPerson, setLocationContactPerson] = useState<string | null>(null);
  const debounceSetLocationContactPerson = useDebounceCallback(setLocationContactPerson, keyboardTextFieldDebounceTimeout);
  const [locationContactEmail, setLocationContactEmail] = useState<string | null>(null);
  const debounceSetLocationContactEmail = useDebounceCallback(setLocationContactEmail, keyboardTextFieldDebounceTimeout);
  const [locationContactPhone, setLocationContactPhone] = useState<string | null>(null);
  const debounceSetLocationContactPhone = useDebounceCallback(setLocationContactPhone, keyboardTextFieldDebounceTimeout);

  const [locationAreaRangeFromSqm, setLocationAreaRangeFromSqm] = useState<string | null>(null);
  const debounceSetLocationAreaRangeFromSqm = useDebounceCallback(setLocationAreaRangeFromSqm, keyboardTextFieldDebounceTimeout);
  const [locationAreaRangeToSqm, setLocationAreaRangeToSqm] = useState<string | null>(null);
  const debounceSetLocationAreaRangeToSqm = useDebounceCallback(setLocationAreaRangeToSqm, keyboardTextFieldDebounceTimeout);

  const [locationPeopleCapacityFrom, setLocationPeopleCapacityFrom] = useState<string | null>(null);
  const debounceSetLocationPeopleCapacityFrom = useDebounceCallback(setLocationPeopleCapacityFrom, keyboardTextFieldDebounceTimeout);
  const [locationPeopleCapacityTo, setLocationPeopleCapacityTo] = useState<string | null>(null);
  const debounceSetLocationPeopleCapacityTo = useDebounceCallback(setLocationPeopleCapacityTo, keyboardTextFieldDebounceTimeout);

  const [locationWebsite, setLocationWebsite] = useState<string | null>(null);
  const debounceSetLocationWebsite = useDebounceCallback(setLocationWebsite, keyboardTextFieldDebounceTimeout);
  const [locationRelatedImageLinks, setLocationRelatedImageLinks] = useState<string | null>(null);
  const debounceSetLocationRelatedImageLinks = useDebounceCallback(setLocationRelatedImageLinks, keyboardTextFieldDebounceTimeout);
  const [locationRelatedVideoLinks, setLocationRelatedVideoLinks] = useState<string | null>(null);
  const debounceSetLocationRelatedVideoLinks = useDebounceCallback(setLocationRelatedVideoLinks, keyboardTextFieldDebounceTimeout);
  const [locationOtherLinks, setLocationOtherLinks] = useState<string | null>(null);
  const debounceSetLocationOtherLinks = useDebounceCallback(setLocationOtherLinks, keyboardTextFieldDebounceTimeout);

  const [physicalAddressOsmType, setPhysicalAddressOsmType] = useState<string | null | undefined>(null);
  const [physicalAddressOsmId, setPhysicalAddressOsmId] = useState<string | null | undefined>(null);
  const [physicalAddressPlaceId, setPhysicalAddressPlaceId] = useState<string | null | undefined>(null);
  const [physicalAddressLongitude, setPhysicalAddressLongitude] = useState<number | null | undefined>(null);
  const [physicalAddressLatitude, setPhysicalAddressLatitude] = useState<number | null | undefined>(null);
  const [physicalAddressFormattedAddress, setPhysicalAddressFormattedAddress] = useState<string | null | undefined>(null);
  const [physicalAddressAddressLine1, setPhysicalAddressAddressLine1] = useState<string>('');
  const debounceSetPhysicalAddressAddressLine1 = useDebounceCallback(setPhysicalAddressAddressLine1, keyboardTextFieldDebounceTimeout);
  const [physicalAddressAddressLine2, setPhysicalAddressAddressLine2] = useState<string | null | undefined>(null);
  const debounceSetPhysicalAddressAddressLine2 = useDebounceCallback(setPhysicalAddressAddressLine2, keyboardTextFieldDebounceTimeout);
  const [physicalAddressSuburb, setPhysicalAddressSuburb] = useState<string | null | undefined>(null);
  const debounceSetPhysicalAddressSuburb = useDebounceCallback(setPhysicalAddressSuburb, keyboardTextFieldDebounceTimeout);
  const [physicalAddressCity, setPhysicalAddressCity] = useState<string | null | undefined>(null);
  const debounceSetPhysicalAddressCity = useDebounceCallback(setPhysicalAddressCity, keyboardTextFieldDebounceTimeout);
  const [physicalAddressProvince, setPhysicalAddressProvince] = useState<string | null | undefined>(null);
  const debounceSetPhysicalAddressProvince = useDebounceCallback(setPhysicalAddressProvince, keyboardTextFieldDebounceTimeout);
  const [physicalAddressZipcode, setPhysicalAddressZipcode] = useState<string>('');
  const debounceSetPhysicalAddressZipcode = useDebounceCallback(setPhysicalAddressZipcode, keyboardTextFieldDebounceTimeout);
  const [physicalAddressCountry, setPhysicalAddressCountry] = useState<string>('');
  const [physicalAddressCountryCode, setPhysicalAddressCountryCode] = useState<string>('');
  const debounceSetPhysicalAddressCountryCode = useDebounceCallback(setPhysicalAddressCountryCode, keyboardTextFieldDebounceTimeout);

  const [featureImages, setFeatureImages] = useState<FileUploadResponse[]>([]);
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(null);

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
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

  const handleLocationAddClick = ({
    name,
    timezone,
    type,
    spaceTypeIds,
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
    addressLine1,
    addressLine2,
    suburb,
    city,
    province,
    zipcode,
    countryCode,
  }: LocationDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding location '${name}'...`} />, infoNotificationOptions);
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    const countryData = getCountryData(countryCode as TCountryCode);
    let country = physicalAddressCountry;
    if (countryData) {
      country = countryData.name;
    }

    commitAddLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          name,
          listingMetadata: {
            about: '',
            title: '',
            subTitle: '',
            includedFeatures: [],
          },
          organizationCustomDomain,
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
          tagIds: spaceTypeIds,
          physicalAddress: {
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
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't add location '${name}'. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${name}' has been added.`} />,
        });

        onAdded(id);
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't add location '${name}'. ${error.message}`} />,
        });
      },
      optimisticResponse: {
        addLocation: {
          location: {
            id,
            name,
            listingMetadata: {
              about: '',
              title: '',
              subTitle: '',
              includedFeatures: [],
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
            spaceTypes: [],
            physicalAddress: {
              id: '',
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

  return (
    <SetupSplitLayout
      asideTitle="Manage Your Co-Working Space Locations"
      asideDescription="Set up physical locations so members can find, book, and interact with your spaces across the network."
      asideChildren={
        <>
          <SetupFeatureCard
            icon={<MeetingRoomIcon sx={{ color: '#4CAF50', fontSize: 40 }} />}
            title="Flexible Room Booking"
            description="Enable members to reserve meeting rooms, hot desks, or private offices."
          />
          <SetupFeatureCard
            icon={<AccessTimeIcon sx={{ color: '#FF9800', fontSize: 40 }} />}
            title="Operating Hours"
            description="Set the daily open and close hours to control when members can access the space."
          />
          <SetupFeatureCard
            icon={<LocalCafeIcon sx={{ color: '#795548', fontSize: 40 }} />}
            title="Location Amenities"
            description="List available amenities like Wi-Fi, coffee, printers, and parking."
          />
          <SetupFeatureCard
            icon={<LocationOnIcon sx={{ color: '#F44336', fontSize: 40 }} />}
            title="Map & Directions"
            description="Add an address and map to help members find your space easily."
          />
          <SetupFeatureCard
            icon={<GridViewIcon sx={{ color: '#3F51B5', fontSize: 40 }} />}
            title="Multi-Zone Support"
            description="Create zones within a location for better desk and room segmentation."
          />
        </>
      }
      mainTitle="Create Your Co-Working Location"
      mainDescription="Add the commercial profile, contact details, and address customers need to discover and trust the space."
    >
      <Form
        onSubmit={handleLocationAddClick}
        initialValues={{
          name: locationName,
          timezone: locationTimezone,
          type: locationType,
          spaceTypeIds,

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

          addressLine1: physicalAddressAddressLine1,
          addressLine2: physicalAddressAddressLine2,
          suburb: physicalAddressSuburb,
          city: physicalAddressCity,
          province: physicalAddressProvince,
          zipcode: physicalAddressZipcode,
          countryCode: physicalAddressCountryCode,
        }}
        validate={validateLocationDetails}
        render={({ handleSubmit, values, form }) => {
          debounceSetLocationName(values!.name);
          debounceSetLocationTimezone(values!.timezone);
          debounceSetLocationType(values!.type);
          debounceSetSpaceTypeIds(values!.spaceTypeIds);

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

          debounceSetPhysicalAddressAddressLine1(values!.addressLine1);
          debounceSetPhysicalAddressAddressLine2(values!.addressLine2);
          debounceSetPhysicalAddressSuburb(values!.suburb);
          debounceSetPhysicalAddressCity(values!.city);
          debounceSetPhysicalAddressProvince(values!.province);
          debounceSetPhysicalAddressZipcode(values!.zipcode);
          debounceSetPhysicalAddressCountryCode(values!.countryCode);

          return (
            <FormStackColumn onSubmit={handleSubmit}>
              <Box sx={{ display: 'grid', gap: 3 }}>
                <SettingsSectionCard
                  title="Location Identity"
                  description="Start with the public name, timezone, and classification that customers and operators will both rely on."
                >
                  <StackColumn>
                    <FormFieldLabel label="Name" required={requiredFields.name}>
                      <TextField
                        name="name"
                        required={requiredFields.name}
                        helperText={
                          <HelperText text="Enter the public name of your co-working location. This will be visible in the marketplace and should clearly represent your space." />
                        }
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Timezone" required={requiredFields.timezone}>
                      <SingleChoinceTimezone
                        name="timezone"
                        required={requiredFields.timezone}
                        helperText="Select the local timezone of this location to ensure accurate scheduling and availability for bookings."
                      />
                    </FormFieldLabel>

                    {rootData.me.emails.some((item) => !!rootData.emailsToShowLatestCapabilities.find((email) => email.toLocaleLowerCase() === item.toLocaleLowerCase())) && (
                      <>
                        <FormFieldLabel label="Space Type">
                          <MultipleChoicesLocationSpaceTypes rootDataRelay={rootData} name="spaceTypeIds" required={requiredFields.spaceTypeIds} />
                        </FormFieldLabel>

                        <FormFieldLabel label="Type" required={requiredFields.type}>
                          <SingleChoiceLocationType rootDataRelay={rootData} name="type" required={requiredFields.type} />
                        </FormFieldLabel>
                      </>
                    )}
                  </StackColumn>
                </SettingsSectionCard>

                <SettingsSectionCard title="Feature Images" description="Set a cover image that helps this location stand out in marketplace discovery and admin lists.">
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

                      <ImageFileUploaderWithCropper
                        onUploadCompleted={handleFeatureImageUploadCompleted}
                        helperText="Upload a high-quality image that best represents your co-working space. This will appear in search results and marketing pages."
                      />
                    </StackColumn>
                  </FormFieldLabel>
                </SettingsSectionCard>

                {rootData.me.emails.some((item) => !!rootData.emailsToShowLatestCapabilities.find((email) => email.toLocaleLowerCase() === item.toLocaleLowerCase())) && (
                  <SettingsSectionCard
                    title="Marketplace Profile"
                    description="Define the commercial profile of the space so customers understand scale, capacity, and supporting links."
                  >
                    <StackColumn>
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
                    </StackColumn>
                  </SettingsSectionCard>
                )}

                <SettingsSectionCard title="Contact Details" description="Add the public contact points customers should use when they need help with this location.">
                  <StackColumn>
                    <FormFieldLabel label="Contact People" required={requiredFields.contactPeople}>
                      <TextField
                        name="contactPeople"
                        required={requiredFields.contactPeople}
                        multiline
                        rows={2}
                        helperText={
                          <HelperText text="Enter the name of the main contact person for this location. This helps visitors and members know who to reach out to for assistance or inquiries." />
                        }
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Emails" required={requiredFields.contactEmails}>
                      <TextField
                        name="contactEmails"
                        required={requiredFields.contactEmails}
                        multiline
                        rows={2}
                        helperText={<HelperText text="Enter a public contact email for this location so visitors and potential members can get in touch easily." />}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Phone Numbers" required={requiredFields.contactPhones}>
                      <TextField
                        name="contactPhones"
                        required={requiredFields.contactPhones}
                        multiline
                        rows={2}
                        helperText={<HelperText text="Provide a phone number where your co-working space can be reached for inquiries or support." />}
                      />
                    </FormFieldLabel>
                  </StackColumn>
                </SettingsSectionCard>

                <SettingsSectionCard title="Address" description="Use the physical address customers will navigate to and operators will manage for onsite access.">
                  <PhysicalAddress
                    addressLine1Name="addressLine1"
                    addressLine1Required={requiredFields.addressLine1}
                    addressLine2Name="addressLine2"
                    addressLine2Required={requiredFields.addressLine2}
                    suburbName="suburb"
                    suburbRequired={requiredFields.suburb}
                    cityName="city"
                    cityRequired={requiredFields.city}
                    provinceName="province"
                    provinceRequired={requiredFields.province}
                    zipcodeName="zipcode"
                    zipcodeRequired={requiredFields.zipcode}
                    countryName="countryCode"
                    countryRequired={requiredFields.countryCode}
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
                </SettingsSectionCard>

                <EditorActionBar
                  secondaryActions={
                    <Button variant="contained" sx={defaultButtonStyle} onClick={handleCloseClick}>
                      <BodyIconTypography label={cancelLabel ?? 'Cancel'} invertDefaultColor={paletteMode === 'dark'} />
                    </Button>
                  }
                  primaryAction={
                    <Button variant="contained" type="submit" sx={{ textTransform: 'none' }} color="primary">
                      <BodyIconTypography label={createLabel ?? 'Add'} invertDefaultColor={paletteMode === 'dark'} />
                    </Button>
                  }
                />
              </Box>
            </FormStackColumn>
          );
        }}
      />
    </SetupSplitLayout>
  );
};

const MemoAddMarketplaceLocation = memo(AddMarketplaceLocation);

type RelayProps = {
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  onAdded: (id: string) => void;
  onCancel: () => void;
  cancelLabel?: string;
  createLabel?: string;
};

const AddMarketplaceLocationWithRelay = ({ onReloadRequired, organizationCustomDomain, onAdded, onCancel, cancelLabel, createLabel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addMarketplaceLocation_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

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
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoAddMarketplaceLocation
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        onAdded={onAdded}
        onCancel={onCancel}
        cancelLabel={cancelLabel}
        createLabel={createLabel}
      />
    </ErrorBoundary>
  );
};

export default memo(AddMarketplaceLocationWithRelay);
