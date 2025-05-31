//src/domain/rackcontrol.ts

"use server";

import {addNewRack, deleteRackById, editRackById, getAllRacks} from "@/data/local/rackRepo";
import {newRackSchema, updateRackSchema} from "@/schema/custom";
import {Rack} from "@prisma/client";
import {z} from "zod";

/**
 * Retrieves all racks from the database without trays.
 * @returns A promise that resolves to an array of Rack objects.
 */
export async function retrieveAllRacks(): Promise<Rack[]> {
    return getAllRacks();
}

/**
 * Creates a new rack in the database.
 * @param values The values for creating a new rack.
 */
export async function createNewRack(values: z.infer<typeof newRackSchema>) {
    const validation = newRackSchema.safeParse(values);

    if (!validation.success) {
        throw new Error("Invalid input");
    }

    const {location, name} = validation.data;

    return await addNewRack({location, name});
}

export async function updateRack(values: z.infer<typeof updateRackSchema>) {
    const validation = updateRackSchema.safeParse(values);

    if (!validation.success) {
        throw new Error("Invalid input");
    }

    const {rackId, location, name} = validation.data;

    return await editRackById(rackId, location, name);
}

/**
 * Delete racks and stuff in the database.
 * @param rackId The ID of the rack to delete.
 */
export async function deleteRack(rackId: number): Promise<void> {
    try {
        await deleteRackById(rackId);
        console.log(`Rack with ID ${rackId} has been deleted.`);
    } catch (error) {
        console.error(`Error deleting rack with ID ${rackId}:`, error);
        throw new Error(`Unable to delete rack with ID ${rackId}`);
    }
}