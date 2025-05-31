import {NextResponse} from "next/server";
import {retrieveAllReaders} from "@/domain/readerControl";

export const dynamic = 'force-dynamic'

export async function GET() {
    try {
        const readers = await retrieveAllReaders();
        return NextResponse.json(readers);
    } catch (error) {
        console.error('Error fetching readers:', error);
        return NextResponse.json({error: 'Failed to fetch readers'}, {status: 500});
    }
}