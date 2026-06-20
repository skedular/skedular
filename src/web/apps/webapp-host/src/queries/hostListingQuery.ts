import { graphql } from 'react-relay';

export const hostListingLocationQuery = graphql`
  query hostListingQuery($locationId: String!) {
    location(id: $locationId) {
      id
      name
      timezone
      physicalAddress {
        multilinesFormattedAddress
      }
      extraMetadata {
        peopleCapacity {
          from
          to
        }
      }
      products {
        id
        inactive
        listingMetadata {
          title
          about
        }
        pricingOptions {
          id
          price
          bookingCadence
          billingMode
          cancellationPolicyType
          minDurationMinutes
          maxDurationMinutes
        }
        currency {
          name
          type
        }
      }
    }
  }
`;
