import Business from '@mui/icons-material/Business';
import CorporateFare from '@mui/icons-material/CorporateFare';
import DateRange from '@mui/icons-material/DateRange';
import Desk from '@mui/icons-material/Desk';
import Diversity3 from '@mui/icons-material/Diversity3';
import Fullscreen from '@mui/icons-material/Fullscreen';
import Home from '@mui/icons-material/Home';
import Info from '@mui/icons-material/Info';
import Link from '@mui/icons-material/Link';
import LocalOffer from '@mui/icons-material/LocalOffer';
import Notes from '@mui/icons-material/Notes';
import NotificationsActive from '@mui/icons-material/NotificationsActive';
import People from '@mui/icons-material/People';
import Place from '@mui/icons-material/Place';
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
export { default as DashboardIcon } from '@mui/icons-material/Dashboard';
export { default as DeleteIcon } from '@mui/icons-material/Delete';
export { default as DoneIcon } from '@mui/icons-material/Done';
export { default as EditIcon } from '@mui/icons-material/Edit';
export { default as FeedIcon } from '@mui/icons-material/Feed';
export { default as FeedbackIcon } from '@mui/icons-material/Feedback';
export { default as AnalyticsIcon } from '@mui/icons-material/Insights';
export { default as ArrowDownIcon } from '@mui/icons-material/KeyboardArrowDown';
export { default as ArrowLeftIcon } from '@mui/icons-material/KeyboardArrowLeft';
export { default as ArrowRightIcon } from '@mui/icons-material/KeyboardArrowRight';
export { default as ArrowUpIcon } from '@mui/icons-material/KeyboardArrowUp';
export { default as LogoutIcon } from '@mui/icons-material/Logout';
export { default as MenuIcon } from '@mui/icons-material/Menu';
export { default as MoreItemsIcon } from '@mui/icons-material/MoreHoriz';
export { default as EllipseMenuIcon } from '@mui/icons-material/MoreVert';
export { default as ExpandIcon } from '@mui/icons-material/OpenInFull';
export { default as BillingAndPaymentIcon } from '@mui/icons-material/Payment';
export { default as CustomerIcon } from '@mui/icons-material/Person';
export { default as RemoveIcon } from '@mui/icons-material/Remove';
export { default as PreferredIcon } from '@mui/icons-material/Star';
export { default as NotPreferredIcon } from '@mui/icons-material/StarOutline';
export { default as TodayIcon } from '@mui/icons-material/Today';
export { default as ViewIcon } from '@mui/icons-material/Visibility';

type Props = {
  fontSize?: 'inherit' | 'large' | 'medium' | 'small';
  color?: 'inherit' | 'action' | 'disabled' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
  excludeTooltip?: boolean;
  tip?: string;
};

export const MembersIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <People fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Members'}>
      <People fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const OrganizationIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <CorporateFare fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Organization'}>
      <CorporateFare fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const LocationIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Place fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Location'}>
      <Place fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const TeamIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Diversity3 fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Team'}>
      <Diversity3 fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const ZoneIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <LocalOffer fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Zone'}>
      <LocalOffer fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const DeskIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Desk fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Desk'}>
      <Desk fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const NotesIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Notes fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Notes'}>
      <Notes fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const WebsiteIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Link fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Website'}>
      <Link fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const SettingsIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Settings fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Settings'}>
      <Settings fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const RefreshIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Refresh fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Refresh'}>
      <Refresh fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const NotificationsIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <NotificationsActive fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Notification'}>
      <NotificationsActive fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const AboutIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <WbIridescent fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'About'}>
      <WbIridescent fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const InfoIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Info fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Info'}>
      <Info fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const CalendarIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <DateRange fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Calendar'}>
      <DateRange fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const BookingIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <DateRange fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Booking'}>
      <DateRange fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const WorkingFromOfficeIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Business fontSize={fontSize} />
  ) : (
    <TooltipIcon tip={tip ?? 'Working from office'}>
      <Business fontSize={fontSize} color={color ?? 'primary'} />
    </TooltipIcon>
  );

export const WorkingFromHomeIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Home fontSize={fontSize} />
  ) : (
    <TooltipIcon tip={tip ?? 'Working from home'}>
      <Home fontSize={fontSize} color={color ?? 'action'} />
    </TooltipIcon>
  );

export const ViewDetailsIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Fullscreen fontSize={fontSize} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'View details'}>
      <Fullscreen fontSize={fontSize} color={color} />
    </TooltipIcon>
  );

export const HomeIcon = ({ fontSize, excludeTooltip, tip, color }: Props) =>
  excludeTooltip ? (
    <Home fontSize={fontSize} />
  ) : (
    <TooltipIcon tip={tip ?? 'Home'}>
      <Home fontSize={fontSize} color={color ?? 'action'} />
    </TooltipIcon>
  );
