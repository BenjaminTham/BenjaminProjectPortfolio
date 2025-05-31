import {db} from "@/lib/db";
import {Tray, TrayStatus} from "@prisma/client";


/**
 * Fetches all trays from the database.
 * @returns A Promise that resolves to an array of trays.
 */
export async function getAllTrays(): Promise<Tray[]> {
    return db.tray.findMany();
}

/**
 * Retrieves a tray by its ID.
 * @param id - The ID of the tray to retrieve.
 * @returns A promise that resolves to the tray object if found, or null if not found.
 */
export async function getTrayById(id: number): Promise<Tray | null> {
    return db.tray.findUnique({
        where: {id},
    });
}

/**
 * Retrieves a tray by its EPC (Electronic Product Code).
 * @param epc - The EPC of the tray to retrieve.
 * @returns A Promise that resolves to the tray object if found, or null if not found.
 */
export async function getTrayByEpc(epc: string): Promise<Tray | null> {
    return db.tray.findUnique({
        where: {epc},
    });
}

export async function getLastTrayId(): Promise<number> {
    const tray = await db.tray.findFirst({
        orderBy: {id: 'desc'},
    });
    return tray ? tray.id : 0;
}

/**
 * Creates a new tray with the provided data.
 * @param data - The data for the tray, including epc and rackId.
 * @returns A Promise that resolves to the created tray.
 */
export async function createTray(data: { id: number, epc: string, rackId?: number }): Promise<Tray> {
    return db.tray.create({
        data: {
            id: data.id,
            epc: data.epc,
            rackId: data.rackId || null,
        },
    });
}

/**
 * Checks if an epc already exists in the database.
 * @param epc - The epc to check.
 * @returns A Promise that resolves to a boolean indicating whether the epc exists.
 */
export async function checkEpcExists(epc: string): Promise<boolean> {
    const tray = await db.tray.findUnique({
        where: {epc},
    });
    return !!tray;
}

/**
 * Updates a tray with the specified ID.
 * @param id - The ID of the tray to update.
 * @param data - The data to update the tray with.
 * @returns A promise that resolves to the updated tray.
 */
export async function updateTray(id: number, data: {
    epc: string,
    status: TrayStatus,
    statusChg: Date,
    rackId?: number
}): Promise<Tray> {
    const {epc, status, statusChg, rackId} = data;

    return db.tray.update({
        where: {id},
        data: {
            epc,
            status,
            statusChg,
            rackId: rackId !== undefined ? rackId : null,
        },
    });
}

/**
 * Deletes a tray by its ID.
 * @param id - The ID of the tray to delete.
 * @returns A promise that resolves to the deleted tray.
 */
export async function deleteTrayById(id: number): Promise<Tray> {
    return db.tray.delete({
        where: {id},
    });
}