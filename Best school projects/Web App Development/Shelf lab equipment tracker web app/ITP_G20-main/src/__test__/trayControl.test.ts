import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";

import { Tray, TrayStatus } from "@prisma/client";
import {
    editTray,
    reassignedTrayToRack,
    readTray,
    createNewTray,
    retrieveAllTray,
    retrieveAllInUsedTrays,
    updateTrayStatuses,
    deleteTray,
    retrieveTrayById,
    setTrayOutOfBound,
    retrieveAllOutOfBoundTrays,
} from "../domain/trayControl";
import {
    checkEpcExists,
    createTray,
    deleteTrayById,
    getAllTrays,
    getLastTrayId,
    getTrayByEpc,
    getTrayById,
    updateTray,

} from "../data/local/trayRepo";

import { readRfidTag, writeRfidTag } from "../domain/rfidControl";
import tds from 'epc-tds';
import { editTraySchema } from "../schema/custom";
import { z } from "zod";

jest.mock("../data/local/trayRepo", () => ({
    checkEpcExists: jest.fn(),
    createTray: jest.fn(),
    deleteTrayById: jest.fn(),
    getAllTrays: jest.fn(),
    getLastTrayId: jest.fn(),
    getTrayByEpc: jest.fn(),
    getTrayById: jest.fn(),
    updateTray: jest.fn(),
}));

jest.mock("../domain/rfidControl", () => ({
    readRfidTag: jest.fn(),
    writeRfidTag: jest.fn(),
}));

jest.mock('epc-tds', () => ({
    Sgtin96: jest.fn(() => ({
        setFilter: jest.fn().mockReturnThis(),
        setPartition: jest.fn().mockReturnThis(),
        setCompanyPrefix: jest.fn().mockReturnThis(),
        setItemReference: jest.fn().mockReturnThis(),
        setSerial: jest.fn().mockReturnThis(),
        toHexString: jest.fn().mockReturnValue('newEPC')
    })),
}));

describe('Tray Control', () => {


    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('edits a tray successfully', async () => {
        const trayData = {
            id: 1,
            epc: 'newEPC',
            status: TrayStatus.IN,
            statusChg: new Date(),
            rackId: 2,
        };

        (updateTray as jest.MockedFunction<typeof updateTray>).mockResolvedValue(trayData as Tray);

        const result = await editTray(trayData.id, trayData.epc, trayData.status, trayData.statusChg, trayData.rackId);

        expect(result).toEqual(trayData);
        expect(updateTray).toHaveBeenCalledWith(trayData.id, {
            epc: trayData.epc,
            status: trayData.status,
            statusChg: trayData.statusChg,
            rackId: trayData.rackId,
        });
    });

    it('reassigns a tray to a rack successfully', async () => {
        const trayData = {
            id: 1,
            epc: 'oldEPC',
            status: TrayStatus.IN,
            statusChg: new Date(),
            rackId: null,
        };

        const newRackId = 2;
        const newEPC = 'newEPC';

        (getTrayById as jest.MockedFunction<typeof getTrayById>).mockResolvedValue(trayData as Tray);
        (writeRfidTag as jest.MockedFunction<typeof writeRfidTag>).mockResolvedValue({ error: "" }); (updateTray as jest.MockedFunction<typeof updateTray>).mockResolvedValue({
            ...trayData,
            epc: newEPC,
            rackId: newRackId,
        } as Tray);

        const value = { id: trayData.id, rackId: newRackId };
        await reassignedTrayToRack(value);

        expect(getTrayById).toHaveBeenCalledWith(trayData.id);
        expect(writeRfidTag).toHaveBeenCalledWith(trayData.epc, newEPC);
        expect(updateTray).toHaveBeenCalledWith(trayData.id, {
            epc: newEPC,
            status: trayData.status,
            statusChg: trayData.statusChg,
            rackId: newRackId,
        });
    });

    it('reads a tray successfully', async () => {
        const trayData = {
            id: 1,
            epc: 'trayEPC',
            status: TrayStatus.IN,
            statusChg: new Date(),
            rackId: 2,
        };

        (readRfidTag as jest.MockedFunction<typeof readRfidTag>).mockResolvedValue('trayEPC');
        (getTrayByEpc as jest.MockedFunction<typeof getTrayByEpc>).mockResolvedValue(trayData as Tray);

        const result = await readTray();

        expect(result).toEqual(trayData);
        expect(readRfidTag).toHaveBeenCalled();
        expect(getTrayByEpc).toHaveBeenCalledWith('trayEPC');
    });

    it('creates a new tray successfully', async () => {
        const originalEpc = 'originalEPC';
        const newEPC = 'newEPC';
        const trayId = 1;
        const rackId = 2;

        (checkEpcExists as jest.MockedFunction<typeof checkEpcExists>).mockResolvedValue(false);
        (getLastTrayId as jest.MockedFunction<typeof getLastTrayId>).mockResolvedValue(trayId - 1);
        // (writeRfidTag as jest.MockedFunction<typeof writeRfidTag>).mockResolvedValue({ success: true });
        // (createTray as jest.MockedFunction<typeof createTray>).mockResolvedValue({ id: trayId, epc: newEPC, rackId });

        await createNewTray(originalEpc, rackId);

        expect(checkEpcExists).toHaveBeenCalledWith(originalEpc);
        expect(getLastTrayId).toHaveBeenCalled();
        expect(writeRfidTag).toHaveBeenCalledWith(originalEpc, newEPC);
        expect(createTray).toHaveBeenCalledWith({ id: trayId, epc: newEPC, rackId });
    });

    it('retrieves all trays successfully', async () => {
        const trays: Tray[] = [
            { id: 1, epc: 'epc1', status: TrayStatus.IN, statusChg: new Date(), rackId: 2 },
            { id: 2, epc: 'epc2', status: TrayStatus.OUT, statusChg: new Date(), rackId: 3 },
        ];

        (getAllTrays as jest.MockedFunction<typeof getAllTrays>).mockResolvedValue(trays);

        const result = await retrieveAllTray();

        expect(result).toEqual(trays);
        expect(getAllTrays).toHaveBeenCalled();
    });

    it('retrieves all in-use trays successfully', async () => {
        const trays: Tray[] = [
            { id: 1, epc: 'epc1', status: TrayStatus.IN, statusChg: new Date(), rackId: 2 },
            { id: 2, epc: 'epc2', status: TrayStatus.OUT, statusChg: new Date(), rackId: 3 },
        ];

        (getAllTrays as jest.MockedFunction<typeof getAllTrays>).mockResolvedValue(trays);

        const result = await retrieveAllInUsedTrays();

        expect(result).toEqual(trays.filter(tray => tray.rackId !== null));
        expect(getAllTrays).toHaveBeenCalled();
    });

    //review update tray status again

    it('deletes a tray successfully', async () => {
        const trayData = { id: 1, epc: 'epc', status: TrayStatus.IN, statusChg: new Date(), rackId: 2 };

        (deleteTrayById as jest.MockedFunction<typeof deleteTrayById>).mockResolvedValue(trayData as Tray);

        const result = await deleteTray(trayData.id);

        expect(result).toEqual(trayData);
        expect(deleteTrayById).toHaveBeenCalledWith(trayData.id);
    });

    it('retrieves a tray by ID successfully', async () => {
        const trayData = { id: 1, epc: 'epc', status: TrayStatus.IN, statusChg: new Date(), rackId: 2 };

        (getTrayById as jest.MockedFunction<typeof getTrayById>).mockResolvedValue(trayData as Tray);

        const result = await retrieveTrayById(trayData.id);

        expect(result).toEqual(trayData);
        expect(getTrayById).toHaveBeenCalledWith(trayData.id);
    });

    it('sets tray out of bound successfully', async () => {
        const trayData = { id: 1, epc: 'epc', status: TrayStatus.IN, statusChg: new Date(), rackId: 2 };

        (getTrayByEpc as jest.MockedFunction<typeof getTrayByEpc>).mockResolvedValue(trayData as Tray);
        (updateTray as jest.MockedFunction<typeof updateTray>).mockResolvedValue(trayData as Tray);

        await setTrayOutOfBound(trayData.epc, true);

        expect(getTrayByEpc).toHaveBeenCalledWith(trayData.epc);
        expect(updateTray).toHaveBeenCalledWith(trayData.id, {
            epc: trayData.epc,
            status: TrayStatus.OUT_OF_BOUND,
            statusChg: expect.any(Date),
            rackId: trayData.rackId,
        });
    });

    it('retrieves all out of bound trays successfully', async () => {
        const trays: Tray[] = [
            { id: 1, epc: 'epc1', status: TrayStatus.OUT_OF_BOUND, statusChg: new Date(), rackId: 2 },
            { id: 2, epc: 'epc2', status: TrayStatus.OUT_OF_BOUND, statusChg: new Date(), rackId: 3 },
        ];

        (getAllTrays as jest.MockedFunction<typeof getAllTrays>).mockResolvedValue(trays);

        const result = await retrieveAllOutOfBoundTrays();

        expect(result).toEqual(trays.filter(tray => tray.status === TrayStatus.OUT_OF_BOUND));
        expect(getAllTrays).toHaveBeenCalled();
    });

    it("retrieves all in-use trays successfully", async () => {
        const trays: Tray[] = [
            { id: 1, epc: "epc1", status: TrayStatus.IN, statusChg: new Date(), rackId: 2 },
            { id: 2, epc: "epc2", status: TrayStatus.OUT, statusChg: new Date(), rackId: null },
            { id: 3, epc: "epc3", status: TrayStatus.IN, statusChg: new Date(), rackId: 3 },
        ];

        (getAllTrays as jest.MockedFunction<typeof getAllTrays>).mockResolvedValue(trays);

        const result = await retrieveAllInUsedTrays();

        expect(result).toEqual(trays.filter(tray => tray.rackId !== null));
        expect(getAllTrays).toHaveBeenCalled();
    });
});