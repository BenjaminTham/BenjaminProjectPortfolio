"use server";

import {addAntenna, updateAntenna} from "@/data/local/antennaRepo";
import {
    addAntennaToReader,
    addReader,
    getAllReaders,
    getReaderByIp,
    ReaderWithAntennas,
    removeReaderByIp
} from "@/data/local/readerRepo";
import {createReaderFormSchema, editAntennaFunctionSchena as editAntennaFunctionSchema} from "@/schema/custom";
import {Function} from "@prisma/client";
import {z} from "zod";
import isPortReachable from "is-port-reachable";

/**
 * Adds a new reader with the provided values.
 *
 * @param values - The values for creating a new reader.
 * @returns An object with the result of the operation. If successful, it returns the added reader.
 * If there's an error, it returns an object with the error message.
 */
export async function addNewReader(values: z.infer<typeof createReaderFormSchema>) {
    const validation = createReaderFormSchema.safeParse(values);

    if (!validation.success) {
        return {error: "Invalid input"};
    }

    const {ip} = validation.data;

    const ipPortReachable = await isPortReachable(8080, {host: ip});

    if (!ipPortReachable) {
        return {error: "IP Address is not reachable!"};
    }

    const existingReader = await getReaderByIp(ip);

    if (existingReader) {
        return {error: "Reader already exists"};
    }
    const reader = await addReader({ip});

    // Create 8 antennas for the reader
    const antennas = Array.from({length: 8}, (_, index) => ({
        readerId: reader.id,
        function: Function.NONE,
        antennaPort: index + 1,
    }));
    const addedAntennas = await Promise.all(antennas.map((antenna) => addAntenna(antenna)));

    // Add the antennas to the reader
    await Promise.all(addedAntennas.map((antenna) => addAntennaToReader({
        readerId: reader.id,
        antennaId: antenna.id
    })));
}

/**
 * Retrieves all readers with antennas.
 * @returns A promise that resolves to an array of `ReaderWithAntennas` objects.
 */
export async function retrieveAllReaders(): Promise<ReaderWithAntennas[]> {
    return await getAllReaders();
}

/**
 * Updates the function of all antennas for a given reader.
 * @param readerIp - The IP address of the reader.
 * @param data - The data containing the updated antenna functions.
 * @returns A promise that resolves to an object with an error message if there was an error, or void if successful.
 */
export async function updateAllAntennaFunction(readerIp: string,
                                               data: z.infer<typeof editAntennaFunctionSchema>): Promise<{
    error: string
} | void> {
    const validation = editAntennaFunctionSchema.safeParse(data);

    if (!validation.success) {
        return {error: "Invalid input"};
    }

    const reader = await getReaderByIp(readerIp);

    if (!reader) {
        return {error: "Reader not found"};
    }

    const antennas = reader.antennas;

    // Update the function of each antenna
    const updatedAntennas = antennas.map((antenna) => {
        // Construct the key for accessing the corresponding function in validation.data
        // based on the current antenna's port number.
        const antennaKey = `antenna${antenna.antennaPort}` as keyof typeof validation.data;

        // Retrieve the updated function for the current antenna using the constructed key.
        const updatedFunction = validation.data[antennaKey];

        // Return a new antenna object with the updated function while preserving other properties.
        return {...antenna, function: updatedFunction};
    });

    console.log(updatedAntennas);

    // If there is more than one READWRITE antenna, return an error.
    if (updatedAntennas.filter((antenna) => antenna.function === "READWRITE").length > 1) {
        return {error: "There can only be one READWRITE antenna"};
    }

    // Check if there is already a READWRITE antenna set.
    const readWriteAntennas = await retrieveAntennaByFunction("READWRITE");
    if (readWriteAntennas.length >= 1 && readWriteAntennas[0].readerIp !== readerIp && updatedAntennas.some((antenna) => antenna.function === "READWRITE")) {
        return {error: `There can only be one READWRITE antenna, currently set on ${readWriteAntennas[0].readerIp}`};
    }

    // Check if there is already a GEOFENCE antenna set.
    const geofenceAntennas = await retrieveAntennaByFunction("GEOFENCE");
    if (geofenceAntennas.length >= 1 && geofenceAntennas[0].readerIp !== readerIp && updatedAntennas.some((antenna) => antenna.function === "GEOFENCE")) {
        return {error: `There can only one reader with GEOFENCE antennas, currently set on ${geofenceAntennas[0].readerIp}`};
    }

    const requestResults = await Promise.all(updatedAntennas.map((antenna) => updateAntenna({
        id: antenna.id,
        antennaPort: antenna.antennaPort,
        function: antenna.function
    })));

    if (requestResults.some((result) => !result)) {
        return {error: "Error updating antennas"};
    }
}

/**
 * Retrieves the reader IP and antenna port number for a specified antenna function.
 * @param antennaFunction The antenna function to search for (e.g., "READWRITE").
 * @returns A promise that resolves to a list of objects containing the reader IP and antenna port number for each matching antenna, or an empty list if none are found.
 */
export async function retrieveAntennaByFunction(antennaFunction: string): Promise<{
    readerIp: string,
    antennaPort: number
}[]> {
    const allReaders = await retrieveAllReaders();
    const result: { readerIp: string, antennaPort: number }[] = [];

    for (const existingReader of allReaders) {
        for (const antenna of existingReader.antennas) {
            if (antenna.function === antennaFunction) {
                result.push({readerIp: existingReader.ip, antennaPort: antenna.antennaPort});
            }
        }
    }

    return result;
}

/**
 * Deletes a reader by its IP address.
 * @param readerIp - The IP address of the reader to delete.
 * @returns A promise that resolves to an object with an error property if an error occurs, or void if the reader is deleted successfully.
 */
export async function deleteReader(readerIp: string): Promise<{ error: string } | void> {
    const reader = await getReaderByIp(readerIp);

    if (!reader) {
        return {error: "Reader not found"};
    }

    const removedReader = await removeReaderByIp(readerIp);

    if (!removedReader) {
        return {error: "Error removing reader"};
    }
}