import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowRightIcon from '@mui/icons-material/KeyboardArrowRight';
import Box from '@mui/material/Box';
import Grow from '@mui/material/Grow';
import MenuItem from '@mui/material/MenuItem';
import MenuList from '@mui/material/MenuList';
import Paper from '@mui/material/Paper';
import Popper from '@mui/material/Popper';
import Typography from '@mui/material/Typography';
import { ReactNode, memo, useRef, useState } from 'react';
import NavigationRootLinkWithoutSubItems from './navigation-root-link-without-subitems';
import { Navigation } from './navigation.data';

interface Props {
  navigation: Navigation;
  children: ReactNode;
}

const NavigationRootLinkWithSubItems = ({ navigation: { subItems }, children }: Props) => {
  const anchorRef = useRef<HTMLDivElement>(null);
  const [open, setOpen] = useState(false);

  const handleToggle = () => {
    setOpen((prevOpen) => !prevOpen);
  };

  const handleClose = (event: Event) => {
    if (anchorRef.current && anchorRef.current.contains(event.target as HTMLElement)) {
      return;
    }

    setOpen(false);
  };

  return (
    <>
      <Box
        onClick={handleToggle}
        ref={anchorRef}
        sx={{
          textDecoration: 'none',
          position: 'relative',
          color: 'text.primary',
          cursor: 'pointer',
          fontWeight: 600,
          display: 'inline-flex',
          alignItems: 'center',
          justifyContent: 'center',
          px: { xs: 0, md: 3 },
          mb: { xs: 3, md: 0 },
          fontSize: { xs: '1.2rem', md: 'inherit' },

          '& > div': { display: 'none' },

          '&.current>div': { display: 'block' },

          '&:hover': {
            color: 'primary.main',
            '&>div': {
              display: 'block',
            },
          },
        }}
      >
        {children}
        {subItems && !open && <KeyboardArrowRightIcon />}
        {subItems && open && <KeyboardArrowDownIcon />}
      </Box>

      <Popper
        sx={{
          zIndex: 1,
        }}
        open={open}
        anchorEl={anchorRef.current}
        role={undefined}
        transition
        disablePortal
      >
        {({ TransitionProps, placement }) => (
          <Grow
            {...TransitionProps}
            style={{
              transformOrigin: placement === 'bottom' ? 'center top' : 'center bottom',
            }}
          >
            <Paper>
              <MenuList id="split-button-menu" autoFocusItem>
                {subItems &&
                  subItems.map((navigation, index) => (
                    <MenuItem key={index}>
                      <NavigationRootLinkWithoutSubItems navigation={navigation}>
                        <Typography variant="body1" component="h1">
                          {navigation.label}
                        </Typography>
                      </NavigationRootLinkWithoutSubItems>
                    </MenuItem>
                  ))}
              </MenuList>
            </Paper>
          </Grow>
        )}
      </Popper>
    </>
  );
};

export default memo(NavigationRootLinkWithSubItems);
