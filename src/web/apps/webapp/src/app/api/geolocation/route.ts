import { type NextRequest, NextResponse } from 'next/server';
import { IPinfoWrapper } from 'node-ipinfo';

export async function GET(req: NextRequest): Promise<NextResponse> {
  const forwarded = req.headers.get('x-forwarded-for');
  const ip = forwarded ? forwarded.split(',')[0].trim() : '';

  const token = process.env.IPINFO_TOKEN ?? '';
  const wrapper = new IPinfoWrapper(token);
  const info = await wrapper.lookupIp(ip);

  const [lat, lng] = info.loc ? info.loc.split(',') : [null, null];

  return NextResponse.json({
    city: info.city ?? null,
    region: info.region ?? null,
    country: info.country ?? null,
    lat: lat ?? null,
    lng: lng ?? null,
  });
}
