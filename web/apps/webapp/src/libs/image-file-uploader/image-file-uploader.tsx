import { FileUploadResponse, SkedularCoreV1Client } from '@/clients/openapi/skedular/v1/core/fetch';
import { BodyIconTypography, StackColumn, TwoButtonsDialogActions } from '@/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext } from '@/libs/providers';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import Input from '@mui/material/Input';
import Slider from '@mui/material/Slider';
import Image from 'next/image';
import React, { memo, useContext, useRef, useState } from 'react';
import ReactCrop, { centerCrop, Crop, makeAspectCrop, PixelCrop } from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';
import { toast } from 'react-toastify';

type Props = {
  defaultImageUrl?: string;
  defaultAspectRatio: number;
  previewImageHeight: number;
  previewImageWidth: number;
  onUploadCompleted: (url: string) => void;
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

const ImageFileUploader = ({ defaultImageUrl, defaultAspectRatio, previewImageHeight, previewImageWidth, onUploadCompleted }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [imageSource, setImgSrc] = useState<string>('');
  const [uploadedImageUrl, setUploadedImageUrl] = useState(defaultImageUrl);
  const [isDialogOpen, setIsDialogOpen] = useState<boolean>(false);
  const previewCanvasRef = useRef<HTMLCanvasElement>(null);
  const imgRef = useRef<HTMLImageElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const [crop, setCrop] = useState<Crop>();
  const [completedCrop, setCompletedCrop] = useState<PixelCrop>();
  const [scale, setScale] = useState<number>(1);

  const onSelectFile = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files.length > 0) {
      setCrop(undefined);

      const reader = new FileReader();
      reader.addEventListener('load', () => {
        setImgSrc(reader.result?.toString() || '');
        setIsDialogOpen(true);
      });

      reader.readAsDataURL(event.target.files[0]);

      // Reset input so same file can be selected again
      if (inputRef.current) {
        inputRef.current.value = '';
      }
    }
  };

  const onImageLoad = (event: React.SyntheticEvent<HTMLImageElement>) => {
    const { width, height } = event.currentTarget;

    setCrop(centerAspectCrop(width, height, defaultAspectRatio));
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

    setIsDialogOpen(false); // Close dialog immediately

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
    const toastId = themedToast(<NotificationContent content={'Uploading feature image file...'} />, infoNotificationOptions);

    const formData = new FormData();
    formData.append('file', file); // must match OpenAPI schema key

    try {
      const client = new SkedularCoreV1Client({ BASE: '/api' });
      const response = (await client.core.uploadPublicAccessFile({ file })) as FileUploadResponse;

      setUploadedImageUrl(response.cdnUrl);
      onUploadCompleted(response.cdnUrl);

      toast.update(toastId, {
        ...successNotificationOptions,
        render: <NotificationContent content={'Feature image file uploaded.'} />,
      });
    } catch (error: any) {
      toast.update(toastId, {
        ...errorNotificationOptions,
        render: <NotificationContent content={`Failed to upload feature image. Error: ${error.message}.`} />,
      });
    }
  };

  return (
    <>
      <StackColumn>
        {uploadedImageUrl && <Image src={uploadedImageUrl} height={previewImageHeight} width={previewImageWidth} alt="" />}
        <Input inputRef={inputRef} type="file" inputProps={{ accept: 'image/*' }} onChange={onSelectFile} />
      </StackColumn>

      <Dialog open={isDialogOpen} fullWidth>
        <DialogContent>
          <StackColumn>
            {!!imageSource && (
              <ReactCrop crop={crop} onChange={(_, percentCrop) => setCrop(percentCrop)} onComplete={(c) => setCompletedCrop(c)} aspect={defaultAspectRatio} minHeight={100}>
                {/* eslint-disable-next-line @next/next/no-img-element */}
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

export default memo(ImageFileUploader);
