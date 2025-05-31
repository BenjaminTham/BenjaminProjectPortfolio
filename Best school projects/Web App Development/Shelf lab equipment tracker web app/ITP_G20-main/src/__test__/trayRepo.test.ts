import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";

import {
    getAllTrays,
    getTrayById,
    getTrayByEpc,
    getLastTrayId,
    createTray,
    checkEpcExists,
    updateTray,
    deleteTrayById,
} from "../data/local/trayRepo";
import { db } from "../lib/db";
import { Tray, TrayStatus } from "@prisma/client";


jest.mock("../lib/db", () => ({
    db: {
        tray: {
            findMany: jest.fn(),
            findUnique: jest.fn(),
            findFirst: jest.fn(),
            create: jest.fn(),
            update: jest.fn(),
            delete: jest.fn(),
        }
    }
}));

describe("deleteTrayById", () => {
    it("should delete a tray by its ID", async () => {
        const id = 1;
        const deletedTray: Tray = { id, epc: "EPC1", status: "IN", statusChg: new Date(), rackId: 1 };

        (db.tray.delete as jest.Mock).mockReturnValueOnce(deletedTray);

        const result = await deleteTrayById(id);

        expect(db.tray.delete).toHaveBeenCalledWith({
            where: { id },
        });
        expect(result).toEqual(deletedTray);
    });
});

describe("updateTray", () => {
    it("should update a tray with the specified ID", async () => {
        const id = 1;
        const data = { epc: "EPC1", status: TrayStatus.IN, statusChg: new Date(), rackId: 2 };
        const updatedTray: Tray = { id, ...data };

        (db.tray.update as jest.Mock).mockReturnValueOnce(updatedTray);

        const result = await updateTray(id, data);

        expect(db.tray.update).toHaveBeenCalledWith({
            where: { id },
            data: {
                epc: data.epc,
                status: data.status,
                statusChg: data.statusChg,
                rackId: data.rackId !== undefined ? data.rackId : null,
            },
        });
        expect(result).toEqual(updatedTray);
    });
});

describe("checkEpcExists", () => {
    it("should check if an EPC already exists in the database", async () => {
        const tray: Tray = { id: 1, epc: "EPC1", status: "IN", statusChg: new Date(), rackId: 1 };

        (db.tray.findUnique as jest.Mock).mockReturnValueOnce(tray);

        const result = await checkEpcExists("EPC1");

        expect(db.tray.findUnique).toHaveBeenCalledWith({
            where: { epc: "EPC1" }
        });
        expect(result).toEqual(true);
    });

    it("should return false if the EPC does not exist", async () => {
        (db.tray.findUnique as jest.Mock).mockReturnValueOnce(null);

        const result = await checkEpcExists("EPC2");

        expect(db.tray.findUnique).toHaveBeenCalledWith({
            where: { epc: "EPC2" }
        });
        expect(result).toEqual(false);
    });
});

describe("createTray", () => {
    it("should create a new tray with the provided data", async () => {
        const data = { id: 6, epc: "EPC6", rackId: 1 };
        const newTray: Tray = { id: data.id, epc: data.epc, status: "IN", statusChg: new Date(), rackId: data.rackId };

        (db.tray.create as jest.Mock).mockReturnValue(newTray);

        const result = await createTray(data);

        expect(db.tray.create).toHaveBeenCalledWith({
            data: {
                id: data.id,
                epc: data.epc,
                rackId: data.rackId || null,
            },
        });
        expect(result).toEqual(newTray);
    });
});

describe("getAllTrays", () => {
    it("should fetch all trays from the database", async () => {
        const trays: Tray[] = [
            { id: 1, epc: "EPC1", status: "IN", statusChg: new Date(), rackId: 1 },
            { id: 2, epc: "EPC2", status: "OUT", statusChg: new Date(), rackId: 2 }
        ];

        (db.tray.findMany as jest.Mock).mockReturnValueOnce(trays);

        const result = await getAllTrays();

        expect(db.tray.findMany).toHaveBeenCalled();
        expect(result).toEqual(trays);
    });
});

describe("getTrayById", () => {
    it("should retrieve a tray by its ID from the database", async () => {
        const tray: Tray = { id: 1, epc: "EPC1", status: "IN", statusChg: new Date(), rackId: 1 };

        (db.tray.findUnique as jest.Mock).mockReturnValueOnce(tray);

        const result = await getTrayById(1);

        expect(db.tray.findUnique).toHaveBeenCalledWith({
            where: { id: 1 }
        });
        expect(result).toEqual(tray);
    });
});

describe("getTrayByEpc", () => {
    it("should retrieve a tray by its EPC from the database", async () => {
        const tray: Tray = { id: 1, epc: "EPC1", status: "IN", statusChg: new Date(), rackId: 1 };

        (db.tray.findUnique as jest.Mock).mockReturnValueOnce(tray);

        const result = await getTrayByEpc("EPC1");

        expect(db.tray.findUnique).toHaveBeenCalledWith({
            where: { epc: "EPC1" }
        });
        expect(result).toEqual(tray);
    });
});


describe("getLastTrayId", () => {
    it("should retrieve the last tray ID from the database", async () => {
        const tray: Tray = { id: 5, epc: "EPC5", status: "IN", statusChg: new Date(), rackId: 1 };

        (db.tray.findFirst as jest.Mock).mockReturnValueOnce(tray);

        const result = await getLastTrayId();

        expect(db.tray.findFirst).toHaveBeenCalledWith({
            orderBy: { id: 'desc' }
        });
        expect(result).toEqual(5);
    });

    it("should return 0 if no trays are found", async () => {
        (db.tray.findFirst as jest.Mock).mockReturnValueOnce(null);

        const result = await getLastTrayId();

        expect(db.tray.findFirst).toHaveBeenCalledWith({
            orderBy: { id: 'desc' }
        });
        expect(result).toEqual(0);
    });
});