import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";

import {
    getAllReaders,
    ReaderWithAntennas,
    getReaderByIp,
    addReader,
    updateReader,
    removeReaderById,
    removeReaderByIp,
    addAntennaToReader,
    removeAntennaFromReader,
    getReadersWithGeofenceAntennas,

} from "../data/local/readerRepo";
import { db } from "../lib/db";
import { Reader } from "@prisma/client";

jest.mock("../lib/db", () => ({
    db: {
        reader: {
            findMany: jest.fn(),
            findUnique: jest.fn(),
            create: jest.fn(),
            update: jest.fn(),
            delete: jest.fn(),
            findFirst: jest.fn(),
        }
    }
}));

describe("getReadersWithGeofenceAntennas", () => {
    it("should retrieve readers with antennas that have the GEOFENCE function", async () => {
        const reader: ReaderWithAntennas = {
            id: "1",
            ip: "192.168.1.1",
            antennas: [{
                id: "1", antennaPort: 1, function: "GEOFENCE",
                readerId: null
            }]
        };

        (db.reader.findFirst as jest.Mock).mockReturnValueOnce(reader);

        const result = await getReadersWithGeofenceAntennas();

        expect(db.reader.findFirst).toHaveBeenCalledWith({
            where: {
                antennas: {
                    some: {
                        function: "GEOFENCE",
                    },
                },
            },
            include: {
                antennas: true,
            },
        });
        expect(result).toEqual(reader);
    });
});

describe("removeAntennaFromReader", () => {
    it("should remove an antenna from a reader", async () => {
        const data = { readerId: "1", antennaId: "2" };
        const updatedReader: Reader = {
            id: data.readerId,
            ip: "192.168.1.1",
            // antennas: []
        };

        (db.reader.update as jest.Mock).mockReturnValueOnce(updatedReader);

        const result = await removeAntennaFromReader(data);

        expect(db.reader.update).toHaveBeenCalledWith({
            where: {
                id: data.readerId,
            },
            data: {
                antennas: {
                    disconnect: {
                        id: data.antennaId,
                    },
                },
            },
        });
        expect(result).toEqual(updatedReader);
    });
});

describe("addAntennaToReader", () => {
    it("should add an antenna to a reader", async () => {
        const data = { readerId: "1", antennaId: "2" };
        const updatedReader: Reader = {
            id: data.readerId,
            ip: "192.168.1.1",
            // antennas: [{ id: data.antennaId, antennaPort: 1, function: "GEOFENCE" }]
        };

        (db.reader.update as jest.Mock).mockReturnValueOnce(updatedReader);

        const result = await addAntennaToReader(data);

        expect(db.reader.update).toHaveBeenCalledWith({
            where: {
                id: data.readerId,
            },
            data: {
                antennas: {
                    connect: {
                        id: data.antennaId,
                    },
                },
            },
        });
        expect(result).toEqual(updatedReader);
    });
});

describe("removeReaderByIp", () => {
    it("should remove a reader by its IP address from the database", async () => {
        const readerIp = "192.168.1.1";
        const removedReader: Reader = { id: "1", ip: readerIp };

        (db.reader.delete as jest.Mock).mockReturnValueOnce(removedReader);

        const result = await removeReaderByIp(readerIp);

        expect(db.reader.delete).toHaveBeenCalledWith({
            where: {
                ip: readerIp,
            },
        });
        expect(result).toEqual(removedReader);
    });
});

describe("removeReaderById", () => {
    it("should remove a reader by its ID from the database", async () => {
        const readerId = "1";
        const removedReader: Reader = { id: readerId, ip: "192.168.1.1" };

        (db.reader.delete as jest.Mock).mockReturnValueOnce(removedReader);

        const result = await removeReaderById(readerId);

        expect(db.reader.delete).toHaveBeenCalledWith({
            where: {
                id: readerId,
            },
        });
        expect(result).toEqual(removedReader);
    });
});

describe("updateReader", () => {
    it("should update a reader's IP address by its ID", async () => {
        const data = { id: "1", ip: "192.168.1.4" };
        const updatedReader: Reader = { id: data.id, ip: data.ip };

        (db.reader.update as jest.Mock).mockReturnValueOnce(updatedReader);

        const result = await updateReader(data);

        expect(db.reader.update).toHaveBeenCalledWith({
            where: {
                id: data.id,
            },
            data: {
                ip: data.ip,
            },
        });
        expect(result).toEqual(updatedReader);
    });
});

describe("getAllReaders", () => {
    it("should retrieve all readers with their antennas from the database", async () => {
        const readers: ReaderWithAntennas[] = [
            {
                id: "1", ip: "192.168.1.1", antennas: [{
                    id: "1", antennaPort: 1, function: "GEOFENCE",
                    readerId: null
                }]
            },
            {
                id: "2", ip: "192.168.1.2", antennas: [{
                    id: "2", antennaPort: 2, function: "INVENTORY",
                    readerId: null
                }]
            },
        ];

        (db.reader.findMany as jest.Mock).mockReturnValueOnce(readers);

        const result = await getAllReaders();

        expect(db.reader.findMany).toHaveBeenCalledWith({
            include: {
                antennas: true,
            },
        });
        expect(result).toEqual(readers);
    });
});

describe("getReaderByIp", () => {
    it("should retrieve a reader with its antennas by IP address from the database", async () => {
        const reader: ReaderWithAntennas = {
            id: "1", ip: "192.168.1.1", antennas: [{
                id: "1", antennaPort: 1, function: "GEOFENCE",
                readerId: null
            }]
        };

        (db.reader.findUnique as jest.Mock).mockReturnValueOnce(reader);

        const result = await getReaderByIp("192.168.1.1");

        expect(db.reader.findUnique).toHaveBeenCalledWith({
            where: {
                ip: "192.168.1.1",
            },
            include: {
                antennas: true,
            },
        });
        expect(result).toEqual(reader);
    });
});

describe("addReader", () => {
    it("should add a new reader to the database", async () => {
        const data = { ip: "192.168.1.3" };
        const newReader: Reader = { id: "3", ip: data.ip };

        (db.reader.create as jest.Mock).mockReturnValueOnce(newReader);

        const result = await addReader(data);

        expect(db.reader.create).toHaveBeenCalledWith({
            data: {
                ip: data.ip,
            },
        });
        expect(result).toEqual(newReader);
    });
});