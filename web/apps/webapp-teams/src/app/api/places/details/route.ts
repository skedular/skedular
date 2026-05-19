import { fetchPlaceDetailsFromGoogle } from '@/libs/address/google-places-server';
import { NextResponse } from 'next/server';

export const dynamic = 'force-dynamic';

export async function POST(request: Request) {
  try {
    const { placeId, sessionToken, languageCode } = (await request.json()) as {
      placeId?: string;
      sessionToken?: string;
      languageCode?: string;
    };

    if (!placeId || typeof placeId !== 'string') {
      return NextResponse.json({ error: 'Missing placeId' }, { status: 400 });
    }

    const details = await fetchPlaceDetailsFromGoogle(placeId, languageCode ?? 'en', sessionToken);
    return NextResponse.json({ details });
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Unexpected error';
    return NextResponse.json({ error: message }, { status: 500 });
  }
}
