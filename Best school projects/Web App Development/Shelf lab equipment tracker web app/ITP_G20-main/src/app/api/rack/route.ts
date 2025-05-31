import {NextResponse} from "next/server";
import {retrieveAllRacks} from "@/domain/rackControl";

export const dynamic = 'force-dynamic'

export async function GET() {
    try {
        const racks = await retrieveAllRacks();
        return NextResponse.json(racks);
    } catch (error) {
        console.error('Error fetching racks:', error);
        return NextResponse.json({error: 'Failed to fetch racks'}, {status: 500});
    }
}