"use server";

import { Tray, TrayStatus } from "@prisma/client";
import { readRfidTag, writeRfidTag } from "@/domain/rfidControl";
import {
    checkEpcExists,
    createTray,
    deleteTrayById,
    getAllTrays,
    getLastTrayId,
    getTrayByEpc,
    getTrayById,
    updateTray
} from "@/data/local/trayRepo";
import tds from 'epc-tds'
import { editTraySchema } from "@/schema/custom";
import { z } from "zod";

/**
 * Edits a tray with the provided information.
 * @returns A promise that resolves to the updated tray object.
 * @throws An error if the `inOutTime` property is null.
 * @param id
 * @param epc
 * @param status
 * @param statusChg
 * @param rackId
 */
export async function editTray(id: number, epc: string, status: TrayStatus, statusChg: Date, rackId?: number): Promise<Tray> {
    if (statusChg === null) {
        throw new Error("statusChg is required");
    }
    return updateTray(id, { epc, status, statusChg, rackId });
}

export async function reassignedTrayToRack(value: z.infer<typeof editTraySchema>) {
    const validation = editTraySchema.safeParse(value);

    if (!validation.success) {
        return { error: "Invalid input" };
    }

    const { id, rackId } = validation.data;

    console.log(`Reassigning tray ${id} to rack ${rackId}`);

    const tray = await getTrayById(id);

    if (!tray) {
        return { error: "Tray not found" };
    }

    if (tray.rackId === rackId) {
        return
    }

    // Write the new EPC to the tray
    const trayId = tray.id;
    const sgtin = new tds.Sgtin96().setFilter(rackId ? 1 : 0)
        .setPartition(6)
        .setCompanyPrefix(534954) // SIT ascii to hex
        .setItemReference(rackId ? rackId : 0) // Rack ID if tray is in a rack else 0
        .setSerial(trayId)

    const newEPC = sgtin.toHexString()

    const response = await writeRfidTag(tray.epc, newEPC);

    if (response?.error) {
        return { error: response.error };
    }

    // Update the tray
    await editTray(trayId, newEPC, tray.status, tray.statusChg, rackId ? rackId : undefined);
}

/**
 * Reads a tray using the RFID reader and retrieves its information from the database.
 * @returns A promise that resolves to the tray object.
 */
export async function readTray(): Promise<{ error: string } | Tray> {
    const epc = await readRfidTag();
    if (typeof epc === "string") {
        const tray = await getTrayByEpc(epc);
        if (!tray) {
            return { error: "Tray not found in the database" };
        }
        else {
            return tray;
        }
    } else {
        return { error: epc.error };
    }
}

export async function createNewTray(originalEpc: string, rackId?: number) {
    // Check if epc already exists
    if (await checkEpcExists(originalEpc)) {
        return { error: "EPC already exists" };
    }

    const trayId = await getLastTrayId() + 1;
    const sgtin = new tds.Sgtin96().setFilter(rackId ? 1 : 0)
        .setPartition(6)
        .setCompanyPrefix(534954) // SIT ascii to hex
        .setItemReference(rackId ? rackId : 0) // Rack ID if tray is in a rack else 0
        .setSerial(trayId)

    const newEPC = sgtin.toHexString()

    const response = await writeRfidTag(originalEpc, newEPC);

    if (response?.error) {
        return { error: response.error };
    }

    if (!rackId) {
        await createTray({ id: trayId, epc: newEPC });
        return;
    }

    // Create the tray
    await createTray({ id: trayId, epc: newEPC, rackId });
}

/**
 * Fetches all trays from the database.
 * @returns A Promise that resolves to an array of trays.
 */
export async function retrieveAllTray(): Promise<Tray[]> {
    return getAllTrays();
}

/**
 * Retrieves all trays from all racks.
 * @returns A promise that resolves to an array of trays.
 */
export async function retrieveAllInUsedTrays(): Promise<Tray[]> {
    const trays = await getAllTrays();
    // Filter out trays that are not in a rack
    return trays.filter(tray => tray.rackId !== null);
}

/**
 * Refreshes the status of trays in the system.
 * @param epcs The EPCs of the trays to refresh.
 * @param tagHistory A map of EPCs to the last time they were detected.
 * @param currentTime The current time.
 * @param timeOut The time out value for marking a tray as OUT.
 */
export async function updateTrayStatuses(epcs: string[], tagHistory: Map<string, Date>, currentTime: Date, timeOut: number): Promise<void> {
    const allTrays = await retrieveAllInUsedTrays();
    const traysDetected = allTrays.filter(tray => epcs.includes(tray.epc));
    const traysNotDetected = allTrays.filter(tray => !epcs.includes(tray.epc));

    // For trays that are not detected in current RFID reporting data , update the status to OUT
    // if the tray is not detected for more than the timeOut value
    await Promise.all(traysNotDetected.map(async tray => {
        const lastTime = tagHistory.get(tray.epc);
        if (lastTime && currentTime.getTime() - lastTime.getTime() > timeOut && tray.status === TrayStatus.IN) {
            tray.status = TrayStatus.OUT;
            tray.statusChg = currentTime;
            await editTray(tray.id, tray.epc, tray.status, tray.statusChg, tray.rackId as number || undefined);
        }
    }));

    // For trays that are detected in current RFID reporting data , update the status to IN
    await Promise.all(traysDetected.map(async tray => {
        if (tray.status === TrayStatus.IN) {
            return;
        }
        tray.status = TrayStatus.IN;
        tray.statusChg = currentTime;
        await editTray(tray.id, tray.epc, tray.status, tray.statusChg, tray.rackId as number || undefined);
    }));
}

/**
 * Deletes a tray by its ID.
 * @param id - The ID of the tray to delete.
 * @returns A promise that resolves to the deleted tray.
 */
export async function deleteTray(id: number): Promise<Tray> {
    return deleteTrayById(id);
}

/**
 * Gets a tray by its ID.
 * @param id - The ID of the tray to delete.
 * @returns A promise that resolves to the deleted tray.
 */
export async function retrieveTrayById(id: number): Promise<Tray | null> {
    return getTrayById(id);
}

export async function setTrayOutOfBound(epc: string, outOfBound: Boolean) {
    const tray = await getTrayByEpc(epc);
    if (tray && tray.status !== TrayStatus.OUT_OF_BOUND && outOfBound) {
        const outOfBoundTime = new Date()
        const status = TrayStatus.OUT_OF_BOUND
        console.log(`Tray ${tray.id} is out of bound at ${outOfBoundTime} with rackId ${tray.rackId || undefined}`)
        await updateTray(tray.id, {
            epc,
            status,
            statusChg: outOfBoundTime,
            rackId: tray.rackId || undefined
        });
        return
    }
    if (tray && tray.status === TrayStatus.OUT_OF_BOUND && !outOfBound) {
        const status = TrayStatus.OUT
        await updateTray(tray.id, {
            epc,
            status,
            statusChg: new Date(),
            rackId: tray.rackId || undefined
        });
        return
    }
}

export async function retrieveAllOutOfBoundTrays(): Promise<Tray[]> {
    const trays = await getAllTrays();
    return trays.filter(tray => tray.status === TrayStatus.OUT_OF_BOUND);
}