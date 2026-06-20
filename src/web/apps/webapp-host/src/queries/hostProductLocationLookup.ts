import { graphql } from 'react-relay';

export const hostProductLocationLookupQuery = graphql`
  query hostProductLocationLookupQuery($productId: String!) {
    product(id: $productId) {
      id
    }
  }
`;
