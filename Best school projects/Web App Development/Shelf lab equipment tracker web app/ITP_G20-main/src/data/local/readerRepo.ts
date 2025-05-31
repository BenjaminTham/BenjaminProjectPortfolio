import {db} from "@/lib/db";
import {Prisma, Reader} from "@prisma/client";

/**
 * Represents a Prisma validator for retrieving readers with antennas.
 */
const readerWithAntennas = Prisma.validator<Prisma.ReaderDefaultArgs>()({
    include: {antennas: true},
});
export type ReaderWithAntennas = Prisma.ReaderGetPayload<typeof readerWithAntennas>;

/**
 * Retrieves all readers from the database.
 * @returns A promise that resolves to an array of Reader objects.
 */
export async function getAllReaders(): Promise<ReaderWithAntennas[]> {
    return await db.reader.findMany({
        include: {
            antennas: true
        }
    })
}


/**
 * Retrieves a reader with antennas by its IP address.
 * @param ip - The IP address of the reader.
 * @returns A promise that resolves to a ReaderWithAntennas object if found, or null if not found.
 */
export async function getReaderByIp(ip: string): Promise<ReaderWithAntennas | null> {
    return await db.reader.findUnique({
        where: {
            ip
        },
        include: {
            antennas: true
        }
    })
}

/**
 * Adds a new reader to the database.
 * @param data - The data for the new reader.
 * @returns A Promise that resolves to the newly created reader.
 */
export async function addReader(data: { ip: string }): Promise<Reader> {
    return await db.reader.create({
        data: {
            ip: data.ip
        }
    })
}

/**
 * Updates the reader with the specified ID and IP address.
 * @param data - An object containing the ID and IP address of the reader.
 * @returns A promise that resolves to the updated reader.
 */
export async function updateReader(data: { id: string, ip: string }): Promise<Reader> {
    return await db.reader.update({
        where: {
            id: data.id
        },
        data: {
            ip: data.ip
        }
    })
}

/**
 * Removes a reader from the database.
 * @param id - The ID of the reader to remove.
 * @returns A Promise that resolves to the removed reader.
 */
export async function removeReaderById(id: string): Promise<Reader> {
    return await db.reader.delete({
        where: {
            id
        }
    })
}

/**
 * Removes a reader from the database.
 * @param ip - The IP address of the reader to remove.
 * @returns A Promise that resolves to the removed reader.
 */
export async function removeReaderByIp(ip: string): Promise<Reader> {
    return await db.reader.delete({
        where: {
            ip
        }
    })
}

/**
 * Adds an antenna to a reader.
 * @param data - The data object containing the readerId and antennaId.
 * @returns A Promise that resolves to the updated Reader object.
 */
export async function addAntennaToReader(data: { readerId: string, antennaId: string }): Promise<Reader> {
    return await db.reader.update({
        where: {
            id: data.readerId
        },
        data: {
            antennas: {
                connect: {
                    id: data.antennaId
                }
            }
        }
    })
}

/**
 * Removes an antenna from a reader.
 * @param data - The data object containing the readerId and antennaId.
 * @returns A promise that resolves to the updated Reader object.
 */
export async function removeAntennaFromReader(data: { readerId: string, antennaId: string }): Promise<Reader> {
    return await db.reader.update({
        where: {
            id: data.readerId
        },
        data: {
            antennas: {
                disconnect: {
                    id: data.antennaId
                }
            }
        }
    })
}


/**
 * Retrieves all readers that have antennas with the GEOFENCE function.
 * @returns A promise that resolves to an array of Reader objects.
 */
export async function getReadersWithGeofenceAntennas(): Promise<ReaderWithAntennas | null> {
    return await db.reader.findFirst({
        where: {
            antennas: {
                some: {
                    function: "GEOFENCE"
                }
            }
        },
        include: {
            antennas: true
        }
    });
}