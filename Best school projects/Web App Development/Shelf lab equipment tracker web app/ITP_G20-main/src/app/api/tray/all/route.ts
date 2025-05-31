import {NextRequest, NextResponse} from 'next/server';
import {retrieveAllTray} from '@/domain/trayControl';

export const dynamic = 'force-dynamic'

export async function GET(req: NextRequest) {
    try {
        const trays = await retrieveAllTray();
        return NextResponse.json(trays, {status: 200});
    } catch (error) {
        if (error instanceof Error) {
            return NextResponse.json({error: error.message}, {status: 500});
        } else {
            return NextResponse.json({error: "An unknown error occurred"}, {status: 500});
        }
    }
}

