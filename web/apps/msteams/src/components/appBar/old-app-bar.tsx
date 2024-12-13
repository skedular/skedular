import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, LeadIconTypography, StackColumn, StackRow, StackRowFullWidth } from '@repo/shared/components/commons';
import { FeedbackIcon, SettingsIcon, ToggleOffIcon, ToggleOnIcon } from '@repo/shared/components/icons';
import { SwitchToModernUIContext, UpdateSwitchToModernUIContext } from '@repo/shared/libs/providers';
import { getCustomerFullName, localNow, toLongDateTime } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { NewFeedbackDialog } from 'components/feedback';
import { memo, useContext, useState } from 'react';
import { useFragment } from 'react-relay';
import { useInterval } from 'usehooks-ts';
import type { oldAppBar_query$key } from './__generated__/oldAppBar_query.graphql';

type Props = {
  rootDataRelay: oldAppBar_query$key;
  onReloadRequired: () => void;
};

const OldAppBar = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<oldAppBar_query$key>(
    graphql`
      fragment oldAppBar_query on Query {
        me {
          email
          givenName
          middleName
          familyName
          photoUrl
        }
        ...newFeedbackDialog_query
      }
    `,
    rootDataRelay,
  );

  const [currentTime, setCurrentTime] = useState(localNow());
  const switchToModernUI = useContext(SwitchToModernUIContext);
  const UpdateSwitchToModernUI = useContext(UpdateSwitchToModernUIContext);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);

  useInterval(() => setCurrentTime(localNow()), 1000);

  const handleProfileMenuOpenClick = (event: React.MouseEvent<HTMLElement>) => {
    setProfileOpenAnchorEl(event.currentTarget);
  };

  const handleProfileMenuCloseClick = () => {
    setProfileOpenAnchorEl(null);
  };

  const handleSubmitFeedbackClicked = () => {
    setProfileOpenAnchorEl(null);
    setSubmitFeedbackDialogOpen(true);
  };

  const handleSubmitFeedbackSendClick = () => {
    setSubmitFeedbackDialogOpen(false);
  };

  const handleSubmitFeedbackCancelClick = () => {
    setSubmitFeedbackDialogOpen(false);
  };

  const handleModernUIClicked = () => {
    UpdateSwitchToModernUI(true);
  };

  const handleClassicUIClicked = () => {
    UpdateSwitchToModernUI(false);
  };

  const customerName = getCustomerFullName({
    name: null,
    givenName: rootData.me?.givenName,
    middleName: rootData.me?.middleName,
    familyName: rootData.me?.familyName,
  });

  return (
    <>
      <StackRowFullWidth sx={{ paddingLeft: 1, paddingRight: 1 }}>
        <LeadIconTypography label={`Welcome ${customerName}`} sx={{ display: { xs: 'none', sm: 'block' } }} />

        <StackRow sx={{ alignItems: 'center' }}>
          <LeadIconTypography label={toLongDateTime(currentTime)} sx={{ display: { xs: 'none', sm: 'block' } }} />
          <Divider orientation="vertical" flexItem />

          <IconButton onClick={handleProfileMenuOpenClick}>
            <CustomerAvatar
              name={{
                name: null,
                givenName: rootData.me?.givenName,
                middleName: rootData.me?.middleName,
                familyName: rootData.me?.familyName,
              }}
              photo={{
                url: rootData.me?.photoUrl,
              }}
            />
          </IconButton>

          <Menu
            sx={{ mt: 4 }}
            anchorEl={profileOpenAnchorEl}
            anchorOrigin={{
              vertical: 'top',
              horizontal: 'right',
            }}
            keepMounted
            transformOrigin={{
              vertical: 'top',
              horizontal: 'right',
            }}
            open={Boolean(profileOpenAnchorEl)}
            onClose={handleProfileMenuCloseClick}
          >
            <MenuItem>
              <StackColumn>
                <LeadIconTypography label={customerName} />
                <BodyIconTypography label={rootData.me?.email} />
              </StackColumn>
            </MenuItem>

            <Divider />

            <MenuItem>
              <Link href="/settings" color="inherit">
                <BodyIconTypography icon={<SettingsIcon />} label="Settings" />
              </Link>
            </MenuItem>

            {!switchToModernUI && (
              <MenuItem onClick={handleModernUIClicked}>
                <BodyIconTypography icon={<ToggleOffIcon />} label="Switch to modern UI" />
              </MenuItem>
            )}

            {switchToModernUI && (
              <MenuItem onClick={handleClassicUIClicked}>
                <BodyIconTypography icon={<ToggleOnIcon />} label="Switch to classic UI" />
              </MenuItem>
            )}

            <Divider />

            <MenuItem onClick={handleSubmitFeedbackClicked}>
              <BodyIconTypography icon={<FeedbackIcon />} label="Send us feedback" />
            </MenuItem>
          </Menu>
        </StackRow>
      </StackRowFullWidth>

      <NewFeedbackDialog
        rootDataRelay={rootData}
        isDialogOpen={submitFeedbackDialogOpen}
        onSendClicked={handleSubmitFeedbackSendClick}
        onCancelClicked={handleSubmitFeedbackCancelClick}
      />
    </>
  );
};

export default memo(OldAppBar);
