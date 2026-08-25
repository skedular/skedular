'use client';

import AddPhotoAlternateRoundedIcon from '@mui/icons-material/AddPhotoAlternateRounded';
import DeleteIcon from '@mui/icons-material/Delete';
import ChevronLeftRoundedIcon from '@mui/icons-material/ChevronLeftRounded';
import ChevronRightRoundedIcon from '@mui/icons-material/ChevronRightRounded';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import IconButton from '@mui/material/IconButton';
import type { ReactNode } from 'react';
import { useEffect, useState } from 'react';
import { BodyIconTypography } from './typography';
import StackColumn from './stack-column';
import StackRow from './stack-row';

export type FeatureImageGalleryImage = {
  original?: { url: string } | null;
  thumbnail?: { url: string } | null;
};

type Props<T extends FeatureImageGalleryImage> = {
  images: T[];
  coverImage: T | null;
  onRemove: (image: T) => void;
  onMakeCover: (image: T) => void;
  uploadControl: ReactNode;
};

const FeatureImageGallery = <T extends FeatureImageGalleryImage>({ images, coverImage, onRemove, onMakeCover, uploadControl }: Props<T>) => {
  const [activeIndex, setActiveIndex] = useState(0);
  const activeImage = images[activeIndex] ?? images[0];

  useEffect(() => {
    if (images.length > 0 && activeIndex >= images.length) setActiveIndex(images.length - 1);
  }, [activeIndex, images.length]);

  return (
    <StackColumn>
      {activeImage ? (
        <>
          <Box sx={{ position: 'relative', aspectRatio: '16 / 9', borderRadius: 3, overflow: 'hidden', border: 1, borderColor: 'divider', backgroundColor: 'grey.50' }}>
            <img src={activeImage.original?.url ?? activeImage.thumbnail?.url ?? ''} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
            {images.length > 1 ? (
              <>
                <IconButton
                  aria-label="Previous feature image"
                  onClick={() => setActiveIndex((index) => (index - 1 + images.length) % images.length)}
                  sx={{ position: 'absolute', left: 8, top: '50%', transform: 'translateY(-50%)', backgroundColor: 'rgba(255,255,255,0.85)' }}
                >
                  <ChevronLeftRoundedIcon />
                </IconButton>
                <IconButton
                  aria-label="Next feature image"
                  onClick={() => setActiveIndex((index) => (index + 1) % images.length)}
                  sx={{ position: 'absolute', right: 8, top: '50%', transform: 'translateY(-50%)', backgroundColor: 'rgba(255,255,255,0.85)' }}
                >
                  <ChevronRightRoundedIcon />
                </IconButton>
              </>
            ) : null}
            <StackRow sx={{ position: 'absolute', top: 8, right: 8 }}>
              <IconButton size="small" aria-label="Remove feature image" onClick={() => onRemove(activeImage)}>
                <DeleteIcon fontSize="small" />
              </IconButton>
            </StackRow>
            <StackRow sx={{ position: 'absolute', left: 8, bottom: 8 }}>
              {coverImage?.original?.url === activeImage.original?.url ? (
                <Chip size="small" color="success" label="Cover image" />
              ) : (
                <Button variant="contained" size="small" onClick={() => onMakeCover(activeImage)} sx={{ textTransform: 'none' }}>
                  Make cover
                </Button>
              )}
            </StackRow>
          </Box>
          {images.length > 1 ? (
            <StackRow sx={{ gap: 0.75, overflowX: 'auto', pt: 0.75 }}>
              {images.map((image, index) => (
                <Box
                  key={image.original?.url ?? index}
                  component="button"
                  type="button"
                  aria-label={`Show feature image ${index + 1}`}
                  onClick={() => setActiveIndex(index)}
                  sx={{
                    flex: '0 0 64px',
                    width: 64,
                    height: 44,
                    p: 0,
                    borderRadius: 1.5,
                    overflow: 'hidden',
                    border: 2,
                    borderColor: index === activeIndex ? 'success.main' : 'divider',
                    cursor: 'pointer',
                    background: 'none',
                  }}
                >
                  <img src={image.thumbnail?.url ?? image.original?.url ?? ''} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                </Box>
              ))}
            </StackRow>
          ) : null}
        </>
      ) : null}
      <Box
        sx={{
          position: 'relative',
          overflow: 'hidden',
          border: 1,
          borderStyle: 'dashed',
          borderColor: 'success.main',
          borderRadius: 2.5,
          p: 2,
          backgroundColor: 'action.hover',
          '& .MuiFormControl-root': { position: 'absolute', inset: 0, width: '100%', height: '100%', opacity: 0, zIndex: 1 },
          '& .MuiInput-root, & input': { width: '100%', height: '100%', cursor: 'pointer' },
        }}
      >
        <StackRow sx={{ alignItems: 'center', justifyContent: 'center', gap: 1 }}>
          <AddPhotoAlternateRoundedIcon color="success" />
          <BodyIconTypography label={images.length === 0 ? 'Choose a cover image' : 'Add another image'} />
        </StackRow>
        {uploadControl}
      </Box>
    </StackColumn>
  );
};

export default FeatureImageGallery;
