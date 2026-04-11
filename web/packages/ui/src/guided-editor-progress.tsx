import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { ReactNode } from 'react';

export type GuidedEditorStep = {
  id: string;
  title: ReactNode;
  subtitle?: ReactNode;
};

type Props = {
  title: ReactNode;
  description?: ReactNode;
  steps: readonly GuidedEditorStep[];
  activeStepId: string;
  onStepChange: (stepId: string) => void;
};

const GuidedEditorProgress = ({ title, description, steps, activeStepId, onStepChange }: Props) => (
  <Stack spacing={2}>
    <Stack spacing={0.5}>
      <Typography variant="h6">{title}</Typography>
      {description ? (
        <Typography variant="body2" sx={{ opacity: 0.8 }}>
          {description}
        </Typography>
      ) : null}
    </Stack>
    <Divider />
    <Stack direction="row" spacing={1} sx={{ gap: 1, flexWrap: 'wrap' }}>
      {steps.map((step) => (
        <Button key={step.id} variant={activeStepId === step.id ? 'contained' : 'outlined'} onClick={() => onStepChange(step.id)} sx={{ textTransform: 'none' }}>
          {step.title}
        </Button>
      ))}
    </Stack>
  </Stack>
);

export default GuidedEditorProgress;
