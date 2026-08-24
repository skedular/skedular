import { redirect } from 'next/navigation';

const Page = async ({ params }: { params: Promise<{ organizationCustomDomain: string }> }) => {
  const { organizationCustomDomain } = await params;
  redirect(`/organizations/${encodeURIComponent(organizationCustomDomain)}/settings?tab=profile&section=marketplace-listing`);
};

export default Page;
