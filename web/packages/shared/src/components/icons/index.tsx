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

export { default as AddIcon } from '@mui/icons-material/Add';
export { default as JoinIcon } from '@mui/icons-material/AddCircle';
export { default as AscDirectionIcon } from '@mui/icons-material/ArrowDownward';
export { default as DescDirectionIcon } from '@mui/icons-material/ArrowUpward';
export { default as CalendarIcon } from '@mui/icons-material/CalendarToday';
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
export { default as MoreItemsIcon } from '@mui/icons-material/MoreHoriz';
export { default as ExpandIcon } from '@mui/icons-material/OpenInFull';
export { default as PaymentIcon } from '@mui/icons-material/Payment';
export { default as CustomerIcon } from '@mui/icons-material/Person';
export { default as RemoveIcon } from '@mui/icons-material/Remove';
export { default as ViewIcon } from '@mui/icons-material/Visibility';

type Props = {
  fontSize?: 'inherit' | 'large' | 'medium' | 'small';
};

export const OrganizationIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Organization">
      <CorporateFare fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const LocationIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Location">
      <HomeWork fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const TeamIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Team">
      <Groups fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const ZoneIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Zone">
      <LocalOffer fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const DeskIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Desk">
      <Desk fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const NotesIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Notes">
      <Notes fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const WebsiteIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Website">
      <Link fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const SettingsIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Settings">
      <Settings fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const RefreshIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Refresh">
      <Refresh fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const NotificationsIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Notification">
      <NotificationsActive fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const AboutIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="About">
      <WbIridescent fontSize={fontSize} />
    </TooltipIcon>
  );
};

export const InfoIcon = ({ fontSize }: Props) => {
  return (
    <TooltipIcon tip="Info">
      <Info fontSize={fontSize} />
    </TooltipIcon>
  );
};
