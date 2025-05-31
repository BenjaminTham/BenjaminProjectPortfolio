import {db} from "@/lib/db";
import {Antenna, Function} from "@prisma/client";

/**
 * Adds an antenna to the database.
 *
 * @param data - The data for the antenna to be added.
 * @returns A promise that resolves to the newly created antenna.
 */
export async function addAntenna(data: { antennaPort: number, function: Function }): Promise<Antenna> {
    return await db.antenna.create({
        data: {
            antennaPort: data.antennaPort,
            function: data.function
        }
    })
}

/**
 * Updates an antenna in the database.
 * @param data - The data object containing the properties of the antenna to be updated.
 * @returns A Promise that resolves to the updated Antenna object.
 */
export async function updateAntenna(data: { id: string, antennaPort: number, function: Function }): Promise<Antenna> {
    return await db.antenna.update({
        where: {
            id: data.id
        },
        data: {
            antennaPort: data.antennaPort,
            function: data.function
        }
    })
}

/**
 * Retrieves all antennas with the GEOFENCE function.
 * @returns A promise that resolves to an array of antennas.
 */
export async function getAllGeofenceAntennas(): Promise<Antenna[]> {
    return await db.antenna.findMany({
        where: {
            function: Function.GEOFENCE
        }
    });
}