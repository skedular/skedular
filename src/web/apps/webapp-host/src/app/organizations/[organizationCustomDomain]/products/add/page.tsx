import { redirect } from 'next/navigation';

const Page = async ({ params }: { params: Promise<{ organizationCustomDomain: string }> }) => {
  const { organizationCustomDomain } = await params;
  redirect(`/organizations/${encodeURIComponent(organizationCustomDomain)}/products`);
};

export default Page;
