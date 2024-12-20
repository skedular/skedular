import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@repo/shared/components/commons';
import { TeamIcon } from '@repo/shared/components/icons';
import graphql from 'babel-plugin-relay/macro';
import { memo } from 'react';
import { useFragment } from 'react-relay';
import type { myTeamCard_TeamDetails$key } from './__generated__/myTeamCard_TeamDetails.graphql';

type Props = {
  teamDetailsRelay: myTeamCard_TeamDetails$key;
  connectionIds: string[];
  teammates: CustomerDetails[];
};

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

const MyTeamCard = ({ teamDetailsRelay, teammates }: Props) => {
  const teamDetails = useFragment(
    graphql`
      fragment myTeamCard_TeamDetails on TeamDetails {
        id
        name
        members {
          organizationMember {
            uniqueId
            customer {
              uniqueId
              givenName
              middleName
              familyName
              name
              photoUrl
            }
          }
        }
      }
    `,
    teamDetailsRelay,
  );

  return (
    <Card sx={{ width: 600 }}>
      <CardHeader
        title={<LeadIconTypography startElement={<TeamIcon />} label={teamDetails.name} sx={{ flexWrap: undefined }} invertDefaultColor />}
      />
      <CardContent>
        <StackColumn sx={{ paddingTop: 1, paddingBottom: 1 }}>
          <SmallIconTypography label="Members of this team" />
          <StackRow>
            <AvatarGroup max={5}>
              {teammates.map((item) => (
                <CustomerAvatar key={item.uniqueId} name={item} photo={{ url: item.photoUrl }} size="medium" showFullName />
              ))}
            </AvatarGroup>
          </StackRow>
        </StackColumn>
      </CardContent>
    </Card>
  );
};

export default memo(MyTeamCard);
