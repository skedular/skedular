import { FileUploadResponse, SkedularCoreV1Client } from '@/clients/openapi/skedular/v1/core/fetch';
import { StackColumn, TwoButtonsDialogActions } from '@/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext } from '@/libs/providers';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import Input from '@mui/material/Input';
import React, { memo, useContext, useRef, useState } from 'react';
import { toast } from 'react-toastify';

type Props = {
  onUploadCompleted: (cdnFile: FileUploadResponse) => void;
};

const ImageFileUploader = ({ onUploadCompleted }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [imageSource, setImageSource] = useState<string>('');
  const [file, setFile] = useState<Blob | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState<boolean>(false);
  const inputRef = useRef<HTMLInputElement>(null);

  const onSelectFile = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files.length > 0) {
      const file = event.target.files[0];

      const reader = new FileReader();
      reader.onload = async () => {
        const result = reader.result as string;
        setImageSource(result);
        setIsDialogOpen(true);

        // Convert base64 data URL to Blob
        const blob = await fetch(result).then((res) => res.blob());
        setFile(blob);
      };
      reader.readAsDataURL(file);

      if (inputRef.current) {
        inputRef.current.value = '';
      }
    }
  };

  const handleCancelClicked = () => {
    setIsDialogOpen(false);
    setFile(null);
    setImageSource('');
  };

  const handleUploadButtonClicked = async () => {
    if (!file) {
      return;
    }

    setIsDialogOpen(false); // Close dialog immediately

    const toastId = themedToast(<NotificationContent content={'Uploading image file...'} />, infoNotificationOptions);
    const formData = new FormData();
    formData.append('file', file);

    try {
      const client = new SkedularCoreV1Client({ BASE: '/api' });
      const response = (await client.core.uploadPublicAccessFile({
        file,
      })) as FileUploadResponse;

      onUploadCompleted(response);

      toast.update(toastId, {
        ...successNotificationOptions,
        render: <NotificationContent content={'Image uploaded successfully.'} />,
      });
    } catch (error: unknown) {
      let errorMessage = 'Unknown error';
      if (error instanceof Error) {
        errorMessage = error.message;
      }

      toast.update(toastId, {
        ...errorNotificationOptions,
        render: <NotificationContent content={`Failed to upload image. Error: ${errorMessage}.`} />,
      });
    }
  };

  return (
    <>
      <StackColumn>
        <Input inputRef={inputRef} type="file" inputProps={{ accept: 'image/*' }} onChange={onSelectFile} />
      </StackColumn>

      <Dialog open={isDialogOpen} fullWidth maxWidth="md">
        <DialogContent>
          <StackColumn>
            {imageSource && (
              // eslint-disable-next-line @next/next/no-img-element
              <img src={imageSource} alt="Preview" style={{ width: '100%', height: 'auto' }} />
            )}
          </StackColumn>
        </DialogContent>
        <TwoButtonsDialogActions onPrimaryClicked={handleUploadButtonClicked} onSecondaryClicked={handleCancelClicked} primaryLabel="Upload" secondaryLabel="Cancel" />
      </Dialog>
    </>
  );
};

export default memo(ImageFileUploader);
