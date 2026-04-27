'use client';

import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Typography from '@mui/material/Typography';
import type { ReactNode } from 'react';
import { StackColumn, StackRow } from './index';

export type GuidedEditorStep = {
  id: string;
  title: ReactNode;
  subtitle?: ReactNode;
};

type Props = {
  title?: ReactNode;
  description?: ReactNode;
  steps: readonly GuidedEditorStep[];
  activeStepId: string;
  onStepChange: (stepId: string) => void;
  variant?: 'default' | 'compact';
};

const GuidedEditorProgress = ({ title, description, steps, activeStepId, onStepChange, variant = 'default' }: Props) => (
  <StackColumn spacing={2}>
    {title || description ? (
      <>
        <StackColumn spacing={0.5}>
          {title ? <Typography variant="h6">{title}</Typography> : null}
          {description ? (
            <Typography variant="body2" sx={{ opacity: 0.8 }}>
              {description}
            </Typography>
          ) : null}
        </StackColumn>
        <Divider />
      </>
    ) : null}
    {variant === 'compact' ? (
      <Box
        sx={{
          width: '100%',
          px: { xs: 2, sm: 3 },
          py: 2,
          border: 1,
          borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
          borderRadius: 4,
          bgcolor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
          boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : theme.shadows[1]),
        }}
      >
        <Box
          sx={{
            display: 'flex',
            gap: 1,
            overflowX: 'auto',
            flex: '1 1 0%',
            minWidth: 0,
            scrollbarWidth: 'none',
            '&::-webkit-scrollbar': {
              display: 'none',
            },
          }}
        >
          {steps.map((step) => (
            <Button
              key={step.id}
              variant={activeStepId === step.id ? 'contained' : 'text'}
              color={activeStepId === step.id ? 'primary' : 'inherit'}
              onClick={() => onStepChange(step.id)}
              aria-current={activeStepId === step.id ? 'step' : undefined}
              sx={{
                flexShrink: 0,
                borderRadius: 999,
                px: 2,
                py: 0.75,
                textTransform: 'none',
                whiteSpace: 'nowrap',
                border: activeStepId === step.id ? undefined : 1,
                borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.12)' : theme.palette.divider),
                bgcolor: (theme) => (activeStepId === step.id ? theme.palette.primary.main : 'transparent'),
                '&:focus-visible': {
                  outline: '3px solid',
                  outlineColor: (theme) => theme.palette.warning.main,
                  outlineOffset: 2,
                },
              }}
            >
              {step.title}
            </Button>
          ))}
        </Box>
      </Box>
    ) : (
      <StackRow
        spacing={1}
        sx={{
          gap: 1.25,
          flexWrap: 'wrap',
          width: '100%',
          justifyContent: 'center',
        }}
      >
        {steps.map((step) => (
          <Button
            key={step.id}
            variant={activeStepId === step.id ? 'contained' : 'outlined'}
            onClick={() => onStepChange(step.id)}
            sx={{
              minWidth: { xs: 'calc(50% - 10px)', sm: 170 },
              px: 1.75,
              py: 1.25,
              borderRadius: 3,
              borderWidth: 1,
              textTransform: 'none',
              alignItems: 'flex-start',
              justifyContent: 'flex-start',
              textAlign: 'left',
              color: (theme) => (activeStepId === step.id ? theme.palette.primary.contrastText : theme.palette.text.primary),
              backgroundColor: (theme) =>
                activeStepId === step.id ? theme.palette.primary.main : theme.palette.mode === 'light' ? 'rgba(248, 250, 252, 0.96)' : theme.palette.background.paper,
              borderColor: (theme) => (activeStepId === step.id ? theme.palette.primary.main : theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.18)' : theme.palette.divider),
              boxShadow: (theme) =>
                activeStepId === step.id ? (theme.palette.mode === 'light' ? '0 10px 24px rgba(15, 23, 42, 0.14)' : '0 8px 18px rgba(0, 0, 0, 0.34)') : 'none',
              '&:hover': {
                borderWidth: 1,
                borderColor: (theme) => (activeStepId === step.id ? theme.palette.primary.dark : theme.palette.text.primary),
                backgroundColor: (theme) =>
                  activeStepId === step.id ? theme.palette.primary.dark : theme.palette.mode === 'light' ? 'rgba(241, 245, 249, 1)' : theme.palette.action.hover,
              },
              '&:focus-visible': {
                outline: '3px solid',
                outlineColor: (theme) => theme.palette.warning.main,
                outlineOffset: 2,
              },
            }}
          >
            <StackColumn spacing={0.25} sx={{ alignItems: 'flex-start' }}>
              <Typography variant="body1" sx={{ fontWeight: 700, lineHeight: 1.2 }}>
                {step.title}
              </Typography>
              {step.subtitle ? (
                <Typography
                  variant="caption"
                  sx={{
                    opacity: activeStepId === step.id ? 0.92 : 0.78,
                    color: 'inherit',
                    whiteSpace: 'normal',
                    lineHeight: 1.35,
                  }}
                >
                  {step.subtitle}
                </Typography>
              ) : null}
            </StackColumn>
          </Button>
        ))}
      </StackRow>
    )}
  </StackColumn>
);

export default GuidedEditorProgress;
