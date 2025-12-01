import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import Box from '@mui/material/Box';
import IconButton from '@mui/material/IconButton';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import { memo, useMemo, useState } from 'react';

type CarouselImage = {
  url: string;
  height?: number | null;
  width?: number | null;
};

type Props = {
  images: CarouselImage[];
};

const ResponsiveImageCarousel = ({ images }: Props) => {
  const theme = useTheme();
  const isMdUp = useMediaQuery(theme.breakpoints.up('md'));
  const isLgUp = useMediaQuery(theme.breakpoints.up('lg'));
  const isXlUp = useMediaQuery(theme.breakpoints.up('xl'));

  const validImages = useMemo(() => images.filter((img) => Boolean(img?.url)), [images]);
  const itemsPerView = useMemo(() => {
    if (!isMdUp) {
      return 1;
    }

    if (isXlUp) {
      return Math.min(6, validImages.length);
    }

    if (isLgUp) {
      return Math.min(5, validImages.length);
    }

    return Math.min(4, validImages.length);
  }, [isLgUp, isMdUp, isXlUp, validImages.length]);

  const [carouselIndex, setCarouselIndex] = useState(0);

  if (validImages.length === 0) {
    return null;
  }

  const handleNext = () => setCarouselIndex((prev) => (prev + 1) % validImages.length);
  const handlePrevious = () => setCarouselIndex((prev) => (prev - 1 + validImages.length) % validImages.length);

  return (
    <Box sx={{ position: 'relative', width: '100%', overflow: 'hidden' }}>
      <Box sx={{ display: 'flex', width: '100%', justifyContent: 'center', alignItems: 'center', gap: 1, flexWrap: 'nowrap' }}>
        {Array.from({ length: itemsPerView }).map((_, idx) => {
          const image = validImages[(carouselIndex + idx) % validImages.length];
          const dimension = `calc((100% - ${(itemsPerView - 1) * 8}px) / ${itemsPerView})`;

          return (
            <Box
              key={`${image.url}-${idx}`}
              component="img"
              src={image.url}
              alt=""
              sx={{
                objectFit: 'cover',
                width: dimension,
                height: dimension,
                aspectRatio: '1 / 1',
                display: 'block',
              }}
            />
          );
        })}
      </Box>
      {validImages.length > 1 && (
        <>
          <IconButton
            onClick={handlePrevious}
            sx={{
              position: 'absolute',
              top: '50%',
              left: 8,
              transform: 'translateY(-50%)',
              bgcolor: 'background.paper',
              '&:hover': { bgcolor: 'background.paper' },
            }}
            size="small"
          >
            <ChevronLeftIcon />
          </IconButton>
          <IconButton
            onClick={handleNext}
            sx={{
              position: 'absolute',
              top: '50%',
              right: 8,
              transform: 'translateY(-50%)',
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

export default memo(ResponsiveImageCarousel);
