import { TransitionProps } from '@mui/material/transitions';
import Zoom from '@mui/material/Zoom';
import { forwardRef } from 'react';

const DialogTransition = forwardRef(function Transition(
  props: TransitionProps & {
    children: React.ReactElement;
  },
  ref: React.Ref<unknown>,
) {
  return <Zoom style={{ transitionDelay: '300ms' }} ref={ref} {...props} />;
});

export default DialogTransition;
