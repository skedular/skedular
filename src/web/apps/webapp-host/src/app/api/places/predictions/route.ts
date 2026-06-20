import { fetchPlacePredictionsFromGoogle } from '@skedular/shared/src/google-places/google-places-server';
import { NextResponse } from 'next/server';

export const dynamic = 'force-dynamic';

export async function POST(request: Request) {
  try {
    const { input, sessionToken, languageCode } = (await request.json()) as {
      input?: string;
      sessionToken?: string;
      languageCode?: string;
    };

    if (!input || typeof input !== 'string') {
      return NextResponse.json({ error: 'Missing input' }, { status: 400 });
    }

    const predictions = await fetchPlacePredictionsFromGoogle(input, languageCode ?? 'en', sessionToken);
    return NextResponse.json({ predictions });
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Unexpected error';
    return NextResponse.json({ error: message }, { status: 500 });
  }
}
