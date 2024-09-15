import Calendar from '@mui/icons-material/CalendarToday';
import CorporateFare from '@mui/icons-material/CorporateFare';
import Desk from '@mui/icons-material/Desk';
import Groups from '@mui/icons-material/Groups';
import HomeWork from '@mui/icons-material/HomeWork';
import Info from '@mui/icons-material/Info';
import Link from '@mui/icons-material/Link';
import LocalOffer from '@mui/icons-material/LocalOffer';
import Notes from '@mui/icons-material/Notes';
import NotificationsActive from '@mui/icons-material/NotificationsActive';
import Refresh from '@mui/icons-material/Refresh';
import Settings from '@mui/icons-material/Settings';
import WbIridescent from '@mui/icons-material/WbIridescent';
import TooltipIcon from './tooltip-icon';

export { default as AddIcon, default as NewIcon } from '@mui/icons-material/Add';
export { default as JoinIcon } from '@mui/icons-material/AddCircle';
export { default as AscDirectionIcon } from '@mui/icons-material/ArrowDownward';
export { default as DescDirectionIcon } from '@mui/icons-material/ArrowUpward';
export { default as CancelIcon } from '@mui/icons-material/Cancel';
export { default as CheckIcon } from '@mui/icons-material/Check';
export { default as CloseIcon } from '@mui/icons-material/Close';
export { default as CollapseIcon } from '@mui/icons-material/CloseFullscreen';
export { default as DangerIcon } from '@mui/icons-material/Dangerous';
export { default as DarkModeIcon } from '@mui/icons-material/DarkMode';
export { default as DeleteIcon } from '@mui/icons-material/Delete';
export { default as DoneIcon } from '@mui/icons-material/Done';
export { default as EditIcon } from '@mui/icons-material/Edit';
export { default as FeedbackIcon } from '@mui/icons-material/Feedback';
export { default as LogoutIcon } from '@mui/icons-material/Logout';
export { default as MenuIcon } from '@mui/icons-material/Menu';
export { default as MoreItemsIcon } from '@mui/icons-material/MoreHoriz';
export { default as ExpandIcon } from '@mui/icons-material/OpenInFull';
export { default as PaymentIcon } from '@mui/icons-material/Payment';
export { default as CustomerIcon } from '@mui/icons-material/Person';
export { default as RemoveIcon } from '@mui/icons-material/Remove';
export { default as ViewIcon } from '@mui/icons-material/Visibility';

type Props = {
  fontSize?: 'inherit' | 'large' | 'medium' | 'small';
  excludeTooltip?: boolean;
};

export const OrganizationIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <CorporateFare fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Organization">
      <CorporateFare fontSize={fontSize} />
    </TooltipIcon>
  );

export const LocationIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <HomeWork fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Location">
      <HomeWork fontSize={fontSize} />
    </TooltipIcon>
  );

export const TeamIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <Groups fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Team">
      <Groups fontSize={fontSize} />
    </TooltipIcon>
  );

export const ZoneIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <LocalOffer fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Zone">
      <LocalOffer fontSize={fontSize} />
    </TooltipIcon>
  );

export const DeskIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <Desk fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Desk">
      <Desk fontSize={fontSize} />
    </TooltipIcon>
  );

export const NotesIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <Notes fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Notes">
      <Notes fontSize={fontSize} />
    </TooltipIcon>
  );

export const WebsiteIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <Link fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Website">
      <Link fontSize={fontSize} />
    </TooltipIcon>
  );

export const SettingsIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <Settings fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Settings">
      <Settings fontSize={fontSize} />
    </TooltipIcon>
  );

export const RefreshIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <Refresh fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Refresh">
      <Refresh fontSize={fontSize} />
    </TooltipIcon>
  );

export const NotificationsIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <NotificationsActive fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Notification">
      <NotificationsActive fontSize={fontSize} />
    </TooltipIcon>
  );

export const AboutIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <WbIridescent fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="About">
      <WbIridescent fontSize={fontSize} />
    </TooltipIcon>
  );

export const InfoIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <Info fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Info">
      <Info fontSize={fontSize} />
    </TooltipIcon>
  );

export const CalendarIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <Calendar fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Calendar">
      <Calendar fontSize={fontSize} />
    </TooltipIcon>
  );

export const BookingIcon = ({ fontSize, excludeTooltip }: Props) =>
  excludeTooltip ? (
    <Calendar fontSize={fontSize} />
  ) : (
    <TooltipIcon tip="Booking">
      <Calendar fontSize={fontSize} />
    </TooltipIcon>
  );
