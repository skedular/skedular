import { CustomerAvatar } from '@/components/avatars';
import { CustomTags } from '@/components/customTag';
import { DeskIcon, OtherResourceIcon, ParkingIcon, RoomIcon } from '@/components/icons';
import { getOrganizationLocationResourceBaseLink } from '@/components/links';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { Zones } from '@/components/zone';
import type { BookingCategory, resourceCard_addPrivateBookingMutation } from '@/queries/__generated__/resourceCard_addPrivateBookingMutation.graphql';
import type { resourceCard_ResourceDetails$key } from '@/queries/__generated__/resourceCard_ResourceDetails.graphql';
import type { resourceCard_query$key } from '@/queries/__generated__/resourceCard_query.graphql';
import Autocomplete from '@mui/material/Autocomplete';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Link from '@mui/material/Link';
import TextField from '@mui/material/TextField';
import { alpha } from '@mui/material/styles';
import { PaletteModeContext, getCustomerFullName, getRelayErrorMessage, useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { Dayjs } from 'dayjs';
import NextLink from 'next/link';
import { memo, useContext, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  rootDataRelay: resourceCard_query$key;
  resourceDetailsRelay: resourceCard_ResourceDetails$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  locationId: string;
  date: Dayjs;
  connectionIds: string[];
};

type CustomerDetails = {
  id: string;
  name: string | null | undefined;
  givenName: string | null | undefined;
  middleName: string | null | undefined;
  familyName: string | null | undefined;
  photoUrl: string | null | undefined;
};

type OrganizationMemberDetails = {
  id: string;
  customer: CustomerDetails;
};

const privateBookingCategory: BookingCategory = 'WORKING_FROM_OFFICE';

const ResourceCard = ({ rootDataRelay, resourceDetailsRelay, onReloadRequired, organizationCustomDomain, locationId, date, connectionIds }: Props) => {
  const rootData = useFragment<resourceCard_query$key>(
    graphql`
      fragment resourceCard_query on Query {
        me {
          id
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        deskResourceType
        roomResourceType
        parkingResourceType
        organization(customDomain: $organizationCustomDomain) {
          members(where: { nameContains: $peopleNameSearchText }, orderBy: $organizationMembersSortingValues) {
            edges {
              node {
                id
                customer {
                  id
                  name
                  givenName
                  middleName
                  familyName
                  photoUrl
                }
              }
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const resourceDetails = useFragment(
    graphql`
      fragment resourceCard_ResourceDetails on ResourceDetails {
        id
        name
        inactive
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
          type
        }
      }
    `,
    resourceDetailsRelay,
  );

  const [commitAddPrivateBooking, isBookingInFlight] = useMutation<resourceCard_addPrivateBookingMutation>(graphql`
    mutation resourceCard_addPrivateBookingMutation($connectionIds: [ID!]!, $input: AddPrivateBookingInput!) @raw_response_type {
      addPrivateBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          until
          notes
          channel {
            channel
            name
          }
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
          }
          involvedLocations {
            uniqueId
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
          marketplaceBooking {
            id
            isPaymentRequired
            paymentStatus {
              type
              name
            }
            invoiceUrl
            refund {
              id
              currency {
                type
                name
              }
              status {
                type
                name
              }
              requestedAt
              lastProcessedAt
              refundAmount
              refundPercentage
              currencyToDisplay
              reason
              lastError
              externalRefundNumber
              requestedByCustomerName
              canProcessInXero
              xeroProcessingBlockedReason
            }
          }
          recurringBooking {
            id
            startDate
            endDate
            frequency {
              name
            }
            marketplaceBooking {
              id
            }
          }
        }
        quotaError {
          errorCode
          reasonCode {
            type
            name
          }
        }
      }
    }
  `);

  const { integratedPlatform } = useIntegratedPlatform();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const members = useMemo(() => rootData.organization?.members.edges.map(({ node }) => node) ?? [], [rootData.organization?.members.edges]);
  const fallbackMember = useMemo<OrganizationMemberDetails>(
    () => ({
      id: rootData.me.id,
      customer: rootData.me,
    }),
    [rootData.me],
  );
  const [selectedCustomerId, setSelectedCustomerId] = useState(rootData.me.id);
  const selectedMember = members.find((member) => member.customer.id === selectedCustomerId) ?? fallbackMember;
  const resourceColor = resourceDetails.color || resourceDetails.resourceType.color || '#2563eb';
  const ResourceIcon =
    resourceDetails.resourceType.type === rootData.deskResourceType
      ? DeskIcon
      : resourceDetails.resourceType.type === rootData.roomResourceType
        ? RoomIcon
        : resourceDetails.resourceType.type === rootData.parkingResourceType
          ? ParkingIcon
          : OtherResourceIcon;

  const handleBookNowClick = () => {
    const id = uuid();
    const from = date.utc().toISOString();
    const until = date.utc().add(1, 'day').toISOString();
    const fullOpeningHoursDate = date.format('YYYY-MM-DD');

    commitAddPrivateBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          from,
          until,
          fullOpeningHoursDate,
          category: privateBookingCategory,
          customerIds: [selectedCustomerId],
          organizationCustomDomains: [organizationCustomDomain],
          resourceIds: [resourceDetails.id],
          teamIds: [],
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to make a booking for ${resourceDetails.name}. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);

          return;
        }

        if (response.addPrivateBooking.quotaError) {
          const reason = response.addPrivateBooking.quotaError.reasonCode?.name ?? 'Booking quota was reached.';
          themedToast(<NotificationContent content={`Failed to make a booking for ${resourceDetails.name}. ${reason}.`} />, errorNotificationOptions);

          return;
        }

        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to make a booking for ${resourceDetails.name}. Error: ${getRelayErrorMessage(error)}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addPrivateBooking: {
          booking: {
            id,
            from,
            until,
            notes: null,
            channel: { channel: 'PRIVATE', name: '' },
            category: {
              category: privateBookingCategory,
              name: '',
            },
            involvedCustomers: [selectedMember.customer],
            involvedOrganizations: [],
            involvedLocations: [{ uniqueId: locationId, name: '' }],
            involvedTeams: [],
            bookingResources: [
              {
                resource: {
                  id: resourceDetails.id,
                  name: resourceDetails.name,
                  color: resourceDetails.color,
                  customTags: resourceDetails.customTags.map(({ id, name, color }) => ({ id, name, color })),
                  zones: resourceDetails.zones.map(({ id, name, color }) => ({ id, name, color })),
                },
              },
            ],
            marketplaceBooking: null,
            recurringBooking: null,
          },
          quotaError: null,
        },
      },
    });
  };

  return (
    <Card
      elevation={8}
      sx={{
        width: { xs: 'min(100vw - 32px, 360px)', sm: 360 },
        borderRadius: '8px',
        overflow: 'hidden',
        border: 1,
        borderColor: 'divider',
        backgroundImage: 'none',
      }}
    >
      <Box
        sx={(theme) => ({
          p: 2,
          background: `linear-gradient(135deg, ${alpha(resourceColor, paletteMode === 'dark' ? 0.3 : 0.16)}, ${alpha(theme.palette.background.paper, 0.96)})`,
          borderBottom: 1,
          borderColor: 'divider',
        })}
      >
        <StackRow spacing={1.25} sx={{ alignItems: 'flex-start', flexWrap: 'nowrap' }}>
          <Box
            sx={(theme) => ({
              width: 48,
              height: 48,
              borderRadius: '8px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              color: resourceColor,
              backgroundColor: alpha(resourceColor, paletteMode === 'dark' ? 0.2 : 0.12),
              border: `1px solid ${alpha(resourceColor, paletteMode === 'dark' ? 0.36 : 0.22)}`,
              flexShrink: 0,
              '& svg': {
                fontSize: 28,
                color: 'inherit',
              },
              boxShadow: paletteMode === 'dark' ? `0 0 0 1px ${alpha(theme.palette.common.white, 0.04)}` : `0 8px 20px ${alpha(resourceColor, 0.12)}`,
            })}
          >
            <ResourceIcon />
          </Box>

          <StackColumn spacing={0.35} sx={{ minWidth: 0, flex: 1 }}>
            <Link
              component={NextLink}
              href={getOrganizationLocationResourceBaseLink(integratedPlatform, organizationCustomDomain, locationId, resourceDetails.id)}
              underline="hover"
              sx={{ color: 'text.primary', minWidth: 0 }}
            >
              <LeadIconTypography label={resourceDetails.name} noWrap fontWeight={700} />
            </Link>
            <CaptionIconTypography label={resourceDetails.resourceType.name} color="text.secondary" />
          </StackColumn>
        </StackRow>
      </Box>

      <CardContent sx={{ p: 2, '&:last-child': { pb: 2 } }}>
        <StackColumn spacing={1.5}>
          <Box
            sx={(theme) => ({
              width: '100%',
              borderRadius: '8px',
              p: 1.25,
              backgroundColor: alpha(theme.palette.text.primary, paletteMode === 'dark' ? 0.08 : 0.04),
              border: 1,
              borderColor: 'divider',
            })}
          >
            <CaptionIconTypography label="Capacity" color="text.secondary" />
            <SmallIconTypography label={`${resourceDetails.capacity}`} fontWeight={700} />
          </Box>

          <Autocomplete
            value={selectedMember}
            options={members}
            getOptionLabel={(option) => getCustomerFullName(option.customer)}
            isOptionEqualToValue={(option, value) => option.customer.id === value.customer.id}
            onChange={(_, option) => setSelectedCustomerId(option?.customer.id ?? rootData.me.id)}
            renderInput={(params) => <TextField {...params} label="Book for" size="small" />}
            renderOption={(props, option) => (
              <li {...props} key={option.customer.id}>
                <BodyIconTypography
                  label={getCustomerFullName(option.customer)}
                  startElement={<CustomerAvatar name={option.customer} photo={{ url: option.customer.photoUrl }} size="small" />}
                />
              </li>
            )}
          />

          <StackColumn spacing={0.75}>
            <SmallIconTypography label="Tags" color="text.secondary" fontWeight={700} />
            <CustomTags customTags={resourceDetails.customTags.map((item) => ({ id: item.id, name: item.name, color: item.color }))} />
          </StackColumn>

          <StackColumn spacing={0.75}>
            <SmallIconTypography label="Zones" color="text.secondary" fontWeight={700} />
            <Zones zones={resourceDetails.zones.map((item) => ({ id: item.id, name: item.name, color: item.color }))} />
          </StackColumn>

          <Button
            variant="contained"
            fullWidth
            disabled={isBookingInFlight || !selectedCustomerId}
            onClick={handleBookNowClick}
            sx={(theme) => ({
              mt: 0.5,
              py: 1.1,
              borderRadius: '8px',
              textTransform: 'none',
              backgroundColor: theme.palette.primary.main,
              borderColor: theme.palette.primary.main,
              boxShadow: 'none',
              '&:hover': {
                backgroundColor: theme.palette.primary.dark,
                borderColor: theme.palette.primary.dark,
                boxShadow: 'none',
              },
            })}
          >
            <BodyIconTypography label={isBookingInFlight ? 'Booking...' : 'Book now'} invertDefaultColor={paletteMode !== 'dark'} />
          </Button>
        </StackColumn>
      </CardContent>
    </Card>
  );
};

export default memo(ResourceCard);
