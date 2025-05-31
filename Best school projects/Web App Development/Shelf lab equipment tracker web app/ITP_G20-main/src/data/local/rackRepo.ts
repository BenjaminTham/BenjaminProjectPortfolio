import {db} from "@/lib/db";
import {Rack} from "@prisma/client";

/**
 * Retrieves all racks from the database.
 * @returns A promise that resolves to an array of Rack objects.
 */
export async function getAllRacks(): Promise<Rack[]> {
    return db.rack.findMany();
}

/**
 * Creates a new rack in the database.
 * @param data
 */
export async function addNewRack(data: { name: string; location: string }): Promise<Rack> {
    return db.rack.create({
        data: {
            location: data.location,
            name: data.name,
        },
    });
}

/**
 * Edits the location of a rack by its ID.
 * @param rackId - The ID of the rack to edit.
 * @param location - The new location of the rack.
 * @param name - The new name of the rack.
 * @returns A promise that resolves when the rack is successfully edited.
 */
export async function editRackById(rackId: number, location: string, name: string): Promise<void> {
    await db.rack.update({
        where: {id: rackId},
        data: {
            location: location,
            name: name
        },
    });
}

/**
 * Deletes a rack from the database.
 * @param rackId The ID of the rack to delete.
 * @returns A promise that resolves when the rack is deleted.
 */
export async function deleteRackById(rackId: number): Promise<void> {
    await db.rack.delete({
        where: {id: rackId},
    });
}

