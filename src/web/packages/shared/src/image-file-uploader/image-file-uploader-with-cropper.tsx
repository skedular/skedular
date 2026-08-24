'use client';

import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import FormControl from '@mui/material/FormControl';
import FormHelperText from '@mui/material/FormHelperText';
import Input from '@mui/material/Input';
import Slider from '@mui/material/Slider';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import { BodyIconTypography, StackColumn, TwoButtonsDialogActions } from '@skedular/ui';
import { NotificationContent } from '../notification';
import React, { memo, useRef, useState } from 'react';
import ReactCrop, { centerCrop, Crop, makeAspectCrop, PixelCrop } from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';
import { toast } from 'react-toastify';

type Props = {
  onUpload: (file: Blob) => Promise<void>;
  helperText?: string;
  trigger?: React.ReactNode;
};

type AspectRatioOption = {
  key: string;
  label: string;
  aspect?: number;
};

const centerAspectCrop = (mediaWidth: number, mediaHeight: number, aspect: number) =>
  centerCrop(
    makeAspectCrop(
      {
        unit: '%',
        width: 90,
      },
      aspect,
      mediaWidth,
      mediaHeight,
    ),
    mediaWidth,
    mediaHeight,
  );

const popularAspectRatioOptions: AspectRatioOption[] = [
  { key: '1:1', label: '1:1', aspect: 1 },
  { key: '4:3', label: '4:3', aspect: 4 / 3 },
  { key: '16:9', label: '16:9', aspect: 16 / 9 },
  { key: '3:2', label: '3:2', aspect: 3 / 2 },
  { key: 'custom', label: 'Custom' },
];

const ImageFileUploaderWithCropper = ({ onUpload, helperText, trigger }: Props) => {
  const [imageSource, setImgSrc] = useState<string>('');
  const [isDialogOpen, setIsDialogOpen] = useState<boolean>(false);
  const previewCanvasRef = useRef<HTMLCanvasElement>(null);
  const imgRef = useRef<HTMLImageElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const [crop, setCrop] = useState<Crop>();
  const [completedCrop, setCompletedCrop] = useState<PixelCrop>();
  const [scale, setScale] = useState<number>(1);
  const [selectedAspectRatioKey, setSelectedAspectRatioKey] = useState<string>('1:1');
  const [selectedAspectRatio, setSelectedAspectRatio] = useState<number | undefined>(1);

  const onSelectFile = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files.length > 0) {
      setCrop(undefined);

      const reader = new FileReader();
      reader.addEventListener('load', () => {
        setImgSrc(reader.result?.toString() || '');
        setIsDialogOpen(true);
      });

      reader.readAsDataURL(event.target.files[0]!);

      if (inputRef.current) {
        inputRef.current.value = '';
      }
    }
  };

  const onImageLoad = (event: React.SyntheticEvent<HTMLImageElement>) => {
    const { width, height } = event.currentTarget;

    if (selectedAspectRatio) {
      setCrop(centerAspectCrop(width, height, selectedAspectRatio));
      return;
    }

    setCrop({
      unit: '%',
      x: 5,
      y: 5,
      width: 90,
      height: 90,
    });
  };

  const handleAspectRatioChanged = (_event: React.MouseEvent<HTMLElement>, nextSelectedAspectRatioKey: string | null) => {
    if (!nextSelectedAspectRatioKey) {
      return;
    }

    const selectedOption = popularAspectRatioOptions.find((option) => option.key === nextSelectedAspectRatioKey);
    if (!selectedOption) {
      return;
    }

    setSelectedAspectRatioKey(nextSelectedAspectRatioKey);
    setSelectedAspectRatio(selectedOption.aspect);

    const image = imgRef.current;
    if (!image) {
      return;
    }

    if (selectedOption.aspect) {
      setCrop(centerAspectCrop(image.width, image.height, selectedOption.aspect));
      return;
    }

    setCrop({
      unit: '%',
      x: 5,
      y: 5,
      width: 90,
      height: 90,
    });
  };

  const handleCropButtonClicked = () => {
    const canvas = previewCanvasRef.current;
    const image = imgRef.current;
    if (!canvas || !completedCrop || !image) {
      return;
    }

    const scaleX = image.naturalWidth / image.width;
    const scaleY = image.naturalHeight / image.height;

    canvas.width = completedCrop.width;
    canvas.height = completedCrop.height;

    const ctx = canvas.getContext('2d');
    if (!ctx) {
      return;
    }

    ctx.drawImage(
      image,
      completedCrop.x * scaleX,
      completedCrop.y * scaleY,
      completedCrop.width * scaleX,
      completedCrop.height * scaleY,
      0,
      0,
      completedCrop.width,
      completedCrop.height,
    );

    setIsDialogOpen(false);

    canvas.toBlob((blob) => {
      if (!blob) {
        return;
      }

      uploadFile(blob);
    }, 'image/png');
  };

  const handleCancelClicked = () => {
    setIsDialogOpen(false);
  };

  const uploadFile = async (file: Blob) => {
    try {
      await onUpload(file);
    } catch (error: unknown) {
      const errorMessage = error instanceof Error ? error.message : 'Unknown error';
      toast.error(<NotificationContent content={`Failed to upload image file. Error: ${errorMessage}.`} />);
    }
  };

  return (
    <>
      <FormControl>
        <Input inputRef={inputRef} type="file" inputProps={{ accept: 'image/*' }} onChange={onSelectFile} sx={trigger ? { display: 'none' } : undefined} />
        {trigger ? React.cloneElement(trigger as React.ReactElement<{ onClick?: () => void }>, { onClick: () => inputRef.current?.click() }) : null}
        {helperText && <FormHelperText>{helperText}</FormHelperText>}
      </FormControl>
      <Dialog open={isDialogOpen} fullWidth>
        <DialogContent>
          <StackColumn>
            <FormControl>
              <BodyIconTypography label="Aspect ratio" />
              <ToggleButtonGroup size="small" value={selectedAspectRatioKey} exclusive onChange={handleAspectRatioChanged} sx={{ flexWrap: 'wrap', mt: 1 }}>
                {popularAspectRatioOptions.map((option) => (
                  <ToggleButton key={option.key} value={option.key} sx={{ textTransform: 'none' }}>
                    {option.label}
                  </ToggleButton>
                ))}
              </ToggleButtonGroup>
            </FormControl>

            {!!imageSource && (
              <ReactCrop crop={crop} onChange={(_, percentCrop) => setCrop(percentCrop)} onComplete={(c) => setCompletedCrop(c)} aspect={selectedAspectRatio} minHeight={100}>
                <img ref={imgRef} alt="Crop me" src={imageSource} style={{ transform: `scale(${scale})`, maxWidth: '100%' }} onLoad={onImageLoad} />
              </ReactCrop>
            )}

            <BodyIconTypography label="Scale" />
            <Slider value={scale} onChange={(_, newValue) => setScale(newValue as number)} min={0.1} max={3} step={0.1} disabled={!imageSource} />
          </StackColumn>
        </DialogContent>
        <TwoButtonsDialogActions
          onPrimaryClicked={handleCropButtonClicked}
          onSecondaryClicked={handleCancelClicked}
          primaryLabel="Crop"
          secondaryLabel="Cancel"
          primaryDisabled={!completedCrop?.width || !completedCrop?.height}
        />
      </Dialog>

      <canvas ref={previewCanvasRef} style={{ display: 'none' }} />
    </>
  );
};

export default memo(ImageFileUploaderWithCropper);
