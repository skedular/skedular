'use client';

import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import type { Variant } from '@mui/material/styles/createTypography';
import { memo } from 'react';

export enum LineType {
  SingleLine = 0,
  BulletPoint = 1,
}

export interface Line {
  lineType: LineType;
  line?: string;
  bulletPointLines?: Line[];
  variant?: Variant;
  breakLineCount?: number;
}

interface Props {
  lines: Line[];
}

const Document = ({ lines }: Props) => {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'space-between',
        p: '3rem',
      }}
    >
      {lines.map(({ lineType, line, bulletPointLines, variant, breakLineCount }, index) => {
        switch (lineType) {
          case LineType.SingleLine:
            return (
              <div key={index}>
                <Typography variant={variant}>{line}</Typography>
                {!!breakLineCount && Array.from(Array(breakLineCount).keys()).map((_, index) => <br key={index} />)}
              </div>
            );

          case LineType.BulletPoint:
            return (
              <div key={index}>
                <ul>
                  {bulletPointLines?.map(({ line, variant }, index) => {
                    return (
                      <li key={index}>
                        <Typography variant={variant}>{line}</Typography>
                      </li>
                    );
                  })}
                </ul>
                {!!breakLineCount && Array.from(Array(breakLineCount).keys()).map((_, index) => <br key={index} />)}
              </div>
            );

          default:
            break;
        }
      })}
    </Box>
  );
};

export default memo(Document);
