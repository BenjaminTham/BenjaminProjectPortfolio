import {NextResponse} from "next/server";
import {retrieveAllInUsedTrays} from "@/domain/trayControl";

export const dynamic = 'force-dynamic'

export async function GET() {
    try {
        const trays = await retrieveAllInUsedTrays()
        return NextResponse.json(trays);
    } catch (error) {
        console.error('Error fetching trays:', error);
        return NextResponse.json({error: 'Failed to fetch trays'}, {status: 500});
    }
}

