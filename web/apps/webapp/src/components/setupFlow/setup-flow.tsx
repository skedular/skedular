import { BodyIconTypography, StackRow } from '@skedular/ui';
import { PaletteModeContext } from '@skedular/shared';
import { defaultButtonStyle } from '@skedular/ui';
import BuildCircleIcon from '@mui/icons-material/BuildCircle';
import BusinessIcon from '@mui/icons-material/Business';
import CoffeeIcon from '@mui/icons-material/Coffee';
import CottageIcon from '@mui/icons-material/Cottage';
import DashboardCustomizeIcon from '@mui/icons-material/DashboardCustomize';
import GroupsIcon from '@mui/icons-material/Groups';
import MeetingRoomIcon from '@mui/icons-material/MeetingRoom';
import PersonIcon from '@mui/icons-material/Person';
import TouchAppIcon from '@mui/icons-material/TouchApp';
import VisibilityIcon from '@mui/icons-material/Visibility';
import Button from '@mui/material/Button';
import { SetupFeatureCard, SetupSplitLayout } from '@skedular/ui';
import { useRouter } from 'next/navigation';
import { memo, useContext } from 'react';
import type { UserType } from './user-type-card';
import UserTypeCard from './user-type-card';

type Props = {
  userTypesToShow?: UserType[];
  onUserTypeClick: (userType: UserType) => void;
  showBackButton?: boolean;
};

const SetupFlow = ({ userTypesToShow, onUserTypeClick, showBackButton }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  return (
    <SetupSplitLayout
      asideTitle="Welcome to Scheduler"
      asideDescription="Smart workspace management for businesses and co-working spaces of all sizes."
      asideChildren={
        <>
          <SetupFeatureCard
            icon={<MeetingRoomIcon sx={{ color: '#4CAF50', fontSize: 40 }} />}
            title="Easy desk and room booking"
            description="Reserve desks and rooms in seconds with a user-friendly booking interface."
          />
          <SetupFeatureCard
            icon={<GroupsIcon sx={{ color: '#2196F3', fontSize: 40 }} />}
            title="Real-time team visibility & collaboration"
            description="See who's in the office and collaborate better with live team presence updates."
          />
          <SetupFeatureCard
            icon={<DashboardCustomizeIcon sx={{ color: '#FF9800', fontSize: 40 }} />}
            title="Interactive tools for managers"
            description="Manage space usage and employee schedules with powerful, interactive dashboards."
          />
          <SetupFeatureCard
            icon={<TouchAppIcon sx={{ color: '#00BCD4', fontSize: 40 }} />}
            title="Frictionless booking experience"
            description="Book your workspace without delays, forms, or confusion with just a few clicks."
          />
          <SetupFeatureCard
            icon={<VisibilityIcon sx={{ color: '#9C27B0', fontSize: 40 }} />}
            title="Real-time visibility and controls"
            description="Instantly monitor workspace usage and make adjustments on the fly."
          />
          <SetupFeatureCard
            icon={<BuildCircleIcon sx={{ color: '#F44336', fontSize: 40 }} />}
            title="Powerful yet intuitive tools"
            description="Feature-rich tools that remain simple and intuitive for everyone to use."
          />
          <SetupFeatureCard
            icon={<CottageIcon sx={{ color: '#FF7043', fontSize: 40 }} />}
            title="Individual hosting"
            description="List a spare studio, loft, or backyard office, keep each listing unique, and still manage payouts under one host profile."
          />
        </>
      }
      mainDescription="To get started, tell us what type of user you are. This lets us tailor the product around the work you actually need to do."
    >
      {(!userTypesToShow || userTypesToShow.some((item) => item === 'private')) && (
        <UserTypeCard
          icon={<BusinessIcon sx={{ color: '#74d77eff', fontSize: 40 }} />}
          title="Enterprise organisation"
          subtitle="I represent a company that needs to manage multiple workspaces and resources"
          onClick={() => onUserTypeClick('private')}
        />
      )}

      {(!userTypesToShow || userTypesToShow.some((item) => item === 'marketplace')) && (
        <UserTypeCard
          icon={<CoffeeIcon sx={{ color: '#6F4E37', fontSize: 40 }} />}
          title="Co-working Space Provider"
          subtitle="I own or manage a co-working space and want to list it on the marketplace"
          onClick={() => onUserTypeClick('marketplace')}
        />
      )}

      {(!userTypesToShow || userTypesToShow.some((item) => item === 'individual-organization')) && (
        <UserTypeCard
          icon={<CottageIcon sx={{ color: '#FF7043', fontSize: 40 }} />}
          title="Individual Host"
          subtitle="I’m an individual who wants to list my own space, take bookings, and manage payouts"
          onClick={() => onUserTypeClick('individual-organization')}
        />
      )}

      {(!userTypesToShow || userTypesToShow.some((item) => item === 'individual-user')) && (
        <UserTypeCard
          icon={<PersonIcon sx={{ color: '#03A9F4', fontSize: 40 }} />}
          title="Individual User"
          subtitle="I'm looking for workspace solutions or want to join an organisation"
          onClick={() => onUserTypeClick('individual-user')}
        />
      )}
      {showBackButton && (
        <StackRow>
          <Button variant="contained" sx={defaultButtonStyle} onClick={handleBackClick}>
            <BodyIconTypography label="Back" invertDefaultColor={paletteMode === 'dark'} />
          </Button>
        </StackRow>
      )}
    </SetupSplitLayout>
  );
};

export default memo(SetupFlow);
