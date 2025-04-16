import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { autoCloseErrorNotificationOptions, infoNotificationOptions, NotificationContent } from '@/components/notification';
import { Zones } from '@/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { isMidnight, startOfDay, toOpeningHoursFromTime, toShortDate } from '@/libs/utils';
import type { bookProduct_addBookingMutation } from '@/queries/__generated__/bookProduct_addBookingMutation.graphql';
import type { bookProduct_availableResources_query$key } from '@/queries/__generated__/bookProduct_availableResources_query.graphql';
import type { bookProduct_availableResources_refetchableFragment } from '@/queries/__generated__/bookProduct_availableResources_refetchableFragment.graphql';
import type { bookProduct_query$key } from '@/queries/__generated__/bookProduct_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { TimePicker } from '@mui/x-date-pickers/TimePicker';
import dayjs, { Dayjs } from 'dayjs';
import { Autocomplete, DatePicker, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { array, date, object } from 'yup';

type Props = {
  rootDataRelay: bookProduct_query$key;
  rootDataAvailableResourcesRelay: bookProduct_availableResources_query$key;
  onReloadRequired?: () => void;
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

type ResourceDetails = {
  uniqueId: string;
  name: string;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
};

type BookingDetails = {
  date: Date;
  resources: string[];
};

const bookingSchema = object({
  date: date().required('Date/Time is required'),
  resources: array().min(1, 'At least one resource is required').required('Resource is required'),
});

const BookProduct = ({ rootDataRelay, rootDataAvailableResourcesRelay, defaultDate }: Props) => {
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
        }
        openingHoursMinutesStep
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
          notes
          type
          customer {
            uniqueId
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          organization {
            uniqueId
            name
          }
          location {
            uniqueId
            name
          }
          team {
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
  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);
  const [from, setFrom] = useState<Dayjs | Date>(defaultDate ?? startOfDay());
  const [timeFrom, setTimeFrom] = useState<Dayjs | null>(toOpeningHoursFromTime('08:00'));
  const [timeUntil, setTimeUntil] = useState<Dayjs | null>(toOpeningHoursFromTime('17:00'));
  const [timeRangeValid, setTimeRangeValid] = useState<boolean>(true);
  const [resourceIds, setResourceIds] = useState<string[]>([]);
  const filterResource = createFilterOptions<ResourceDetails>();

  const resources = useMemo<ResourceDetails[]>(
    () =>
      timeRangeValid
        ? rootDataAvailableResources.availableResources.map(({ uniqueId, name, customTags, zones }) => ({
            uniqueId,
            name,
            customTags: customTags.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
            zones: zones.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
          }))
        : [],
    [rootDataAvailableResources.availableResources, timeRangeValid],
  );

  const handleRefetchAvailableResources = useCallback(
    ({ from, until }: { from: Dayjs | Date; until: Dayjs | Date }, locationId?: string) => {
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
      const allDayFrom = dayjs(date).utc();
      const allDayUntil = dayjs(date).utc().add(1, 'day');

      if (!timeFrom || !timeUntil) {
        themedToast(<NotificationContent content={`Time required when not booking full day.`} />, autoCloseErrorNotificationOptions);

        return { valid: false, from: allDayFrom, until: allDayUntil };
      }

      if (isMidnight(timeFrom) && isMidnight(timeUntil)) {
        return { valid: true, from: allDayFrom, until: allDayUntil };
      }

      const utcDate = dayjs(date).utc();
      const from = utcDate.set('hour', timeFrom.get('hour')).set('minute', timeFrom.get('minute'));
      const until = utcDate.set('hour', timeUntil.get('hour')).set('minute', timeUntil.get('minute'));

      if (from.isAfter(until)) {
        themedToast(<NotificationContent content={`Time values are incorrect.`} />, autoCloseErrorNotificationOptions);

        return { valid: false, from: allDayFrom, until: allDayUntil };
      }

      return {
        valid: true,
        from,
        until,
      };
    },
    [themedToast],
  );

  useEffect(() => {
    const range = getDateRange(from, { timeFrom, timeUntil });
    if (range.valid) {
      setTimeRangeValid(true);
      handleRefetchAvailableResources(range);
    } else {
      setTimeRangeValid(false);
    }
  }, [handleRefetchAvailableResources, from, timeFrom, timeUntil, getDateRange]);

  const handleCloseClick = () => {
    router.back();
  };

  const handleAddClick = ({ date }: BookingDetails) => {
    if (!rootData.me) {
      return;
    }

    const id = nanoid();
    const start = date as unknown as Dayjs;
    const dateRange = getDateRange(start, { timeFrom, timeUntil });
    if (!dateRange.valid) {
      return;
    }

    const from = dateRange.from.toISOString();
    const until = dateRange.until.toISOString();
    const fromToPrint = toShortDate(dateRange.from);
    const customerId = rootData.me?.id;
    const toastId = themedToast(<NotificationContent content={`Making a booking on '${fromToPrint}'...`} />, infoNotificationOptions);
    const type = 'WorkingFromOffice';
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
              date: from,
              resources: resourceIds,
            }}
            validate={validate}
            render={({ handleSubmit, values }) => {
              setFrom(values.date);
              setResourceIds(values.resources);

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
                        <DatePicker name="date" required={requiredFields.date} />

                        <StackRow>
                          <TimePicker minutesStep={rootData.openingHoursMinutesStep} defaultValue={timeFrom} onChange={setTimeFrom} />
                          <TimePicker minutesStep={rootData.openingHoursMinutesStep} defaultValue={timeUntil} onChange={setTimeUntil} />
                        </StackRow>
                      </StackColumn>
                    </FormFieldLabel>

                    <FormFieldLabel label="Resources">
                      {resources.length > 0 && (
                        <Autocomplete
                          name="resources"
                          multiple={true}
                          required={requiredFields.resources}
                          options={resources}
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
                          filterOptions={(options, params) => filterResource(options as ResourceDetails[], params)}
                          selectOnFocus
                          clearOnBlur
                          handleHomeEndKeys
                        />
                      )}

                      {resources.length === 0 && <BodyIconTypography label="There are currently no available resources." />}
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
