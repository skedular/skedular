import { redirect } from 'next/navigation';

const Page = async ({ params }: { params: Promise<{ organizationCustomDomain: string }> }) => {
  const { organizationCustomDomain } = await params;
  redirect(`/organizations/${encodeURIComponent(organizationCustomDomain)}/admin?tab=profile&section=marketplace-listing`);
};

export default Page;
