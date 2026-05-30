'use client';

import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import FormHelperText from '@mui/material/FormHelperText';
import Input from '@mui/material/Input';
import { PaletteModeContext, StackColumn, TwoButtonsDialogActions } from '@skedular/ui';
import React, { memo, useContext, useRef, useState } from 'react';
import { toast } from 'react-toastify';

type Props = {
  onUpload: (file: Blob) => Promise<void>;
  helperText?: string;
};

const ImageFileUploader = ({ onUpload, helperText }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [imageSource, setImageSource] = useState<string>('');
  const [file, setFile] = useState<Blob | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState<boolean>(false);
  const inputRef = useRef<HTMLInputElement>(null);

  const onSelectFile = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files.length > 0) {
      const selectedFile = event.target.files[0];

      const reader = new FileReader();
      reader.onload = async () => {
        const result = reader.result as string;
        setImageSource(result);
        setIsDialogOpen(true);

        const blob = await fetch(result).then((res) => res.blob());
        setFile(blob);
      };
      reader.readAsDataURL(selectedFile!);

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

    setIsDialogOpen(false);

    const toastId = themedToast('Uploading image file...');

    try {
      await onUpload(file);

      toast.update(toastId, { type: 'success', render: 'Image uploaded successfully.' });
    } catch (error: unknown) {
      const errorMessage = error instanceof Error ? error.message : 'Unknown error';

      toast.update(toastId, { type: 'error', render: `Failed to upload image. Error: ${errorMessage}.` });
    }
  };

  return (
    <>
      <StackColumn>
        <Input inputRef={inputRef} type="file" inputProps={{ accept: 'image/*' }} onChange={onSelectFile} />
        {helperText && <FormHelperText>{helperText}</FormHelperText>}
      </StackColumn>

      <Dialog open={isDialogOpen} fullWidth maxWidth="md">
        <DialogContent>
          <StackColumn>{imageSource && <img src={imageSource} alt="Preview" style={{ width: '100%', height: 'auto' }} />}</StackColumn>
        </DialogContent>
        <TwoButtonsDialogActions onPrimaryClicked={handleUploadButtonClicked} onSecondaryClicked={handleCancelClicked} primaryLabel="Upload" secondaryLabel="Cancel" />
      </Dialog>
    </>
  );
};

export default memo(ImageFileUploader);
