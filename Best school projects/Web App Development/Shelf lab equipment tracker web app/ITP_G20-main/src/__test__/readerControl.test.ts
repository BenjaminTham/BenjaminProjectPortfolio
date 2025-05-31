import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";

import { addNewReader, deleteReader, retrieveAllReaders, updateAllAntennaFunction } from "../domain/readerControl";
import { addReader, getReaderByIp, addAntennaToReader, getAllReaders, removeReaderByIp } from "../data/local/readerRepo";
import { addAntenna, updateAntenna } from "../data/local/antennaRepo";
import { retrieveAntennaByFunction } from "../domain/readerControl";
import isPortReachable from "is-port-reachable";

jest.mock("../data/local/readerRepo", () => ({
    addReader: jest.fn(),
    getReaderByIp: jest.fn(),
    addAntennaToReader: jest.fn(),
    getAllReaders: jest.fn(),

}));

jest.mock("../data/local/antennaRepo", () => ({
    addAntenna: jest.fn(),
    updateAntenna: jest.fn(),

}));

jest.mock("../domain/readerControl", () => ({
    retrieveAntennaByFunction: jest.fn()
}));

jest.mock("is-port-reachable");

describe("deleteReader", () => {
    it("should delete a reader by its IP address", async () => {
        const readerIp = "192.168.1.1";
        const reader = { id: "1", ip: readerIp };

        (getReaderByIp as jest.Mock).mockReturnValueOnce(reader);
        (removeReaderByIp as jest.Mock).mockReturnValueOnce(reader);

        const result = await deleteReader(readerIp);

        expect(getReaderByIp).toHaveBeenCalledWith(readerIp);
        expect(removeReaderByIp).toHaveBeenCalledWith(readerIp);
        expect(result).toBeUndefined();
    });

    it("should return an error if the reader is not found", async () => {
        const readerIp = "192.168.1.1";

        (getReaderByIp as jest.Mock).mockReturnValueOnce(null);

        const result = await deleteReader(readerIp);

        expect(getReaderByIp).toHaveBeenCalledWith(readerIp);
        expect(result).toEqual({ error: "Reader not found" });
    });

    it("should return an error if there is an error removing the reader", async () => {
        const readerIp = "192.168.1.1";
        const reader = { id: "1", ip: readerIp };

        (getReaderByIp as jest.Mock).mockReturnValueOnce(reader);
        (removeReaderByIp as jest.Mock).mockReturnValueOnce(null);

        const result = await deleteReader(readerIp);

        expect(getReaderByIp).toHaveBeenCalledWith(readerIp);
        expect(removeReaderByIp).toHaveBeenCalledWith(readerIp);
        expect(result).toEqual({ error: "Error removing reader" });
    });
});

describe("retrieveAntennaByFunction", () => {
    it("should retrieve the reader IP and antenna port number for a specified antenna function", async () => {
        const antennaFunction = "READWRITE";
        const readers = [{ id: "1", ip: "192.168.1.1", antennas: [{ id: "1", antennaPort: 1, function: "READWRITE" }] }];

        (getAllReaders as jest.Mock).mockReturnValueOnce(readers);

        const result = await retrieveAntennaByFunction(antennaFunction);

        expect(getAllReaders).toHaveBeenCalled();
        expect(result).toEqual([{ readerIp: "192.168.1.1", antennaPort: 1 }]);
    });
});

describe("updateAllAntennaFunction", () => {
    it("should update the function of all antennas for a given reader", async () => {
        const readerIp = "192.168.1.1";
        const data = {
            antenna1: "INVENTORY",
            antenna2: "GEOFENCE",
            antenna3: "READWRITE",
            antenna4: "NONE",
            antenna5: "NONE",
            antenna6: "NONE",
            antenna7: "NONE",
            antenna8: "NONE"
        };
        const reader = { id: "1", ip: readerIp, antennas: [{ id: "1", antennaPort: 1, function: "NONE" }] };

        (getReaderByIp as jest.Mock).mockReturnValueOnce(reader);
        (retrieveAntennaByFunction as jest.Mock).mockReturnValueOnce([]);

        const result = await updateAllAntennaFunction(readerIp, data);

        expect(getReaderByIp).toHaveBeenCalledWith(readerIp);
        expect(updateAntenna).toHaveBeenCalled();
        expect(result).toBeUndefined();
    });

    it("should return an error if the reader is not found", async () => {
        const readerIp = "192.168.1.1";
        const data = {
            antenna1: "INVENTORY",
            antenna2: "GEOFENCE",
            antenna3: "READWRITE",
            antenna4: "NONE",
            antenna5: "NONE",
            antenna6: "NONE",
            antenna7: "NONE",
            antenna8: "NONE"
        };

        (getReaderByIp as jest.Mock).mockReturnValueOnce(null);

        const result = await updateAllAntennaFunction(readerIp, data);

        expect(getReaderByIp).toHaveBeenCalledWith(readerIp);
        expect(result).toEqual({ error: "Reader not found" });
    });

    it("should return an error if there is more than one READWRITE antenna", async () => {
        const readerIp = "192.168.1.1";
        const data = {
            antenna1: "READWRITE",
            antenna2: "READWRITE",
            antenna3: "INVENTORY",
            antenna4: "NONE",
            antenna5: "NONE",
            antenna6: "NONE",
            antenna7: "NONE",
            antenna8: "NONE"
        };
        const reader = { id: "1", ip: readerIp, antennas: [{ id: "1", antennaPort: 1, function: "NONE" }] };

        (getReaderByIp as jest.Mock).mockReturnValueOnce(reader);

        const result = await updateAllAntennaFunction(readerIp, data);

        expect(result).toEqual({ error: "There can only be one READWRITE antenna" });
    });
});

describe("retrieveAllReaders", () => {
    it("should retrieve all readers with antennas", async () => {
        const readers = [{ id: "1", ip: "192.168.1.1", antennas: [{ id: "1", antennaPort: 1, function: "INVENTORY" }] }];

        (getAllReaders as jest.Mock).mockReturnValueOnce(readers);

        const result = await retrieveAllReaders();

        expect(getAllReaders).toHaveBeenCalled();
        expect(result).toEqual(readers);
    });
});

describe("addNewReader", () => {
    it("should add a new reader with the provided values", async () => {
        const values = { ip: "192.168.1.1" };
        const reader = { id: "1", ip: values.ip };

        (isPortReachable as jest.Mock).mockReturnValueOnce(true);
        (getReaderByIp as jest.Mock).mockReturnValueOnce(null);
        (addReader as jest.Mock).mockReturnValueOnce(reader);
        (addAntenna as jest.Mock).mockReturnValueOnce({ id: "1", antennaPort: 1, function: "NONE" });

        await addNewReader(values);

        expect(isPortReachable).toHaveBeenCalledWith(8080, { host: values.ip });
        expect(getReaderByIp).toHaveBeenCalledWith(values.ip);
        expect(addReader).toHaveBeenCalledWith({ ip: values.ip });
        expect(addAntenna).toHaveBeenCalledTimes(8);
        expect(addAntennaToReader).toHaveBeenCalledTimes(8);
    });

    it("should return an error if the IP is not reachable", async () => {
        const values = { ip: "192.168.1.1" };

        (isPortReachable as jest.Mock).mockReturnValueOnce(false);

        const result = await addNewReader(values);

        expect(isPortReachable).toHaveBeenCalledWith(8080, { host: values.ip });
        expect(result).toEqual({ error: "IP Address is not reachable!" });
    });

    it("should return an error if the reader already exists", async () => {
        const values = { ip: "192.168.1.1" };

        (isPortReachable as jest.Mock).mockReturnValueOnce(true);
        (getReaderByIp as jest.Mock).mockReturnValueOnce({ id: "1", ip: values.ip });

        const result = await addNewReader(values);

        expect(isPortReachable).toHaveBeenCalledWith(8080, { host: values.ip });
        expect(getReaderByIp).toHaveBeenCalledWith(values.ip);
        expect(result).toEqual({ error: "Reader already exists" });
    });
});