import Link from '@mui/material/Link';
import graphql from 'babel-plugin-relay/macro';
import { Checkboxes } from 'mui-rff';
import { memo } from 'react';
import { useFragment } from 'react-relay';
import type { organizationTermsOfUse_query$key } from './__generated__/organizationTermsOfUse_query.graphql';

type Props = {
  rootDataRelay: organizationTermsOfUse_query$key;
  name: string;
  required?: boolean;
};

const OrganizationTermsOfUse = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationTermsOfUse_query on Query {
        activeOrganizationTermsOfUse {
          id
          terms
        }
      }
    `,
    rootDataRelay,
  );

  return (
    <>
      <Checkboxes
        name={name}
        required={required}
        data={{
          label: rootData.activeOrganizationTermsOfUse.terms,
          value: true,
        }}
      />
      <Link href="https://unityhub.io/terms-of-service" target="_blank" rel="noopener noreferrer">
        UnityHub Terms of Use.
      </Link>
    </>
  );
};

export default memo(OrganizationTermsOfUse);
