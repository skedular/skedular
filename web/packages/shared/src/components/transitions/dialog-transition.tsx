import { TransitionProps } from '@mui/material/transitions';
import Zoom from '@mui/material/Zoom';
import { forwardRef } from 'react';

const DialogTransition = forwardRef(function Transition(
  props: TransitionProps & {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    children: React.ReactElement<any, any>;
  },
  ref: React.Ref<unknown>,
) {
  return <Zoom style={{ transitionDelay: '300ms' }} ref={ref} {...props} />;
});

export default DialogTransition;
