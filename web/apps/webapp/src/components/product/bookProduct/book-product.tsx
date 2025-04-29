import { LocationAvatar } from '@/components/avatars';
import { SingleChoiceMarketplaceBookingType } from '@/components/booking';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  ErrorTypography,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { CustomTagIcon, LocationIcon, ZoneIcon } from '@/components/icons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DefaultSelect } from '@/components/styled';
import { Zones } from '@/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getCustomerFullName, isMidnight, joinErrors, startOfDay, toOpeningHoursFromTime, toShortDate } from '@/libs/utils';
import type { BookingType, bookProduct_addBookingMutation } from '@/queries/__generated__/bookProduct_addBookingMutation.graphql';
import type { bookProduct_availableResources_query$key } from '@/queries/__generated__/bookProduct_availableResources_query.graphql';
import type { bookProduct_availableResources_refetchableFragment } from '@/queries/__generated__/bookProduct_availableResources_refetchableFragment.graphql';
import type { bookProduct_query$key } from '@/queries/__generated__/bookProduct_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import dayjs, { Dayjs } from 'dayjs';
import { Autocomplete, DatePicker, makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { array, date, number, object, string } from 'yup';

type Props = {
  rootDataRelay: bookProduct_query$key;
  rootDataAvailableResourcesRelay: bookProduct_availableResources_query$key;
  onReloadRequired?: () => void;
  connectionIds: string[];
  organizationId: string;
  defaultDate?: Dayjs;
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

type LocationDetails = {
  uniqueId: string;
  name: string;
};

type ResourceDetails = {
  uniqueId: string;
  name: string;
  location: LocationDetails | null;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
};

type BookingDetails = {
  date: Date;
  quantity: number;
  resources: string[];
  type: string;
};

const bookingSchema = (numberOfResourcesToBook: number) =>
  object({
    date: date().required('Date is required'),
    quantity: number().min(1, 'At least one resource is required').required('Quantity is required'),
    resources: array()
      .min(1, 'At least one resource is required')
      .required('Resource is required')
      .test(
        'less-equal-number-of-resources-to-book',
        `You can book only up to ${numberOfResourcesToBook} resources for this product`,
        (value) => value?.length <= numberOfResourcesToBook,
      ),
    type: string().required('Type is required'),
  });

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const BookProduct = ({ rootDataRelay, rootDataAvailableResourcesRelay, connectionIds, organizationId, defaultDate }: Props) => {
  const rootData = useFragment<bookProduct_query$key>(
    graphql`
      fragment bookProduct_query on Query {
        me {
          id
        }
        product(id: $productId) {
          id
          name
          description
          price
          priceUnit {
            type
            name
          }
          currencyToDisplay
          currency {
            type
            name
          }
          numberOfResourcesToBook
          minDurationMinutes
          maxDurationMinutes
          bookAllLocationResources
          recurrenceWindowDays
          requireConsecutiveDays
          maxBookingSpreadDays
          latestProductVersionId
        }
        openingHoursMinutesStep
        ...singleChoiceMarketplaceBookingType_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataAvailableResources, refetchAvailableResources] = useRefetchableFragment<
    bookProduct_availableResources_refetchableFragment,
    bookProduct_availableResources_query$key
  >(
    graphql`
      fragment bookProduct_availableResources_query on Query @refetchable(queryName: "bookProduct_availableResources_refetchableFragment") {
        availableResources(where: { organizationId: $organizationId, productId: $productId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources }) {
          uniqueId
          name
          location {
            uniqueId
            name
          }
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
    `,
    rootDataAvailableResourcesRelay,
  );

  const [commitAddBooking] = useMutation<bookProduct_addBookingMutation>(graphql`
    mutation bookProduct_addBookingMutation($connectionIds: [ID!]!, $input: AddBookingInput!) @raw_response_type {
      addBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          until
          type {
            type
            name
          }
          involvedCustomers {
            uniqueId
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          involvedOrganizations {
            uniqueId
            name
          }
          resources {
            uniqueId
            name
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
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const [, startTransition] = useTransition();
  const [date, setDate] = useState<Dayjs | Date>(defaultDate ?? startOfDay());
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>(() => {
    const start = toOpeningHoursFromTime('08:00');

    if (rootData.product?.minDurationMinutes) {
      return [start, start!.add(rootData.product?.minDurationMinutes, 'minutes')];
    } else if (rootData.product?.maxDurationMinutes) {
      return [start, start!.add(rootData.product?.maxDurationMinutes, 'minutes')];
    } else {
      return [start, toOpeningHoursFromTime('17:00')];
    }
  });

  const [timeRangeValid, setTimeRangeValid] = useState<boolean>(true);
  const [quantity, setQuantity] = useState(1);
  const filterResource = createFilterOptions<ResourceDetails>();
  const [selectedLocationId, setSelectedLocationId] = useState<string>(allId);
  const [selectedCustomTagId, setSelectedCustomTagId] = useState<string>(allId);
  const [selectedZoneId, setSelectedZoneId] = useState<string>(allId);
  const [dateTimeErrorMessage, setDateTimeErrorMessage] = useState('');
  const [bookingType, setBookingType] = useState<string>('WorkingFromCoworkingSpace');

  const resources = useMemo<ResourceDetails[]>(
    () =>
      timeRangeValid
        ? rootDataAvailableResources.availableResources.map(({ uniqueId, name, location, customTags, zones }) => ({
            uniqueId,
            name,
            location: location ? { uniqueId: location.uniqueId, name: location.name } : null,
            customTags: customTags.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
            zones: zones.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
          }))
        : [],
    [rootDataAvailableResources.availableResources, timeRangeValid],
  );

  const locations = useMemo<LocationDetails[]>(
    () => Array.from(new Map<string, LocationDetails>(resources.filter((item) => item.location !== null).map((item) => [item.location!.uniqueId, item.location!])).values()),
    [resources],
  );
  const customTags = useMemo<CustomTagDetails[]>(
    () => Array.from(new Map<string, CustomTagDetails>(resources.flatMap((item) => item.customTags).map((item) => [item.id, item])).values()),
    [resources],
  );
  const zones = useMemo<ZoneDetails[]>(
    () => Array.from(new Map<string, ZoneDetails>(resources.flatMap((item) => item.zones).map((item) => [item.id, item])).values()),
    [resources],
  );

  const filteredResources = useMemo<ResourceDetails[]>(() => {
    let filtered = resources;

    if (selectedLocationId !== allId) {
      filtered = filtered.filter((item) => item.location?.uniqueId === selectedLocationId);
    }

    if (selectedCustomTagId !== allId) {
      filtered = filtered.filter((item) => item.customTags.some((tag) => tag.id === selectedCustomTagId));
    }

    if (selectedZoneId !== allId) {
      filtered = filtered.filter((item) => item.zones.some((zone) => zone.id === selectedZoneId));
    }

    return filtered;
  }, [resources, selectedLocationId, selectedCustomTagId, selectedZoneId]);

  const [resourceIds, setResourceIds] = useState<string[]>(
    filteredResources.slice(0, (rootData.product?.numberOfResourcesToBook ?? 1) * quantity).map((resource) => resource.uniqueId),
  );
  const validate = makeValidate(bookingSchema(rootData.product?.numberOfResourcesToBook ?? 1));
  const requiredFields = makeRequired(bookingSchema(rootData.product?.numberOfResourcesToBook ?? 1));

  const handleRefetchAvailableResources = useCallback(
    ({ from, until }: { from: Dayjs | Date; until: Dayjs | Date }) => {
      startTransition(() => {
        refetchAvailableResources(
          {
            dateFromToGetAvailableResources: dayjs(from).utc().toISOString(),
            dateUntilToGetAvailableResources: dayjs(until).utc().toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchAvailableResources],
  );

  const getDateRange = useCallback(
    (date: Dayjs | Date, { timeFrom, timeUntil }: { timeFrom: Dayjs | null; timeUntil: Dayjs | null }) => {
      const product = rootData.product;
      const allDayFrom = dayjs(date).utc();
      const allDayUntil = dayjs(date).utc().add(1, 'day');
      const invalidResult = { valid: false, from: allDayFrom, until: allDayUntil };

      if (!product) {
        return invalidResult;
      }

      if (!timeFrom || !timeUntil) {
        setDateTimeErrorMessage('Time required.');

        return invalidResult;
      }

      if (isMidnight(timeFrom) && isMidnight(timeUntil)) {
        if (product.maxDurationMinutes && allDayUntil.diff(allDayFrom, 'minutes') > product.maxDurationMinutes) {
          setDateTimeErrorMessage(`You can only book resources for a maximum duration of ${product.maxDurationMinutes} minutes for this product.`);

          return invalidResult;
        }

        setDateTimeErrorMessage('');

        return { valid: true, from: allDayFrom, until: allDayUntil };
      }

      const utcDate = dayjs(date).utc();
      const from = utcDate.set('hour', timeFrom.get('hour')).set('minute', timeFrom.get('minute'));
      const until = utcDate.set('hour', timeUntil.get('hour')).set('minute', timeUntil.get('minute'));

      if (from.isAfter(until)) {
        setDateTimeErrorMessage('Time values are incorrect.');

        return invalidResult;
      }

      const durationInMinutes = until.diff(from, 'minutes');
      if (product.minDurationMinutes && durationInMinutes < product.minDurationMinutes) {
        setDateTimeErrorMessage(`You can only book resources for a minimum duration of ${product.minDurationMinutes} minutes for this product.`);

        return invalidResult;
      }

      if (product.maxDurationMinutes && durationInMinutes > product.maxDurationMinutes) {
        setDateTimeErrorMessage(`You can only book resources for a maximum duration of ${product.maxDurationMinutes} minutes for this product.`);

        return invalidResult;
      }

      setDateTimeErrorMessage('');

      if (rootData.product?.priceUnit.type === 'PerHour' && durationInMinutes % 60 !== 0) {
        setDateTimeErrorMessage('You can only book resources for a duration that is a multiple of hours for this product.');

        return invalidResult;
      }

      return {
        valid: true,
        from,
        until,
      };
    },
    [rootData.product],
  );

  useEffect(() => {
    const [timeFrom, timeUntil] = timeRange;
    const range = getDateRange(date, { timeFrom, timeUntil });
    if (range.valid) {
      setTimeRangeValid(true);
      handleRefetchAvailableResources(range);
    } else {
      setTimeRangeValid(false);
    }
  }, [handleRefetchAvailableResources, date, timeRange, getDateRange]);

  const totalPrice = useMemo(() => {
    const product = rootData.product;
    if (!product || quantity < 1) {
      return 'N/A';
    }

    const start = date as unknown as Dayjs;
    const [timeFrom, timeUntil] = timeRange;
    const dateRange = getDateRange(start, { timeFrom, timeUntil });
    if (!dateRange.valid) {
      return 'N/A';
    }

    const totalMinutes = dateRange.until.diff(dateRange.from, 'minutes');
    let price = 0.0;
    switch (product.priceUnit.type) {
      case 'PerMinute':
        price = parseFloat(product.price) * quantity * totalMinutes;
        break;

      case 'PerHour':
        price = (parseFloat(product.price) / 60) * quantity * totalMinutes;
        break;

      case 'PerUse':
        price = parseFloat(product.price) * quantity;
        break;
    }

    return `${product.currencyToDisplay}${price.toFixed(2)}`;
  }, [getDateRange, rootData.product, quantity, date, timeRange]);

  const handleCloseClick = () => {
    router.back();
  };

  const handleAddClick = ({ date, quantity, resources: resourceIds, type }: BookingDetails) => {
    if (!rootData.me) {
      return;
    }

    const id = nanoid();
    const start = date as unknown as Dayjs;
    const [timeFrom, timeUntil] = timeRange;
    const dateRange = getDateRange(start, { timeFrom, timeUntil });
    if (!dateRange.valid) {
      return;
    }

    const from = dateRange.from.toISOString();
    const until = dateRange.until.toISOString();
    const fromToPrint = toShortDate(dateRange.from);
    const customerId = rootData.me?.id;
    const toastId = themedToast(<NotificationContent content={`Making a booking on '${fromToPrint}'...`} />, infoNotificationOptions);

    commitAddBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          customerIds: [customerId],
          from,
          until,
          organizationIds: [organizationId],
          teamIds: [],
          resourceIds,
          type: type as BookingType,
          lineItems: [{ productVersionId: product.latestProductVersionId, quantity }],
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to make a booking '${fromToPrint}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        const booking = response.addBooking?.booking!;
        let message = `Booking made for ${getCustomerFullName(booking.involvedCustomers[0])} to work`;

        if (booking.resources.length > 0) {
          message += ` at resource "${booking.resources.map(({ name }) => name).join(', ')}"`;

          const zones = booking.resources.flatMap(({ zones }) => zones);
          if (zones.length > 0) {
            const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

            message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
          }
        }

        message += ` on ${toShortDate(booking.from)}.`;

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={message} />,
        });

        UpdateGlobalReloadId();
        router.back();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to make a booking '${fromToPrint}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addBooking: {
          booking: {
            id,
            from,
            until,
            type: {
              type: type as BookingType,
              name: '',
            },
            involvedCustomers: [
              {
                uniqueId: rootData.me.id,
                name: '',
                givenName: '',
                middleName: '',
                familyName: '',
                photoUrl: '',
              },
            ],
            involvedOrganizations: organizationId ? [{ uniqueId: organizationId, name: '' }] : [],
            resources: [],
          },
        },
      },
    });
  };

  const handleLocationChanged = (event: SelectChangeEvent<unknown>) => {
    const id = event.target.value as string;

    setSelectedLocationId(id);
  };

  const handleCustomTagChanged = (event: SelectChangeEvent<unknown>) => {
    const id = event.target.value as string;

    setSelectedCustomTagId(id);
  };

  const handleZoneChanged = (event: SelectChangeEvent<unknown>) => {
    const id = event.target.value as string;

    setSelectedZoneId(id);
  };

  if (!rootData.product) {
    return <></>;
  }

  const product = rootData.product;

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Book Product">
          <Form
            onSubmit={handleAddClick}
            initialValues={{
              date,
              resources: resourceIds,
              quantity,
              type: bookingType,
            }}
            validate={validate}
            render={({ handleSubmit, values }) => {
              setDate(values.date);
              setResourceIds(values.resources);
              setQuantity(values.quantity);
              setBookingType(values.type);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <SectionIconTypography label="Book Product" />
                    <BodyIconTypography label="Enter your booking details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Date/Time">
                      <StackColumn>
                        <StackRow>
                          <Box sx={{ width: 'fit-content' }}>
                            <DatePicker name="date" required={requiredFields.date} />
                          </Box>
                          <TimeRangePicker minutesStep={rootData.openingHoursMinutesStep} defaultValue={timeRange} onChange={setTimeRange} />
                        </StackRow>
                      </StackColumn>
                    </FormFieldLabel>

                    <FormFieldLabel>
                      <ErrorTypography errorMessage={dateTimeErrorMessage} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Quantity">
                      <TextField name="quantity" required={requiredFields.quantity} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Type">
                      <SingleChoiceMarketplaceBookingType rootDataRelay={rootData} name="type" required={requiredFields.type} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Filters">
                      <StackColumn>
                        <DefaultSelect
                          value={selectedLocationId}
                          onChange={handleLocationChanged}
                          size="small"
                          renderValue={(selectedId) => {
                            const selectedItem = locations.find((item) => item.uniqueId === selectedId);
                            if (selectedItem) {
                              return (
                                <StackRow>
                                  <LeadIconTypography label="Location" startElement={<LocationIcon />} />
                                  <Divider orientation="vertical" flexItem />
                                  <PushToRight />
                                  <SmallIconTypography label={selectedItem.name} />
                                </StackRow>
                              );
                            }

                            return (
                              <StackRow>
                                <LeadIconTypography label="Location" startElement={<LocationIcon />} />
                                <Divider orientation="vertical" flexItem />
                                <PushToRight />
                                <SmallIconTypography label="All" />
                              </StackRow>
                            );
                          }}
                        >
                          <MenuItem value={allId}>
                            <BodyIconTypography label="All" />
                          </MenuItem>

                          {locations.map((item) => (
                            <MenuItem key={item.uniqueId} value={item.uniqueId}>
                              <BodyIconTypography startElement={<LocationAvatar name={{ name: item.name }} size="small" />} label={item.name} />
                            </MenuItem>
                          ))}
                        </DefaultSelect>

                        <StackRow>
                          <DefaultSelect
                            value={selectedCustomTagId}
                            onChange={handleCustomTagChanged}
                            size="small"
                            renderValue={(selectedId) => {
                              const selectedItem = customTags.find((item) => item.id === selectedId);
                              if (selectedItem) {
                                return (
                                  <StackRow>
                                    <LeadIconTypography label="Tag" startElement={<CustomTagIcon />} />
                                    <Divider orientation="vertical" flexItem />
                                    <PushToRight />
                                    <SmallIconTypography label={selectedItem.name} />
                                  </StackRow>
                                );
                              }

                              return (
                                <StackRow>
                                  <LeadIconTypography label="Tag" startElement={<CustomTagIcon />} />
                                  <Divider orientation="vertical" flexItem />
                                  <PushToRight />
                                  <SmallIconTypography label="All" />
                                </StackRow>
                              );
                            }}
                          >
                            <MenuItem value={allId}>
                              <BodyIconTypography label="All" />
                            </MenuItem>

                            {customTags.map((item) => (
                              <MenuItem key={item.id} value={item.id}>
                                <BodyIconTypography startElement={<CustomTagIcon />} label={item.name} />
                              </MenuItem>
                            ))}
                          </DefaultSelect>

                          <DefaultSelect
                            value={selectedZoneId}
                            onChange={handleZoneChanged}
                            size="small"
                            renderValue={(selectedId) => {
                              const selectedItem = zones.find((item) => item.id === selectedId);
                              if (selectedItem) {
                                return (
                                  <StackRow>
                                    <LeadIconTypography label="Zone" startElement={<ZoneIcon />} />
                                    <Divider orientation="vertical" flexItem />
                                    <PushToRight />
                                    <SmallIconTypography label={selectedItem.name} />
                                  </StackRow>
                                );
                              }

                              return (
                                <StackRow>
                                  <LeadIconTypography label="Zone" startElement={<ZoneIcon />} />
                                  <Divider orientation="vertical" flexItem />
                                  <PushToRight />
                                  <SmallIconTypography label="All" />
                                </StackRow>
                              );
                            }}
                          >
                            <MenuItem value={allId}>
                              <BodyIconTypography label="All" />
                            </MenuItem>

                            {zones.map((item) => (
                              <MenuItem key={item.id} value={item.id}>
                                <BodyIconTypography startElement={<ZoneIcon />} label={item.name} />
                              </MenuItem>
                            ))}
                          </DefaultSelect>
                        </StackRow>
                      </StackColumn>
                    </FormFieldLabel>

                    <FormFieldLabel label="Resources">
                      {filteredResources.length > 0 && quantity > 0 && (
                        <BodyIconTypography label={`Feel free to choose up to ${product.numberOfResourcesToBook * quantity} resources from the list below!`} />
                      )}
                      {filteredResources.length > 0 && (
                        <Autocomplete
                          name="resources"
                          multiple={true}
                          required={requiredFields.resources}
                          options={filteredResources}
                          getOptionValue={(option) => (option as ResourceDetails).uniqueId}
                          getOptionLabel={(option: string | ResourceDetails) => (option as ResourceDetails).name}
                          renderOption={(props, option) => {
                            const castedOption = option as ResourceDetails;

                            return (
                              <li {...props} key={castedOption.uniqueId}>
                                <StackRow sx={{ alignItems: 'center' }}>
                                  <BodyIconTypography label={castedOption.name} />
                                  <CustomTags customTags={castedOption.customTags} hideNAText />
                                  <Zones zones={castedOption.zones} hideIcon hideNAText />
                                </StackRow>
                              </li>
                            );
                          }}
                          disableCloseOnSelect
                          filterOptions={(options, params) => filterResource(options as ResourceDetails[], params)}
                          selectOnFocus
                          clearOnBlur
                          handleHomeEndKeys
                        />
                      )}

                      {filteredResources.length === 0 && <BodyIconTypography label="There are currently no available resources." />}
                    </FormFieldLabel>

                    <FormFieldLabel label="Total Price">
                      <BodyIconTypography label={totalPrice} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Book
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              );
            }}
          />
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(BookProduct);
