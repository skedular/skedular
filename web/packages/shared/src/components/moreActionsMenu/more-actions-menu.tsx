import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { SmallIconTypography } from '../commons';
import { DeleteIcon, EditIcon, NotPreferredIcon, PreferredIcon } from '../icons';

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
  RemoveOrganizationMember,
  DeactivateOrganizationMember,
  ActivateOrganizationMember,
  RemoveTeamMember,
  DeactivateTeamMember,
  ActivateTeamMember,
  EditZone,
  DeleteZone,
  EditDeskType,
  DeleteDeskType,
  EditDesk,
  DeleteDesk,
  ActivateDesk,
  DeactivateDesk,
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
    icon: <EditIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.DeleteBooking]: {
    id: MoreActionsMenuOptionType.DeleteBooking,
    label: 'Delete',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.EditTeam]: {
    id: MoreActionsMenuOptionType.EditTeam,
    label: 'Edit Team',
    icon: <EditIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.DeleteTeam]: {
    id: MoreActionsMenuOptionType.DeleteTeam,
    label: 'Remove Team',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.SetAsPreferredTeam]: {
    id: MoreActionsMenuOptionType.SetAsPreferredTeam,
    label: 'Set as preferred team',
    icon: <NotPreferredIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.RemoveAsPreferredTeam]: {
    id: MoreActionsMenuOptionType.RemoveAsPreferredTeam,
    label: 'Remove as preferred team',
    icon: <PreferredIcon color="primary" />,
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
    icon: <NotPreferredIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.RemoveAsPreferredLocation]: {
    id: MoreActionsMenuOptionType.RemoveAsPreferredLocation,
    label: 'Remove as preferred location',
    icon: <PreferredIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.EditLocation]: {
    id: MoreActionsMenuOptionType.EditLocation,
    label: 'Edit Location',
    icon: <EditIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.DeleteLocation]: {
    id: MoreActionsMenuOptionType.DeleteLocation,
    label: 'Remove location',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.MarkAsDefaultOrganization]: {
    id: MoreActionsMenuOptionType.MarkAsDefaultOrganization,
    label: 'Mark as default organization',
    icon: <NotPreferredIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.ClearAsPreferredOrganization]: {
    id: MoreActionsMenuOptionType.ClearAsPreferredOrganization,
    label: 'Clear as default organization',
    icon: <PreferredIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.RemoveOrganization]: {
    id: MoreActionsMenuOptionType.RemoveOrganization,
    label: 'Remove organization',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.RemoveOrganizationMember]: {
    id: MoreActionsMenuOptionType.RemoveOrganizationMember,
    label: 'Remove member',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.DeactivateOrganizationMember]: {
    id: MoreActionsMenuOptionType.DeactivateOrganizationMember,
    label: 'Deactivate member',
  },
  [MoreActionsMenuOptionType.ActivateOrganizationMember]: {
    id: MoreActionsMenuOptionType.ActivateOrganizationMember,
    label: 'Activate member',
  },
  [MoreActionsMenuOptionType.RemoveTeamMember]: {
    id: MoreActionsMenuOptionType.RemoveTeamMember,
    label: 'Remove member',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.DeactivateTeamMember]: {
    id: MoreActionsMenuOptionType.DeactivateTeamMember,
    label: 'Deactivate member',
  },
  [MoreActionsMenuOptionType.ActivateTeamMember]: {
    id: MoreActionsMenuOptionType.ActivateTeamMember,
    label: 'Activate member',
  },
  [MoreActionsMenuOptionType.EditZone]: {
    id: MoreActionsMenuOptionType.EditZone,
    label: 'Edit Zone',
    icon: <EditIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.DeleteZone]: {
    id: MoreActionsMenuOptionType.DeleteZone,
    label: 'Remove Zone',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.EditDeskType]: {
    id: MoreActionsMenuOptionType.EditDeskType,
    label: 'Edit Desk Type',
    icon: <EditIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.DeleteDeskType]: {
    id: MoreActionsMenuOptionType.DeleteDeskType,
    label: 'Remove Desk Type',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.EditDesk]: {
    id: MoreActionsMenuOptionType.EditDesk,
    label: 'Edit Desk',
    icon: <EditIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.DeleteDesk]: {
    id: MoreActionsMenuOptionType.DeleteDesk,
    label: 'Remove Desk',
    icon: <DeleteIcon color="warning" />,
  },
  [MoreActionsMenuOptionType.ActivateDesk]: {
    id: MoreActionsMenuOptionType.ActivateDesk,
    label: 'Activate desk',
  },
  [MoreActionsMenuOptionType.DeactivateDesk]: {
    id: MoreActionsMenuOptionType.DeactivateDesk,
    label: 'Dectivate desk',
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
