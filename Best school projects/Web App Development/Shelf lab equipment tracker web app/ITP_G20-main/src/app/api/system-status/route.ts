import {NextResponse} from "next/server";
import {retrieveSystemStatus} from "@/domain/systemControl";

export const dynamic = 'force-dynamic'

export async function GET() {
    try {
        const system = await retrieveSystemStatus();
        return NextResponse.json(system);
    } catch (error) {
        console.error('Error fetching system:', error);
        return NextResponse.json({error: 'Failed to fetch system'}, {status: 500});
    }
}