import { SxProps } from '@mui/material';
import Avatar from '@mui/material/Avatar';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';
import { stringToColor } from '../../libs/utils';

type NameProps = {
  name?: string | null;
};

type PhotoProps = {
  url?: string | null;
};

type Props = {
  size?: 'small' | 'medium' | 'large';
  name?: NameProps;
  photo?: PhotoProps;
  sx?: SxProps | null;
  showFullName?: boolean;
  tip?: string;
  onClick?: (event: React.MouseEvent<HTMLElement>) => void;
};

const OrganizationAvatar = ({ name, photo, size, sx, showFullName, tip, onClick }: Props) => {
  let avatarLetters = '';

  if (name) {
    if (name.name && typeof name.name[0] !== 'undefined') {
      avatarLetters = name.name[0];
    } else {
      avatarLetters = '';
    }
  } else {
    avatarLetters = '';
  }

  let finalSx = { ...sx };
  if (size === 'small') {
    finalSx = { width: 24, height: 24 };
  } else if (size === 'medium') {
    finalSx = { width: 32, height: 32 };
  } else if (size === 'large') {
    finalSx = { width: 40, height: 40 };
  }

  finalSx = { ...finalSx, backgroundColor: stringToColor(name?.name ?? '') };

  if (!showFullName) {
    return (
      <Avatar src={photo?.url ?? undefined} alt={avatarLetters} sx={finalSx} onClick={onClick}>
        {avatarLetters}
      </Avatar>
    );
  }

  return (
    <Tooltip title={tip ?? name?.name}>
      <Avatar src={photo?.url ?? undefined} alt={avatarLetters} sx={finalSx} onClick={onClick}>
        {avatarLetters}
      </Avatar>
    </Tooltip>
  );
};

export default memo(OrganizationAvatar);
