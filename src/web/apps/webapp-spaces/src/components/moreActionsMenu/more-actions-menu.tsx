import { SmallIconTypography } from '@skedular/ui';
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
  EditRecurringBooking,
  DeleteBooking,
  DeleteRecurringBooking,
  ConfirmBookingPayment,
  RejectBookingPayment,
  MakeBookingPaymentNotRequired,
  SetAsPreferredLocation,
  RemoveAsPreferredLocation,
  EditLocation,
  DeleteLocation,
  MarkAsDefaultOrganization,
  ClearAsPreferredOrganization,
  RemoveOrganization,
  RemoveOrganizationUser,
  EditOrganizationUser,
  DeactivateOrganizationUser,
  ActivateOrganizationUser,
  EditZone,
  DeleteZone,
  SetAsPreferredZone,
  RemoveAsPreferredZone,
  EditCustomTag,
  DeleteCustomTag,
  SetAsPreferredCustomTag,
  RemoveAsPreferredCustomTag,
  EditProductTag,
  DeleteProductTag,
  EditResource,
  DeleteResource,
  ActivateResource,
  DeactivateResource,
  SetAsPreferredResource,
  RemoveAsPreferredResource,
  ViewUserBookings,
  ViewLocationBookings,
  EditProduct,
  DeleteProduct,
  ActivateProduct,
  DeactivateProduct,
  EditOrganizationStripeConnectAccount,
  DeleteOrganizationStripeConnectAccount,
  SetOrganizationStripeConnectAccountAsDefault,
  EditFloorPlan,
  DeleteFloorPlan,
  EditOrganizationBankAccount,
  DeleteOrganizationBankAccount,
  SetOrganizationBankAccountAsDefault,
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
  [MoreActionsMenuOptionType.EditRecurringBooking]: {
    id: MoreActionsMenuOptionType.EditRecurringBooking,
    label: 'Edit recurring booking',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteBooking]: {
    id: MoreActionsMenuOptionType.DeleteBooking,
    label: 'Cancel Booking',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.DeleteRecurringBooking]: {
    id: MoreActionsMenuOptionType.DeleteRecurringBooking,
    label: 'Cancel recurring series',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.ConfirmBookingPayment]: {
    id: MoreActionsMenuOptionType.ConfirmBookingPayment,
    label: 'Confirm Payment',
  },
  [MoreActionsMenuOptionType.RejectBookingPayment]: {
    id: MoreActionsMenuOptionType.RejectBookingPayment,
    label: 'Reject Payment',
  },
  [MoreActionsMenuOptionType.MakeBookingPaymentNotRequired]: {
    id: MoreActionsMenuOptionType.MakeBookingPaymentNotRequired,
    label: 'Make Payment Not Required',
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
  [MoreActionsMenuOptionType.SetAsPreferredZone]: {
    id: MoreActionsMenuOptionType.SetAsPreferredZone,
    label: 'Set as preferred zone',
    icon: <NotPreferredIcon />,
  },
  [MoreActionsMenuOptionType.RemoveAsPreferredZone]: {
    id: MoreActionsMenuOptionType.RemoveAsPreferredZone,
    label: 'Remove as preferred zone',
    icon: <PreferredIcon />,
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
  [MoreActionsMenuOptionType.SetAsPreferredCustomTag]: {
    id: MoreActionsMenuOptionType.SetAsPreferredCustomTag,
    label: 'Set as preferred tag',
    icon: <NotPreferredIcon />,
  },
  [MoreActionsMenuOptionType.RemoveAsPreferredCustomTag]: {
    id: MoreActionsMenuOptionType.RemoveAsPreferredCustomTag,
    label: 'Remove as preferred tag',
    icon: <PreferredIcon />,
  },
  [MoreActionsMenuOptionType.EditProductTag]: {
    id: MoreActionsMenuOptionType.EditProductTag,
    label: 'Edit Booking Group',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteProductTag]: {
    id: MoreActionsMenuOptionType.DeleteProductTag,
    label: 'Remove Booking Group',
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
  [MoreActionsMenuOptionType.SetAsPreferredResource]: {
    id: MoreActionsMenuOptionType.SetAsPreferredResource,
    label: 'Set as preferred resource',
    icon: <NotPreferredIcon />,
  },
  [MoreActionsMenuOptionType.RemoveAsPreferredResource]: {
    id: MoreActionsMenuOptionType.RemoveAsPreferredResource,
    label: 'Remove as preferred resource',
    icon: <PreferredIcon />,
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
  [MoreActionsMenuOptionType.SetOrganizationStripeConnectAccountAsDefault]: {
    id: MoreActionsMenuOptionType.SetOrganizationStripeConnectAccountAsDefault,
    label: 'Set as Default',
  },
  [MoreActionsMenuOptionType.EditFloorPlan]: {
    id: MoreActionsMenuOptionType.EditFloorPlan,
    label: 'Edit Floor Plan',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteFloorPlan]: {
    id: MoreActionsMenuOptionType.DeleteFloorPlan,
    label: 'Remove Floor Plan',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.EditOrganizationBankAccount]: {
    id: MoreActionsMenuOptionType.EditOrganizationBankAccount,
    label: 'Edit Bank Account',
    icon: <EditIcon />,
  },
  [MoreActionsMenuOptionType.DeleteOrganizationBankAccount]: {
    id: MoreActionsMenuOptionType.DeleteOrganizationBankAccount,
    label: 'Remove Bank Account',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.SetOrganizationBankAccountAsDefault]: {
    id: MoreActionsMenuOptionType.SetOrganizationBankAccountAsDefault,
    label: 'Set as Default',
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
