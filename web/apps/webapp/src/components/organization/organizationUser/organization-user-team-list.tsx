import { CustomerAvatar, TeamAvatar } from '@/components/avatars';
import { LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { getCustomerFullName } from '@skedular/shared';
import AvatarGroup from '@mui/material/AvatarGroup';
import Box from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import { memo } from 'react';

export type OrganizationUserTeamListItem = {
  id: string;
  name: string;
  featureImageUrl?: string | null;
  members: {
    id: string;
    givenName?: string | null;
    middleName?: string | null;
    familyName?: string | null;
    name?: string | null;
    photoUrl?: string | null;
  }[];
};

type Props = {
  items: OrganizationUserTeamListItem[];
};

const OrganizationUserTeamList = ({ items }: Props) => {
  if (items.length === 0) {
    return (
      <Box
        sx={{
          border: 1,
          borderStyle: 'dashed',
          borderColor: 'divider',
          borderRadius: 3,
          p: 3,
          backgroundColor: 'background.paper',
        }}
      >
        <LeadIconTypography label="No teams found" />
        <SmallIconTypography label="This user is not currently assigned to any teams in this organisation." />
      </Box>
    );
  }

  return (
    <StackColumn spacing={1.5}>
      {items.map((item) => (
        <Box
          key={item.id}
          sx={{
            border: 1,
            borderColor: 'divider',
            borderRadius: 2.5,
            px: 1.5,
            py: 1.25,
            backgroundColor: 'background.paper',
            boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 2px 10px rgba(15, 23, 42, 0.04)' : theme.shadows[1]),
          }}
        >
          <StackRow sx={{ alignItems: 'flex-start', gap: 1.5, minWidth: 0, flexWrap: { xs: 'wrap', md: 'nowrap' } }}>
            <TeamAvatar name={{ name: item.name }} photo={{ url: item.featureImageUrl }} size="medium" />

            <StackColumn sx={{ minWidth: 0, flex: '1 1 auto' }} spacing={0.75}>
              <StackRow sx={{ alignItems: 'center', gap: 1, minWidth: 0, flexWrap: 'wrap' }}>
                <Box sx={{ minWidth: 0, maxWidth: 360, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                  <LeadIconTypography label={item.name} />
                </Box>
                <Chip size="small" label={`${item.members.length} member${item.members.length === 1 ? '' : 's'}`} />
              </StackRow>

              <StackRow sx={{ alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
                {item.members.length === 0 && <SmallIconTypography label="No other members listed" />}
                {item.members.length > 0 && (
                  <>
                    <AvatarGroup
                      max={8}
                      sx={{
                        justifyContent: 'flex-end',
                        '& .MuiAvatar-root': {
                          width: 28,
                          height: 28,
                          fontSize: 12,
                          borderColor: 'background.paper',
                        },
                      }}
                    >
                      {item.members.map((member) => (
                        <CustomerAvatar
                          key={member.id}
                          name={member}
                          photo={{ url: member.photoUrl }}
                          size="small"
                          showFullName
                          tip={getCustomerFullName(member) || 'Unnamed member'}
                        />
                      ))}
                    </AvatarGroup>
                  </>
                )}
              </StackRow>
            </StackColumn>
          </StackRow>
        </Box>
      ))}
    </StackColumn>
  );
};

export default memo(OrganizationUserTeamList);
