import type { SxProps, Theme } from '@mui/system';
import TooltipIcon from './tooltip-icon';

import AddCircle from '@mui/icons-material/AddCircle';
import Business from '@mui/icons-material/Business';
import Category from '@mui/icons-material/Category';
import CorporateFare from '@mui/icons-material/CorporateFare';
import DateRange from '@mui/icons-material/DateRange';
import Diversity3 from '@mui/icons-material/Diversity3';
import EventSeat from '@mui/icons-material/EventSeat';
import Fullscreen from '@mui/icons-material/Fullscreen';
import Home from '@mui/icons-material/Home';
import Info from '@mui/icons-material/Info';
import Link from '@mui/icons-material/Link';
import LocalOffer from '@mui/icons-material/LocalOffer';
import NotificationsActive from '@mui/icons-material/NotificationsActive';
import Person from '@mui/icons-material/Person';
import Place from '@mui/icons-material/Place';
import Refresh from '@mui/icons-material/Refresh';
import Settings from '@mui/icons-material/Settings';
import Sms from '@mui/icons-material/Sms';
import WbIridescent from '@mui/icons-material/WbIridescent';

export { default as BankAccountIcon, default as StripeConnectAccountIcon } from '@mui/icons-material/AccountBalanceWallet';
export { default as AddIcon, default as NewIcon } from '@mui/icons-material/AddCircle';
export { default as ContactEmailIcon } from '@mui/icons-material/AlternateEmail';
export { default as AnalyticsIcon } from '@mui/icons-material/Analytics';
export { default as CollpaseDrawerIcon } from '@mui/icons-material/ArrowBackIos';
export { default as AscDirectionIcon } from '@mui/icons-material/ArrowDownward';
export { default as ExpanDrawerIcon } from '@mui/icons-material/ArrowForwardIos';
export { default as DescDirectionIcon } from '@mui/icons-material/ArrowUpward';
export { default as PaymentStatusIcon, default as TaxDetailsIcon } from '@mui/icons-material/AttachMoney';
export { default as CancelIcon } from '@mui/icons-material/Cancel';
export { default as CheckIcon } from '@mui/icons-material/Check';
export { default as SelectedTickIcon } from '@mui/icons-material/CheckCircle';
export { default as CloseIcon } from '@mui/icons-material/Close';
export { default as CollapseIcon } from '@mui/icons-material/CloseFullscreen';
export { default as ContactPeopleIcon } from '@mui/icons-material/ConnectWithoutContact';
export { default as ClaimOwnership } from '@mui/icons-material/Copyright';
export { default as AreaIcon } from '@mui/icons-material/Crop';
export { default as DangerIcon } from '@mui/icons-material/Dangerous';
export { default as DarkModeIcon } from '@mui/icons-material/DarkMode';
export { default as DashboardIcon } from '@mui/icons-material/Dashboard';
export { default as DeleteIcon } from '@mui/icons-material/Delete';
export { default as DeskIcon } from '@mui/icons-material/Desk';
export { default as TickIcon } from '@mui/icons-material/Done';
export { default as EditIcon } from '@mui/icons-material/Edit';
export { default as ErrorIcon } from '@mui/icons-material/Error';
export { default as FeedIcon } from '@mui/icons-material/Feed';
export { default as FeedbackIcon } from '@mui/icons-material/Feedback';
export { default as GridViewIcon } from '@mui/icons-material/GridView';
export { default as InstallIcon } from '@mui/icons-material/InstallDesktop';
export { default as ArrowDownIcon } from '@mui/icons-material/KeyboardArrowDown';
export { default as ArrowLeftIcon } from '@mui/icons-material/KeyboardArrowLeft';
export { default as ArrowRightIcon } from '@mui/icons-material/KeyboardArrowRight';
export { default as ArrowUpIcon } from '@mui/icons-material/KeyboardArrowUp';
export { default as ListViewIcon } from '@mui/icons-material/List';
export { default as ParkingIcon } from '@mui/icons-material/LocalParking';
export { default as ClosedAllDayIcon } from '@mui/icons-material/Lock';
export { default as SignInIcon } from '@mui/icons-material/Login';
export { default as SignOutIcon } from '@mui/icons-material/Logout';
export { default as FloorPlanIcon } from '@mui/icons-material/Map';
export { default as RoomIcon } from '@mui/icons-material/MeetingRoom';
export { default as MenuIcon } from '@mui/icons-material/Menu';
export { default as HamburgerMenuIcon } from '@mui/icons-material/MenuRounded';
export { default as MoreItemsIcon } from '@mui/icons-material/MoreHoriz';
export { default as EllipseMenuIcon } from '@mui/icons-material/MoreVert';
export { default as AddressIcon } from '@mui/icons-material/NearMe';
export { default as OtherResourceIcon } from '@mui/icons-material/NotListedLocation';
export { default as ExpandIcon } from '@mui/icons-material/OpenInFull';
export { default as BillingAndPaymentIcon } from '@mui/icons-material/Payment';
export { default as CustomerIcon, default as PersonIcon, default as ProfileIcon } from '@mui/icons-material/Person';
export { default as InviteMemberIcon, default as SignUpIcon } from '@mui/icons-material/PersonAdd';
export { default as InvitePeopleIcon } from '@mui/icons-material/PersonAddAlt1';
export { default as ContactPhoneIcon } from '@mui/icons-material/PhoneEnabled';
export { default as PdfIcon } from '@mui/icons-material/PictureAsPdf';
export { default as OpeningHoursIcon } from '@mui/icons-material/QueryBuilder';
export { default as BillingIcon } from '@mui/icons-material/Receipt';
export { default as RemoveIcon } from '@mui/icons-material/Remove';
export { default as CustomOpeningHoursIcon } from '@mui/icons-material/Schedule';
export { default as SearchRoundedIcon } from '@mui/icons-material/SearchRounded';
export { default as SsoSigninIcon } from '@mui/icons-material/Security';
export { default as PreferredIcon } from '@mui/icons-material/Star';
export { default as NotPreferredIcon } from '@mui/icons-material/StarOutline';
export { default as MarketplaceIcon } from '@mui/icons-material/Store';
export { default as SetupMarketplaceIcon } from '@mui/icons-material/Storefront';
export { default as SubscriptionsIcon } from '@mui/icons-material/Subscriptions';
export { default as SsoSettingsIcon } from '@mui/icons-material/SyncLock';
export { default as TodayIcon } from '@mui/icons-material/Today';
export { default as ToggleOffIcon } from '@mui/icons-material/ToggleOff';
export { default as ToggleOnIcon } from '@mui/icons-material/ToggleOn';
export { default as UpgradeIcon } from '@mui/icons-material/Upgrade';
export { default as ViewIcon } from '@mui/icons-material/Visibility';
export { default as OpenAllDayIcon } from '@mui/icons-material/WbSunny';

type Props = {
  fontSize?: 'inherit' | 'large' | 'medium' | 'small';
  sx?: SxProps<Theme>;
  color?: 'inherit' | 'action' | 'disabled' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
  excludeTooltip?: boolean;
  tip?: string;
};

export const JoinIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <AddCircle fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Join'}>
      <AddCircle fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const UserIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Person fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'User'}>
      <Person fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const MembersIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Person fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Users'}>
      <Person fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const OrganizationIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <CorporateFare fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Organization'}>
      <CorporateFare fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const LocationIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Place fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Location'}>
      <Place fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const TeamIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Diversity3 fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Team'}>
      <Diversity3 fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const ZoneIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <LocalOffer fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Zone'}>
      <LocalOffer fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const CustomTagIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <LocalOffer fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Tag'}>
      <LocalOffer fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const ResourceIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <EventSeat fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Resource'}>
      <EventSeat fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const NotesIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Sms fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Notes'}>
      <Sms fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const WebsiteIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Link fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Website'}>
      <Link fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const SettingsIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Settings fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Settings'}>
      <Settings fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const RefreshIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Refresh fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Refresh'}>
      <Refresh fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const NotificationsIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <NotificationsActive fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Notification'}>
      <NotificationsActive fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const AboutIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <WbIridescent fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'About'}>
      <WbIridescent fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const InfoIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Info fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Info'}>
      <Info fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const CalendarIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <DateRange fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Calendar'}>
      <DateRange fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const BookingIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <DateRange fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Booking'}>
      <DateRange fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const WorkingFromOfficeIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Business fontSize={fontSize} sx={sx} color={color ?? 'primary'} />
  ) : (
    <TooltipIcon tip={tip ?? 'Working From Office'}>
      <Business fontSize={fontSize} sx={sx} color={color ?? 'primary'} />
    </TooltipIcon>
  );

export const WorkingFromHomeIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Home fontSize={fontSize} sx={sx} color={color ?? 'action'} />
  ) : (
    <TooltipIcon tip={tip ?? 'Working From Home'}>
      <Home fontSize={fontSize} sx={sx} color={color ?? 'action'} />
    </TooltipIcon>
  );

export const ViewDetailsIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Fullscreen fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'View Details'}>
      <Fullscreen fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const HomeIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Home fontSize={fontSize} sx={sx} color={color ?? 'action'} />
  ) : (
    <TooltipIcon tip={tip ?? 'Home'}>
      <Home fontSize={fontSize} sx={sx} color={color ?? 'action'} />
    </TooltipIcon>
  );

export const ProductTagIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <LocalOffer fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Product Tag'}>
      <LocalOffer fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const LocationTagIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <LocalOffer fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Location Tag'}>
      <LocalOffer fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );

export const ProductIcon = ({ fontSize, excludeTooltip, tip, sx, color }: Props) =>
  excludeTooltip ? (
    <Category fontSize={fontSize} sx={sx} color={color} />
  ) : (
    <TooltipIcon tip={tip ?? 'Product'}>
      <Category fontSize={fontSize} sx={sx} color={color} />
    </TooltipIcon>
  );
