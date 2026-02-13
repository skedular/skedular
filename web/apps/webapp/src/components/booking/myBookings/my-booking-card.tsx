import { CustomerAvatar } from '@/components/avatars';
import { LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { CalendarIcon, EllipseMenuIcon, LocationIcon, NotesIcon, PaymentStatusIcon, PdfIcon, TeamIcon } from '@/components/icons';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import Resources from '@/components/resource/resources';
import { Zones } from '@/components/zone';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import { dateRangeToShortDateWithAdditionalDayInfo, getCustomerFullName, joinErrors, toShortDate } from '@/libs/utils';
import type { myBookingCard_BookingDetails$key } from '@/queries/__generated__/myBookingCard_BookingDetails.graphql';
import type { myBookingCard_deletePrivateBookingMutation } from '@/queries/__generated__/myBookingCard_deletePrivateBookingMutation.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  bookingDetailsRelay: myBookingCard_BookingDetails$key;
  organizationUniqueAlphanumericName: string;
  connectionIds: string[];
  otherTeammates: CustomerDetails[];
};

type CustomTagDetails = {
  id: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type CustomerDetails = {
  id: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

const MyBookingCard = ({ bookingDetailsRelay, organizationUniqueAlphanumericName, otherTeammates, connectionIds }: Props) => {
  const bookingDetails = useFragment(
    graphql`
      fragment myBookingCard_BookingDetails on BookingDetails {
        id
        from
        until
        notes
        involvedCustomers {
          id
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        involvedLocations {
          id
          name
        }
        involvedTeams {
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
        isPaymentRequired
        paymentStatus {
          type
          name
        }
        invoiceUrl
      }
    `,
    bookingDetailsRelay,
  );

  const [commitDeletePrivateBooking] = useMutation<myBookingCard_deletePrivateBookingMutation>(graphql`
    mutation myBookingCard_deletePrivateBookingMutation($connectionIds: [ID!]!, $input: DeletePrivateBookingInput!) {
      deletePrivateBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const shortDateFormatFrom = toShortDate(bookingDetails.from);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditBooking],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteBooking],
  ];

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditBooking:
        if (bookingDetails) {
          router.push(getOrganizationBookingBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, bookingDetails.id));
        }

        break;

      case MoreActionsMenuOptionType.DeleteBooking:
        handleRemoveBookingClick();
        break;
    }
  };

  const handleRemoveBookingClick = () => {
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Removing booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitDeletePrivateBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const customTags = bookingDetails.bookingResources
    .flatMap(({ resource }) => resource.customTags)
    .reduce((acc: CustomTagDetails[], customTag) => {
      if (!acc.some((item) => item.id === customTag.id)) {
        acc.push(customTag);
      }

      return acc;
    }, []);
  const zones = bookingDetails.bookingResources
    .flatMap(({ resource }) => resource.zones)
    .reduce((acc: ZoneDetails[], zone) => {
      if (!acc.some((item) => item.id === zone.id)) {
        acc.push(zone);
      }

      return acc;
    }, []);

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 380 } }}>
        <CardHeader
          title={
            <Link component={NextLink} href={getOrganizationBookingBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, bookingDetails.id)}>
              {bookingDetails.involvedLocations.map((item) => (
                <LeadIconTypography key={item.id} startElement={<LocationIcon />} label={item?.name} sx={{ flexWrap: undefined }} invertDefaultColor />
              ))}
            </Link>
          }
          action={
            <>
              {moreActionsOption.length > 0 && (
                <Box color={paletteMode === 'dark' ? coal : sandstone} sx={{ paddingTop: 0.5 }}>
                  <IconButton onClick={handleMoreActionsMenuClick} color="inherit">
                    <EllipseMenuIcon />
                  </IconButton>
                </Box>
              )}
            </>
          }
        />
        <CardContent>
          {bookingDetails.isPaymentRequired && (
            <>
              <SmallIconTypography startElement={<PaymentStatusIcon />} label={bookingDetails.paymentStatus.name} sx={{ paddingTop: 1, paddingBottom: 1 }} />
              {bookingDetails.invoiceUrl && (
                <Link component={NextLink} href={bookingDetails.invoiceUrl} target="_blank" rel="noopener noreferrer">
                  <SmallIconTypography label="Download Invoice" startElement={<PdfIcon />} />
                </Link>
              )}
              <Divider />
            </>
          )}
          <SmallIconTypography
            startElement={<CalendarIcon />}
            label={dateRangeToShortDateWithAdditionalDayInfo(dayjs(bookingDetails.from), dayjs(bookingDetails.until))}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          {bookingDetails.involvedTeams.length === 0 && <SmallIconTypography startElement={<TeamIcon />} label="N/A" sx={{ paddingTop: 1, paddingBottom: 1 }} />}
          {bookingDetails.involvedTeams.length > 0 &&
            bookingDetails.involvedTeams.map((item) => (
              <SmallIconTypography key={item.id} startElement={<TeamIcon />} label={item ? item.name : 'N/A'} sx={{ paddingTop: 1, paddingBottom: 1 }} />
            ))}
          <Divider />
          <Resources
            resources={bookingDetails.bookingResources.map((item) => ({ id: item.resource.id, name: item.resource.name, color: item.resource.color }))}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          <CustomTags
            customTags={customTags.map((customTag: CustomTagDetails) => ({ id: customTag.id, name: customTag.name, color: customTag.color }))}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          <Zones zones={zones.map((zone: ZoneDetails) => ({ id: zone.id, name: zone.name, color: zone.color }))} sx={{ paddingTop: 1, paddingBottom: 1 }} />
          <Divider />
          <SmallIconTypography startElement={<NotesIcon />} label={bookingDetails.notes ? bookingDetails.notes : 'N/A'} sx={{ paddingTop: 1, paddingBottom: 1 }} />
          <Divider />
          <StackColumn sx={{ paddingTop: 1, paddingBottom: 1 }}>
            <SmallIconTypography label="Other teammates coming" />
            <StackRow>
              <AvatarGroup max={5}>
                {otherTeammates.map((item) => (
                  <CustomerAvatar key={item.id} name={item} photo={{ url: item.photoUrl }} size="medium" showFullName />
                ))}
              </AvatarGroup>
            </StackRow>
          </StackColumn>
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(MyBookingCard);
