import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import Box from '@mui/material/Box';
import CardMedia from '@mui/material/CardMedia';
import IconButton from '@mui/material/IconButton';
import type { SxProps, Theme } from '@mui/material/styles';
import { memo, MouseEvent, useMemo, useState } from 'react';

type Thumbnail = {
  url: string;
  height?: number | null;
  width?: number | null;
};

type ImageWithThumbnail = {
  thumbnail?: Thumbnail | null;
};

type Props = {
  images: ImageWithThumbnail[] | readonly ImageWithThumbnail[];
  fallbackHeight?: number;
  showPlaceholderWhenEmpty?: boolean;
  placeholderSx?: SxProps<Theme>;
  placeholderImageUrl?: string;
};

const CardMediaCarousel = ({ images, fallbackHeight = 200, showPlaceholderWhenEmpty = true, placeholderSx, placeholderImageUrl }: Props) => {
  const featureImages = useMemo(() => images.filter((image): image is { thumbnail: Thumbnail } => Boolean(image?.thumbnail?.url)), [images]);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);

  if (featureImages.length === 0) {
    if (!showPlaceholderWhenEmpty) {
      return null;
    }

    if (placeholderImageUrl) {
      return <CardMedia component="img" image={placeholderImageUrl} sx={{ objectFit: 'fill', height: fallbackHeight, width: '100%', ...placeholderSx }} />;
    }

    return <Box sx={{ height: fallbackHeight, width: '100%', bgcolor: 'background.default', ...placeholderSx }} />;
  }

  const safeIndex = currentImageIndex % featureImages.length;
  const currentImage = featureImages[safeIndex].thumbnail!;

  const handleNextImage = () => {
    setCurrentImageIndex((prev) => (prev + 1) % featureImages.length);
  };

  const handlePreviousImage = () => {
    setCurrentImageIndex((prev) => (prev - 1 + featureImages.length) % featureImages.length);
  };

  const stopCardNavigation = (action: () => void) => (event: MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    event.stopPropagation();
    action();
  };

  return (
    <Box sx={{ position: 'relative' }}>
      <CardMedia component="img" image={currentImage.url} sx={{ objectFit: 'fill', height: currentImage.height ?? fallbackHeight, width: '100%' }} />
      {featureImages.length > 1 && (
        <>
          <IconButton
            onClick={stopCardNavigation(handlePreviousImage)}
            sx={{
              position: 'absolute',
              top: '50%',
              left: 8,
              transform: 'translateY(-50%)', // offset half the icon height to vertically center
              bgcolor: 'background.paper',
              '&:hover': { bgcolor: 'background.paper' },
            }}
            size="small"
          >
            <ChevronLeftIcon />
          </IconButton>
          <IconButton
            onClick={stopCardNavigation(handleNextImage)}
            sx={{
              position: 'absolute',
              top: '50%',
              right: 8,
              transform: 'translateY(-50%)', // offset half the icon height to vertically center
              bgcolor: 'background.paper',
              '&:hover': { bgcolor: 'background.paper' },
            }}
            size="small"
          >
            <ChevronRightIcon />
          </IconButton>
        </>
      )}
    </Box>
  );
};

export default memo(CardMediaCarousel);
