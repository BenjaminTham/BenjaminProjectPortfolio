import { db } from "@/lib/db";
import { SystemStatus } from "@prisma/client";

/**
 * Adds a new system status record to the database.
 * @param data - The system status data to be added.
 * @returns A Promise that resolves to the created SystemStatus object.
 */
export async function addSystemStatus(data: { systemStatus: boolean, systemTime: Date }): Promise<SystemStatus> {
    return await db.systemStatus.create({
        data: {
            systemStatus: data.systemStatus,
            systemTime: data.systemTime
        }
    })
}

/**
 * Retrieves the latest system status from the database.
 * @returns A Promise that resolves to the latest SystemStatus object, or null if no status is found.
 */
export async function getLatestSystemStatus(): Promise<SystemStatus | null> {
    return await db.systemStatus.findFirst({
        orderBy: {
            systemTime: "desc"
        }
    })
}