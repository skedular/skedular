import { ImageFileUploaderWithCropper as BaseImageFileUploaderWithCropper } from '@skedular/shared';
import { FileUploadResponse, SkedularCoreCoreV1Client } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { memo } from 'react';

type Props = {
  onUploadCompleted: (cdnFile: FileUploadResponse) => void;
  helperText?: string;
};

const ImageFileUploaderWithCropper = ({ onUploadCompleted, helperText }: Props) => {
  const handleUpload = async (file: Blob) => {
    const client = new SkedularCoreCoreV1Client({ BASE: '/api' });
    const response = (await client.core.uploadPublicAccessFile({ file })) as FileUploadResponse;
    onUploadCompleted(response);
  };

  return <BaseImageFileUploaderWithCropper onUpload={handleUpload} helperText={helperText} />;
};

export default memo(ImageFileUploaderWithCropper);
