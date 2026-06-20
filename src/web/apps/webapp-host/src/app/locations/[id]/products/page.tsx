import { redirect } from 'next/navigation';

type Props = {
  params: { id: string };
};

export default function ProductsPage({ params }: Props) {
  const { id } = params;
  redirect(`/locations/${id}`);
}
