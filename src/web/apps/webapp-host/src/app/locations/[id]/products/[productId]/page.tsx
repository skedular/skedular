import { redirect } from 'next/navigation';

type Props = {
  params: { id: string };
};

export default function ProductDetailsPage({ params }: Props) {
  const { id } = params;
  redirect(`/locations/${id}`);
}
