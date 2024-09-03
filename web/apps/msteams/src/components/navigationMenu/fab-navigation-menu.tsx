import NavigationIcon from '@mui/icons-material/Navigation';
import Box from '@mui/material/Box';
import SpeedDial from '@mui/material/SpeedDial';
import SpeedDialAction from '@mui/material/SpeedDialAction';
import { styled } from '@mui/material/styles';
import { BookingIcon, LocationIcon, NotificationsIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { memo } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const StyledSpeedDial = styled(SpeedDial)(({ theme }) => ({
  position: 'fixed',
  '&.MuiSpeedDial-directionUp, &.MuiSpeedDial-directionLeft': {
    bottom: theme.spacing(2),
    right: theme.spacing(2),
  },
  '&.MuiSpeedDial-directionDown, &.MuiSpeedDial-directionRight': {
    top: theme.spacing(2),
    left: theme.spacing(2),
  },
}));

const FabNavigationMenu = () => {
  const navigate = useNavigate();
  const { organizationId } = useParams();
  let finalOrganizationId = '';
  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  return (
    <Box sx={{ position: 'relative', mt: 3, height: 320 }}>
      <StyledSpeedDial ariaLabel="UnityHub" icon={<NavigationIcon />} direction="up">
        <SpeedDialAction
          icon={<BookingIcon excludeTooltip={true} />}
          tooltipTitle="Booking"
          onClick={() => navigate(`/organization/${finalOrganizationId}`)}
        />
        <SpeedDialAction
          icon={<LocationIcon excludeTooltip={true} />}
          tooltipTitle="Locations"
          onClick={() => navigate(`/organization/${finalOrganizationId}/location`)}
        />
        <SpeedDialAction
          icon={<TeamIcon excludeTooltip={true} />}
          tooltipTitle="Teams"
          onClick={() => navigate(`/organization/${finalOrganizationId}/team`)}
        />
        <SpeedDialAction
          icon={<NotificationsIcon excludeTooltip={true} />}
          tooltipTitle="Notifications"
          onClick={() => navigate(`/${finalOrganizationId}/notification`)}
        />
        <SpeedDialAction
          icon={<SettingsIcon excludeTooltip={true} />}
          tooltipTitle="Settings"
          onClick={() => navigate(`/${finalOrganizationId}/settings`)}
        />
      </StyledSpeedDial>
    </Box>
  );
};

export default memo(FabNavigationMenu);
