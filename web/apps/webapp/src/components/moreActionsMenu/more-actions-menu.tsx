import { SmallIconTypography } from '@/components/commons';
import { BookingIcon, DeleteIcon, EditIcon, NotPreferredIcon, PreferredIcon } from '@/components/icons';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import type { JSX } from 'react';

type Props = {
  anchorEl: null | HTMLElement;
  open: boolean;
  onMenuItemClick: (id: MoreActionsMenuOptionType) => void;
  options: MoreActionsMenuItemType[];
};

export enum MoreActionsMenuOptionType {
  EditBooking,
  DeleteBooking,
  EditTeam,
  DeleteTeam,
  EnableDeskApprovalRequirement,
  RemoveDeskApprovalRequirement,
  SetAsPreferredLocation,
  RemoveAsPreferredLocation,
  EditLocation,
  DeleteLocation,
  MarkAsDefaultOrganization,
  ClearAsPreferredOrganization,
  RemoveOrganization,
  SetAsPreferredTeam,
  RemoveAsPreferredTeam,
  RemoveTeam,
  RemoveOrganizationUser,
  EditOrganizationUser,
  DeactivateOrganizationUser,
  ActivateOrganizationUser,
  RemoveTeamMember,
  DeactivateTeamMember,
  ActivateTeamMember,
  EditZone,
  DeleteZone,
  EditCustomTag,
  DeleteCustomTag,
  EditDesk,
  DeleteDesk,
  ActivateDesk,
  DeactivateDesk,
  ViewUserBookings,
  ViewLocationBookings,
  ViewTeamBookings,
  EditRoom,
  DeleteRoom,
  ActivateRoom,
  DeactivateRoom,
  EditResourceType,
  DeleteResourceType,
}

export type MoreActionsMenuItemType = {
  id: MoreActionsMenuOptionType;
  label: string;
  icon?: JSX.Element;
};

export const moreActionsMenuAllOptions: Record<MoreActionsMenuOptionType, MoreActionsMenuItemType> = {
  [MoreActionsMenuOptionType.EditBooking]: {
    id: MoreActionsMenuOptionType.EditBooking,
    label: 'Edit Booking',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteBooking]: {
    id: MoreActionsMenuOptionType.DeleteBooking,
    label: 'Delete',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.EditTeam]: {
    id: MoreActionsMenuOptionType.EditTeam,
    label: 'Edit Team',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteTeam]: {
    id: MoreActionsMenuOptionType.DeleteTeam,
    label: 'Remove Team',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.SetAsPreferredTeam]: {
    id: MoreActionsMenuOptionType.SetAsPreferredTeam,
    label: 'Set as preferred team',
    icon: <NotPreferredIcon />,
  },
  [MoreActionsMenuOptionType.RemoveAsPreferredTeam]: {
    id: MoreActionsMenuOptionType.RemoveAsPreferredTeam,
    label: 'Remove as preferred team',
    icon: <PreferredIcon />,
  },
  [MoreActionsMenuOptionType.RemoveTeam]: {
    id: MoreActionsMenuOptionType.RemoveTeam,
    label: 'Remove team',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.EnableDeskApprovalRequirement]: {
    id: MoreActionsMenuOptionType.EnableDeskApprovalRequirement,
    label: 'Enable desk approval requirement',
  },
  [MoreActionsMenuOptionType.RemoveDeskApprovalRequirement]: {
    id: MoreActionsMenuOptionType.RemoveDeskApprovalRequirement,
    label: 'Remove desk approval requirement',
  },
  [MoreActionsMenuOptionType.SetAsPreferredLocation]: {
    id: MoreActionsMenuOptionType.SetAsPreferredLocation,
    label: 'Set as preferred location',
    icon: <NotPreferredIcon />,
  },
  [MoreActionsMenuOptionType.RemoveAsPreferredLocation]: {
    id: MoreActionsMenuOptionType.RemoveAsPreferredLocation,
    label: 'Remove as preferred location',
    icon: <PreferredIcon />,
  },
  [MoreActionsMenuOptionType.EditLocation]: {
    id: MoreActionsMenuOptionType.EditLocation,
    label: 'Edit Location',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteLocation]: {
    id: MoreActionsMenuOptionType.DeleteLocation,
    label: 'Remove location',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.MarkAsDefaultOrganization]: {
    id: MoreActionsMenuOptionType.MarkAsDefaultOrganization,
    label: 'Mark as default organization',
    icon: <NotPreferredIcon />,
  },
  [MoreActionsMenuOptionType.ClearAsPreferredOrganization]: {
    id: MoreActionsMenuOptionType.ClearAsPreferredOrganization,
    label: 'Clear as default organization',
    icon: <PreferredIcon />,
  },
  [MoreActionsMenuOptionType.RemoveOrganization]: {
    id: MoreActionsMenuOptionType.RemoveOrganization,
    label: 'Remove organization',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.RemoveOrganizationUser]: {
    id: MoreActionsMenuOptionType.RemoveOrganizationUser,
    label: 'Remove User',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.DeactivateOrganizationUser]: {
    id: MoreActionsMenuOptionType.DeactivateOrganizationUser,
    label: 'Deactivate User',
  },
  [MoreActionsMenuOptionType.EditOrganizationUser]: {
    id: MoreActionsMenuOptionType.EditOrganizationUser,
    label: 'Edit User',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.ActivateOrganizationUser]: {
    id: MoreActionsMenuOptionType.ActivateOrganizationUser,
    label: 'Activate User',
  },
  [MoreActionsMenuOptionType.RemoveTeamMember]: {
    id: MoreActionsMenuOptionType.RemoveTeamMember,
    label: 'Remove Member',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.DeactivateTeamMember]: {
    id: MoreActionsMenuOptionType.DeactivateTeamMember,
    label: 'Deactivate Member',
  },
  [MoreActionsMenuOptionType.ActivateTeamMember]: {
    id: MoreActionsMenuOptionType.ActivateTeamMember,
    label: 'Activate Member',
  },
  [MoreActionsMenuOptionType.EditZone]: {
    id: MoreActionsMenuOptionType.EditZone,
    label: 'Edit Zone',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteZone]: {
    id: MoreActionsMenuOptionType.DeleteZone,
    label: 'Remove Zone',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.EditCustomTag]: {
    id: MoreActionsMenuOptionType.EditCustomTag,
    label: 'Edit Tag',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteCustomTag]: {
    id: MoreActionsMenuOptionType.DeleteCustomTag,
    label: 'Remove Tag',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.EditDesk]: {
    id: MoreActionsMenuOptionType.EditDesk,
    label: 'Edit Desk',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteDesk]: {
    id: MoreActionsMenuOptionType.DeleteDesk,
    label: 'Remove Desk',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.ActivateDesk]: {
    id: MoreActionsMenuOptionType.ActivateDesk,
    label: 'Activate Desk',
  },
  [MoreActionsMenuOptionType.DeactivateDesk]: {
    id: MoreActionsMenuOptionType.DeactivateDesk,
    label: 'Dectivate Desk',
  },
  [MoreActionsMenuOptionType.ViewUserBookings]: {
    id: MoreActionsMenuOptionType.ViewUserBookings,
    label: 'View Bookings',
    icon: <BookingIcon />,
  },
  [MoreActionsMenuOptionType.ViewLocationBookings]: {
    id: MoreActionsMenuOptionType.ViewLocationBookings,
    label: 'View Bookings',
    icon: <BookingIcon />,
  },
  [MoreActionsMenuOptionType.ViewTeamBookings]: {
    id: MoreActionsMenuOptionType.ViewTeamBookings,
    label: 'View Bookings',
    icon: <BookingIcon />,
  },
  [MoreActionsMenuOptionType.EditRoom]: {
    id: MoreActionsMenuOptionType.EditRoom,
    label: 'Edit Room',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteRoom]: {
    id: MoreActionsMenuOptionType.DeleteRoom,
    label: 'Remove Room',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.ActivateRoom]: {
    id: MoreActionsMenuOptionType.ActivateRoom,
    label: 'Activate Room',
  },
  [MoreActionsMenuOptionType.DeactivateRoom]: {
    id: MoreActionsMenuOptionType.DeactivateRoom,
    label: 'Dectivate Room',
  },
  [MoreActionsMenuOptionType.EditResourceType]: {
    id: MoreActionsMenuOptionType.EditResourceType,
    label: 'Edit Resource Type',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteResourceType]: {
    id: MoreActionsMenuOptionType.DeleteResourceType,
    label: 'Remove Resource Type',
    icon: <DeleteIcon color="warning" />,
  },
};

const MoreActionsMenu = ({ anchorEl: moreActionsAnchorEl, open, onMenuItemClick, options }: Props) => (
  <Menu anchorEl={moreActionsAnchorEl} open={open} onClose={onMenuItemClick}>
    {options.map((option) => (
      <MenuItem key={option.id} onClick={() => onMenuItemClick(option.id)}>
        <SmallIconTypography label={option.label} startElement={option.icon} />
      </MenuItem>
    ))}
  </Menu>
);

export default MoreActionsMenu;
