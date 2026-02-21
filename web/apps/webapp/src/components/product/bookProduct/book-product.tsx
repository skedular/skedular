import { LocationAvatar } from '@/components/avatars';
import { SingleChoiceBookingPaymentMethodType, SingleChoiceMarketplaceBookingCategory } from '@/components/booking';
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
import { getOrganizationBookingBaseLink } from '@/components/links';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DefaultSelect } from '@/components/styled';
import { MultipleChoicesUserEmails } from '@/components/user';
import { Zones } from '@/components/zone';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getCustomerFullName, isMidnight, joinErrors, startOfDay, toOpeningHoursFromTime, toShortDate } from '@/libs/utils';
import type { BookingCategory, bookProduct_addMarketplaceBookingMutation, PaymentMethod } from '@/queries/__generated__/bookProduct_addMarketplaceBookingMutation.graphql';
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
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { array, mixed, number, object, string } from 'yup';

type Props = {
  rootDataRelay: bookProduct_query$key;
  rootDataAvailableResourcesRelay: bookProduct_availableResources_query$key;
  onReloadRequired?: () => void;
  connectionIds: string[];
  organizationUniqueAlphanumericName: string;
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
  id: string;
  name: string;
};

type ResourceDetails = {
  id: string;
  name: string;
  location: LocationDetails | null;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
};

type BookingDetails = {
  date: Dayjs;
  notes: string;
  quantity: number;
  resources: string[];
  category: string;
  paymentMethod: string;
  invoiceEmailList: string[];
};

type DateRangeValidationResult = {
  valid: boolean;
  from: Dayjs;
  until: Dayjs;
  errorMessage: string;
};

const bookingSchema = (numberOfResourcesToBook: number) =>
  object({
    date: mixed<Dayjs>()
      .test('is-dayjs', 'Date must be a valid Dayjs object', (value) => {
        return value != null && dayjs.isDayjs(value);
      })
      .required('Date is required'),
    quantity: number().min(1, 'At least one resource is required').required('Quantity is required'),
    resources: array().test('less-equal-number-of-resources-to-book', `You have selected more resources than allowed for this product.`, function (value) {
      if (!value) {
        return true;
      }
      const { quantity } = this.parent;
      if (!quantity) {
        return true;
      }

      return value?.length <= numberOfResourcesToBook * quantity;
    }),
    notes: string().notRequired(),
    category: string().required('Category is required'),
    paymentMethod: string().required('Payment method is required'),
    invoiceEmailList: array().notRequired(),
  });

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const BookProduct = ({ rootDataRelay, rootDataAvailableResourcesRelay, connectionIds, organizationUniqueAlphanumericName, defaultDate }: Props) => {
  const rootData = useFragment<bookProduct_query$key>(
    graphql`
      fragment bookProduct_query on Query {
        me {
          id
          emails
        }
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
          taxDetails {
            taxId
            taxRatePercentage
          }
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
          acceptedBookingPaymentMethods {
            type
          }
          isPriceTaxInclusive
        }
        openingHoursMinutesStep
        ...singleChoiceMarketplaceBookingCategory_query
        ...singleChoiceBookingPaymentMethodType_query
        ...multipleChoicesUserEmails_query
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
        availableResources(
          where: {
            organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName
            productId: $productId
            from: $dateFromToGetAvailableResources
            until: $dateUntilToGetAvailableResources
          }
        ) {
          location {
            id
            name
          }
          resource {
            id
            name
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
          }
        }
      }
    `,
    rootDataAvailableResourcesRelay,
  );

  const [commitAddMarketplaceBooking] = useMutation<bookProduct_addMarketplaceBookingMutation>(graphql`
    mutation bookProduct_addMarketplaceBookingMutation($connectionIds: [ID!]!, $input: AddMarketplaceBookingInput!) @raw_response_type {
      addMarketplaceBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          notes
          until
          category {
            category
            name
          }
          involvedCustomers {
            id
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          involvedOrganizations {
            id
            name
          }
          bookingResources {
            resource {
              id
              name
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
            }
          }
          marketplaceBooking {
            paymentMethod {
              type
              name
            }
            invoiceEmailList
          }
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [, startTransition] = useTransition();
  const [date, setDate] = useState<Dayjs>(defaultDate ?? startOfDay());
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

  const [quantity, setQuantity] = useState(1);
  const filterResource = createFilterOptions<ResourceDetails>();
  const [selectedLocationId, setSelectedLocationId] = useState<string>(allId);
  const [selectedCustomTagId, setSelectedCustomTagId] = useState<string>(allId);
  const [selectedZoneId, setSelectedZoneId] = useState<string>(allId);
  const [notes, setNotes] = useState<string>('');
  const [category, setCategory] = useState<string>('WORKING_FROM_COWORKING_SPACE');
  const [paymentMethod, setPaymentMethod] = useState<string>('');
  const [invoiceEmailList, setInvoiceEmailList] = useState<string[]>([]);

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
    [startTransition, refetchAvailableResources],
  );

  const getDateRange = useCallback(
    (date: Dayjs | Date, { timeFrom, timeUntil }: { timeFrom: Dayjs | null; timeUntil: Dayjs | null }): DateRangeValidationResult => {
      const product = rootData.product;
      const allDayFrom = dayjs(date).utc();
      const allDayUntil = dayjs(date).utc().add(1, 'day');
      const invalidResult = (errorMessage: string): DateRangeValidationResult => ({
        valid: false,
        from: allDayFrom,
        until: allDayUntil,
        errorMessage,
      });

      if (!product) {
        return invalidResult('');
      }

      if (!timeFrom || !timeUntil) {
        return invalidResult('Time required.');
      }

      if (isMidnight(timeFrom) && isMidnight(timeUntil)) {
        if (product.maxDurationMinutes && allDayUntil.diff(allDayFrom, 'minutes') > product.maxDurationMinutes) {
          return invalidResult(`You can only book resources for a maximum duration of ${product.maxDurationMinutes} minutes for this product.`);
        }

        return { valid: true, from: allDayFrom, until: allDayUntil, errorMessage: '' };
      }

      const utcDate = dayjs(date).utc();
      const from = utcDate.set('hour', timeFrom.get('hour')).set('minute', timeFrom.get('minute'));
      const until = utcDate.set('hour', timeUntil.get('hour')).set('minute', timeUntil.get('minute'));

      if (!from.isValid() || !until.isValid() || from.isAfter(until)) {
        return invalidResult('Time values are incorrect.');
      }

      const durationInMinutes = until.diff(from, 'minutes');
      if (product.minDurationMinutes && durationInMinutes < product.minDurationMinutes) {
        return invalidResult(`You can only book resources for a minimum duration of ${product.minDurationMinutes} minutes for this product.`);
      }

      if (product.maxDurationMinutes && durationInMinutes > product.maxDurationMinutes) {
        return invalidResult(`You can only book resources for a maximum duration of ${product.maxDurationMinutes} minutes for this product.`);
      }

      if (rootData.product?.priceUnit.type === 'PER_HOUR' && durationInMinutes % 60 !== 0) {
        return invalidResult('You can only book resources for a duration that is a multiple of hours for this product.');
      }

      return {
        valid: true,
        from,
        until,
        errorMessage: '',
      };
    },
    [rootData.product],
  );

  const dateRangeValidation = useMemo(() => {
    const [timeFrom, timeUntil] = timeRange;

    return getDateRange(date, { timeFrom, timeUntil });
  }, [date, timeRange, getDateRange]);

  const { valid: timeRangeValid, errorMessage: dateTimeErrorMessage } = dateRangeValidation;

  useEffect(() => {
    if (!dateRangeValidation.valid) {
      return;
    }

    handleRefetchAvailableResources(dateRangeValidation);
  }, [dateRangeValidation, handleRefetchAvailableResources]);

  const resources = useMemo<ResourceDetails[]>(
    () =>
      timeRangeValid
        ? rootDataAvailableResources.availableResources.map(({ resource: { id, name, customTags, zones }, location }) => ({
            id,
            name,
            location: location ? { id: location.id, name: location.name } : null,
            customTags: customTags.map(({ id, name, color }) => ({ id, name, color })),
            zones: zones.map(({ id, name, color }) => ({ id, name, color })),
          }))
        : [],
    [rootDataAvailableResources.availableResources, timeRangeValid],
  );

  const locations = useMemo<LocationDetails[]>(
    () => Array.from(new Map<string, LocationDetails>(resources.filter((item) => item.location !== null).map((item) => [item.location!.id, item.location!])).values()),
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
      filtered = filtered.filter((item) => item.location?.id === selectedLocationId);
    }

    if (selectedCustomTagId !== allId) {
      filtered = filtered.filter((item) => item.customTags.some((tag) => tag.id === selectedCustomTagId));
    }

    if (selectedZoneId !== allId) {
      filtered = filtered.filter((item) => item.zones.some((zone) => zone.id === selectedZoneId));
    }

    return filtered;
  }, [resources, selectedLocationId, selectedCustomTagId, selectedZoneId]);

  const [resourceIds, setResourceIds] = useState<string[]>(filteredResources.slice(0, (rootData.product?.numberOfResourcesToBook ?? 1) * quantity).map((resource) => resource.id));

  const { totalAmountExcludeTax, totalAmount, taxAmount } = useMemo(() => {
    const product = rootData.product;
    if (!product || quantity < 1) {
      return {
        totalAmountExcludeTax: '',
        totalAmount: 'N/A',
        taxAmount: '',
      };
    }

    const start = date as unknown as Dayjs;
    const [timeFrom, timeUntil] = timeRange;
    const dateRange = getDateRange(start, { timeFrom, timeUntil });
    if (!dateRange.valid) {
      return {
        totalAmountExcludeTax: '',
        totalAmount: 'N/A',
        taxAmount: '',
      };
    }

    const totalMinutes = dateRange.until.diff(dateRange.from, 'minutes');
    let totalPrice = 0.0;
    switch (product.priceUnit.type) {
      case 'PER_MINUTE':
        totalPrice = parseFloat(product.price) * quantity * totalMinutes;
        break;

      case 'PER_HOUR':
        totalPrice = (parseFloat(product.price) / 60) * quantity * totalMinutes;
        break;

      case 'PER_USE':
        totalPrice = parseFloat(product.price) * quantity;
        break;
    }

    const taxRatePercentageStr = rootData.organization?.taxDetails?.taxRatePercentage;
    if (!taxRatePercentageStr) {
      return {
        totalAmountExcludeTax: '',
        totalAmount: `${product.currencyToDisplay}${totalPrice.toFixed(2)}`,
        taxAmount: '',
      };
    }

    const taxRatePercentage = parseFloat(taxRatePercentageStr);
    const taxToPay = (totalPrice * taxRatePercentage) / 100;

    if (product.isPriceTaxInclusive) {
      const totalAmountExcludeTax = (totalPrice * 100) / (100 + taxRatePercentage);

      return {
        totalAmount: `${product.currencyToDisplay}${totalPrice.toFixed(2)}`,
        totalAmountExcludeTax: `${product.currencyToDisplay}${totalAmountExcludeTax.toFixed(2)}`,
        taxAmount: `${product.currencyToDisplay}${(totalPrice - totalAmountExcludeTax).toFixed(2)}`,
      };
    } else {
      return {
        totalAmountExcludeTax: `${product.currencyToDisplay}${totalPrice.toFixed(2)}`,
        taxAmount: `${product.currencyToDisplay}${((totalPrice * taxRatePercentage) / 100).toFixed(2)}`,
        totalAmount: `${product.currencyToDisplay}${(totalPrice + taxToPay).toFixed(2)}`,
      };
    }
  }, [getDateRange, rootData.product, quantity, date, timeRange, rootData.organization?.taxDetails?.taxRatePercentage]);

  const handleCloseClick = () => {
    router.back();
  };

  const handleAddClick = ({ date, notes, quantity, resources: resourceIds, category, paymentMethod, invoiceEmailList }: BookingDetails) => {
    const id = uuid();
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

    commitAddMarketplaceBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          customerIds: [customerId],
          from,
          until,
          notes,
          organizationUniqueAlphanumericNames: [organizationUniqueAlphanumericName],
          teamIds: [],
          resourceIds,
          category: category as BookingCategory,
          lineItems: [{ productVersionId: product.latestProductVersionId, quantity: Number(quantity) }],
          paymentMethod: paymentMethod as PaymentMethod,
          invoiceEmailList,
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

        const booking = response.addMarketplaceBooking?.booking;

        let message = `Booking made for ${getCustomerFullName(booking.involvedCustomers[0])} to work`;

        if (booking.bookingResources.length > 0) {
          message += ` at resource "${booking.bookingResources.map(({ resource }) => resource.name).join(', ')}"`;

          const zones = booking.bookingResources.flatMap(({ resource }) => resource.zones);
          if (zones.length > 0) {
            const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.id, zone), new Map()).values());

            message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
          }
        }

        message += ` on ${toShortDate(booking.from)}.`;

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={message} />,
        });

        router.push(getOrganizationBookingBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, response.addMarketplaceBooking!.booking.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to make a booking '${fromToPrint}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addMarketplaceBooking: {
          booking: {
            id,
            from,
            until,
            notes,
            category: {
              category: category as BookingCategory,
              name: '',
            },
            involvedCustomers: [
              {
                id: rootData.me.id,
                name: '',
                givenName: '',
                middleName: '',
                familyName: '',
                photoUrl: '',
              },
            ],
            involvedOrganizations: [],
            bookingResources: [],
            marketplaceBooking: {
              id: uuid(),
              paymentMethod: {
                type: paymentMethod as PaymentMethod,
                name: '',
              },
              invoiceEmailList,
            },
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
    return null;
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
              category,
              paymentMethod,
              notes,
              invoiceEmailList,
            }}
            validate={validate}
            render={({ handleSubmit, values }) => {
              setDate(values!.date);
              setResourceIds(values!.resources);
              setQuantity(values!.quantity);
              setCategory(values!.category);
              setPaymentMethod(values!.paymentMethod);
              setNotes(values!.notes);
              setInvoiceEmailList(values!.invoiceEmailList);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn
                    sx={{
                      paddingLeft: defaultPadding,
                      paddingRight: defaultPadding,
                      paddingTop: defaultPadding,
                    }}
                  >
                    <SectionIconTypography label="Book Product" />
                    <BodyIconTypography label="Enter your booking details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn
                    sx={{
                      paddingLeft: defaultPadding,
                      paddingRight: defaultPadding,
                      paddingTop: defaultPadding,
                    }}
                  >
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

                    <FormFieldLabel label="Notes">
                      <TextField name="notes" required={requiredFields.notes} helperText="e.g. I will be half an hour late this morning" multiline rows={2} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Quantity">
                      <TextField name="quantity" required={requiredFields.quantity} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Category">
                      <SingleChoiceMarketplaceBookingCategory rootDataRelay={rootData} name="category" required={requiredFields.category} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Filters">
                      <StackColumn>
                        <DefaultSelect
                          value={selectedLocationId}
                          onChange={handleLocationChanged}
                          size="small"
                          renderValue={(selectedId) => {
                            const selectedItem = locations.find((item) => item.id === selectedId);
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
                            <MenuItem key={item.id} value={item.id}>
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
                          getOptionValue={(option) => (option as ResourceDetails).id}
                          getOptionLabel={(option: string | ResourceDetails) => (option as ResourceDetails).name}
                          renderOption={(props, option) => {
                            const castedOption = option as ResourceDetails;

                            return (
                              <li {...props} key={castedOption.id}>
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

                    <FormFieldLabel label="Payment Method">
                      <SingleChoiceBookingPaymentMethodType
                        rootDataRelay={rootData}
                        name="paymentMethod"
                        required={requiredFields.paymentMethod}
                        acceptedBookingPaymentMethods={rootData.product?.acceptedBookingPaymentMethods.map(({ type }) => type)}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Email Invoice To">
                      <MultipleChoicesUserEmails rootDataRelay={rootData} name="invoiceEmailList" required={requiredFields.invoiceEmailList} />
                    </FormFieldLabel>

                    {taxAmount && (
                      <>
                        <FormFieldLabel label={`Total Exclude GST/VAT`}>
                          <BodyIconTypography label={totalAmountExcludeTax} />
                        </FormFieldLabel>

                        <FormFieldLabel label={`Total GST/VAT ${rootData.organization?.taxDetails?.taxRatePercentage}%`}>
                          <BodyIconTypography label={taxAmount} />
                        </FormFieldLabel>
                      </>
                    )}

                    <FormFieldLabel label="Total Amount">
                      <BodyIconTypography label={totalAmount} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Book & Pay
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
