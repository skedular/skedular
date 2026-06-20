import { redirect } from 'next/navigation';

type Props = {
  searchParams: Record<string, string | string[] | undefined>;
};

export default function EditProductPage({ searchParams }: Props) {
  const params = searchParams;
  const locationId = params.locationId;
  if (typeof locationId === 'string' && locationId.length > 0) {
    redirect(`/locations/${locationId}`);
  }

  redirect('/locations');
}
