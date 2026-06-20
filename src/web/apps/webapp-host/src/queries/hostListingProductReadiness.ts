import { graphql } from 'react-relay';

export const hostListingProductReadySubscription = graphql`
  subscription hostListingProductReadySubscription($locationId: String!) {
    listingProductReady(locationId: $locationId) {
      locationId
      product {
        id
        inactive
      }
    }
  }
`;

export const hostListingProductReadinessQuery = graphql`
  query hostListingProductReadinessQuery($locationId: String!) {
    location(id: $locationId) {
      id
      products {
        id
        inactive
      }
    }
  }
`;

export const isLocationProductReady = (products: ReadonlyArray<{ id: string }> | null | undefined) => {
  return Boolean(products && products.length > 0);
};
