import { graphql } from 'react-relay';

export const locationCardFragment = graphql`
  fragment LocationCard_location on LocationDetails {
    id
    name
    organization {
      id
      type {
        type
        name
      }
    }
  }
`;
