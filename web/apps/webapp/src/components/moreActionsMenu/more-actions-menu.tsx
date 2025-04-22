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
  EditProductTag,
  DeleteProductTag,
  EditLocationTag,
  DeleteLocationTag,
  EditResource,
  DeleteResource,
  ActivateResource,
  DeactivateResource,
  ViewUserBookings,
  ViewLocationBookings,
  ViewTeamBookings,
  EditProduct,
  DeleteProduct,
  ActivateProduct,
  DeactivateProduct,
  EditOrganizationStripeConnectAccount,
  DeleteOrganizationStripeConnectAccount,
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
  [MoreActionsMenuOptionType.EditProductTag]: {
    id: MoreActionsMenuOptionType.EditProductTag,
    label: 'Edit Product Tag',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteProductTag]: {
    id: MoreActionsMenuOptionType.DeleteProductTag,
    label: 'Remove Product Tag',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.EditLocationTag]: {
    id: MoreActionsMenuOptionType.EditLocationTag,
    label: 'Edit Location Tag',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteLocationTag]: {
    id: MoreActionsMenuOptionType.DeleteLocationTag,
    label: 'Remove Location Tag',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.EditResource]: {
    id: MoreActionsMenuOptionType.EditResource,
    label: 'Edit Resource',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteResource]: {
    id: MoreActionsMenuOptionType.DeleteResource,
    label: 'Remove Resource',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.ActivateResource]: {
    id: MoreActionsMenuOptionType.ActivateResource,
    label: 'Activate Resource',
  },
  [MoreActionsMenuOptionType.DeactivateResource]: {
    id: MoreActionsMenuOptionType.DeactivateResource,
    label: 'Dectivate Resource',
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
  [MoreActionsMenuOptionType.EditProduct]: {
    id: MoreActionsMenuOptionType.EditProduct,
    label: 'Edit Product',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteProduct]: {
    id: MoreActionsMenuOptionType.DeleteProduct,
    label: 'Remove Product',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.ActivateProduct]: {
    id: MoreActionsMenuOptionType.ActivateProduct,
    label: 'Activate Product',
  },
  [MoreActionsMenuOptionType.DeactivateProduct]: {
    id: MoreActionsMenuOptionType.DeactivateProduct,
    label: 'Dectivate Product',
  },
  [MoreActionsMenuOptionType.EditOrganizationStripeConnectAccount]: {
    id: MoreActionsMenuOptionType.EditOrganizationStripeConnectAccount,
    label: 'Edit Stripe Connect Account',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteOrganizationStripeConnectAccount]: {
    id: MoreActionsMenuOptionType.DeleteOrganizationStripeConnectAccount,
    label: 'Remove Stripe Connect Account',
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
