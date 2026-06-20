import { redirect } from 'next/navigation';

type Props = {
  params: { id: string };
};

const Page = ({ params }: Props) => {
  const { id } = params;
  redirect(`/locations/${id}`);
};

export default Page;
