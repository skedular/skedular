import { redirect } from 'next/navigation';

// This route has moved to the org-scoped pricing page which uses the proper app shell.
// Redirect back to the location overview so users can navigate from there.
export default function EditLocationPage({ params }: { params: { id: string } }) {
  redirect(`/locations/${params.id}`);
}
